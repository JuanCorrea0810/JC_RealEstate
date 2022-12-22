using Microsoft.AspNetCore.Mvc;
using PeliculasAPI.Tests;
using RealEstate.Controllers;
using RealEstate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateTests.PruebasUnitarias
{
    [TestClass]
    public class CustomBaseControllerTests:BasePruebas
    {
        [TestMethod]
        public async Task DevuelvePropiedad() 
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();
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
            Assert.IsNotNull(respuesta);
            Assert.AreEqual("Casa 1",respuesta.Alias);
        }
        [TestMethod]
        public async Task DevuelveNotFoundSiPropiedadNoExiste_MetodoDevuelvePropiedad()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();
            await context.SaveChangesAsync();

            var controller = new CustomBaseControllerParaPruebas(context, mapper);
            //Prueba
            var resultado = await controller.DevolverPropiedad("Usuario1", 1);

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;
            Assert.IsNotNull(codigo);
            Assert.AreEqual(404, codigo.StatusCode);
        }
        [TestMethod]
        public async Task DevuelveNotFoundSiPropiedadNoExiste_MetodoSaberSiExistePropiedad()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();
            await context.SaveChangesAsync();

            var controller = new CustomBaseControllerParaPruebas(context, mapper);
            //Prueba
            var resultado = await controller.SaberSiExistePropiedad("Usuario1", 1);

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;
            Assert.IsNotNull(codigo);
            Assert.AreEqual(404, codigo.StatusCode);
        }
        [TestMethod]
        public async Task DevuelveTrueSiPropiedadExiste()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();
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
            Assert.IsTrue(respuesta);
        }
        [TestMethod]
        public async Task DevuelveTrueSiRenterExiste()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();
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
            Assert.IsTrue(respuesta);
        }
        [TestMethod]
        public async Task DevuelveNotFoundSiRenterNoExiste()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();
            await context.SaveChangesAsync();

            var controller = new CustomBaseControllerParaPruebas(context, mapper);
            //Prueba
            var resultado = await controller.SaberSiExisteRenter(1);

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;
            Assert.IsNotNull(codigo);
            Assert.AreEqual(404, codigo.StatusCode);
        }
        [TestMethod]
        public async Task DevuelveTrueSiGuarantorExiste()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();
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
            Assert.IsTrue(respuesta);
        }
        [TestMethod]
        public async Task DevuelveNotFoundSiGuarantorNoExiste()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();
            await context.SaveChangesAsync();

            var controller = new CustomBaseControllerParaPruebas(context, mapper);
            //Prueba
            var resultado = await controller.SaberSiExisteGuarantor(1);

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;
            Assert.IsNotNull(codigo);
            Assert.AreEqual(404, codigo.StatusCode);
        }
        [TestMethod]
        public async Task DevuelveTrueSiPropiedadYRenterCoinciden()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();
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
            Assert.IsTrue(respuesta);
        }

        [TestMethod]
        public async Task DevuelveNotFoundSiPropiedadYRenterNoCoinciden()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();
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
            Assert.IsNotNull(codigo);
            Assert.AreEqual(404, codigo.StatusCode);
        }
        [TestMethod]
        public async Task DevuelveTrueSiBuyerExiste()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();
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
            Assert.IsTrue(respuesta);
        }
        [TestMethod]
        public async Task DevuelveNotFoundSiBuyerNoExiste()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();
            await context.SaveChangesAsync();

            var controller = new CustomBaseControllerParaPruebas(context, mapper);
            //Prueba
            var resultado = await controller.SaberSiExisteBuyer(1);

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;
            Assert.IsNotNull(codigo);
            Assert.AreEqual(404, codigo.StatusCode);
        }
        [TestMethod]
        public async Task DevuelveTrueSiPropiedadYBuyerCoinciden()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();
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
            Assert.IsTrue(respuesta);
        }
        [TestMethod]
        public async Task DevuelveNotFoundSiPropiedadYBuyerNoCoinciden()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();
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
            Assert.IsNotNull(codigo);
            Assert.AreEqual(404, codigo.StatusCode);
        }
    }
}
