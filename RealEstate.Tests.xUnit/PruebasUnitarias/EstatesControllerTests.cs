using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Moq;
using RealEstate.Controllers;
using RealEstate.DTO_s.EstatesDTO_s;
using RealEstate.Models;
using RealEstate.Utilities;

namespace RealEstate.Tests.xUnit
{
    public class EstatesControllerTests : BasePruebas
    {
        private IMapper mapper;
        private Mock<IGetUserInfo> mock;


        public EstatesControllerTests()
        {
            mapper = ConfigurarAutoMapper();
            mock = new Mock<IGetUserInfo>();
            mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));
        }
        [Fact]
        public async Task DevuelveTodasLasPropiedades()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);


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
            var controller = new EstatesController(context2, mapper, null);

            //Prueba
            var resultado = await controller.GetAllEstates();

            //Verificación
            var respuesta = resultado.Value;

            respuesta.Should().NotBeNull();
            respuesta.Count.Should().Be(3);
        }

        [Fact]
        public async Task DevuelveErrorSiElUsuarioNoTienePropiedades()
        {

            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);



            var controller = new EstatesController(context, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetUserAndTheirEstates();

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;

            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task DevuelvePropiedadesDelUsuario()
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

            respuesta.Should().NotBeNull();
            respuesta.Count.Should().Be(2);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task DevuelveErrorSiPropiedadNoExiste()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);

            var controller = new EstatesController(context, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetById(1);

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;

            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task DevuelvePropiedadPorId()
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
            await context.SaveChangesAsync();
            var context2 = ConstruirContext(nombreDB);
            var controller = new EstatesController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetById(2);

            //Verificación
            var respuesta = resultado.Value;

            respuesta.Should().NotBeNull();
            respuesta.IdEstate.Should().Be(2);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task DevuelvePropiedadPorAlias()
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
            var controller = new EstatesController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetByAlias("Casa 1");

            //Verificación
            var respuesta = resultado.Value;

            respuesta.Should().NotBeNull();
            respuesta.Alias.Should().Be("Casa 1");
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task DevuelveErrorSiAliasNoExiste()
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
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreDB);
            var controller = new EstatesController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetByAlias("Casa 12");

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;

            codigo.Should().NotBeNull();
            codigo.StatusCode.Should().Be(404);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task SeCreaNuevaPropiedad()
        {

            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);

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

            propiedadCreada.Should().NotBeNull();
            propiedadCreada.Count.Should().Be(1);
            propiedadCreada[0].Alias.Should().Be("Casa Creada");
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task DevuelveErrorSiSeCreaPropiedadConElMismoAlias()
        {

            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);


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

            propiedadCreada.Should().NotBeNull();
            respuesta.Should().NotBeNull();
            propiedadCreada.Count.Should().Be(1);
            propiedadCreada[0].Alias.Should().Be("Casa Repetida");
            respuesta.StatusCode.Should().Be(400);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task NoSePuedeBorrarPropiedadQueNoExiste()
        {

            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);

            var controller = new EstatesController(context, mapper, mock.Object);


            //Prueba
            var resultado = await controller.Delete(1);

            //Verificación
            var respuesta = resultado as NotFoundObjectResult;

            respuesta.Should().NotBeNull();
            respuesta.StatusCode.Should().Be(404);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task SeBorraPropiedadDelUsuario()
        {

            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);

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

            var context2 = ConstruirContext(nombreDB);

            var controller = new EstatesController(context2, mapper, mock.Object);


            //Prueba
            var resultado = await controller.Delete(1);

            //Verificación
            var respuesta = resultado as OkObjectResult;
            var context3 = ConstruirContext(nombreDB);
            var ExisteAlgunaPropiedad = await context3.Estates.AnyAsync();

            respuesta.Should().NotBeNull();
            ExisteAlgunaPropiedad.Should().BeFalse();
            respuesta.StatusCode.Should().Be(200);
            mock.Invocations.Count.Should().Be(1);
        }


        [Fact]
        public async Task PatchRetornaErrorSiPropiedadNoExiste()
        {
            //Preparación
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);


            var controller = new EstatesController(contexto, mapper, mock.Object);
            var patchDoc = new JsonPatchDocument<PatchEstatesDTO>();
            //Prueba
            var respuesta = await controller.Patch(patchDoc, 1);
            //Verificación
            var resultado = respuesta as NotFoundObjectResult;

            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be(404);
            mock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task PatchDevuelveErrorSiSeQuiereActualizarAliasQueYaExiste()
        {
            //Preparación
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);

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


            var context2 = ConstruirContext(nombreBD);
            var controller = new EstatesController(context2, mapper, mock.Object);

            var patchDoc = new JsonPatchDocument<PatchEstatesDTO>();
            patchDoc.Operations.Add(new Operation<PatchEstatesDTO>("replace", "/Alias", null, "Casa 1"));

            //Prueba
            var respuesta = await controller.Patch(patchDoc, 1);

            //Verificación
            var resultado = respuesta as BadRequestObjectResult;

            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be(400);
            mock.Invocations.Count.Should().Be(1);
        }
        [Fact]
        public async Task PatchActualizaSoloUnCampo()
        {
            //Preparación
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);

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

            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be(204);
            Entidad.Alias.Should().Be("Casa Actualizada");
            Entidad.Country.Should().Be("Colombia");
            mock.Invocations.Count.Should().Be(1);
            objectValidator.Invocations.Count.Should().Be(1);
        }
    }
}
