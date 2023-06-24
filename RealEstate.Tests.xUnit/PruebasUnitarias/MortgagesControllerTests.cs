using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using RealEstate.Controllers;
using RealEstate.DTO_s.MortgagesDTO_s;
using RealEstate.Models;
using RealEstate.Utilities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using AutoMapper;
using FluentAssertions;

namespace RealEstate.Tests.xUnit
{
    public class MortgagesControllerTests : BasePruebas
    {
        private IMapper mapper;
        private Mock<IGetUserInfo> mock;
        public MortgagesControllerTests()
        {
            mapper = ConfigurarAutoMapper();
            mock = new Mock<IGetUserInfo>();
            mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));
        }
        [Fact]
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

            respuesta.Should().NotBeNull();
            respuesta.Count.Should().Be(2);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
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

            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
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

            respuesta.Should().NotBeNull();
            respuesta.TotalValue.Should().Be(100000);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
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

            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
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

            respuesta.Should().NotBeNull();
            SeCreaMortgage.Should().BeTrue();
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
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

            respuesta.Should().NotBeNull();
            respuesta.StatusCode.Should().Be(400);
            Mortgages.Should().Be(1);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
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

            respuesta.Should().NotBeNull();
            respuesta.StatusCode.Should().Be(400);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
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

            respuesta.Should().NotBeNull();
            respuesta.StatusCode.Should().Be(200);
            Mortgages.Should().BeFalse();
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
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

            respuesta.Should().NotBeNull();
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
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

            respuesta.Should().NotBeNull();
            MortgageActualizada.TotalValue.Should().Be(100000);
            MortgageActualizada.FeeValue.Should().Be(1500);
            mock.Invocations.Count.Should().Be(1);
        }
    }
}
