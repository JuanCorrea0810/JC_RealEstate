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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace RealEstateTests.PruebasUnitarias
{
    [TestClass]
    public class RolesControllerTests: BasePruebas
    {
        [TestMethod]
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
            Assert.IsNotNull(respuesta);
            Assert.AreEqual(3,respuesta.Count);

        }
        [TestMethod]
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
            Assert.IsNotNull(respuesta);
            Assert.AreEqual(400, respuesta.StatusCode);

        }
        [TestMethod]
        public async Task SeCreaNuevoRole()
        {
            var nombreBD = Guid.NewGuid().ToString();
          
            var controller = ConstruirRolesController(nombreBD);
            

            var resultado = await controller.CreateRole("User");

            var respuesta = resultado as NoContentResult;
            var context = ConstruirContext(nombreBD);
            var SeCreaRol = await context.Roles.AnyAsync();
            Assert.IsNotNull(respuesta);
            Assert.IsTrue(SeCreaRol);
            Assert.AreEqual(204, respuesta.StatusCode);

        }
        [TestMethod]
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
            Assert.IsNotNull(respuesta);
            Assert.IsFalse(ExisteRol);
            Assert.AreEqual(204, respuesta.StatusCode);

        }
        [TestMethod]
        public async Task NoSePuedeBorrarRolQueNoExiste()
        {
            var nombreBD = Guid.NewGuid().ToString();
            
            var controller = ConstruirRolesController(nombreBD);


            var resultado = await controller.DeleteRole("User");

            var respuesta = resultado as NotFoundResult;
            Assert.IsNotNull(respuesta);
            Assert.AreEqual(404, respuesta.StatusCode);

        }
        [TestMethod]
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
            Assert.IsNotNull(respuesta);
            Assert.IsNotNull(SeRelacionaUsuarioYRol);
            Assert.IsTrue(SeAgreganClaims);
            Assert.AreEqual(200, respuesta.StatusCode);

        }
        [TestMethod]
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
            
            Assert.IsNotNull(respuesta);
            Assert.AreEqual(404, respuesta.StatusCode);
        }
        [TestMethod]
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

            Assert.IsNotNull(respuesta);
            Assert.AreEqual(404, respuesta.StatusCode);
        }
        [TestMethod]
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
            Assert.IsNotNull(respuesta);
            Assert.IsFalse(ExisteRelacionUsuarioYRol);
            Assert.IsFalse(ExisteClaimsDelUsuario);
            Assert.AreEqual(200, respuesta.StatusCode);

        }
        [TestMethod]
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
            Assert.IsNotNull(respuesta);
            Assert.AreEqual(2,UserRoles.Count);
            Assert.AreEqual(2,UserClaims.Count);
            Assert.AreEqual(200, respuesta.StatusCode);

        }
        [TestMethod]
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
            
            Assert.IsNotNull(respuesta);
    
            Assert.AreEqual(400, respuesta.StatusCode);

        }
        [TestMethod]
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
            Assert.IsNotNull(respuesta);
            Assert.IsFalse(UserRoles);
            Assert.IsFalse(UserClaims);
            Assert.AreEqual(200, respuesta.StatusCode);
        }

        [TestMethod]
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
            Assert.IsNotNull(respuesta);
            Assert.AreEqual(2, UserRoles.Count);
            Assert.AreEqual(2, UserClaims.Count);
            Assert.AreEqual(200, respuesta.StatusCode);

        }
        [TestMethod]
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
            Assert.IsNotNull(respuesta);
            Assert.AreEqual(1, UserRoles.Count);
            Assert.AreEqual(1, UserClaims.Count);
            Assert.AreEqual(200, respuesta.StatusCode);

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
