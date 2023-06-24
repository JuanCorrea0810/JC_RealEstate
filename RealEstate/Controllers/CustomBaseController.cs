﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstate.DTO_s.EstatesDTO_s;
using RealEstate.Models;

namespace RealEstate.Controllers
{
    
    public class CustomBaseController : ControllerBase
    {
        private readonly RealEstateProjectContext context;
        private readonly IMapper mapper;

        public CustomBaseController(RealEstateProjectContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        // Clase que guardará los métodos más comúnes que se repiten en varios controladores
        protected async Task<ActionResult<List<GetEstatesDTO>>> ElUsuarioTienePropiedades(string IdUser)
        {
            var Estates = await context.Estates.Where(x => x.IdUser == IdUser).ToListAsync();
            if (Estates.Count == 0)
            {
                return NotFound("El usuario no ha registrado ninguna propiedad");
            }
            return mapper.Map<List<GetEstatesDTO>>(Estates);
        }

        protected async Task<ActionResult<GetEstatesDTO>> DevolverPropiedad(string IdUser, int IdEstate)
        {
            var Estate = await context.Estates.FirstOrDefaultAsync(x => x.IdEstate == IdEstate && x.IdUser == IdUser);
            if (Estate == null)
            {
                return NotFound("La propiedad no existe o el usuario no es dueño de dicha propiedad");
            }
            
            //Esto se hace para que una instancia de la Entidad ExisteEstate no quede atada a un contexto de EntityFramework
            //Y por eso se pueda hacer diferentes operaciones con esta entidad desde el mismo contexto HTTP
            context.Entry(Estate).State = EntityState.Detached;
            
            return mapper.Map<GetEstatesDTO>(Estate);
        }

        protected async Task<ActionResult<bool>> SaberSiExistePropiedad(string IdUser, int IdEstate)
        {
            var ExisteEstate = await context.Estates.AnyAsync(x => x.IdEstate == IdEstate && x.IdUser == IdUser);
            if (!ExisteEstate)
            {
                return NotFound("La propiedad no existe o el usuario no es dueño de dicha propiedad");
            }
            return ExisteEstate;
        }

        protected async Task<ActionResult<bool>> SaberSiExisteRenter(int IdRenter)
        {
            var Renter = await context.Renters.AnyAsync(x => x.IdRenter == IdRenter);
            if (!Renter)
            {
                return NotFound("No existe dicho Renter");
            }
            return Renter;
        }
        protected async Task<ActionResult<bool>> SaberSiHayRelacionEntreRenterYEstate(int IdEstate, int IdRenter)
        {
            var EstatesinRenters = await context.Renters.AnyAsync(x => x.IdEstate == IdEstate && x.IdRenter == IdRenter);
            if (!EstatesinRenters)
            {
                return NotFound($"No hay registros que coincidan con id de la propiedad: {IdEstate}, y el id del renter: {IdRenter}.");
            }
            return EstatesinRenters;
        }
        protected async Task<ActionResult<bool>> SaberSiExisteGuarantor(int Id)
        {
            var Guarantor = await context.Guarantors.AnyAsync(x => x.IdGuarantor == Id);
            if (!Guarantor)
            {
                return NotFound("No existe dicho Fiador");
            }
            return Guarantor;
        }

        protected async Task<ActionResult<bool>> SaberSiExisteMortgage(int IdEstate)
        {
            var Mortgage = await context.Mortgages.AnyAsync(x => x.IdEstate == IdEstate);
            if (!Mortgage)
            {
                return NotFound("La propiedad no tiene hipotecas registradas");
            }
            return Mortgage;
        }
        protected async Task<ActionResult<bool>> SaberSiHayRelacionEntreHipotecaYPropiedad(int IdEstate,int IdMortgage)
        {
            var ExistsEstateInMortgage = await context.Mortgages.AnyAsync(x => x.IdEstate == IdEstate && x.IdMortgage == IdMortgage);
            if (!ExistsEstateInMortgage)
            {
                return NotFound("La hipoteca no coincide con la propiedad");
            }
            return ExistsEstateInMortgage;
        }
        protected async Task<ActionResult<bool>> SaberSiExisteBuyer(int IdBuyer)
        {
            var ExistsBuyer = await context.Buyers.AnyAsync(x => x.IdBuyer == IdBuyer);
            if (!ExistsBuyer)
            {
                return NotFound("El Comprador no existe");
            }
            return ExistsBuyer;
        }
        protected async Task<ActionResult<bool>> SaberSiHayRelacionEntreBuyerYPropiedad(int IdEstate, int IdBuyer)
        {
            var ExistsEstateInBuyer = await context.Buyers.AnyAsync(x => x.IdBuyer == IdBuyer && x.IdEstate == IdEstate);
            if (!ExistsEstateInBuyer)
            {
                return NotFound("El comprador no coincide con esta propiedad");
            }
            return ExistsEstateInBuyer;
        }
    }
}
