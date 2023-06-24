using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Moq;
using RealEstate.Controllers;
using RealEstate.DTO_s.RentingContractsDTO_s;
using RealEstate.Models;
using RealEstate.Utilities;
using AutoMapper;
using FluentAssertions;

namespace RealEstate.Tests.xUnit
{
    public class RentingContractsControllerTests : BasePruebas
    {
        private IMapper mapper;
        private Mock<IGetUserInfo> mock;
        public RentingContractsControllerTests()
        {
            mapper = ConfigurarAutoMapper();
            mock = new Mock<IGetUserInfo>();
            mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));
        }
        [Fact]
        public async Task DevuelveLosRentingContractsDelUsuario()
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
            context.Renters.Add(new Renter
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
                IdRenter = 2,
                IdEstate = 1
            });
            context.Rentingcontracts.Add(new Rentingcontract
            {
                IdRentingContract = 1,
                IdRenter = 1,
                IdEst = 1,
                Value = 1200,
                Date = DateTime.Now
            });
            context.Rentingcontracts.Add(new Rentingcontract
            {
                IdRentingContract = 2,
                IdRenter = 2,
                IdEst = 1,
                Value = 1500,
                Date = DateTime.Now
            }); 
            context.Rentingcontracts.Add(new Rentingcontract
            {
                IdRentingContract = 3,
                IdRenter = 3,
                IdEst = 3,
                Value = 2200,
                Date = DateTime.Now
            });

            await context.SaveChangesAsync();

           
            var context2 = ConstruirContext(nombreDB);
            var controller = new RentingContractsController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetUsersAndTheirRentingContracts();

            //Verificación
            var respuesta = resultado.Value;

            respuesta.Should().NotBeNull();
            respuesta.Count.Should().Be(2);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task DeuelveNotFoundSiElUsuarioNoTieneContratosDeArrendamiento()
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
            var controller = new RentingContractsController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetUsersAndTheirRentingContracts();

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;

            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task DevuelveLosRentingContractsDeUnArrendatarioEspecifico()
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
            context.Rentingcontracts.Add(new Rentingcontract
            {
                IdRentingContract = 1,
                IdRenter = 1,
                IdEst = 1,
                Value = 1200,
                Date = DateTime.Now
            });
            context.Rentingcontracts.Add(new Rentingcontract
            {
                IdRentingContract = 2,
                IdRenter = 2,
                IdEst = 1,
                Value = 1500,
                Date = DateTime.Now
            });
            context.Rentingcontracts.Add(new Rentingcontract
            {
                IdRentingContract = 3,
                IdRenter = 1,
                IdEst = 1,
                Value = 2200,
                Date = DateTime.Now
            });

            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreDB);
            var controller = new RentingContractsController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.Get(1,1);

            //Verificación
            var respuesta = resultado.Value;

            respuesta.Should().NotBeNull();
            respuesta.Count.Should().Be(2);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task DevuelveUnContratoEspecifico()
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
            context.Rentingcontracts.Add(new Rentingcontract
            {
                IdRentingContract = 1,
                IdRenter = 1,
                IdEst = 1,
                Value = 1200,
                Date = DateTime.Now
            });
            
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreDB);
            var controller = new RentingContractsController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetById(1, 1,1);

            //Verificación
            var respuesta = resultado.Value;

            respuesta.Should().NotBeNull();
            respuesta.Value.Should().Be(1200);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task DevuelveErrorSiContratoNoExiste()
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
            var controller = new RentingContractsController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetById(1, 1, 1);

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;

            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task DevuelveErrorSiContratoNoCoincideConElRenterOPropiedad()
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
            context.Rentingcontracts.Add(new Rentingcontract
            {
                IdRentingContract = 1,
                IdRenter = 5,
                IdEst = 3,
                Value = 1200,
                Date = DateTime.Now
            });

            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreDB);
            var controller = new RentingContractsController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetById(1, 1, 1);

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;

            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task NoSePuedeCrearUnContratoAUnRenterInactivo()
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
                IdEstate = 1,
                Active = false
            });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreDB);
            var controller = new RentingContractsController(context2, mapper, mock.Object);
            var dto = new PostRentingContractsDTO() 
            {
            Date = DateTime.Now,
            Value = 1300
            };

            //Prueba
            var resultado = await controller.Post(dto,1, 1);

            //Verificación
            var respuesta = resultado as BadRequestObjectResult;

            respuesta.Should().NotBeNull();
            respuesta.StatusCode.Should().Be(400);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task SeCreaNuevoContratoDeArrendamiento()
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
                IdEstate = 1,
                Active = true
            });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreDB);
            var controller = new RentingContractsController(context2, mapper, mock.Object);
            var dto = new PostRentingContractsDTO()
            {
                Date = DateTime.Now,
                Value = 1300
            };

            //Prueba
            var resultado = await controller.Post(dto, 1, 1);

            //Verificación
            var respuesta = resultado as CreatedAtRouteResult;
            var context3 = ConstruirContext(nombreDB);
            var Contrato = await context3.Rentingcontracts.CountAsync();

            respuesta.Should().NotBeNull();
            Contrato.Should().Be(1);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task SeBorraContrato()
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
            context.Rentingcontracts.Add(new Rentingcontract
            {
                IdRentingContract = 1,
                IdRenter = 1,
                IdEst = 1,
                Value = 1200,
                Date = DateTime.Now
            });

            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreDB);
            var controller = new RentingContractsController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.Delete(1, 1, 1);

            //Verificación
            var respuesta = resultado as OkObjectResult;
            var context3 = ConstruirContext(nombreDB);
            var ExisteContratos = await context3.Rentingcontracts.AnyAsync();

            respuesta.Should().NotBeNull();
            ExisteContratos.Should().BeFalse();
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
                SecondName = "Manuel",
                SecondSurName = "dsadsdas",
                IdRenter = 1,
                IdEstate = 1
            });
            var Fecha = DateTime.Now;
            context.Rentingcontracts.Add(new Rentingcontract
            {
                IdRentingContract = 1,
                IdRenter = 1,
                IdEst = 1,
                Value = 1200,
                Date = Fecha
            });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreDB);
 
            var controller = new RentingContractsController(context2, mapper, mock.Object);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(x => x.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<object>()));

            controller.ObjectValidator = objectValidator.Object;
            var jsonPatch = new JsonPatchDocument<PatchRentingContractsDTO>();
            jsonPatch.Operations.Add(new Operation<PatchRentingContractsDTO>("replace", "/Value", null, 666));

            //Prueba
            var resultado = await controller.Patch(jsonPatch, 1, 1,1);

            //Verificación
            var respuesta = resultado as NoContentResult;
            var context3 = ConstruirContext(nombreDB);
            var ContractActualizado = await context3.Rentingcontracts.FirstAsync();

            respuesta.Should().NotBeNull();
            ContractActualizado.Value.Should().NotBe(100);
            ContractActualizado.Date.Should().Be(Fecha);
            mock.Invocations.Count.Should().Be(1);
        }
    }
}
