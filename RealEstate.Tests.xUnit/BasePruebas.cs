using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using RealEstate.Controllers;
using RealEstate.Models;
using RealEstate.Utilities;

namespace RealEstate.Tests.xUnit
{
    public class BasePruebas
    {
        

        protected RealEstateProjectContext ConstruirContext(string nombreDB)
        {
            var opciones = new DbContextOptionsBuilder<RealEstateProjectContext>()
                .UseInMemoryDatabase(nombreDB).Options;

            var dbContext = new RealEstateProjectContext(opciones);
            return dbContext;
        }

        protected IMapper ConfigurarAutoMapper()
        {
            var config = new MapperConfiguration(options =>
            {
                
                options.AddProfile(new AutoMapperProfile());
            });

            return config.CreateMapper();
        }

        /*Método que sirve para todos los controladores. En donde cada acción de cada controlador se busca saber si la propiedad existe en primer lugar.
         siempre va antes de realizar cualquier otro paso. Ya que si no existe propiedad no se puede hacer nada con los demás controladores.*/
        protected async Task<bool> ExistePropiedad(int IdEstate,string IdUser, string nombreDB) 
        {
            var context = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();
            var mock = new Mock<IGetUserInfo>();
            mock.Setup(x => x.GetId()).Returns(Task.FromResult($"{IdUser}"));
            var controller = new EstatesController(context, mapper, mock.Object);

            var resultado = await controller.GetById(IdEstate);
            if (resultado.Value != null)
            {
                return true;
            }
            return false;
        }

        

        // Source: https://github.com/dotnet/aspnetcore/blob/master/src/Identity/test/Shared/MockHelpers.cs
        // Source: https://github.com/dotnet/aspnetcore/blob/master/src/Identity/test/Identity.Test/SignInManagerTest.cs
        protected UserManager<TUser> BuildUserManager<TUser>(IUserStore<TUser> store = null) where TUser : class
        {
            store = store ?? new Mock<IUserStore<TUser>>().Object;
            var options = new Mock<IOptions<IdentityOptions>>();
            var idOptions = new IdentityOptions();
            idOptions.Lockout.AllowedForNewUsers = true;
            idOptions.Password.RequireNonAlphanumeric = false;
            idOptions.Lockout.MaxFailedAccessAttempts = 6;
            idOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.Zero;

            options.Setup(o => o.Value).Returns(idOptions);

            var userValidators = new List<IUserValidator<TUser>>();

            var validator = new Mock<IUserValidator<TUser>>();
            userValidators.Add(validator.Object);
            var pwdValidators = new List<PasswordValidator<TUser>>();
            pwdValidators.Add(new PasswordValidator<TUser>());

            var userManager = new UserManager<TUser>(store, options.Object, new PasswordHasher<TUser>(),
                userValidators, pwdValidators, new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(), null,
                new Mock<ILogger<UserManager<TUser>>>().Object);

            validator.Setup(v => v.ValidateAsync(userManager, It.IsAny<TUser>()))
                .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();

            return userManager;
        }

        protected static SignInManager<TUser> SetupSignInManager<TUser>(UserManager<TUser> manager,
            HttpContext context, ILogger logger = null, IdentityOptions identityOptions = null,
            IAuthenticationSchemeProvider schemeProvider = null) where TUser : class
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(a => a.HttpContext).Returns(context);
            identityOptions = identityOptions ?? new IdentityOptions();
            var options = new Mock<IOptions<IdentityOptions>>();
            options.Setup(a => a.Value).Returns(identityOptions);
            var claimsFactory = new UserClaimsPrincipalFactory<TUser>(manager, options.Object);
            schemeProvider = schemeProvider ?? new Mock<IAuthenticationSchemeProvider>().Object;
            var sm = new SignInManager<TUser>(manager, contextAccessor.Object, claimsFactory, options.Object, null, schemeProvider, new DefaultUserConfirmation<TUser>());
            sm.Logger = logger ?? (new Mock<ILogger<SignInManager<TUser>>>()).Object;
            return sm;
        }
        protected static RoleManager<TRole> BuildRoleManager<TRole>(IRoleStore<TRole> store = null) where TRole : class
        {
            store = store ?? new Mock<IRoleStore<TRole>>().Object;
            var roles = new List<IRoleValidator<TRole>>();
            roles.Add(new RoleValidator<TRole>());
            return new RoleManager<TRole>(store, roles,
                null,
                new IdentityErrorDescriber(),
                null);
        }

        //Pruebas de Integración
        protected WebApplicationFactory<Startup> ConstruirWebApplicationFactory(string nombreDB, bool ignorarSeguridad = true) 
        {
            var factory = new WebApplicationFactory<Startup>();
            factory = factory.WithWebHostBuilder(builder => 
            {
                builder.ConfigureTestServices(services => 
                {
                    //Remover el servicio que está inyectando la clase DbContext, esto es porque queremos usar un proveedor en memoria de EF
                    // Y en la clase Startup se está inyectando es con un proveedor de base de datos completo.
                    //Así que tenemos que eliminar ese servicio al momento de correr nuestras pruebas de integración.
                    var descriptorDbContext = services.SingleOrDefault(x=> x.ServiceType == typeof(DbContextOptions<RealEstateProjectContext>));

                    //Si no es nulo entonces removemos ese servicio
                    if (descriptorDbContext != null)
                    {
                        services.Remove(descriptorDbContext);
                    }

                    //Ahora agregamos el proveedor en memoria a nuestra colección de servicios
                    services.AddDbContext<RealEstateProjectContext>(options => options.UseInMemoryDatabase(nombreDB));

                    //Si queremos saltar todos los filtros de autorización
                    if (ignorarSeguridad)
                    {
                        //Se agrega un filtro de acción para que se ejecute antes y después de cada endpoint
                        // Este filtro lo que hará es que proporcionará acceso a claims cuando se necesite información del usuario
                        services.AddControllers(options => 
                        {
                            options.Filters.Add(new UsuarioFalso());
                        });

                        //Se agrega la funcionalidad que me permite saltarme todos lo filtros de autorización
                        services.AddSingleton<IAuthorizationHandler,AuthorizationRequirements>();
                    }
                
                });
            });
            return factory;
        }
    }
}
