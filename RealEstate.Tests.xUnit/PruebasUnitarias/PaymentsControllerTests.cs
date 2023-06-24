using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Moq;
using RealEstate.Controllers;
using RealEstate.DTO_s.PaymentsDTO_s;
using RealEstate.Models;
using RealEstate.Utilities;
using AutoMapper;
using FluentAssertions;

namespace RealEstate.Tests.xUnit
{
    public class PaymentsControllerTests : BasePruebas
    {
        private IMapper mapper;
        private Mock<IGetUserInfo> mock;
        public PaymentsControllerTests()
        {
            mapper = ConfigurarAutoMapper();
            mock = new Mock<IGetUserInfo>();
            mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));
        }

        [Fact]
        public async Task DevuelveLosPaymentsDelUsuario()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            
            context.Mortgages.Add(new Mortgage
            {
                IdMortgage = 1,
                FeesNumber = 10,
                FeeValue = 10000,
                IdUser = "Usuario1",
                IdEstate = 1,
                TotalValue = 100000
            });
            context.Payments.Add(new Payment
            {
                IdMortgage = 1,
                IdPayments = 1,
                Date = DateTime.Now,
                Value = 1500
            });
            context.Payments.Add(new Payment
            {
                IdMortgage = 1,
                IdPayments = 2,
                Date = DateTime.Now,
                Value = 2500
            });
            context.Payments.Add(new Payment
            {
                IdMortgage = 1,
                IdPayments = 3,
                Date = DateTime.Now,
                Value = 3500
            });

            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreDB);
            var controller = new PaymentsController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetUsersAndPayments();

            //Verificación
            var respuesta = resultado.Value;

            respuesta.Should().NotBeNull();
            respuesta.Count.Should().Be(3);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task DevuelveNotFoundSiElUsuarioNoTienePaymentsRegistrados()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
           
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
            var controller = new PaymentsController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetUsersAndPayments();

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;

            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task DevuelveNotFoundSiElUsuarioNoTieneHipotecasRegistradas()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            
            var controller = new PaymentsController(context, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetUsersAndPayments();

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;

            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task DevuelveLosPaymentsDeUnaHipoteca()
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
            context.Mortgages.Add(new Mortgage
            {
                IdMortgage = 1,
                FeesNumber = 10,
                FeeValue = 10000,
                IdUser = "Usuario1",
                IdEstate = 1,
                TotalValue = 100000
            });
            context.Payments.Add(new Payment
            {
                IdMortgage = 1,
                IdPayments = 1,
                Date = DateTime.Now,
                Value = 1500
            });
            context.Payments.Add(new Payment
            {
                IdMortgage = 1,
                IdPayments = 2,
                Date = DateTime.Now,
                Value = 2500
            });
            context.Payments.Add(new Payment
            {
                IdMortgage = 2,
                IdPayments = 3,
                Date = DateTime.Now,
                Value = 3500
            });

            await context.SaveChangesAsync();

            
            var context2 = ConstruirContext(nombreDB);
            var controller = new PaymentsController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.Get(1, 1);

            //Verificación
            var respuesta = resultado.Value;

            respuesta.Should().NotBeNull();
            respuesta.Count.Should().Be(2);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task DevuelveElPago()
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
            context.Mortgages.Add(new Mortgage
            {
                IdMortgage = 1,
                FeesNumber = 10,
                FeeValue = 10000,
                IdUser = "Usuario1",
                IdEstate = 1,
                TotalValue = 100000
            });
            context.Payments.Add(new Payment
            {
                IdMortgage = 1,
                IdPayments = 1,
                Date = DateTime.Now,
                Value = 1500
            });
            context.Payments.Add(new Payment
            {
                IdMortgage = 1,
                IdPayments = 2,
                Date = DateTime.Now,
                Value = 2500
            });


            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreDB);
            var controller = new PaymentsController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetById(1, 1, 1);

            //Verificación
            var respuesta = resultado.Value;

            respuesta.Should().NotBeNull();
            respuesta.Value.Should().Be(1500);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task SeCreaElPago()
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


            var dto = new PostPaymentsDTO()
            {
                Date = DateTime.Now,
                Value = 1555
            };
            var context2 = ConstruirContext(nombreDB);

            var controller = new PaymentsController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.Post(dto, 1, 1);

            //Verificación
            var respuesta = resultado as CreatedAtRouteResult;
            var context3 = ConstruirContext(nombreDB);
            var Payment = await context3.Payments.CountAsync();

            respuesta.Should().NotBeNull();
            Payment.Should().Be(1);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task SeBorraElPago()
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
            context.Payments.Add(new Payment
            {
                IdMortgage = 1,
                IdPayments = 1,
                Date = DateTime.Now,
                Value = 1500
            });

            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreDB);

            var controller = new PaymentsController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.Delete(1, 1, 1);

            //Verificación
            var respuesta = resultado as OkObjectResult;
            var context3 = ConstruirContext(nombreDB);
            var Payment = await context3.Payments.CountAsync();

            respuesta.Should().NotBeNull();
            Payment.Should().Be(0);
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
            context.Mortgages.Add(new Mortgage
            {
                IdMortgage = 1,
                FeesNumber = 10,
                FeeValue = 10000,
                IdUser = "Usuario1",
                IdEstate = 1,
                TotalValue = 100000
            });
            var Fecha = DateTime.Now;
            context.Payments.Add(new Payment
            {
                IdMortgage = 1,
                IdPayments = 1,
                Date = Fecha,
                Value = 1500
            });

            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreDB);
 
            var controller = new PaymentsController(context2, mapper, mock.Object);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(x => x.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<object>()));

            controller.ObjectValidator = objectValidator.Object;
            var jsonPatch = new JsonPatchDocument<PatchPaymentsDTO>();
            jsonPatch.Operations.Add(new Operation<PatchPaymentsDTO>("replace", "/Date", null, DateTime.Now));

            //Prueba
            var resultado = await controller.Patch(jsonPatch, 1, 1, 1);

            //Verificación
            var respuesta = resultado as NoContentResult;
            var context3 = ConstruirContext(nombreDB);
            var PaymentActualizado = await context3.Payments.FirstAsync();

            respuesta.Should().NotBeNull();
            PaymentActualizado.Date.Should().NotBe(Fecha);
            PaymentActualizado.Value.Should().Be(1500);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task NoSePuedeActualizarPatchAPagoQueNoExiste()
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
  
            var controller = new PaymentsController(context2, mapper, mock.Object);
            var jsonPatch = new JsonPatchDocument<PatchPaymentsDTO>();
            jsonPatch.Operations.Add(new Operation<PatchPaymentsDTO>("replace", "/Date", null, DateTime.Now));

            //Prueba
            var resultado = await controller.Patch(jsonPatch, 1, 1, 1);


            //Verificación
            var respuesta = resultado as NotFoundObjectResult;

            respuesta.Should().NotBeNull();
            mock.Invocations.Count.Should().Be(1);
        }
    }
}
