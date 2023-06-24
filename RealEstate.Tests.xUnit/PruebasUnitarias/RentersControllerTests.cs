using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using RealEstate.Controllers;
using RealEstate.Models;
using RealEstate.Utilities;
using RealEstate.DTO_s.RentersDTO_s;
using AutoMapper;
using FluentAssertions;

namespace RealEstate.Tests.xUnit
{
    public class RentersControllerTests : BasePruebas
    {
        private IMapper mapper;
        private Mock<IGetUserInfo> mock;
        public RentersControllerTests()
        {
            mapper = ConfigurarAutoMapper();
            mock = new Mock<IGetUserInfo>();
            mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));
        }
        [Fact]
        public async Task DevuelveLosRentersDelUsuario()
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
                IdEstate = 2
            });

            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreDB);
            var controller = new RentersController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetUsersAndTheirRenters();

            //Verificación
            var respuesta = resultado.Value;

            respuesta.Should().NotBeNull();
            respuesta.Count.Should().Be(2);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task DevuelveNotFoundSiNoSeTienePropiedadesORentersRegistrados()
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
            var controller = new RentersController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetUsersAndTheirRenters();

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;

            respuesta.Should().NotBeNull();
            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task DevuelveErrorSiElRenterNoExisteONoLePerteneceAlUsuario()
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
                IdEstate = 2
            });

            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreDB);
            var controller = new RentersController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetRentersOfEstate(1);

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;

            respuesta.Should().NotBeNull();
            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task DevuelveLosRenter()
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

            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreDB);
            var controller = new RentersController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetRentersOfEstate(1);

            //Verificación
            var respuesta = resultado.Value;

            respuesta.Should().NotBeNull();
            respuesta.Count.Should().Be(2);
            respuesta[0].Dni.Should().Be(1234567890);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task DevuelveErrorAlIntentarTraerUnRenterQueNoExiste()
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
                IdEstate = 2
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

            await context.SaveChangesAsync();
 
            var context2 = ConstruirContext(nombreDB);
            var controller = new RentersController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetById(1, 1);

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;

            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task DevuelveErrorSiLaPropiedadNoEsVálidaAlCrearNuevoRenter()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);

            var controller = new RentersController(context, mapper, mock.Object);
            var dto = new PostRentersDTO()
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
                Active = true
            };

            //Prueba
            var resultado = await controller.Post(1, dto);

            //Verificación
            var respuesta = resultado as NotFoundObjectResult;

            respuesta.Should().NotBeNull();
            respuesta.StatusCode.Should().Be(404);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task NoSePuedeRegistrarUnRenterAUnaPropiedadYaComprada()
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
                Sold = true
            });

            await context.SaveChangesAsync();
            var dto = new PostRentersDTO()
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
                Active = true
            };
            var context2 = ConstruirContext(nombreDB);

            var controller = new RentersController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.Post(1, dto);

            //Verificación
            var respuesta = resultado as BadRequestObjectResult;

            respuesta.Should().NotBeNull();
            respuesta.StatusCode.Should().Be(400);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task NoSePuedeRegistrarDosRentersConElMismoDniAUnaMismaPropiedad()
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
                IdRenter = 1,
                IdEstate = 1
            });

            await context.SaveChangesAsync();
            var dto = new PostRentersDTO()
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
                Active = true
            };
            var context2 = ConstruirContext(nombreDB);

            var controller = new RentersController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.Post(1, dto);

            //Verificación
            var respuesta = resultado as BadRequestObjectResult;
            var context3 = ConstruirContext(nombreDB);
            var CantidadRenters = await context3.Renters.CountAsync();

            respuesta.Should().NotBeNull();
            CantidadRenters.Should().Be(1);
            respuesta.StatusCode.Should().Be(400);
            mock.Invocations.Count.Should().Be(1);
        }
        [Fact]
        public async Task SiSePuedeRegistrarDosRentersConElMismoDniADiferentesPropiedadesYMarcaComoInactivosLosAnterioresRenters()
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
                City = "Bogotá"
            ,
                Country = "Colombia",
                IdEstate = 2,
                KmsGround = 1500,
                Rooms = 12,
                Rented = false,
                Sold = false
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
                IdRenter = 1,
                IdEstate = 1,
                Active = true
            });
            context.Renters.Add(new Renter
            {
                Dni = 0987654322,
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
                IdEstate = 1,
                Active = true
            });

            await context.SaveChangesAsync();
            var dto = new PostRentersDTO()
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
                Active = true
            };
            var context2 = ConstruirContext(nombreDB);

            var controller = new RentersController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.Post(1, dto);

            //Verificación
            var respuesta = resultado as CreatedAtRouteResult;
            var context3 = ConstruirContext(nombreDB);
            var Renters = await context3.Renters.ToListAsync();

            respuesta.Should().NotBeNull();
            Renters.Count.Should().Be(3);
            Renters[2].Active.Should().NotBe(Renters[0].Active);
            mock.Invocations.Count.Should().Be(1);
        }
        [Fact]
        public async Task SeRegistraNuevoRenter()
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
            var dto = new PostRentersDTO()
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
                Active = true
            };
            var context2 = ConstruirContext(nombreDB);

            var controller = new RentersController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.Post(1, dto);

            //Verificación
            var respuesta = resultado as CreatedAtRouteResult;
            var context3 = ConstruirContext(nombreDB);
            var ExisteRenter = await context3.Renters.AnyAsync();

            respuesta.Should().NotBeNull();
            ExisteRenter.Should().BeTrue();
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task SeBorraElRenter()
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
            var controller = new RentersController(context2, mapper, mock.Object);
            //Prueba
            var resultado = await controller.Delete(1, 1);

            //Verificación
            var respuesta = resultado as OkObjectResult;
            var context3 = ConstruirContext(nombreDB);
            var ExisteRenter = await context3.Renters.AnyAsync();

            respuesta.Should().NotBeNull();
            ExisteRenter.Should().BeFalse();
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task NoSePuedeBorrarRenterQueNoExisteOLaPropiedadNoEsVálida()
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
                IdEstate = 2,
                Active = true
            });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreDB);
            var controller = new RentersController(context2, mapper, mock.Object);
            //Prueba
            var resultado = await controller.Delete(1, 1);

            //Verificación
            var respuesta = resultado as NotFoundObjectResult;

            respuesta.Should().NotBeNull();
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task NoSePuedeActualizarPatchARenterQueNoExiste()
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
            var controller = new RentersController(context2, mapper, mock.Object);
            var jsonPatch = new JsonPatchDocument<PatchRentersDTO>();
            jsonPatch.Operations.Add(new Operation<PatchRentersDTO>("replace", "/SecondName", null, "Juan"));
            //Prueba
            var resultado = await controller.Patch(jsonPatch, 1, 1);

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
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreDB);
            var controller = new RentersController(context2, mapper, mock.Object);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(x => x.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<object>()));

            controller.ObjectValidator = objectValidator.Object;
            var jsonPatch = new JsonPatchDocument<PatchRentersDTO>();
            jsonPatch.Operations.Add(new Operation<PatchRentersDTO>("replace", "/SecondName", null, "Pablo"));

            //Prueba
            var resultado = await controller.Patch(jsonPatch, 1, 1);

            //Verificación
            var respuesta = resultado as NoContentResult;
            var context3 = ConstruirContext(nombreDB);
            var RenterActualizado = await context3.Renters.FirstAsync();

            respuesta.Should().NotBeNull();
            RenterActualizado.Country.Should().Be("Colombia");
            RenterActualizado.SecondName.Should().Be("Pablo");
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task PatchMarcaComoInactivosAntiguosRenters()
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
                IdEstate = 1,
                Active = true
            });
            context.Renters.Add(new Renter
            {
                Dni = 1234567891,
                Address = "dsadasds",
                Age = 19,
                Email = "dsadsad@gmail.com",
                Country = "Colombia",
                CellPhoneNumber = 4528065891,
                FirsName = "dsads",
                FirstSurName = "dsadadsas",
                SecondName = "Manuel",
                SecondSurName = "dsadsdas",
                IdRenter = 2,
                IdEstate = 1,
                Active = true

            });
            context.Renters.Add(new Renter
            {
                Dni = 1234567892,
                Address = "dsadasds",
                Age = 19,
                Email = "dsadsad@gmail.com",
                Country = "Colombia",
                CellPhoneNumber = 4528065891,
                FirsName = "dsads",
                FirstSurName = "dsadadsas",
                SecondName = "Manuel",
                SecondSurName = "dsadsdas",
                IdRenter = 3,
                IdEstate = 1,
                Active = false
            });
            await context.SaveChangesAsync();


            var context2 = ConstruirContext(nombreDB);
            var controller = new RentersController(context2, mapper, mock.Object);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(x => x.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<object>()));

            controller.ObjectValidator = objectValidator.Object;
            var jsonPatch = new JsonPatchDocument<PatchRentersDTO>();
            jsonPatch.Operations.Add(new Operation<PatchRentersDTO>("replace", "/Active", null, "true"));

            //Prueba
            var resultado = await controller.Patch(jsonPatch, 1, 3);

            //Verificación
            var respuesta = resultado as NoContentResult;
            var context3 = ConstruirContext(nombreDB);
            var RenterActualizado = await context3.Renters.ToListAsync();

            respuesta.Should().NotBeNull();
            RenterActualizado[0].Active.Should().NotBe(RenterActualizado[2].Active);
            mock.Invocations.Count.Should().Be(1);
        }
    }
}

