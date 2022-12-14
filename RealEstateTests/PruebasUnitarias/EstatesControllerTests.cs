using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Moq;
using PeliculasAPI.Tests;
using RealEstate.Controllers;
using RealEstate.DTO_s.EstatesDTO_s;
using RealEstate.Models;
using RealEstate.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateTests.PruebasUnitarias
{
    [TestClass]
    public class EstatesControllerTests:BasePruebas
    {
        [TestMethod]
        public async Task DevuelveTodasLasPropiedades() 
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            context.Estates.Add(new Estate
            {
                IdUser = "mdaslmdasknd",
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
                IdUser = "ldpasdokqpdie",
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

            context.Estates.Add(new Estate
            {
                IdUser = "juandasd",
                Address = "daas",
                Alias = "Casa 3",
                City = "Cali"
            ,
                Country = "Colombia",
                IdEstate = 3,
                KmsGround = 1200,
                Rooms = 8,
                Rented = true,
                Sold = false
            });

            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreDB);
            var controller = new EstatesController(context2,mapper,null);

            //Prueba
            var resultado = await controller.GetAllEstates();
                
            //Verificación
            var respuesta = resultado.Value;
            Assert.IsNotNull(respuesta);
            Assert.AreEqual(3, respuesta.Count);
        }
        [TestMethod]
        public async Task DevuelveErrorSiElUsuarioNoTienePropiedades() 
        {

            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            var mock = new Mock<IGetUserInfo>();
            mock.Setup(x => x.GetId()).Returns(Task.FromResult("UsuarioNoExiste"));
            var controller = new EstatesController(context, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetUserAndTheirEstates();

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;
            Assert.IsNotNull(codigo);
            Assert.AreEqual(404,codigo.StatusCode);
            Assert.AreEqual(1,mock.Invocations.Count);
        }

        [TestMethod]
        public async Task DevuelvePropiedadesDelUsuario()
        {

            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            var mock = new Mock<IGetUserInfo>();
            mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));

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

            context.Estates.Add(new Estate
            {
                IdUser = "juandasd",
                Address = "daas",
                Alias = "Casa 3",
                City = "Cali"
            ,
                Country = "Colombia",
                IdEstate = 3,
                KmsGround = 1200,
                Rooms = 8,
                Rented = true,
                Sold = false
            });

            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreDB);
            var controller = new EstatesController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetUserAndTheirEstates();

            //Verificación
            var respuesta = resultado.Value;
            Assert.IsNotNull(respuesta);
            Assert.AreEqual(2, respuesta.Count);
            Assert.AreEqual(1, mock.Invocations.Count);
        }

        [TestMethod]
        public async Task DevuelveErrorSiPropiedadNoExiste()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            var mock = new Mock<IGetUserInfo>();
            mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));
            var controller = new EstatesController(context, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetById(1);

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;
            Assert.IsNotNull(codigo);
            Assert.AreEqual(404,codigo.StatusCode);
            Assert.AreEqual(1,mock.Invocations.Count);

        }
        [TestMethod]
        public async Task DevuelvePropiedadPorId()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            var mock = new Mock<IGetUserInfo>();
            mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));
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
            await context.SaveChangesAsync();
            var context2 = ConstruirContext(nombreDB);
            var controller = new EstatesController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetById(2);

            //Verificación
            var respuesta = resultado.Value;
            Assert.IsNotNull(respuesta);
            Assert.AreEqual(2,respuesta.IdEstate);
            Assert.AreEqual(1, mock.Invocations.Count);

        }

        [TestMethod]
        public async Task DevuelvePropiedadPorAlias()
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
            var mock = new Mock<IGetUserInfo>();
            mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));
            var context2 = ConstruirContext(nombreDB);
            var controller = new EstatesController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetByAlias("Casa 1");

            //Verificación
            var respuesta = resultado.Value;
            Assert.IsNotNull(respuesta);
            Assert.AreEqual("Casa 1", respuesta.Alias);
            Assert.AreEqual(1, mock.Invocations.Count);

        }

        [TestMethod]
        public async Task DevuelveErrorSiAliasNoExiste()
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
            var mock = new Mock<IGetUserInfo>();
            mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));
            var context2 = ConstruirContext(nombreDB);
            var controller = new EstatesController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetByAlias("Casa 12");

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;
            Assert.IsNotNull(codigo);
            Assert.AreEqual(404, codigo.StatusCode);
            Assert.AreEqual(1, mock.Invocations.Count);
        }

        [TestMethod]
        public async Task SeCreaNuevaPropiedad()
        {

            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            var mock = new Mock<IGetUserInfo>();
            mock.Setup(x => x.GetId()).Returns(Task.FromResult("UsuarioNoExiste"));
            var controller = new EstatesController(context, mapper, mock.Object);
            var dto = new PostEstatesDTO()
            {
                Address = "dasdas",
                KmsGround = 1200,
                Sold = false,
                Alias = "Casa Creada",
                City = "Medellín",
                Country = "Colombia",
                Rented = false,
                Rooms = 4
            };

            //Prueba
            var resultado = await controller.Post(dto);

            //Verificación
            var context2 = ConstruirContext(nombreDB);
            var propiedadCreada = await context2.Estates.ToListAsync();
            Assert.IsNotNull(propiedadCreada);
            Assert.AreEqual(1,propiedadCreada.Count);
            Assert.AreEqual("Casa Creada", propiedadCreada[0].Alias);
            Assert.AreEqual(1, mock.Invocations.Count);
        }

        [TestMethod]
        public async Task DevuelveErrorSiSeCreaPropiedadConElMismoAlias()
        {

            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            context.Estates.Add(new Estate
            {
                IdUser = "Usuario1",
                Address = "dmkasmdkasnmdjqndjew",
                Alias = "Casa Repetida",
                City = "Bogotá",
                Country = "Colombia",
                IdEstate = 1,
                KmsGround = 1500,
                Rooms = 12,
                Rented = false,
                Sold = false
            });

            await context.SaveChangesAsync();
            var mock = new Mock<IGetUserInfo>();
            mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));
            
            var controller = new EstatesController(context, mapper, mock.Object);
            var dto = new PostEstatesDTO()
            {
                Address = "dasdas",
                KmsGround = 1200,
                Sold = false,
                Alias = "Casa Repetida",
                City = "Medellín",
                Country = "Colombia",
                Rented = false,
                Rooms = 4
            };

            //Prueba
            var resultado = await controller.Post(dto);

            //Verificación
            var respuesta = resultado as BadRequestObjectResult;
            var context2 = ConstruirContext(nombreDB);
            var propiedadCreada = await context2.Estates.ToListAsync();
            Assert.IsNotNull(propiedadCreada);
            Assert.IsNotNull(respuesta);
            Assert.AreEqual(1, propiedadCreada.Count);
            Assert.AreEqual("Casa Repetida", propiedadCreada[0].Alias);
            Assert.AreEqual(400,respuesta.StatusCode);
            Assert.AreEqual(1, mock.Invocations.Count);
        }
        [TestMethod]
        public async Task NoSePuedeBorrarPropiedadQueNoExiste()
        {

            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();
            var mock = new Mock<IGetUserInfo>();
            mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));

            var controller = new EstatesController(context, mapper, mock.Object);


            //Prueba
            var resultado = await controller.Delete(1);

            //Verificación
            var respuesta = resultado as NotFoundObjectResult;
            Assert.IsNotNull(respuesta);
            Assert.AreEqual(404, respuesta.StatusCode);
            Assert.AreEqual(1, mock.Invocations.Count);
        }
        [TestMethod]
        public async Task SeBorraPropiedadDelUsuario()
        {

            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();
            context.Estates.Add(new Estate
            {
                IdUser = "Usuario1",
                Address = "dmkasmdkasnmdjqndjew",
                Alias = "Casa Repetida",
                City = "Bogotá",
                Country = "Colombia",
                IdEstate = 1,
                KmsGround = 1500,
                Rooms = 12,
                Rented = false,
                Sold = false
            });

            await context.SaveChangesAsync();
            var mock = new Mock<IGetUserInfo>();
            mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));
            var context2 = ConstruirContext(nombreDB);

            var controller = new EstatesController(context2, mapper, mock.Object);


            //Prueba
            var resultado = await controller.Delete(1);

            //Verificación
            var respuesta = resultado as OkObjectResult;
            var context3 = ConstruirContext(nombreDB);
            var ExisteAlgunaPropiedad = await context3.Estates.AnyAsync(); 
            Assert.IsNotNull(respuesta);
            Assert.IsFalse(ExisteAlgunaPropiedad);
            Assert.AreEqual(200, respuesta.StatusCode);
            Assert.AreEqual(1, mock.Invocations.Count);
        }


        [TestMethod]
        public async Task PatchRetornaErrorSiPropiedadNoExiste()
        {
            //Preparación
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();
            var mock = new Mock<IGetUserInfo>();
            mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));

            var controller = new EstatesController(contexto, mapper, mock.Object);
            var patchDoc = new JsonPatchDocument<PatchEstatesDTO>();
            //Prueba
            var respuesta = await controller.Patch(patchDoc, 1);
            //Verificación
            var resultado = respuesta as NotFoundObjectResult;
            Assert.IsNotNull(resultado);
            Assert.AreEqual(404, resultado.StatusCode);
            Assert.AreEqual(1, mock.Invocations.Count);
        }

        [TestMethod]
        public async Task PatchDevuelveErrorSiSeQuiereActualizarAliasQueYaExiste()
        {
            //Preparación
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();
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
            
            var mock = new Mock<IGetUserInfo>();
            mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));
            
            var context2 = ConstruirContext(nombreBD);
            var controller = new EstatesController(context2, mapper, mock.Object);
            
            var patchDoc = new JsonPatchDocument<PatchEstatesDTO>();
            patchDoc.Operations.Add(new Operation<PatchEstatesDTO>("replace", "/Alias", null, "Casa 1"));

            //Prueba
            var respuesta = await controller.Patch(patchDoc, 1);
           
            //Verificación
            var resultado = respuesta as BadRequestObjectResult;
            Assert.IsNotNull(resultado);
            Assert.AreEqual(400, resultado.StatusCode);
            Assert.AreEqual(1, mock.Invocations.Count);
        }
        [TestMethod]
        public async Task PatchActualizaSoloUnCampo()
        {
            //Preparación
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();
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

            var mock = new Mock<IGetUserInfo>();
            mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));

            var context2 = ConstruirContext(nombreBD);
            var controller = new EstatesController(context2, mapper, mock.Object);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(x => x.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<object>()));

            controller.ObjectValidator = objectValidator.Object;
            var patchDoc = new JsonPatchDocument<PatchEstatesDTO>();
            patchDoc.Operations.Add(new Operation<PatchEstatesDTO>("replace", "/Alias", null, "Casa Actualizada"));

            //Prueba
            var respuesta = await controller.Patch(patchDoc, 1);
            //Verificación
            var resultado = respuesta as NoContentResult;
            var context3 = ConstruirContext(nombreBD);
            var Entidad = await context3.Estates.FirstAsync();
            Assert.IsNotNull(resultado);
            Assert.AreEqual(204, resultado.StatusCode);
            Assert.AreEqual("Casa Actualizada", Entidad.Alias);
            Assert.AreEqual("Colombia", Entidad.Country);
            Assert.AreEqual(1, mock.Invocations.Count);
            Assert.AreEqual(1, objectValidator.Invocations.Count);
        }
    }
}
