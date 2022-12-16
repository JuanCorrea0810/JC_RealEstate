using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Moq;
using PeliculasAPI.Tests;
using RealEstate.Controllers;
using RealEstate.DTO_s.BuyersDTO_s;
using RealEstate.DTO_s.EstatesDTO_s;
using RealEstate.Models;
using RealEstate.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateTests.PruebasUnitarias
{
    [TestClass]
    public class BuyersControllersTests : BasePruebas
    {
        [TestMethod]
        public async Task DevuelveLosCompradoresDelUsuario()
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

            context.Buyers.Add(new Buyer {
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
                IdBuyer = 2,
                IdEstate = 2
            });

            await context.SaveChangesAsync();

            var mock = new Mock<IGetUserInfo>();
            mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));


            var context2 = ConstruirContext(nombreDB);
            var controller = new BuyersController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetUsersAndTheirBuyers();

            //Verificación
            var respuesta = resultado.Value;
            Assert.IsNotNull(respuesta);
            Assert.AreEqual(2, respuesta.Count);
            Assert.AreEqual(1, mock.Invocations.Count);

        }

        [TestMethod]
        public async Task DevuelveNotFoundSiNoSeTienePropiedadesOCompradoresRegistrados()
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
            var controller = new BuyersController(context2, mapper, mock.Object);

            //Prueba
            var resultado = await controller.GetUsersAndTheirBuyers();

            //Verificación
            var respuesta = resultado.Result;
            var codigo = respuesta as NotFoundObjectResult;
            Assert.IsNotNull(respuesta);
            Assert.IsNotNull(codigo);
            Assert.AreEqual(404, codigo.StatusCode);
            Assert.AreEqual(1, mock.Invocations.Count);

        }

        [TestMethod]
        public async Task DevuelveErrorSiElCompradorNoExisteONoLePerteneceAlUsuario()
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
            var ExisteEstate = await ExistePropiedad(1, "Usuario1", nombreDB);
            if (ExisteEstate)
            {
                var mock = new Mock<IGetUserInfo>();
                mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));


                var context2 = ConstruirContext(nombreDB);
                var controller = new BuyersController(context2, mapper, mock.Object);

                //Prueba
                var resultado = await controller.GetById(1);

                //Verificación
                var respuesta = resultado.Result;
                var codigo = respuesta as NotFoundObjectResult;
                Assert.IsNotNull(respuesta);
                Assert.IsNotNull(codigo);
                Assert.AreEqual(404, codigo.StatusCode);
                Assert.AreEqual(1, mock.Invocations.Count);
            }
            else
            {
                //En caso de que la propiedad no exista la siguiente linea nos avisa que la prueba no salió bien
                Assert.AreEqual(1, 0);
            }
        }

        [TestMethod]
        public async Task DevuelveElComprador()
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
            var ExisteEstate = await ExistePropiedad(1, "Usuario1", nombreDB);
            if (ExisteEstate)
            {
                var mock = new Mock<IGetUserInfo>();
                mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));

                var context2 = ConstruirContext(nombreDB);
                var controller = new BuyersController(context2, mapper, mock.Object);

                //Prueba
                var resultado = await controller.GetById(1);

                //Verificación
                var respuesta = resultado.Value;
                Assert.IsNotNull(respuesta);
                Assert.AreEqual(1234567890, respuesta.Dni);
                Assert.AreEqual(1, mock.Invocations.Count);
            }
            else
            {
                //En caso de que la propiedad no exista la siguiente linea nos avisa que la prueba no salió bien
                Assert.AreEqual(1, 0);
            }
        }

        [TestMethod]
        public async Task DevuelveErrorSiLaPropiedadNoEsVálidaAlCrearNuevoComprador()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            var mock = new Mock<IGetUserInfo>();
            mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));
            var dto = new PostBuyersDTO()
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

            var controller = new BuyersController(context, mapper, mock.Object);

            //Prueba
            var resultado = await controller.Post(1, dto);

            //Verificación
            var respuesta = resultado as BadRequestObjectResult;
            Assert.IsNotNull(respuesta);
            Assert.AreEqual(400, respuesta.StatusCode);
            Assert.AreEqual(1, mock.Invocations.Count);
        }
        [TestMethod]
        public async Task NoSePuedeRegistrarUnCompradorAUnaPropiedadYaComprada()
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
            var ExisteEstate = await ExistePropiedad(1, "Usuario1", nombreDB);
            if (ExisteEstate)
            {
                var mock = new Mock<IGetUserInfo>();
                mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));

                var dto = new PostBuyersDTO()
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
                var context2 = ConstruirContext(nombreDB);

                var controller = new BuyersController(context2, mapper, mock.Object);

                //Prueba
                var resultado = await controller.Post(1, dto);

                //Verificación
                var respuesta = resultado as BadRequestObjectResult;
                Assert.IsNotNull(respuesta);
                Assert.AreEqual(400, respuesta.StatusCode);
                Assert.AreEqual(1, mock.Invocations.Count);
            }
            else
            {
                //En caso de que la propiedad no exista la siguiente linea nos avisa que la prueba no salió bien
                Assert.AreEqual(1, 0);
            }
        }

        [TestMethod]
        public async Task SeRegistraNuevoComprador()
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
            var ExisteEstate = await ExistePropiedad(1, "Usuario1", nombreDB);
            if (ExisteEstate)
            {
                var context2 = ConstruirContext(nombreDB);
                var mock = new Mock<IGetUserInfo>();
                mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));
                var dto = new PostBuyersDTO()
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

                var controller = new BuyersController(context2, mapper, mock.Object);

                //Prueba
                var resultado = await controller.Post(1, dto);

                //Verificación
                var respuesta = resultado as CreatedAtRouteResult;
                var context3 = ConstruirContext(nombreDB);
                var Buyer = await context3.Buyers.FirstAsync();
                Assert.IsNotNull(Buyer);
                Assert.IsNotNull(respuesta);
                Assert.AreEqual(1234567890, Buyer.Dni);
                Assert.AreEqual(1, mock.Invocations.Count);
            }
            else
            {
                //En caso de que la propiedad no exista la siguiente linea nos avisa que la prueba no salió bien
                Assert.AreEqual(1, 0);
            }

            
        }
        [TestMethod]
        public async Task SeBorraElComprador()
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
            var mock = new Mock<IGetUserInfo>();
            mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));

            var controller = new BuyersController(context2, mapper, mock.Object);
            var ExisteEstate = await ExistePropiedad(1, "Usuario1", nombreDB);
            if (ExisteEstate)
            {
                //Prueba
                var resultado = await controller.Delete(1);

                //Verificación
                var respuesta = resultado as OkObjectResult;
                var context3 = ConstruirContext(nombreDB);
                var ExisteComprador = await context3.Buyers.AnyAsync();
                Assert.IsNotNull(respuesta);
                Assert.IsFalse(ExisteComprador);
                Assert.AreEqual(1, mock.Invocations.Count);
            }
            else
            {
                //En caso de que la propiedad no exista la siguiente linea nos avisa que la prueba no salió bien
                Assert.AreEqual(1, 0);
            }
        }
        [TestMethod]
        public async Task NoSePuedeBorrarCompradorQueNoExiste()
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

            var ExisteEstate = await ExistePropiedad(1, "Usuario1", nombreDB);
            if (ExisteEstate)
            {
                var context2 = ConstruirContext(nombreDB);
                var mock = new Mock<IGetUserInfo>();
                mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));

                var controller = new BuyersController(context2, mapper, mock.Object);
                //Prueba
                var resultado = await controller.Delete(1);

                //Verificación
                var respuesta = resultado as NotFoundObjectResult;
                Assert.IsNotNull(respuesta);
                Assert.AreEqual(1, mock.Invocations.Count);
            }
            else
            {
                //En caso de que la propiedad no exista la siguiente linea nos avisa que la prueba no salió bien
                Assert.AreEqual(1, 0);
            }
        }
        [TestMethod]
        public async Task NoSePuedeActualizarPatchACompradorQueNoExiste()
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

            var ExisteEstate = await ExistePropiedad(1, "Usuario1", nombreDB);
            if (ExisteEstate)
            {
                var context2 = ConstruirContext(nombreDB);
                var mock = new Mock<IGetUserInfo>();
                mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));
                var controller = new BuyersController(context2, mapper, mock.Object);
                var jsonPatch = new JsonPatchDocument<PatchBuyersDTO>();
                jsonPatch.Operations.Add(new Operation<PatchBuyersDTO>("replace", "/SecondName", null, "Juan"));
                //Prueba
                var resultado = await controller.Patch(jsonPatch, 1);

                //Verificación
                var respuesta = resultado as NotFoundObjectResult;
                Assert.IsNotNull(respuesta);
                Assert.AreEqual(1, mock.Invocations.Count);
            }
            else
            {
                //En caso de que la propiedad no exista la siguiente linea nos avisa que la prueba no salió bien
                Assert.AreEqual(1, 0);
            }
        }
 
        [TestMethod]
        public async Task PatchActualizaSoloUnCampo()
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
                SecondName = "Manuel",
                SecondSurName = "dsadsdas",
                IdBuyer = 1,
                IdEstate = 1
            });
            await context.SaveChangesAsync();

            var ExisteEstate = await ExistePropiedad(1, "Usuario1", nombreDB);
            if (ExisteEstate)
            {
                var context2 = ConstruirContext(nombreDB);
                var mock = new Mock<IGetUserInfo>();
                mock.Setup(x => x.GetId()).Returns(Task.FromResult("Usuario1"));
                
                var controller = new BuyersController(context2, mapper, mock.Object);
                
                var objectValidator = new Mock<IObjectModelValidator>();
                objectValidator.Setup(x => x.Validate(It.IsAny<ActionContext>(),
                    It.IsAny<ValidationStateDictionary>(),
                    It.IsAny<string>(),
                    It.IsAny<object>()));

                controller.ObjectValidator = objectValidator.Object;
                var jsonPatch = new JsonPatchDocument<PatchBuyersDTO>();
                jsonPatch.Operations.Add(new Operation<PatchBuyersDTO>("replace", "/SecondName", null, "Pablo"));
                
                //Prueba
                var resultado = await controller.Patch(jsonPatch, 1);

                //Verificación
                var respuesta = resultado as NoContentResult;
                var context3 = ConstruirContext(nombreDB);
                var BuyerActualizado = await context3.Buyers.FirstAsync();
                Assert.IsNotNull(respuesta);
                Assert.AreEqual("Pablo",BuyerActualizado.SecondName);
                Assert.AreEqual(1, mock.Invocations.Count);
            }
            else
            {
                //En caso de que la propiedad no exista la siguiente linea nos avisa que la prueba no salió bien
                Assert.AreEqual(1, 0);
            }
        }
    }
}
