using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using RealEstate.Controllers;
using RealEstate.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FluentAssertions;
using Moq;

namespace RealEstate.Tests.xUnit
{
    public class RolesControllerTests: BasePruebas
    {
        [Fact]
        public async Task DevuelveListaDeRoles() 
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            context.Roles.Add(new IdentityRole
            {
                Id = "Rol1",
                Name = "Admin",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "Admin"
            });
            context.Roles.Add(new IdentityRole
            {
                Id = "Rol2",
                Name = "User",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "User"
            });
            context.Roles.Add(new IdentityRole
            {
                Id = "Rol3",
                Name = "Owner",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "Owner"
            });
            await context.SaveChangesAsync();
            var controller = ConstruirRolesController(nombreBD);

            var resultado = await controller.Roles();

            var respuesta = resultado.Value;

            respuesta.Should().NotBeNull();
            respuesta.Count.Should().Be(3);
        }

        [Fact]
        public async Task NoSePuedeCrearDosRolesConElMismoName()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            context.Roles.Add(new IdentityRole
            {
                Id = "Rol1",
                Name = "User",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "User"
            });
            await context.SaveChangesAsync();

            var controller = ConstruirRolesController(nombreBD);
            

            var resultado = await controller.CreateRole("User");

            var respuesta = resultado as BadRequestObjectResult;

            respuesta.Should().NotBeNull();
            respuesta.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task SeCreaNuevoRole()
        {
            var nombreBD = Guid.NewGuid().ToString();
          
            var controller = ConstruirRolesController(nombreBD);
            

            var resultado = await controller.CreateRole("User");

            var respuesta = resultado as NoContentResult;
            var context = ConstruirContext(nombreBD);
            var SeCreaRol = await context.Roles.AnyAsync();

            respuesta.Should().NotBeNull();
            respuesta.StatusCode.Should().Be(204);
            SeCreaRol.Should().BeTrue();
        }

        [Fact]
        public async Task SeBorraRol()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            context.Roles.Add(new IdentityRole
            {
                Id = "Rol1",
                Name = "Admin",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "Admin"
            });
            await context.SaveChangesAsync();

            var controller = ConstruirRolesController(nombreBD);


            var resultado = await controller.DeleteRole("Admin");

            var respuesta = resultado as NoContentResult;
            var context2 = ConstruirContext(nombreBD);
            var ExisteRol = await context2.Roles.AnyAsync();

            respuesta.Should().NotBeNull();
            respuesta.StatusCode.Should().Be(204);
            ExisteRol.Should().BeFalse();
        }

        [Fact]
        public async Task NoSePuedeBorrarRolQueNoExiste()
        {
            var nombreBD = Guid.NewGuid().ToString();
            
            var controller = ConstruirRolesController(nombreBD);


            var resultado = await controller.DeleteRole("User");

            var respuesta = resultado as NotFoundResult;

            respuesta.Should().NotBeNull();
            respuesta.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task SeAgregaUsuarioAUnRol()
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
            context.Roles.Add(new IdentityRole
            {
                Id = "Rol1",
                Name = "USER",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "USER"
            });
            await context.SaveChangesAsync();

            var controller = ConstruirRolesController(nombreBD);


            var resultado = await controller.AddToRole("Usuario1","USER");

            var respuesta = resultado as OkResult;
            var context2 = ConstruirContext(nombreBD);
            var SeRelacionaUsuarioYRol = await context2.UserRoles.FirstOrDefaultAsync(x=> x.UserId == "Usuario1" && x.RoleId == "Rol1");
            var SeAgreganClaims = await context2.UserClaims.AnyAsync(x=> x.UserId == "Usuario1");

            respuesta.Should().NotBeNull();
            SeRelacionaUsuarioYRol.Should().NotBeNull();
            respuesta.StatusCode.Should().Be(200);
            SeAgreganClaims.Should().BeTrue();
        }

        [Fact]
        public async Task NoSePuedeAgregaUsuarioAUnRolQueNoExiste()
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

            var controller = ConstruirRolesController(nombreBD);


            var resultado = await controller.AddToRole("Usuario1", "USER");

            var respuesta = resultado as NotFoundResult;
            
            respuesta.Should().NotBeNull();
            respuesta.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task NoSePuedeAgregarAUnRolUnUsuarioQueNoExiste()
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

            var controller = ConstruirRolesController(nombreBD);


            var resultado = await controller.AddToRole("Usuario1", "USER");

            var respuesta = resultado as NotFoundResult;

            respuesta.Should().NotBeNull();
            respuesta.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task SeEliminaUsuarioDeUnRol()
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
            context.Roles.Add(new IdentityRole
            {
                Id = "Rol1",
                Name = "USER",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "USER"
            });
            context.UserRoles.Add(new IdentityUserRole<string>
            {
            UserId = "Usuario1",
            RoleId = "Rol1"
            });
            context.UserClaims.Add(new IdentityUserClaim<string>
            {

                UserId = "Usuario1",
                ClaimType = ClaimTypes.Role,
                ClaimValue = "USER",
                Id = 1
            }) ;
            await context.SaveChangesAsync();

            var controller = ConstruirRolesController(nombreBD);
  
            var resultado = await controller.DeleteFromRole("Usuario1", "USER");

            var respuesta = resultado as OkResult;
            var context2 = ConstruirContext(nombreBD);
            var ExisteRelacionUsuarioYRol = await context2.UserRoles.AnyAsync();
            var ExisteClaimsDelUsuario = await context2.UserClaims.AnyAsync();

            respuesta.Should().NotBeNull();
            respuesta.StatusCode.Should().Be(200);
            ExisteRelacionUsuarioYRol.Should().BeFalse();
            ExisteClaimsDelUsuario.Should().BeFalse();
        }

        [Fact]
        public async Task SeAgregaUsuarioAVariosRoles()
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
            context.Roles.Add(new IdentityRole
            {
                Id = "Rol1",
                Name = "USER",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "USER"
            });
            context.Roles.Add(new IdentityRole
            {
                Id = "Rol2",
                Name = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "ADMIN"
            });
            context.Roles.Add(new IdentityRole
            {
                Id = "Rol3",
                Name = "OWNER",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "OWNER"
            });
            await context.SaveChangesAsync();

            var controller = ConstruirRolesController(nombreBD);

            List<string> Roles = new List<string>() {"USER","ADMIN" };
            var resultado = await controller.AddToRoles("Usuario1", Roles);

            var respuesta = resultado as OkResult;
            var context2 = ConstruirContext(nombreBD);
            var UserRoles = await context2.UserRoles.ToListAsync();
            var UserClaims = await context2.UserClaims.ToListAsync();

            respuesta.Should().NotBeNull();
            UserRoles.Count.Should().Be(2);
            UserClaims.Count.Should().Be(2);
            respuesta.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task DevuelverErrorSiAlgunRolEnLaListaEsIncorrecto()
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
            context.Roles.Add(new IdentityRole
            {
                Id = "Rol1",
                Name = "USER",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "USER"
            });
            context.Roles.Add(new IdentityRole
            {
                Id = "Rol2",
                Name = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "ADMIN"
            });
            
            await context.SaveChangesAsync();

            var controller = ConstruirRolesController(nombreBD);

            List<string> Roles = new List<string>() { "USER", "ADMIN","OWNER" };
            var resultado = await controller.AddToRoles("Usuario1", Roles);

            var respuesta = resultado as BadRequestObjectResult;

            respuesta.Should().NotBeNull();
            respuesta.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task SeEliminaUsuarioDeVariosRoles()
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
            context.Roles.Add(new IdentityRole
            {
                Id = "Rol1",
                Name = "USER",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "USER"
            });
            context.Roles.Add(new IdentityRole
            {
                Id = "Rol2",
                Name = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "ADMIN"
            });
            context.Roles.Add(new IdentityRole
            {
                Id = "Rol3",
                Name = "OWNER",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "OWNER"
            });
            context.UserRoles.Add(new IdentityUserRole<string>
            {
                UserId = "Usuario1",
                RoleId = "Rol1"
            });
            context.UserRoles.Add(new IdentityUserRole<string>
            {
                UserId = "Usuario1",
                RoleId = "Rol2"
            });
            context.UserRoles.Add(new IdentityUserRole<string>
            {
                UserId = "Usuario1",
                RoleId = "Rol3"
            });
            context.UserClaims.Add(new IdentityUserClaim<string>
            {

                UserId = "Usuario1",
                ClaimType = ClaimTypes.Role,
                ClaimValue = "USER",
                Id = 1
            });
            context.UserClaims.Add(new IdentityUserClaim<string>
            {

                UserId = "Usuario1",
                ClaimType = ClaimTypes.Role,
                ClaimValue = "ADMIN",
                Id = 2
            });
            context.UserClaims.Add(new IdentityUserClaim<string>
            {

                UserId = "Usuario1",
                ClaimType = ClaimTypes.Role,
                ClaimValue = "OWNER",
                Id = 3
            });
            await context.SaveChangesAsync();

            var controller = ConstruirRolesController(nombreBD);

            List<string> Roles = new List<string>() { "OWNER", "USER", "ADMIN" };
            var resultado = await controller.DeleteFromRoles("Usuario1", Roles);

            var respuesta = resultado as OkResult;
            var context2 = ConstruirContext(nombreBD);
            var UserRoles = await context2.UserRoles.AnyAsync();
            var UserClaims = await context2.UserClaims.AnyAsync();

            respuesta.Should().NotBeNull();
            UserRoles.Should().BeFalse();
            UserClaims.Should().BeFalse();
            respuesta.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task SeAgregaUnUsuarioARolAdminCorrectamente()
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
            context.Roles.Add(new IdentityRole
            {
                Id = "Rol1",
                Name = "USER",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "USER"
            });
            context.Roles.Add(new IdentityRole
            {
                Id = "Rol2",
                Name = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "ADMIN"
            });
            context.UserRoles.Add(new IdentityUserRole<string>
            {
                UserId = "Usuario1",
                RoleId = "Rol1"
            });
            context.UserClaims.Add(new IdentityUserClaim<string>
            {

                UserId = "Usuario1",
                ClaimType = ClaimTypes.Role,
                ClaimValue = "USER",
                Id = 1
            });

            await context.SaveChangesAsync();

            var controller = ConstruirRolesController(nombreBD);

            var resultado = await controller.CreateAdmin("Usuario1");

            var respuesta = resultado as OkResult;
            var context2 = ConstruirContext(nombreBD);
            var UserRoles = await context2.UserRoles.ToListAsync();
            var UserClaims = await context2.UserClaims.ToListAsync();

            respuesta.Should().NotBeNull();
            UserRoles.Count.Should().Be(2);
            UserClaims.Count.Should().Be(2);
            respuesta.StatusCode.Should().Be(200);
        }
        [Fact]
        public async Task SeEliminaUnUsuarioDeRolAdminCorrectamente()
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
            context.Roles.Add(new IdentityRole
            {
                Id = "Rol1",
                Name = "USER",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "USER"
            });
            context.Roles.Add(new IdentityRole
            {
                Id = "Rol2",
                Name = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedName = "ADMIN"
            });
            context.UserRoles.Add(new IdentityUserRole<string>
            {
                UserId = "Usuario1",
                RoleId = "Rol1"
            });
            context.UserRoles.Add(new IdentityUserRole<string>
            {
                UserId = "Usuario1",
                RoleId = "Rol2"
            });
            context.UserClaims.Add(new IdentityUserClaim<string>
            {

                UserId = "Usuario1",
                ClaimType = ClaimTypes.Role,
                ClaimValue = "USER",
                Id = 1
            });
            context.UserClaims.Add(new IdentityUserClaim<string>
            {

                UserId = "Usuario1",
                ClaimType = ClaimTypes.Role,
                ClaimValue = "Admin",
                Id = 2
            });

            await context.SaveChangesAsync();

            var controller = ConstruirRolesController(nombreBD);

            var resultado = await controller.RemoveAdmin("Usuario1");

            var respuesta = resultado as OkResult;
            var context2 = ConstruirContext(nombreBD);
            var UserRoles = await context2.UserRoles.ToListAsync();
            var UserClaims = await context2.UserClaims.ToListAsync();

            respuesta.Should().NotBeNull();
            UserRoles.Count.Should().Be(1);
            UserClaims.Count.Should().Be(1);
            respuesta.StatusCode.Should().Be(200);
        }
        private RolesController ConstruirRolesController(string nombreBD)
        {
            var context = ConstruirContext(nombreBD);
            var miUserStore = new UserStore<NewIdentityUser>(context);
            var miRoleStore = new RoleStore<IdentityRole>(context);
            var userManager = BuildUserManager(miUserStore);
            var RoleManager = BuildRoleManager(miRoleStore);
            var mapper = ConfigurarAutoMapper();

            return new RolesController(RoleManager,userManager,mapper);
        }
        
    }
}
