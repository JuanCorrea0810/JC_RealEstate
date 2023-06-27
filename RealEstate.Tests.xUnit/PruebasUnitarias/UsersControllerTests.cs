using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using RealEstate.Controllers;
using RealEstate.Models;
using RealEstate.DTO_s.UsersDTO_s;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace RealEstate.Tests.xUnit
{
    public class UsersControllerTests: BasePruebas
    {
        [Fact]
        public async Task DevuelveUsuariosPaginados() 
        {           
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
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
                UserName = "JuanCorrea0810",
                Id = Guid.NewGuid().ToString(),
                EmailConfirmed = false,
                AccessFailedCount = 0,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = true,
                LockoutEnd = null,
                NormalizedEmail = "JUANCORREAR08102@GMAIL.COM",
                NormalizedUserName = "JUANCORREA0810",
                PasswordHash = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = false,
                SecurityStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = false
            }) ;
            context.Users.Add(new NewIdentityUser
            {
                Dni = 1098765432,
                Email = "juancorrear081021@gmail.com",
                Address = "dadasdasdasd",
                Age = 23,
                Country = "Colombia",
                FirstName = "Juan",
                SecondName = "Manuel",
                FirstSurName = "Correa",
                SecondSurName = "Rojas",
                PhoneNumber = "3774895628",
                UserName = "JuanCorrea08101",
                Id = Guid.NewGuid().ToString(),
                EmailConfirmed = false,
                AccessFailedCount = 0,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = true,
                LockoutEnd = null,
                NormalizedEmail = "JUANCORREAR081021@GMAIL.COM",
                NormalizedUserName = "JUANCORREA08101",
                PasswordHash = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = false,
                SecurityStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = false
            });
            context.Users.Add(new NewIdentityUser
            {
                Dni = 1098765432,
                Email = "juancorrear081022@gmail.com",
                Address = "dadasdasdasd",
                Age = 23,
                Country = "Colombia",
                FirstName = "Juan",
                SecondName = "Manuel",
                FirstSurName = "Correa",
                SecondSurName = "Rojas",
                PhoneNumber = "3774895628",
                UserName = "JuanCorrea08102",
                Id = Guid.NewGuid().ToString(),
                EmailConfirmed = false,
                AccessFailedCount = 0,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = true,
                LockoutEnd = null,
                NormalizedEmail = "JUANCORREAR081022@GMAIL.COM",
                NormalizedUserName = "JUANCORREA08102",
                PasswordHash = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = false,
                SecurityStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = false
            });
            await context.SaveChangesAsync();
            var controller = ConstruirUsersController(nombreBD);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var dto = new PaginacionDTO() { Pagina = 1, CantidadRegistrosPorPagina = 2 };
            
            var resultado1 = await controller.GetUsers(dto);
            
            var Pagina1 = resultado1.Value;

            Pagina1.Should().NotBeNull();
            Pagina1.Count.Should().Be(2);

            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var dto2 = new PaginacionDTO() { Pagina = 2, CantidadRegistrosPorPagina = 2 };
            var resultado2 = await controller.GetUsers(dto2);
            
            var Pagina2 = resultado2.Value;

            Pagina2.Should().NotBeNull();
            Pagina2.Count.Should().Be(1);

            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var dto3 = new PaginacionDTO() { Pagina = 3, CantidadRegistrosPorPagina = 2 };
            var resultado3 = await controller.GetUsers(dto3);

            var Pagina3 = resultado3.Value;

            Pagina3.Should().NotBeNull();
            Pagina3.Count.Should().Be(0);
        }

        [Fact]
        public async Task DevuelveLosAdminsPaginados()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
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
                UserName = "JuanCorrea0810",
                Id = "Usuario1",
                EmailConfirmed = false,
                AccessFailedCount = 0,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = true,
                LockoutEnd = null,
                NormalizedEmail = "JUANCORREAR08102@GMAIL.COM",
                NormalizedUserName = "JUANCORREA0810",
                PasswordHash = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = false,
                SecurityStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = false
            });
            context.Users.Add(new NewIdentityUser
            {
                Dni = 1098765432,
                Email = "juancorrear081021@gmail.com",
                Address = "dadasdasdasd",
                Age = 23,
                Country = "Colombia",
                FirstName = "Juan",
                SecondName = "Manuel",
                FirstSurName = "Correa",
                SecondSurName = "Rojas",
                PhoneNumber = "3774895628",
                UserName = "JuanCorrea08101",
                Id = "Usuario2",
                EmailConfirmed = false,
                AccessFailedCount = 0,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = true,
                LockoutEnd = null,
                NormalizedEmail = "JUANCORREAR081021@GMAIL.COM",
                NormalizedUserName = "JUANCORREA08101",
                PasswordHash = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = false,
                SecurityStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = false
            });
            context.Users.Add(new NewIdentityUser
            {
                Dni = 1098765432,
                Email = "juancorrear081022@gmail.com",
                Address = "dadasdasdasd",
                Age = 23,
                Country = "Colombia",
                FirstName = "Juan",
                SecondName = "Manuel",
                FirstSurName = "Correa",
                SecondSurName = "Rojas",
                PhoneNumber = "3774895628",
                UserName = "JuanCorrea08102",
                Id = "Usuario3",
                EmailConfirmed = false,
                AccessFailedCount = 0,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = true,
                LockoutEnd = null,
                NormalizedEmail = "JUANCORREAR081022@GMAIL.COM",
                NormalizedUserName = "JUANCORREA08102",
                PasswordHash = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = false,
                SecurityStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = false
            });
            context.Roles.Add(new IdentityRole {
            Id= "Rol1",
            Name = "Admin",
            ConcurrencyStamp= Guid.NewGuid().ToString(),
            NormalizedName = "NAME"
            });
            context.UserRoles.Add(new IdentityUserRole<string> {
            RoleId = "Rol1",
            UserId = "Usuario1"
            });
            context.UserRoles.Add(new IdentityUserRole<string>
            {
                RoleId = "Rol1",
                UserId = "Usuario2"
            });
            await context.SaveChangesAsync();
            
            var controller = ConstruirUsersController(nombreBD);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var dto = new PaginacionDTO() { Pagina = 1, CantidadRegistrosPorPagina = 2 };

            var resultado1 = await controller.GetAdmins(dto);

            var Pagina1 = resultado1.Value;
            Pagina1.Should().NotBeNull();
            Pagina1.Count.Should().Be(2);

            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var dto2 = new PaginacionDTO() { Pagina = 2, CantidadRegistrosPorPagina = 2 };
            var resultado2 = await controller.GetAdmins(dto2);

            var Pagina2 = resultado2.Value;

            Pagina2.Should().NotBeNull();
            Pagina2.Count.Should().Be(0);
        }

        [Fact]
        public async Task SeBloqueaUsuario()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
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
                UserName = "JuanCorrea0810",
                Id = "Usuario1",
                EmailConfirmed = false,
                AccessFailedCount = 0,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = true,
                LockoutEnd = null,
                NormalizedEmail = "JUANCORREAR08102@GMAIL.COM",
                NormalizedUserName = "JUANCORREA0810",
                PasswordHash = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = false,
                SecurityStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = false
            });

            await context.SaveChangesAsync();
            var controller = ConstruirUsersController(nombreBD);
            
            var Resultado = await controller.LockOutUser("Usuario1");

            var Respuesta = Resultado as OkResult;
            var context2 = ConstruirContext(nombreBD);
            var UsuarioSeEncuentraBloqueado = await context2.Users.AnyAsync(x=> x.LockoutEnd != null);

            Respuesta.Should().NotBeNull();
            UsuarioSeEncuentraBloqueado.Should().BeTrue();
            Respuesta.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task NoSePuedeBloquearUsuarioQueNoExiste()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var controller = ConstruirUsersController(nombreBD);
            
            var Resultado = await controller.LockOutUser("Usuario1");

            var Respuesta = Resultado as NotFoundResult; 

            Respuesta.Should().NotBeNull();
            Respuesta.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task NoSePuedeDesbloquearUsuarioQueNoExiste()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var controller = ConstruirUsersController(nombreBD);

            var Resultado = await controller.UnLock("Usuario1");

            var Respuesta = Resultado as NotFoundResult;

            Respuesta.Should().NotBeNull();
            Respuesta.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task SeDesbloqueaUsuario()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
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
                UserName = "JuanCorrea0810",
                Id = "Usuario1",
                EmailConfirmed = false,
                AccessFailedCount = 0,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = true,
                LockoutEnd = DateTime.Now.AddDays(2),
                NormalizedEmail = "JUANCORREAR08102@GMAIL.COM",
                NormalizedUserName = "JUANCORREA0810",
                PasswordHash = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = false,
                SecurityStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = false
            });

            await context.SaveChangesAsync();
            var controller = ConstruirUsersController(nombreBD);

            var Resultado = await controller.UnLock("Usuario1");

            var Respuesta = Resultado as OkResult;
            var context2 = ConstruirContext(nombreBD);
            var UsuarioSeEncuentraDesbloqueado = await context2.Users.AnyAsync(x => x.LockoutEnd < DateTime.Now);

            Respuesta.Should().NotBeNull();
            UsuarioSeEncuentraDesbloqueado.Should().BeTrue();
            Respuesta.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task SeEliminaUnUsuario()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
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
                UserName = "JuanCorrea0810",
                Id = "Usuario1",
                EmailConfirmed = false,
                AccessFailedCount = 0,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = true,
                LockoutEnd = DateTime.Now.AddDays(2),
                NormalizedEmail = "JUANCORREAR08102@GMAIL.COM",
                NormalizedUserName = "JUANCORREA0810",
                PasswordHash = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = false,
                SecurityStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = false
            });

            await context.SaveChangesAsync();
            var controller = ConstruirUsersController(nombreBD);

            var Resultado = await controller.DeleteUser("Usuario1");

            var Respuesta = Resultado as OkResult;
            var context2 = ConstruirContext(nombreBD);
            var ExisteUsuario = await context2.Users.AnyAsync();

            Respuesta.Should().NotBeNull();
            ExisteUsuario.Should().BeFalse();
            Respuesta.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task NoSePuedeEliminarUsuarioQueNoExiste()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var controller = ConstruirUsersController(nombreBD);

            var Resultado = await controller.DeleteUser("Usuario1");

            var Respuesta = Resultado as NotFoundResult;
            
            Respuesta.Should().NotBeNull();
            Respuesta.StatusCode.Should().Be(404);
        }
        private UsersController ConstruirUsersController(string nombreBD)
        {
            var context = ConstruirContext(nombreBD);
            var miUserStore = new UserStore<NewIdentityUser>(context);
            var miRoleStore = new RoleStore<IdentityRole>(context);
            var userManager = BuildUserManager(miUserStore);
            var RoleManager = BuildRoleManager(miRoleStore);
            var mapper = ConfigurarAutoMapper();

            return new UsersController(userManager, mapper, RoleManager, context);
        }
       
    }
}
