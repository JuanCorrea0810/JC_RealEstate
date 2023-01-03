using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using PeliculasAPI.Tests;
using RealEstate.Controllers;
using RealEstate.DTO_s.MortgagesDTO_s;
using RealEstate.DTO_s.RentersDTO_s;
using RealEstate.Models;
using RealEstate.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using AutoMapper;

namespace RealEstateTests.PruebasUnitarias
{
    [TestClass]
    public class MortgagesControllerTests : BasePruebas
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
        public async Task DevuelveLosMortgagesDelUsuario()
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
            context.Mortgages.Add(new Mortgage
            {
                IdMortgage = 1,
                FeesNumber = 10,
                FeeValue = 10000,
                IdUser = "Usuario1",
                IdEstate = 1,
                TotalValue = 100000
            });
            context.Mortgages.Add(new Mortgage
            {
                IdMortgage = 2,
                FeesNumber = 20,
                FeeValue = 5000,
                IdUser = "Usuario1",
                IdEstate = 2,
                TotalValue = 100000
            });


            await context.SaveChangesAsync();

          
            var context2 = ConstruirContext(nombreDB);
            var controller = new MortgagesController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetUserAndTheirMortgages();

            //Verificación
            var respuesta = resultado.Value;
            Assert.IsNotNull(respuesta);
            Assert.AreEqual(2, respuesta.Count);
            Assert.AreEqual(1, mock.Invocations.Count);

        }
        [TestMethod]
        public async Task DevuelveNotFoundSiNoSeHaRegistradoNingunaHipoteca()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            
            var controller = new MortgagesController(context, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetUserAndTheirMortgages();

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;
            Assert.IsNotNull(codigo);
            Assert.AreEqual(404, codigo.StatusCode);
            Assert.AreEqual(1, mock.Invocations.Count);

        }
        [TestMethod]
        public async Task DevuelveLaHipotecaDeLaPropiedad()
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
            context.Mortgages.Add(new Mortgage
            {
                IdMortgage = 1,
                FeesNumber = 10,
                FeeValue = 10000,
                IdUser = "Usuario1",
                IdEstate = 1,
                TotalValue = 100000
            });

            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreDB);
            var controller = new MortgagesController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.Get(1);

            //Verificación
            var respuesta = resultado.Value;
            Assert.IsNotNull(respuesta);
            Assert.AreEqual(100000, respuesta.TotalValue);
            Assert.AreEqual(1, mock.Invocations.Count);

        }
        [TestMethod]
        public async Task DevuelveNotFoundSiLaPropiedadNoTieneHipoteca()
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
            var controller = new MortgagesController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.Get(1);

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;
            Assert.IsNotNull(codigo);
            Assert.AreEqual(404, codigo.StatusCode);
            Assert.AreEqual(1, mock.Invocations.Count);

        }
        [TestMethod]
        public async Task SeCreaNuevaHipoteca()
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
            var controller = new MortgagesController(context2, mapper, mock.Object);
            var dto = new PostMortgagesDTO()
            {
                FeesNumber = 10,
                FeeValue = 10000,
                TotalValue = 100000
            };

            //Prueba
            var resultado = await controller.Post(dto, 1);

            //Verificación
            var respuesta = resultado as CreatedAtRouteResult;
            var context3 = ConstruirContext(nombreDB);
            var SeCreaMortgage = await context3.Mortgages.AnyAsync();
            Assert.IsNotNull(respuesta);
            Assert.IsTrue(SeCreaMortgage);
            Assert.AreEqual(1, mock.Invocations.Count);

        }
        [TestMethod]
        public async Task NoSePuedeCrearDosHipotecasParaLaMismaPropiedad()
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
            context.Mortgages.Add(new Mortgage
            {
                IdMortgage = 1,
                FeesNumber = 10,
                FeeValue = 10000,
                IdUser = "Usuario1",
                IdEstate = 1,
                TotalValue = 100000
            });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreDB);
            var controller = new MortgagesController(context2, mapper, mock.Object);
            var dto = new PostMortgagesDTO()
            {
                FeesNumber = 10,
                FeeValue = 10000,
                TotalValue = 100000
            };

            //Prueba
            var resultado = await controller.Post(dto, 1);

            //Verificación
            var respuesta = resultado as BadRequestObjectResult;
            var context3 = ConstruirContext(nombreDB);
            var Mortgages = await context3.Mortgages.CountAsync();
            Assert.IsNotNull(respuesta);
            Assert.AreEqual(1, Mortgages);
            Assert.AreEqual(400, respuesta.StatusCode);
            Assert.AreEqual(1, mock.Invocations.Count);

        }
        [TestMethod]
        public async Task NoSeBorraMortgageQueNoExiste()
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
            var controller = new MortgagesController(context2, mapper, mock.Object);


            //Prueba
            var resultado = await controller.Delete(1);

            //Verificación
            var respuesta = resultado as BadRequestObjectResult;

            Assert.IsNotNull(respuesta);
            Assert.AreEqual(400, respuesta.StatusCode);
            Assert.AreEqual(1, mock.Invocations.Count);

        }
        [TestMethod]
        public async Task SeBorraMortgage()
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
            context.Mortgages.Add(new Mortgage
            {
                IdMortgage = 1,
                FeesNumber = 10,
                FeeValue = 10000,
                IdUser = "Usuario1",
                IdEstate = 1,
                TotalValue = 100000
            });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreDB);
            var controller = new MortgagesController(context2, mapper, mock.Object);


            //Prueba
            var resultado = await controller.Delete(1);

            //Verificación
            var respuesta = resultado as OkObjectResult;
            var context3 = ConstruirContext(nombreDB);
            var Mortgages = await context3.Mortgages.AnyAsync();
            Assert.IsNotNull(respuesta);
            Assert.IsFalse(Mortgages);
            Assert.AreEqual(200, respuesta.StatusCode);
            Assert.AreEqual(1, mock.Invocations.Count);
        }
        [TestMethod]
        public async Task NoSePuedeActualizarPatchAMortgageQueNoExiste()
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
            
            var controller = new MortgagesController(context2, mapper, mock.Object);
            var jsonPatch = new JsonPatchDocument<PatchMortgagesDTO>();
            jsonPatch.Operations.Add(new Operation<PatchMortgagesDTO>("replace", "/FeeValue", null, 1500));
            //Prueba
            var resultado = await controller.Patch(jsonPatch, 1);

            //Verificación
            var respuesta = resultado as NotFoundObjectResult;
            Assert.IsNotNull(respuesta);
            Assert.AreEqual(1, mock.Invocations.Count);

        }
        [TestMethod]
        public async Task SeActualizarPatchSoloUnCampo()
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
            context.Mortgages.Add(new Mortgage
            {
                IdMortgage = 1,
                FeesNumber = 10,
                FeeValue = 10000,
                IdUser = "Usuario1",
                IdEstate = 1,
                TotalValue = 100000
            });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreDB);
            
            var controller = new MortgagesController(context2, mapper, mock.Object);
            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(x => x.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<object>()));

            controller.ObjectValidator = objectValidator.Object;
            var jsonPatch = new JsonPatchDocument<PatchMortgagesDTO>();
            jsonPatch.Operations.Add(new Operation<PatchMortgagesDTO>("replace", "/FeeValue", null, 1500));
            //Prueba
            var resultado = await controller.Patch(jsonPatch, 1);

            //Verificación
            var respuesta = resultado as NoContentResult;
            var context3 = ConstruirContext(nombreDB);
            var MortgageActualizada = await context3.Mortgages.FirstAsync();
            Assert.IsNotNull(respuesta);
            Assert.AreEqual(100000, MortgageActualizada.TotalValue);
            Assert.AreEqual(1500, MortgageActualizada.FeeValue);
            Assert.AreEqual(1, mock.Invocations.Count);

        }
    }
}
