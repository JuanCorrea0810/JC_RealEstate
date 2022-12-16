using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RealEstate.Controllers;
using RealEstate.Models;
using RealEstate.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace PeliculasAPI.Tests
{
    public class BasePruebas
    {
        protected string usuarioPorDefectoId = "9722b56a-77ea-4e41-941d-e319b6eb3712";
        protected string usuarioPorDefectoEmail = "ejemplo@hotmail.com";

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

        

        //private UserManager<TUser> BuildUserManager<TUser>(IUserStore<TUser> store = null) where TUser : class
        //{
        //    store = store ?? new Mock<IUserStore<TUser>>().Object;
        //    var options = new Mock<IOptions<IdentityOptions>>();
        //    var idOptions = new IdentityOptions();
        //    idOptions.Lockout.AllowedForNewUsers = false;

        //    options.Setup(o => o.Value).Returns(idOptions);

        //    var userValidators = new List<IUserValidator<TUser>>();

        //    var validator = new Mock<IUserValidator<TUser>>();
        //    userValidators.Add(validator.Object);
        //    var pwdValidators = new List<PasswordValidator<TUser>>();
        //    pwdValidators.Add(new PasswordValidator<TUser>());

        //    var userManager = new UserManager<TUser>(store, options.Object, new PasswordHasher<TUser>(),
        //        userValidators, pwdValidators, new UpperInvariantLookupNormalizer(),
        //        new IdentityErrorDescriber(), null,
        //        new Mock<ILogger<UserManager<TUser>>>().Object);

        //    validator.Setup(v => v.ValidateAsync(userManager, It.IsAny<TUser>()))
        //        .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();

        //    return userManager;
        //}

        //private static SignInManager<TUser> SetupSignInManager<TUser>(UserManager<TUser> manager,
        //    HttpContext context, ILogger logger = null, IdentityOptions identityOptions = null,
        //    IAuthenticationSchemeProvider schemeProvider = null) where TUser : class
        //{
        //    var contextAccessor = new Mock<IHttpContextAccessor>();
        //    contextAccessor.Setup(a => a.HttpContext).Returns(context);
        //    identityOptions = identityOptions ?? new IdentityOptions();
        //    var options = new Mock<IOptions<IdentityOptions>>();
        //    options.Setup(a => a.Value).Returns(identityOptions);
        //    var claimsFactory = new UserClaimsPrincipalFactory<TUser>(manager, options.Object);
        //    schemeProvider = schemeProvider ?? new Mock<IAuthenticationSchemeProvider>().Object;
        //    var sm = new SignInManager<TUser>(manager, contextAccessor.Object, claimsFactory, options.Object, null, schemeProvider, new DefaultUserConfirmation<TUser>());
        //    sm.Logger = logger ?? (new Mock<ILogger<SignInManager<TUser>>>()).Object;
        //    return sm;
        //}

        //protected ControllerContext ConstruirControllerContext()
        //{
        //    var usuario = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        //    {
        //        new Claim(ClaimTypes.Name, usuarioPorDefectoEmail),
        //        new Claim(ClaimTypes.Email, usuarioPorDefectoEmail),
        //        new Claim(ClaimTypes.NameIdentifier, usuarioPorDefectoId)
        //    }));

        //    return new ControllerContext()
        //    {
        //        HttpContext = new DefaultHttpContext() { User = usuario }
        //    };
        //}

        //protected WebApplicationFactory<Startup> ConstruirWebApplicationFactory(string nombreBD,
        //    bool ignorarSeguridad = true)
        //{
        //    var factory = new WebApplicationFactory<Startup>();

        //    factory = factory.WithWebHostBuilder(builder =>
        //    {
        //        builder.ConfigureTestServices(services =>
        //        {
        //            var descriptorDBContext = services.SingleOrDefault(d =>
        //            d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

        //            if (descriptorDBContext != null)
        //            {
        //                services.Remove(descriptorDBContext);
        //            }

        //            services.AddDbContext<ApplicationDbContext>(options => 
        //            options.UseInMemoryDatabase(nombreBD));

        //            if (ignorarSeguridad)
        //            {
        //                services.AddSingleton<IAuthorizationHandler, AllowAnonymousHandler>();

        //                services.AddControllers(options =>
        //                {
        //                    options.Filters.Add(new UsuarioFalsoFiltro());
        //                });
        //            }
        //        });
        //    });

        //    return factory;
        //}
    }
}
