using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RealEstate.Controllers;
using RealEstate.Models;
using RealEstate.Utilities;
using Microsoft.EntityFrameworkCore;
using RealEstate.DTO_s.GuarantorDTO_s;
using AutoMapper;
using FluentAssertions;

namespace RealEstate.Tests.xUnit
{
    public class GuarantosControllerTests : BasePruebas
    {
        private IMapper mapper;
        private Mock<IGetUserInfo> mock;
        public GuarantosControllerTests()
        {
            mapper = ConfigurarAutoMapper();
            mock = new Mock<IGetUserInfo>();
            mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));
        }
        [Fact]
        public async Task DevuelveLosGuarantorsDelUsuario()
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
            context.Guarantors.Add(new Guarantor
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
                IdRenter = 1,
                IdGuarantor = 2

            });


            await context.SaveChangesAsync();

 
            var context2 = ConstruirContext(nombreDB);
            var controller = new GuarantorController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetUsersAndTheirGuarantors();

            //Verificación
            var respuesta = resultado.Value;

            respuesta.Should().NotBeNull();
            respuesta.Count.Should().Be(2);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task DevuelveNotFoundSiElUsuarioNoTieneGuarantorsRegistrados()
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
            var controller = new GuarantorController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetUsersAndTheirGuarantors();

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;

            respuesta.Should().NotBeNull();
            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
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
            var controller = new GuarantorController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetUsersAndTheirGuarantors();

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;

            respuesta.Should().NotBeNull();
            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task DevuelveErrorSiElRenterNoTieneGuarantors()
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
                IdRenter = 2,
                IdGuarantor = 1
            });

            await context.SaveChangesAsync();

 
            var context2 = ConstruirContext(nombreDB);
            var controller = new GuarantorController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetGuarantorsOfRenter(1, 1);

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;

            respuesta.Should().NotBeNull();
            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task DevuelveErrorSiElRenterNoExiste()
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
            var controller = new GuarantorController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetGuarantorsOfRenter(1, 1);

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;

            respuesta.Should().NotBeNull();
            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
            mock.Invocations.Count.Should().Be(1);
        }
        [Fact]
        public async Task DevuelveErrorSiPropiedadYRenterNoCoinciden()
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
            var controller = new GuarantorController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetGuarantorsOfRenter(1, 1);

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;

            respuesta.Should().NotBeNull();
            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task NoSePuedeTraerUnGuarantorQueNoExisteONoCoincideConElRenterOLaPropiedad()
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
                IdRenter = 2,
                IdGuarantor = 1
            });


            await context.SaveChangesAsync();

  
            var context2 = ConstruirContext(nombreDB);
            var controller = new GuarantorController(context2, mapper, mock.Object);

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
        public async Task NoSePuedeCrearGuarantorConElMismoDniAUnMismoRenter()
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
            var controller = new GuarantorController(context2, mapper, mock.Object);
            var dto = new PostGuarantorDTO()
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

            //Prueba
            var resultado = await controller.Post(dto, 1, 1);

            //Verificación
            var respuesta = resultado as BadRequestObjectResult;

            respuesta.Should().NotBeNull();
            respuesta.StatusCode.Should().Be(400);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task SiSePuedeCrearGuarantorConElMismoDniAUnDiferenteRenter()
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
                IdRenter = 2,
                IdEstate = 1
            });
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
            var controller = new GuarantorController(context2, mapper, mock.Object);
            var dto = new PostGuarantorDTO()
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

            //Prueba
            var resultado = await controller.Post(dto, 2, 1);

            //Verificación
            var respuesta = resultado as CreatedAtRouteResult;
            var context3 = ConstruirContext(nombreDB);
            var Guarantors = await context3.Guarantors.ToListAsync();

            respuesta.Should().NotBeNull();
            Guarantors.Count.Should().Be(2);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task SeBorraElGuarantor()
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
      
            var controller = new GuarantorController(context2, mapper, mock.Object);
            //Prueba
            var resultado = await controller.Delete(1, 1, 1);

            //Verificación
            var respuesta = resultado as OkObjectResult;
            var context3 = ConstruirContext(nombreDB);
            var ExisteGuarantor = await context3.Guarantors.AnyAsync();

            respuesta.Should().NotBeNull();
            ExisteGuarantor.Should().BeFalse();
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task NoSePuedeActualizarPatchAGuarantorQueNoExiste()
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
            
            var controller = new GuarantorController(context2, mapper, mock.Object);
            var jsonPatch = new JsonPatchDocument<PatchGuarantorsDTO>();
            jsonPatch.Operations.Add(new Operation<PatchGuarantorsDTO>("replace", "/SecondName", null, "Juan"));
            //Prueba
            var resultado = await controller.Patch(jsonPatch, 1, 1, 1);

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
                SecondName = "Manuel",
                SecondSurName = "dsadsdas",
                IdRenter = 1,
                IdGuarantor = 1
            });
            await context.SaveChangesAsync();


            var context2 = ConstruirContext(nombreDB);
            
            var controller = new GuarantorController(context2, mapper, mock.Object);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(x => x.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<object>()));

            controller.ObjectValidator = objectValidator.Object;
            var jsonPatch = new JsonPatchDocument<PatchGuarantorsDTO>();
            jsonPatch.Operations.Add(new Operation<PatchGuarantorsDTO>("replace", "/SecondName", null, "Pablo"));

            //Prueba
            var resultado = await controller.Patch(jsonPatch, 1, 1, 1);

            //Verificación
            var respuesta = resultado as NoContentResult;
            var context3 = ConstruirContext(nombreDB);
            var GuarantorActualizado = await context3.Guarantors.FirstAsync();

            respuesta.Should().NotBeNull();
            GuarantorActualizado.Country.Should().Be("Colombia");
            GuarantorActualizado.SecondName.Should().Be("Pablo");
            mock.Invocations.Count.Should().Be(1);
        }

    }
}

