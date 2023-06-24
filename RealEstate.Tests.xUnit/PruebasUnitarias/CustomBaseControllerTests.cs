using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Models;

namespace RealEstate.Tests.xUnit
{
    public class CustomBaseControllerTests:BasePruebas
    {
        private IMapper mapper;
        public CustomBaseControllerTests()
        {
            mapper = ConfigurarAutoMapper();
        }
        [Fact]
        public async Task DevuelvePropiedad() 
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            
            context.Estates.Add(new Estate
            {
                IdUser = "Usuario1",
                Address = "dmkasmdkasnmdjqndjew",
                Alias = "Casa 1",
                City = "Bogotá",
                Country = "Colombia",
                IdEstate = 1,
                KmsGround = 1500,
                Rooms = 12,
                Rented = false,
                Sold = false
            });
            await context.SaveChangesAsync();
            var context2 = ConstruirContext(nombreDB);

            var controller = new CustomBaseControllerParaPruebas(context2, mapper);
            //Prueba
            var resultado = await controller.DevolverPropiedad("Usuario1", 1);

            //Verificación
            var respuesta = resultado.Value;

            respuesta.Should().NotBeNull();
            respuesta.Alias.Should().Be("Casa 1");
        }

        [Fact]
        public async Task DevuelveNotFoundSiPropiedadNoExiste_MetodoDevuelvePropiedad()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            
            await context.SaveChangesAsync();

            var controller = new CustomBaseControllerParaPruebas(context, mapper);
            //Prueba
            var resultado = await controller.DevolverPropiedad("Usuario1", 1);

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;

            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task DevuelveNotFoundSiPropiedadNoExiste_MetodoSaberSiExistePropiedad()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            
            await context.SaveChangesAsync();

            var controller = new CustomBaseControllerParaPruebas(context, mapper);
            //Prueba
            var resultado = await controller.SaberSiExistePropiedad("Usuario1", 1);

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;

            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
        }
        [Fact]
        public async Task DevuelveTrueSiPropiedadExiste()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            
            context.Estates.Add(new Estate
            {
                IdUser = "Usuario1",
                Address = "dmkasmdkasnmdjqndjew",
                Alias = "Casa 1",
                City = "Bogotá",
                Country = "Colombia",
                IdEstate = 1,
                KmsGround = 1500,
                Rooms = 12,
                Rented = false,
                Sold = false
            });
            await context.SaveChangesAsync();
            var context2 = ConstruirContext(nombreDB);

            var controller = new CustomBaseControllerParaPruebas(context2, mapper);
            //Prueba
            var resultado = await controller.SaberSiExistePropiedad("Usuario1", 1);

            //Verificación
            var respuesta = resultado.Value;

            respuesta.Should().BeTrue();
        }

        [Fact]
        public async Task DevuelveTrueSiRenterExiste()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            
            context.Renters.Add(new Renter
            {
                Dni = 1234567890,
                Address = "dsadasds",
                Age = 19,
                Email = "dsadsad@gmail.com",
                Country = "Colombia",
                CellPhoneNumber = 4528065891,
                FirsName = "dsads",
                FirstSurName = "dsadadsas",
                SecondName = "dsadasdasd",
                SecondSurName = "dsadsdas",
                IdRenter = 1,
                IdEstate = 1
            });
            await context.SaveChangesAsync();
            var context2 = ConstruirContext(nombreDB);

            var controller = new CustomBaseControllerParaPruebas(context2, mapper);
            //Prueba
            var resultado = await controller.SaberSiExisteRenter(1);

            //Verificación
            var respuesta = resultado.Value;

            respuesta.Should().BeTrue();
        }

        [Fact]
        public async Task DevuelveNotFoundSiRenterNoExiste()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            
            await context.SaveChangesAsync();

            var controller = new CustomBaseControllerParaPruebas(context, mapper);
            //Prueba
            var resultado = await controller.SaberSiExisteRenter(1);

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;

            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task DevuelveTrueSiGuarantorExiste()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
           
            context.Guarantors.Add(new Guarantor
            {
                Dni = 1234567890,
                Address = "dsadasds",
                Age = 19,
                Email = "dsadsad@gmail.com",
                Country = "Colombia",
                CellPhoneNumber = 4528065891,
                FirsName = "dsads",
                FirstSurName = "dsadadsas",
                SecondName = "dsadasdasd",
                SecondSurName = "dsadsdas",
                IdRenter = 1,
                IdGuarantor = 1
            });
            await context.SaveChangesAsync();
            var context2 = ConstruirContext(nombreDB);

            var controller = new CustomBaseControllerParaPruebas(context2, mapper);
            //Prueba
            var resultado = await controller.SaberSiExisteGuarantor(1);

            //Verificación
            var respuesta = resultado.Value;

            respuesta.Should().BeTrue();
        }

        [Fact]
        public async Task DevuelveNotFoundSiGuarantorNoExiste()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            
            await context.SaveChangesAsync();

            var controller = new CustomBaseControllerParaPruebas(context, mapper);
            //Prueba
            var resultado = await controller.SaberSiExisteGuarantor(1);

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;

            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task DevuelveTrueSiPropiedadYRenterCoinciden()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            
            context.Estates.Add(new Estate
            {
                IdUser = "Usuario1",
                Address = "dmkasmdkasnmdjqndjew",
                Alias = "Casa 1",
                City = "Bogotá",
                Country = "Colombia",
                IdEstate = 1,
                KmsGround = 1500,
                Rooms = 12,
                Rented = false,
                Sold = false
            });
            context.Renters.Add(new Renter
            {
                Dni = 1234567890,
                Address = "dsadasds",
                Age = 19,
                Email = "dsadsad@gmail.com",
                Country = "Colombia",
                CellPhoneNumber = 4528065891,
                FirsName = "dsads",
                FirstSurName = "dsadadsas",
                SecondName = "dsadasdasd",
                SecondSurName = "dsadsdas",
                IdRenter = 1,
                IdEstate = 1
            });
            await context.SaveChangesAsync();
            var context2 = ConstruirContext(nombreDB);

            var controller = new CustomBaseControllerParaPruebas(context2, mapper);
            //Prueba
            var resultado = await controller.SaberSiHayRelacionEntreRenterYEstate(1,1);

            //Verificación
            var respuesta = resultado.Value;

            respuesta.Should().BeTrue();
        }

        [Fact]
        public async Task DevuelveNotFoundSiPropiedadYRenterNoCoinciden()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            
            context.Estates.Add(new Estate
            {
                IdUser = "Usuario1",
                Address = "dmkasmdkasnmdjqndjew",
                Alias = "Casa 1",
                City = "Bogotá",
                Country = "Colombia",
                IdEstate = 1,
                KmsGround = 1500,
                Rooms = 12,
                Rented = false,
                Sold = false
            });
            context.Renters.Add(new Renter
            {
                Dni = 1234567890,
                Address = "dsadasds",
                Age = 19,
                Email = "dsadsad@gmail.com",
                Country = "Colombia",
                CellPhoneNumber = 4528065891,
                FirsName = "dsads",
                FirstSurName = "dsadadsas",
                SecondName = "dsadasdasd",
                SecondSurName = "dsadsdas",
                IdRenter = 1,
                IdEstate = 2
            });
            await context.SaveChangesAsync();
            var context2 = ConstruirContext(nombreDB);

            var controller = new CustomBaseControllerParaPruebas(context2, mapper);
            //Prueba
            var resultado = await controller.SaberSiHayRelacionEntreRenterYEstate(1, 1);

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;

            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task DevuelveTrueSiBuyerExiste()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            
            context.Buyers.Add(new Buyer
            {
                Dni = 1234567890,
                Address = "dsadasds",
                Age = 19,
                Email = "dsadsad@gmail.com",
                Country = "Colombia",
                CellPhoneNumber = 4528065891,
                FirsName = "dsads",
                FirstSurName = "dsadadsas",
                SecondName = "dsadasdasd",
                SecondSurName = "dsadsdas",
                IdBuyer = 1,
                IdEstate = 1
            });
            await context.SaveChangesAsync();
            var context2 = ConstruirContext(nombreDB);

            var controller = new CustomBaseControllerParaPruebas(context2, mapper);
            //Prueba
            var resultado = await controller.SaberSiExisteBuyer(1);

            //Verificación
            var respuesta = resultado.Value;

            respuesta.Should().BeTrue();
        }

        [Fact]
        public async Task DevuelveNotFoundSiBuyerNoExiste()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            
            await context.SaveChangesAsync();

            var controller = new CustomBaseControllerParaPruebas(context, mapper);
            //Prueba
            var resultado = await controller.SaberSiExisteBuyer(1);

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;

            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task DevuelveTrueSiPropiedadYBuyerCoinciden()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            
            context.Estates.Add(new Estate
            {
                IdUser = "Usuario1",
                Address = "dmkasmdkasnmdjqndjew",
                Alias = "Casa 1",
                City = "Bogotá",
                Country = "Colombia",
                IdEstate = 1,
                KmsGround = 1500,
                Rooms = 12,
                Rented = false,
                Sold = false
            });
            context.Buyers.Add(new Buyer
            {
                Dni = 1234567890,
                Address = "dsadasds",
                Age = 19,
                Email = "dsadsad@gmail.com",
                Country = "Colombia",
                CellPhoneNumber = 4528065891,
                FirsName = "dsads",
                FirstSurName = "dsadadsas",
                SecondName = "dsadasdasd",
                SecondSurName = "dsadsdas",
                IdBuyer = 1,
                IdEstate = 1
            });
            await context.SaveChangesAsync();
            var context2 = ConstruirContext(nombreDB);

            var controller = new CustomBaseControllerParaPruebas(context2, mapper);
            //Prueba
            var resultado = await controller.SaberSiHayRelacionEntreBuyerYPropiedad(1, 1);

            //Verificación
            var respuesta = resultado.Value;

            respuesta.Should().BeTrue();
        }

        [Fact]
        public async Task DevuelveNotFoundSiPropiedadYBuyerNoCoinciden()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            
            context.Estates.Add(new Estate
            {
                IdUser = "Usuario1",
                Address = "dmkasmdkasnmdjqndjew",
                Alias = "Casa 1",
                City = "Bogotá",
                Country = "Colombia",
                IdEstate = 1,
                KmsGround = 1500,
                Rooms = 12,
                Rented = false,
                Sold = false
            });
            context.Buyers.Add(new Buyer
            {
                Dni = 1234567890,
                Address = "dsadasds",
                Age = 19,
                Email = "dsadsad@gmail.com",
                Country = "Colombia",
                CellPhoneNumber = 4528065891,
                FirsName = "dsads",
                FirstSurName = "dsadadsas",
                SecondName = "dsadasdasd",
                SecondSurName = "dsadsdas",
                IdBuyer = 1,
                IdEstate = 2
            });
            await context.SaveChangesAsync();
            var context2 = ConstruirContext(nombreDB);

            var controller = new CustomBaseControllerParaPruebas(context2, mapper);
            //Prueba
            var resultado = await controller.SaberSiHayRelacionEntreBuyerYPropiedad(1, 1);

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;

            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
        }
    }
}
