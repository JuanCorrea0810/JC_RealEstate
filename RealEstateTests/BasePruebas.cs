using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RealEstate.Models;
using RealEstate.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace PeliculasAPI.Tests
{
    public class BasePruebas
    {
        //protected string usuarioPorDefectoId = "9722b56a-77ea-4e41-941d-e319b6eb3712";
        //protected string usuarioPorDefectoEmail = "ejemplo@hotmail.com";

        protected RealEstateProjectContext ConstruirContext(string nombreDB)
        {
            var opciones = new DbContextOptionsBuilder<RealEstateProjectContext>()
                .UseInMemoryDatabase(nombreDB).Options;

            var dbContext = new RealEstateProjectContext(opciones);
            return dbContext;
        }

        protected IMapper ConfigurarAutoMapper()
        {
            var config = new MapperConfiguration(options =>
            {
                
                options.AddProfile(new AutoMapperProfile());
            });

            return config.CreateMapper();
        }

        //protected ControllerContext ConstruirControllerContext()
        //{
        //    var usuario = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        //    {
        //        new Claim(ClaimTypes.Name, usuarioPorDefectoEmail),
        //        new Claim(ClaimTypes.Email, usuarioPorDefectoEmail),
        //        new Claim(ClaimTypes.NameIdentifier, usuarioPorDefectoId)
        //    }));

        //    return new ControllerContext()
        //    {
        //        HttpContext = new DefaultHttpContext() { User = usuario }
        //    };
        //}

        //protected WebApplicationFactory<Startup> ConstruirWebApplicationFactory(string nombreBD,
        //    bool ignorarSeguridad = true)
        //{
        //    var factory = new WebApplicationFactory<Startup>();

        //    factory = factory.WithWebHostBuilder(builder =>
        //    {
        //        builder.ConfigureTestServices(services =>
        //        {
        //            var descriptorDBContext = services.SingleOrDefault(d =>
        //            d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

        //            if (descriptorDBContext != null)
        //            {
        //                services.Remove(descriptorDBContext);
        //            }

        //            services.AddDbContext<ApplicationDbContext>(options => 
        //            options.UseInMemoryDatabase(nombreBD));

        //            if (ignorarSeguridad)
        //            {
        //                services.AddSingleton<IAuthorizationHandler, AllowAnonymousHandler>();

        //                services.AddControllers(options =>
        //                {
        //                    options.Filters.Add(new UsuarioFalsoFiltro());
        //                });
        //            }
        //        });
        //    });

        //    return factory;
        //}
    }
}
