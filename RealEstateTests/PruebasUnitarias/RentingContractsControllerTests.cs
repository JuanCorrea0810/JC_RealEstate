using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Moq;
using PeliculasAPI.Tests;
using RealEstate.Controllers;
using RealEstate.DTO_s.RentersDTO_s;
using RealEstate.DTO_s.RentingContractsDTO_s;
using RealEstate.Models;
using RealEstate.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace RealEstateTests.PruebasUnitarias
{
    [TestClass]
    public class RentingContractsControllerTests : BasePruebas
    {
        private IMapper mapper;
        private Mock<IGetUserInfo> mock;

        [TestInitialize]
        public Task Inicializar()
        {
            mapper = ConfigurarAutoMapper();
            mock = new Mock<IGetUserInfo>();
            mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));

            return Task.CompletedTask;
        }
        [TestMethod]
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
            Assert.IsNotNull(respuesta);
            Assert.AreEqual(2, respuesta.Count);
            Assert.AreEqual(1, mock.Invocations.Count);
        }
        [TestMethod]
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
            Assert.IsNotNull(codigo);
            Assert.AreEqual(404, codigo.StatusCode);
            Assert.AreEqual(1, mock.Invocations.Count);
        }
        [TestMethod]
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
            Assert.IsNotNull(respuesta);
            Assert.AreEqual(2, respuesta.Count);
            Assert.AreEqual(1, mock.Invocations.Count);
        }
        [TestMethod]
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
            Assert.IsNotNull(respuesta);
            Assert.AreEqual(1200, respuesta.Value);
            Assert.AreEqual(1, mock.Invocations.Count);
        }
        [TestMethod]
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
            Assert.IsNotNull(codigo);
            Assert.AreEqual(404, codigo.StatusCode);
            Assert.AreEqual(1, mock.Invocations.Count);
        }
        [TestMethod]
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
            Assert.IsNotNull(codigo);
            Assert.AreEqual(404, codigo.StatusCode);
            Assert.AreEqual(1, mock.Invocations.Count);
        }
        [TestMethod]
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
            Assert.IsNotNull(respuesta);
            Assert.AreEqual(400, respuesta.StatusCode);
            Assert.AreEqual(1, mock.Invocations.Count);
        }
        [TestMethod]
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
            Assert.IsNotNull(respuesta);
            Assert.AreEqual(1,Contrato);
            Assert.AreEqual(1, mock.Invocations.Count);
        }
        [TestMethod]
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
            Assert.IsNotNull(respuesta);
            Assert.IsFalse(ExisteContratos);
            Assert.AreEqual(1, mock.Invocations.Count);
        }
        [TestMethod]
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
            Assert.IsNotNull(respuesta);
            Assert.AreNotEqual(100, ContractActualizado.Value);
            Assert.AreEqual(Fecha, ContractActualizado.Date);
            Assert.AreEqual(1, mock.Invocations.Count);

        }
    }
}
