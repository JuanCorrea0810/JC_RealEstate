﻿using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using RealEstate.Controllers;
using RealEstate.Models;
using RealEstate.Utilities;
using RealEstate.DTO_s.BuyContractsDTO_s;
using AutoMapper;
using FluentAssertions;

namespace RealEstate.Tests.xUnit
{
    public class BuyingContractsControllerTests: BasePruebas
    {
        private IMapper mapper;
        private Mock<IGetUserInfo> mock;
        public BuyingContractsControllerTests()
        {
            mapper = ConfigurarAutoMapper();
            mock = new Mock<IGetUserInfo>();
            mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));
        }
        [Fact]
        public async Task DevuelveLosBuyingContractsDelUsuario()
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
                Address = "dmkasmdkasnmdjqndjew",
                Alias = "Casa 1",
                City = "Bogotá",
                Country = "Colombia",
                IdEstate = 2,
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
            context.Buyers.Add(new Buyer
            {
                Dni = 1234567891,
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
            context.Buycontracts.Add(new Buycontract
            {
                IdBuyContract = 1,
                IdBuyer = 1,
                IdEst = 1,
                SalePrice = 150000,
                Date = DateTime.Now
            });
            context.Buycontracts.Add(new Buycontract
            {
                IdBuyContract = 2,
                IdBuyer = 2,
                IdEst = 2,
                SalePrice = 160000,
                Date = DateTime.Now
            });
            context.Buycontracts.Add(new Buycontract
            {
                IdBuyContract = 3,
                IdBuyer = 3,
                IdEst = 3,
                SalePrice = 180000,
                Date = DateTime.Now
            });

            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreDB);
            var controller = new BuyContractsController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetBuyContractsAndUsers();

            //Verificación
            var respuesta = resultado.Value;

            respuesta.Should().NotBeNull();
            respuesta.Count.Should().Be(2);
            mock.Invocations.Count.Should().Be(1);
        }
        [Fact]
        public async Task DeuelveNotFoundSiElUsuarioNoTieneContratosDeCompra()
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
            var controller = new BuyContractsController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetBuyContractsAndUsers();

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;

            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
            mock.Invocations.Count.Should().Be(1);
        }
        [Fact]
        public async Task DevuelveElContratoDeCompraDeUnaPropiedad()
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
            context.Buycontracts.Add(new Buycontract
            {
                IdBuyContract = 1,
                IdBuyer = 1,
                IdEst = 1,
                SalePrice = 150000,
                Date = DateTime.Now
            });

            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreDB);
            var controller = new BuyContractsController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.Get(1, 1);

            //Verificación
            var respuesta = resultado.Value;

            respuesta.Should().NotBeNull();
            respuesta.SalePrice.Should().Be(150000);
            mock.Invocations.Count.Should().Be(1);
        }
        [Fact]
        public async Task DevuelveNotFoundSiPropiedadNoTieneContratoDeCompra()
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
            var controller = new BuyContractsController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.Get(1, 1);

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;

            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
            mock.Invocations.Count.Should().Be(1);
        }
        [Fact]
        public async Task NoSePuedeCrearUnContratoAUnaPropiedadPreviamenteVendida()
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
            context.Buycontracts.Add(new Buycontract
            {
                IdBuyContract = 1,
                IdBuyer = 1,
                IdEst = 1,
                SalePrice = 150000,
                Date = DateTime.Now
            });
            await context.SaveChangesAsync();

            
            var context2 = ConstruirContext(nombreDB);
            var controller = new BuyContractsController(context2, mapper, mock.Object);
            var dto = new PostBuyContractsDTO()
            {
                Date = DateTime.Now,
                SalePrice = 9999
            };

            //Prueba
            var resultado = await controller.Post(dto, 1, 1);

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
            var controller = new BuyContractsController(context2, mapper, mock.Object);
            var dto = new PostBuyContractsDTO()
            {
                Date = DateTime.Now,
                SalePrice = 9999
            };

            //Prueba
            var resultado = await controller.Post(dto, 1, 1);

            //Verificación
            var respuesta = resultado as CreatedAtRouteResult;
            var context3 = ConstruirContext(nombreDB);
            var Contrato = await context3.Buycontracts.CountAsync();

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
            context.Buycontracts.Add(new Buycontract
            {
                IdBuyContract = 1,
                IdBuyer = 1,
                IdEst = 1,
                SalePrice = 150000,
                Date = DateTime.Now
            });

            await context.SaveChangesAsync();


            var context2 = ConstruirContext(nombreDB);
            var controller = new BuyContractsController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.Delete(1, 1);

            //Verificación
            var respuesta = resultado as OkObjectResult;
            var context3 = ConstruirContext(nombreDB);
            var ExisteContratos = await context3.Buycontracts.AnyAsync();

            respuesta.Should().NotBeNull();
            ExisteContratos.Should().BeFalse();
            mock.Invocations.Count.Should().Be(1);
        }
        [Fact]
        public async Task DevuelveErrorAlIntentarBorrarContratoQueNoExiste()
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
            var controller = new BuyContractsController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.Delete(1, 1);

            //Verificación
            var respuesta = resultado as NotFoundObjectResult; 
      
            respuesta.Should().NotBeNull();
            respuesta.StatusCode.Should().Be(404);
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
                SecondName = "dsadasdasd",
                SecondSurName = "dsadsdas",
                IdBuyer = 1,
                IdEstate = 1
            });
            var Fecha = DateTime.Now;
            context.Buycontracts.Add(new Buycontract
            {
                IdBuyContract = 1,
                IdBuyer = 1,
                IdEst = 1,
                SalePrice = 150000,
                Date = Fecha
            });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreDB);

            var controller = new BuyContractsController(context2, mapper, mock.Object);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(x => x.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<object>()));

            controller.ObjectValidator = objectValidator.Object;
            var jsonPatch = new JsonPatchDocument<PatchBuyContractsDTO>();
            jsonPatch.Operations.Add(new Operation<PatchBuyContractsDTO>("replace", "/SalePrice", null, 666));

            //Prueba
            var resultado = await controller.Patch(jsonPatch, 1, 1);

            //Verificación
            var respuesta = resultado as NoContentResult;
            var context3 = ConstruirContext(nombreDB);
            var ContractActualizado = await context3.Buycontracts.FirstAsync();

            respuesta.Should().NotBeNull();
            ContractActualizado.SalePrice.Should().NotBe(150000);
            ContractActualizado.Date.Should().Be(Fecha);
            mock.Invocations.Count.Should().Be(1);
        }
    }
}
