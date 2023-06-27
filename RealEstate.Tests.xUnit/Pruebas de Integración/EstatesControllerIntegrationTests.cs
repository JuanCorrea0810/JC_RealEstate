using FluentAssertions;
using Newtonsoft.Json;
using RealEstate.DTO_s.EstatesDTO_s;
using RealEstate.Models;
using System.Net;

namespace RealEstate.Tests.xUnit.Pruebas_de_Integración
{
    public class EstatesControllerIntegrationTests: BasePruebas
    {
        private static readonly string url = "/api/Estates";
        [Fact]
        public async Task DevuelveErrorSiElUsuarioNoTienePropiedades()
        {

            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDB);
            var context = ConstruirContext(nombreDB);
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
            });
            await context.SaveChangesAsync();
            var httpClient = factory.CreateClient();
            //Prueba
            var Request = await httpClient.GetAsync(url);

            //Verificación
            Request.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        [Fact]
        public async Task DevuelveLasPropiedadesDelUsuario()
        {

            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDB);
            var context = ConstruirContext(nombreDB);
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
                Dni = 1098765430,
                Email = "juancho0810@gmail.com",
                Address = "dadasdasdasd",
                Age = 23,
                Country = "Colombia",
                FirstName = "Juan",
                SecondName = "Manuel",
                FirstSurName = "Correa",
                SecondSurName = "Rojas",
                PhoneNumber = "3774895628",
                UserName = "JuanchoCorrea0810",
                Id = "Usuario2",
                EmailConfirmed = false,
                AccessFailedCount = 0,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = true,
                LockoutEnd = null,
                NormalizedEmail = "JUANCHO0810@GMAIL.COM",
                NormalizedUserName = "JUANCHOCORREA0810",
                PasswordHash = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = false,
                SecurityStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = false
            });
            context.Estates.Add(new Estate
            {
                IdUser = "Usuario1",
                Address = "Mi Casa",
                Alias = "Casa 1",
                City = "Bogotá"
        ,
                Country = "Colombia",
                IdEstate = 1,
                KmsGround = 1500,
                Rooms = 12,
                Rented = false,
                Sold = false
            }); context.Estates.Add(new Estate
            {
                IdUser = "Usuario1",
                Address = "Su Casa",
                Alias = "Casa 2",
                City = "Medellín"
            ,
                Country = "Colombia",
                IdEstate = 2,
                KmsGround = 1500,
                Rooms = 12,
                Rented = false,
                Sold = false
            });
            context.Estates.Add(new Estate
            {
                IdUser = "Usuario2",
                Address = "Otra Casa",
                Alias = "Casa 3",
                City = "Buenos Aires"
           ,
                Country = "Argentina",
                IdEstate = 3,
                KmsGround = 1500,
                Rooms = 12,
                Rented = false,
                Sold = false
            });
            await context.SaveChangesAsync();
            var httpClient = factory.CreateClient();
            //Prueba
            var Request = await httpClient.GetAsync(url);
            var resultado = JsonConvert.DeserializeObject<List<GetEstatesDTO>>(await Request.Content.ReadAsStringAsync());
            //Verificación
            resultado.Count.Should().Be(2);
        }
        [Fact]
        public async Task DevuelveUnauthorizedAlIntentarVerTodasLasPropiedades()
        {
            //Preparar
            var nombreDB = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDB, ignorarSeguridad: false);
            var context = ConstruirContext(nombreDB);
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
            var httpClient = factory.CreateClient();
            //Prueba
            var Request = await httpClient.GetAsync($"{url}/ListOfEstates");

            //Verificación
            Request.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        [Fact]
        public async Task DevuelveErrorSiElPaisNoEsValido() 
        {
            //Preparar
            var nombreDB = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDB);
            var context = ConstruirContext(nombreDB);
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
            var dto = new PostEstatesDTO() { 
            Address = "Ejemplo",
            Alias = "Ejemplo",
            City = "Ejemplo",
            Country = "Ejemplo",
            KmsGround = 1200,
            Rented = true,  
            Rooms = 2,
            Sold = false
            };
            
            var httpClient = factory.CreateClient();
            //Prueba
            var Request = await httpClient.PostAsJsonAsync(url,dto);
            //Verificar
            Request.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
