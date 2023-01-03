using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using PeliculasAPI.Tests;
using RealEstate.Controllers;
using RealEstate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using RealEstate.Utilities.Auth;
using RealEstate.Models.Auth;
using RealEstate.Utilities;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using RealEstate.DTO_s.UsersDTO_s;

namespace RealEstateTests.PruebasUnitarias
{
    [TestClass]
    public class CuentasControllerTests:BasePruebas
    {
        [TestMethod]
        public async Task SeRegistraUsuario() 
        { 
            var nombreBD= Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            context.Roles.Add(new IdentityRole
            {
                Id = "Rol1",
                Name = "USER",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "USER"
            });
            await context.SaveChangesAsync();
            var controller = ConstruirCuentasController(nombreBD);
            controller.Url = new UrlHelperMock();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            
            var Credentials = new InfoUser() { Email = "juancorrear08102@gmail.com", Password = "Aa123456!", ConfirmPassword = "Aa123456!" };

            var resultado = await controller.Sign_Up(Credentials);

            var respuesta = resultado.Value;
            var context2 = ConstruirContext(nombreBD);
            var ExisteUsuario = await context.Users.AnyAsync();
            var SeRegistraEnRolUser = await context2.UserRoles.AnyAsync();
            var SeAgregaClaimUser = await context2.UserClaims.AnyAsync();
            Assert.IsNotNull(respuesta);
            Assert.IsNotNull(respuesta.Token);
            Assert.IsTrue(ExisteUsuario);
            Assert.IsTrue(SeRegistraEnRolUser);
            Assert.IsTrue(SeAgregaClaimUser);
        }

        [TestMethod]
        public async Task SeIniciaSesion()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
           context.Roles.Add(new IdentityRole
            {
                Id = "Rol1",
                Name = "USER",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "USER"
            }); 
            await context.SaveChangesAsync();
            await CrearUsuarioHelper(nombreBD);
            var controller = ConstruirCuentasController(nombreBD);
            

            var Credentials = new LoginAuth() { Email = "juancorrear08102@gmail.com", Password = "Aa123456!" };

            var resultado = await controller.LogIn(Credentials);

            var respuesta = resultado.Value;
            
            Assert.IsNotNull(respuesta);
            Assert.IsNotNull(respuesta.Token);

        }
        [TestMethod]
        public async Task NoSePuedeIniciarSesionSiUsuarioEstáBloqueado()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            context.Roles.Add(new IdentityRole
            {
                Id = "Rol1",
                Name = "USER",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "USER"
            });
            await context.SaveChangesAsync();
            await CrearUsuarioHelper(nombreBD);
            var Usuario = await context.Users.FirstOrDefaultAsync();
            Usuario.LockoutEnd = DateTime.Now.AddMonths(2);
            Usuario.LockoutEnabled = true;
            context.Entry(Usuario).State = EntityState.Modified;
            await context.SaveChangesAsync();
            var controller = ConstruirCuentasController(nombreBD);


            var Credentials = new LoginAuth() { Email = "juancorrear08102@gmail.com", Password = "Aa123456!" };

            var resultado = await controller.LogIn(Credentials);

            var respuesta = resultado.Result as BadRequestObjectResult;

            Assert.IsNotNull(respuesta);
            Assert.AreEqual(400,respuesta.StatusCode);

        }
        [TestMethod]
        public async Task NoSePuedeIniciarSesionConDatosIncorrectos()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            context.Roles.Add(new IdentityRole
            {
                Id = "Rol1",
                Name = "USER",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "USER"
            });
            await context.SaveChangesAsync();
            await CrearUsuarioHelper(nombreBD);
            var Usuario = await context.Users.FirstOrDefaultAsync();
            Usuario.AccessFailedCount = 2;
            context.Entry(Usuario).State = EntityState.Modified;
            await context.SaveChangesAsync();
            var controller = ConstruirCuentasController(nombreBD);


            var Credentials = new LoginAuth() { Email = "juancorrear08102@gmail.com", Password = "123455678995" };

            var resultado = await controller.LogIn(Credentials);

            var respuesta = resultado.Result as BadRequestObjectResult;

            Assert.IsNotNull(respuesta);
            Assert.AreEqual(400, respuesta.StatusCode);
            Assert.AreEqual("Datos Incorrectos. Vuelva a intentarlo.",respuesta.Value);
        }
        [TestMethod]
        public async Task MuestraMensajeDeAdvertenciaCuandoSoloQuedaUnIntentoDeInicioDeSesion()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            context.Roles.Add(new IdentityRole
            {
                Id = "Rol1",
                Name = "USER",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "USER"
            });
            await context.SaveChangesAsync();
            await CrearUsuarioHelper(nombreBD);
            var Usuario = await context.Users.FirstOrDefaultAsync();
            Usuario.AccessFailedCount = 3;
            context.Entry(Usuario).State = EntityState.Modified;
            await context.SaveChangesAsync();
            var controller = ConstruirCuentasController(nombreBD);


            var Credentials = new LoginAuth() { Email = "juancorrear08102@gmail.com", Password = "123455678995" };

            var resultado = await controller.LogIn(Credentials);

            var respuesta = resultado.Result as BadRequestObjectResult;

            Assert.IsNotNull(respuesta);
            Assert.AreEqual(400, respuesta.StatusCode);
            Assert.AreEqual("Solo le queda un intento, si vuelve a fallar se bloqueará la cuenta temporalmente", respuesta.Value);
        }
        [TestMethod]
        public async Task MuestraMensajeDeCuentaBloqueadaTemporalmente()
        {

            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            context.Roles.Add(new IdentityRole
            {
                Id = "Rol1",
                Name = "USER",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "USER"
            });
            await context.SaveChangesAsync();
            await CrearUsuarioHelper(nombreBD);
            var Usuario = await context.Users.FirstOrDefaultAsync();
            Usuario.AccessFailedCount = 4;
            context.Entry(Usuario).State = EntityState.Modified;
            await context.SaveChangesAsync();
            var controller = ConstruirCuentasController(nombreBD);
       

            var Credentials = new LoginAuth() { Email = "juancorrear08102@gmail.com", Password = "123455678995" };

            var resultado = await controller.LogIn(Credentials);

            var respuesta = resultado.Result as BadRequestObjectResult;

            Assert.IsNotNull(respuesta);
            Assert.AreEqual(400, respuesta.StatusCode);
            Assert.AreEqual("Cuenta bloqueda temporalmente. Intente más tarde", respuesta.Value);
        }
        [TestMethod]
        public async Task SeCompletaPerfil()
        {

            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            context.Roles.Add(new IdentityRole
            {
                Id = "Rol1",
                Name = "USER",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "USER"
            });
            context.Users.Add(new NewIdentityUser
            {
                
                Email = "juancorrear08102@gmail.com",               
                UserName = "juancorrear08102@gmail.com",
                Id = "Usuario1",
                EmailConfirmed = false,
                AccessFailedCount = 0,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = true,
                LockoutEnd = null,
                NormalizedEmail = "JUANCORREAR08102@GMAIL.COM",
                NormalizedUserName = "JUANCORREAR08102@GMAIL.COM",
                PasswordHash = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = false,
                SecurityStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = false
            });
            await context.SaveChangesAsync();
            var Usuario = (NewIdentityUser)await context.Users.FirstOrDefaultAsync();
            Assert.IsNull(Usuario.Country);
            var controller = ConstruirCuentasController(nombreBD);

            var dto = new PutUsersDTO() {
                Dni = 1098765431,
                Address = "dadasdasdasd",
                Age = 23,
                Country = "Colombia",
                FirstName = "Juan",
                SecondName = "Manuel",
                FirstSurName = "Correa",
                SecondSurName = "Rojas",
                PhoneNumber = "1234567890",
            };

            var resultado = await controller.CompleteProfile(dto);

            var respuesta = resultado as OkResult;
            var context2 = ConstruirContext(nombreBD);
            var UsuarioActualizado = (NewIdentityUser)await context2.Users.FirstOrDefaultAsync();

            Assert.IsNotNull(respuesta);
            Assert.AreEqual(200, respuesta.StatusCode);
            Assert.IsNotNull(UsuarioActualizado.Country);
            Assert.AreEqual(1098765431,UsuarioActualizado.Dni);
        }
        [TestMethod]
        public async Task SeCompletaPerfilSiElDniEsElMismo()
        {

            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            context.Roles.Add(new IdentityRole
            {
                Id = "Rol1",
                Name = "USER",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "USER"
            });
            context.Users.Add(new NewIdentityUser
            {
                Dni = 1098765431,
                Email = "juancorrear08102@gmail.com",
                Address = "dadasdasdasd",
                Age = 23,
                Country = "Colombia",
                FirstName = "Juan",
                SecondName = "Manuel",
                FirstSurName = "Correa",
                SecondSurName = "Rojas",
                PhoneNumber = "3774895628",
                UserName = "juancorrear08102@gmail.com",
                Id = "Usuario1",
                EmailConfirmed = false,
                AccessFailedCount = 0,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = true,
                LockoutEnd = null,
                NormalizedEmail = "JUANCORREAR08102@GMAIL.COM",
                NormalizedUserName = "JUANCORREAR08102@GMAIL.COM",
                PasswordHash = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = false,
                SecurityStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = false                
            });
            await context.SaveChangesAsync();
     
            var controller = ConstruirCuentasController(nombreBD);

            var dto = new PutUsersDTO()
            {
                Dni = 1098765431,
                Address = "dadasdasdasd",
                Age = 23,
                Country = "Argentina",
                FirstName = "Juan",
                SecondName = "Manuel",
                FirstSurName = "Correa",
                SecondSurName = "Rojas",
                PhoneNumber = "0987654321",
            };

            var resultado = await controller.CompleteProfile(dto);

            var respuesta = resultado as OkResult;
            var context2 = ConstruirContext(nombreBD);
            var UsuarioActualizado = (NewIdentityUser)await context2.Users.FirstOrDefaultAsync();

            Assert.IsNotNull(respuesta);
            Assert.AreEqual(200, respuesta.StatusCode);
            Assert.AreEqual("Argentina",UsuarioActualizado.Country);
            Assert.AreEqual(1098765431, UsuarioActualizado.Dni);
            Assert.AreEqual("0987654321",UsuarioActualizado.PhoneNumber);
        }
        [TestMethod]
        public async Task NoDejaActualizarSiElDniLoTieneOtroUsuario()
        {

            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            context.Roles.Add(new IdentityRole
            {
                Id = "Rol1",
                Name = "USER",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "USER"
            });
            context.Users.Add(new NewIdentityUser
            {
                Dni = 1234156890,
                Email = "juancorrear08102@gmail.com",
                Address = "dadasdasdasd",
                Age = 23,
                Country = "Colombia",
                FirstName = "Juan",
                SecondName = "Manuel",
                FirstSurName = "Correa",
                SecondSurName = "Rojas",
                PhoneNumber = "3774895628",
                UserName = "juancorrear08102@gmail.com",
                Id = "Usuario1",
                EmailConfirmed = false,
                AccessFailedCount = 0,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = true,
                LockoutEnd = null,
                NormalizedEmail = "JUANCORREAR08102@GMAIL.COM",
                NormalizedUserName = "JUANCORREAR08102@GMAIL.COM",
                PasswordHash = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = false,
                SecurityStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = false
            });
            context.Users.Add(new NewIdentityUser
            {
                Dni = 1098765431,
                Email = "juancorrear08102@gmail.com",
                Address = "dadasdasdasd",
                Age = 23,
                Country = "Colombia",
                FirstName = "Juan",
                SecondName = "Manuel",
                FirstSurName = "Correa",
                SecondSurName = "Rojas",
                PhoneNumber = "3774895628",
                UserName = "juancorrear08102@gmail.com",
                Id = "Usuario2",
                EmailConfirmed = false,
                AccessFailedCount = 0,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = true,
                LockoutEnd = null,
                NormalizedEmail = "JUANCORREAR08102@GMAIL.COM",
                NormalizedUserName = "JUANCORREAR08102@GMAIL.COM",
                PasswordHash = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = false,
                SecurityStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = false
            });
            await context.SaveChangesAsync();

            var controller = ConstruirCuentasController(nombreBD);

            var dto = new PutUsersDTO()
            {
                Dni = 1098765431,
                Address = "dadasdasdasd",
                Age = 23,
                Country = "Argentina",
                FirstName = "Juan",
                SecondName = "Manuel",
                FirstSurName = "Correa",
                SecondSurName = "Rojas",
                PhoneNumber = "0987654321",
            };

            var resultado = await controller.CompleteProfile(dto);

            var respuesta = resultado as BadRequestObjectResult;
            
            var context2 = ConstruirContext(nombreBD);
            
            Assert.IsNotNull(respuesta);
            Assert.AreEqual(400, respuesta.StatusCode);
            Assert.AreEqual("DNI no aceptado", respuesta.Value);
            
        }
        [TestMethod]
        public async Task CambiarPassword()
        {

            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            context.Roles.Add(new IdentityRole
            {
                Id = "Rol1",
                Name = "USER",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "USER"
            });
            await context.SaveChangesAsync();
            await CrearUsuarioHelper(nombreBD);

            var controller = ConstruirCuentasController(nombreBD);
            
            //Esto se hace para poder utilizar el Id del usuario que recién se creó en la base de datos
            var UsuarioId = await context.Users.Select(x => x.Id).FirstAsync();
            
            //Aqui hacemos un nuevo mock y no utilizamos el que por defecto se crea en el método donde construimos el controlador
            var mockUser = new Mock<IGetUserInfo>();
            mockUser.Setup(x => x.GetId()).Returns(Task.FromResult(UsuarioId));
            
            //Aqui gracias a la propiedad publica GetUserInfo podemos añadir un nuevo mock con diferentes especificaciones
            //del que se inyectó por el constructor en el método que construye el controlador
            controller.GetUserInfo = mockUser.Object;

            var dto = new ChangePassword() { CurrentPassword = "Aa123456!", NewPassword = "Juancho0810!", ConfirmPassword = "Juancho0810!" };

            var resultado = await controller.ChangePassword(dto);

            var respuesta = resultado as OkResult;
            Assert.IsNotNull(respuesta);
            Assert.AreEqual(200, respuesta.StatusCode);

            var Credentials = new LoginAuth() { Email = "juancorrear08102@gmail.com", Password = "Juancho0810!", RememberMe = true};

            var resultado2 = await controller.LogIn(Credentials);
            var respuesta2 = resultado2.Value;
            Assert.IsNotNull(respuesta2);
            Assert.IsNotNull(respuesta2.Token);
            
        }

        private async Task CrearUsuarioHelper(string nombreBD)
        {
            var controller = ConstruirCuentasController(nombreBD);
            controller.Url = new UrlHelperMock();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var userInfo = new InfoUser() { Email = "juancorrear08102@gmail.com", Password = "Aa123456!", ConfirmPassword = "Aa123456!" };
            await controller.Sign_Up(userInfo);
        }

        private CuentasController ConstruirCuentasController(string nombreBD)
        {
            var context = ConstruirContext(nombreBD);
            var miUserStore = new UserStore<NewIdentityUser>(context);
            var userManager = BuildUserManager(miUserStore);
            var mapper = ConfigurarAutoMapper();

            //Se agrega un proveedor de tokens para que los métodos que tienen que generar algún token puedan funcionar
            userManager.RegisterTokenProvider("Default", new DefaultTokenProvider<NewIdentityUser>());
            
            var HttpContext = new DefaultHttpContext();
            //Se hace mock para poder simular la autenticación del IAuthenticationService cuando se llame al método SignInAsync
            MockAuth(HttpContext);

            var signInManager = SetupSignInManager(userManager, HttpContext);
            var miConfiguracion = new Dictionary<string, string>
            {
                {"SecretKey", "AJDSKMAJKNVUJNQUENIMCOKANMXKLSAMKOSADNQIDMPLANSKDLANKCNKS" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(miConfiguracion)
                .Build();

            var TokenLog_In = new TokenAuthLogIn(configuration,userManager);
            var TokenSign_Up = new TokenAuthSignUp(configuration, userManager);

            var mockUser = new Mock<IGetUserInfo>();
            mockUser.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));

   
            var mockEmailSender = new Mock<IEmailSender>();
            mockEmailSender.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));


            return new CuentasController(userManager,context ,signInManager, TokenLog_In, TokenSign_Up, mockUser.Object,mapper, mockEmailSender.Object);
        }
        private Mock<IAuthenticationService> MockAuth(HttpContext context)
        {
            var auth = new Mock<IAuthenticationService>();
            context.RequestServices = new ServiceCollection().AddSingleton(auth.Object).BuildServiceProvider();
            return auth;
        }
    }
}
