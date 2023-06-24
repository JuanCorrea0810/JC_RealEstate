using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Moq;
using RealEstate.Controllers;
using RealEstate.DTO_s.BuyersDTO_s;
using RealEstate.Models;
using RealEstate.Utilities;

namespace RealEstate.Tests.xUnit
{
    public class BuyersControllersTests : BasePruebas
    {
        private IMapper mapper;
        private Mock<IGetUserInfo> mock;
        public BuyersControllersTests() 
        {
            mapper = ConfigurarAutoMapper();
            mock = new Mock<IGetUserInfo>();
            mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));
        }
        [Fact]
        public async Task DevuelveLosCompradoresDelUsuario()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            

            context.Estates.Add(new Estate
            {
                IdUser = "Usuario1",
                Address = "dmkasmdkasnmdjqndjew",
                Alias = "Casa 1",
                City = "Bogotá"
            ,
                Country = "Colombia",
                IdEstate = 1,
                KmsGround = 1500,
                Rooms = 12,
                Rented = false,
                Sold = false
            });

            context.Estates.Add(new Estate
            {
                IdUser = "Usuario1",
                Address = ",mdueausnyqbdy",
                Alias = "Casa 2",
                City = "Medellín"
            ,
                Country = "Colombia",
                IdEstate = 2,
                KmsGround = 1600,
                Rooms = 10,
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
            context.Buyers.Add(new Buyer
            {
                Dni = 0987654321,
                Address = "dsadasds",
                Age = 19,
                Email = "dsadsad@gmail.com",
                Country = "Colombia",
                CellPhoneNumber = 4528065891,
                FirsName = "dsads",
                FirstSurName = "dsadadsas",
                SecondName = "dsadasdasd",
                SecondSurName = "dsadsdas",
                IdBuyer = 2,
                IdEstate = 2
            });

            await context.SaveChangesAsync();

            

            var context2 = ConstruirContext(nombreDB);
            var controller = new BuyersController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetUsersAndTheirBuyers();

            //Verificación
            var respuesta = resultado.Value;
            respuesta.Should().NotBeNull();
            respuesta.Count.Should().Be(2);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task DevuelveNotFoundSiNoSeTienePropiedadesOCompradoresRegistrados()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            

            context.Estates.Add(new Estate
            {
                IdUser = "Usuario1",
                Address = "dmkasmdkasnmdjqndjew",
                Alias = "Casa 1",
                City = "Bogotá"
            ,
                Country = "Colombia",
                IdEstate = 1,
                KmsGround = 1500,
                Rooms = 12,
                Rented = false,
                Sold = false
            });

            await context.SaveChangesAsync();

            

            var context2 = ConstruirContext(nombreDB);
            var controller = new BuyersController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetUsersAndTheirBuyers();

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;
            respuesta.Should().NotBeNull();
            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task DevuelveErrorSiElCompradorNoExisteONoLePerteneceAlUsuario()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            
            context.Estates.Add(new Estate
            {
                IdUser = "Usuario1",
                Address = "dmkasmdkasnmdjqndjew",
                Alias = "Casa 1",
                City = "Bogotá"
            ,
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
            var controller = new BuyersController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetById(1);

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;
            respuesta.Should().NotBeNull();
            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task DevuelveElComprador()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
           

            context.Estates.Add(new Estate
            {
                IdUser = "Usuario1",
                Address = "dmkasmdkasnmdjqndjew",
                Alias = "Casa 1",
                City = "Bogotá"
            ,
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
            var controller = new BuyersController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetById(1);

            //Verificación
            var respuesta = resultado.Value;
            respuesta.Should().NotBeNull();
            respuesta.Dni.Should().Be(1234567890);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task DevuelveErrorSiLaPropiedadNoEsVálidaAlCrearNuevoComprador()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            
            var dto = new PostBuyersDTO()
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
                SecondSurName = "dsadsdas"
            };

            var controller = new BuyersController(context, mapper, mock.Object);

            //Prueba
            var resultado = await controller.Post(1, dto);

            //Verificación
            var respuesta = resultado as NotFoundObjectResult;

            respuesta.Should().NotBeNull();
            respuesta.StatusCode.Should().Be(404);
            mock.Invocations.Count.Should().Be(1);
        }
        [Fact]
        public async Task NoSePuedeRegistrarUnCompradorAUnaPropiedadYaComprada()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            
            context.Estates.Add(new Estate
            {
                IdUser = "Usuario1",
                Address = "dmkasmdkasnmdjqndjew",
                Alias = "Casa 1",
                City = "Bogotá"
            ,
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

            
            var dto = new PostBuyersDTO()
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
                SecondSurName = "dsadsdas"
            };
            var context2 = ConstruirContext(nombreDB);

            var controller = new BuyersController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.Post(1, dto);

            //Verificación
            var respuesta = resultado as BadRequestObjectResult;

            respuesta.Should().NotBeNull();
            respuesta.StatusCode.Should().Be(400);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task SeRegistraNuevoComprador()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            
            context.Estates.Add(new Estate
            {
                IdUser = "Usuario1",
                Address = "dmkasmdkasnmdjqndjew",
                Alias = "Casa 1",
                City = "Bogotá"
            ,
                Country = "Colombia",
                IdEstate = 1,
                KmsGround = 1500,
                Rooms = 12,
                Rented = false,
                Sold = false
            });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreDB);
            
            var dto = new PostBuyersDTO()
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
                SecondSurName = "dsadsdas"
            };

            var controller = new BuyersController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.Post(1, dto);

            //Verificación
            var respuesta = resultado as CreatedAtRouteResult;
            var context3 = ConstruirContext(nombreDB);
            var Buyer = await context3.Buyers.FirstAsync();

            respuesta.Should().NotBeNull();
            Buyer.Should().NotBeNull();
            Buyer.Dni.Should().Be(1234567890);
            mock.Invocations.Count.Should().Be(1);
        }
        [Fact]
        public async Task SeBorraElComprador()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            
            context.Estates.Add(new Estate
            {
                IdUser = "Usuario1",
                Address = "dmkasmdkasnmdjqndjew",
                Alias = "Casa 1",
                City = "Bogotá"
            ,
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
           

            var controller = new BuyersController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.Delete(1);

            //Verificación
            var respuesta = resultado as OkObjectResult;
            var context3 = ConstruirContext(nombreDB);
            var ExisteComprador = await context3.Buyers.AnyAsync();
                        
            respuesta.Should().NotBeNull();
            ExisteComprador.Should().BeFalse();
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task NoSePuedeBorrarCompradorQueNoExiste()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            
            context.Estates.Add(new Estate
            {
                IdUser = "Usuario1",
                Address = "dmkasmdkasnmdjqndjew",
                Alias = "Casa 1",
                City = "Bogotá"
            ,
                Country = "Colombia",
                IdEstate = 1,
                KmsGround = 1500,
                Rooms = 12,
                Rented = false,
                Sold = false
            });
            await context.SaveChangesAsync();


            var context2 = ConstruirContext(nombreDB);
            

            var controller = new BuyersController(context2, mapper, mock.Object);
            //Prueba
            var resultado = await controller.Delete(1);

            //Verificación
            var respuesta = resultado as NotFoundObjectResult;

            respuesta.Should().NotBeNull();
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task NoSePuedeActualizarPatchACompradorQueNoExiste()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            
            context.Estates.Add(new Estate
            {
                IdUser = "Usuario1",
                Address = "dmkasmdkasnmdjqndjew",
                Alias = "Casa 1",
                City = "Bogotá"
            ,
                Country = "Colombia",
                IdEstate = 1,
                KmsGround = 1500,
                Rooms = 12,
                Rented = false,
                Sold = false
            });
            await context.SaveChangesAsync();


            var context2 = ConstruirContext(nombreDB);
            
            var controller = new BuyersController(context2, mapper, mock.Object);
            var jsonPatch = new JsonPatchDocument<PatchBuyersDTO>();
            jsonPatch.Operations.Add(new Operation<PatchBuyersDTO>("replace", "/SecondName", null, "Juan"));
            //Prueba
            var resultado = await controller.Patch(jsonPatch, 1);

            //Verificación
            var respuesta = resultado as NotFoundObjectResult;

            respuesta.Should().NotBeNull();
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task PatchActualizaSoloUnCampo()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            
            context.Estates.Add(new Estate
            {
                IdUser = "Usuario1",
                Address = "dmkasmdkasnmdjqndjew",
                Alias = "Casa 1",
                City = "Bogotá"
            ,
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
                SecondName = "Manuel",
                SecondSurName = "dsadsdas",
                IdBuyer = 1,
                IdEstate = 1
            });
            await context.SaveChangesAsync();


            var context2 = ConstruirContext(nombreDB);
            

            var controller = new BuyersController(context2, mapper, mock.Object);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(x => x.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<object>()));

            controller.ObjectValidator = objectValidator.Object;
            var jsonPatch = new JsonPatchDocument<PatchBuyersDTO>();
            jsonPatch.Operations.Add(new Operation<PatchBuyersDTO>("replace", "/SecondName", null, "Pablo"));

            //Prueba
            var resultado = await controller.Patch(jsonPatch, 1);

            //Verificación
            var respuesta = resultado as NoContentResult;
            var context3 = ConstruirContext(nombreDB);
            var BuyerActualizado = await context3.Buyers.FirstAsync();

            respuesta.Should().NotBeNull();
            BuyerActualizado.Country.Should().Be("Colombia");
            BuyerActualizado.SecondName.Should().Be("Pablo");
            mock.Invocations.Count.Should().Be(1);
        }
    }
}
