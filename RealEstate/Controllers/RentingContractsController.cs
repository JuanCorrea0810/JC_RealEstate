﻿using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstate.DTO_s.RentingContractsDTO_s;
using RealEstate.Models;
using RealEstate.Utilities;

namespace RealEstate.Controllers
{
    [ApiController]
    [Route("api/Estates/{IdEstate:int}/Renters/{IdRenter:int}/RentingContract")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RentingContractsController : ControllerBase
    {
        private readonly RealEstateProjectContext context;
        private readonly IMapper mapper;
        private readonly GetUserInfo getUser;

        public RentingContractsController(RealEstateProjectContext context, IMapper mapper,
            GetUserInfo getUser)
        {
            this.context = context;
            this.mapper = mapper;
            this.getUser = getUser;
        }
        [HttpGet("/api/Users/RentingContracts", Name = "UsersAndTheirRentingContracts")]
        public async Task<ActionResult<List<GetRentingContractsDTO>>> GetUsersAndTheirRentingContracts()
        {
            var IdUser = await getUser.GetId();
            var _estates = await context.Estates.Where(x => x.IdUser == IdUser).Select(x => x.IdEstate).ToListAsync();
            if (_estates.Count == 0)
            {
                return NotFound("El usuario no tiene ninguna propiedad registrada");
            }
            var Renters = await context.Renters.Where(x => _estates.Contains(x.IdEstate)).Select(x => x.IdEstate).ToListAsync();

            if (Renters.Count == 0)
            {
                return NotFound("El usuario no tiene ningún Renter registrado");
            }
            var RentingContracts = await context.Rentingcontracts.Where(x => Renters.Contains(x.IdEst)).ToListAsync();
            if (RentingContracts.Count == 0)
            {
                return NotFound("EL usuario no ha registrado ningún contrato de arrendamiento");
            }
            return mapper.Map<List<GetRentingContractsDTO>>(RentingContracts);
        }
        [HttpGet]
        public async Task<ActionResult<List<GetRentingContractsDTO>>> Get([FromRoute] int IdEstate, [FromRoute] int IdRenter)
        {
            var IdUser = await getUser.GetId();
            var Estates = await context.Estates.AnyAsync(x => x.IdEstate == IdEstate && x.IdUser == IdUser);
            if (!Estates)
            {
                return NotFound("No existe dicha propiedad o no le pertenece");
            }
            var ExistsRenter = await context.Renters.AnyAsync(x => x.IdRenter == IdRenter);
            if (!ExistsRenter)
            {
                return NotFound("El Renter no existe");
            }
            var ExistsEstateInRenter = await context.Renters.AnyAsync(x => x.IdRenter == IdRenter && x.IdEstate == IdEstate);
            if (!ExistsEstateInRenter)
            {
                return NotFound("El Renter no coincide con esta propiedad");
            }
            var RentingContracts = await context.Rentingcontracts.Where(x => x.IdRenter == IdRenter && x.IdEst == IdEstate).ToListAsync();
            if (RentingContracts.Count == 0)
            {
                return NotFound("No existe ningpun contrato de arrendamiento registrado");
            }
            return mapper.Map<List<GetRentingContractsDTO>>(RentingContracts);
        }

        [HttpGet("{id:int}", Name = "GetRentingContract")]
        public async Task<ActionResult<GetRentingContractsDTO>> GetById(int IdRenter, int IdEstate, int id)
        {
            var IdUser = await getUser.GetId();
            var Estates = await context.Estates.AnyAsync(x => x.IdEstate == IdEstate && x.IdUser == IdUser);
            if (!Estates)
            {
                return NotFound("No existe dicha propiedad o no le pertenece");
            }
            var ExistsRenter = await context.Renters.AnyAsync(x => x.IdRenter == IdRenter);
            if (!ExistsRenter)
            {
                return NotFound("El Renter no existe");
            }
            var ExistsEstateInRenter = await context.Renters.AnyAsync(x => x.IdRenter == IdRenter && x.IdEstate == IdEstate);
            if (!ExistsEstateInRenter)
            {
                return NotFound("El Renter no coincide con esta propiedad");
            }
            var ExistsContract = await context.Rentingcontracts.AnyAsync(x => x.IdRentingContract == id);
            if (!ExistsContract)
            {
                return NotFound("No existe dicho contrato");
            }
            var RentingContracts = await context.Rentingcontracts.FirstOrDefaultAsync(x => x.IdRenter == IdRenter && x.IdEst == IdEstate && x.IdRentingContract == id);
            if (RentingContracts == null)
            {
                return NotFound("Este contrato no está asociado a este renter");
            }
            return mapper.Map<GetRentingContractsDTO>(RentingContracts);
        }



        [HttpPost]
        public async Task<ActionResult> Post(PostRentingContractsDTO postRentingContract, int IdRenter, int IdEstate)
        {
            var IdUser = await getUser.GetId();
            var Estates = await context.Estates.AnyAsync(x => x.IdEstate == IdEstate && x.IdUser == IdUser);
            if (!Estates)
            {
                return NotFound("No existe dicha propiedad o no le pertenece");
            }
            var ExistsRenter = await context.Renters.AnyAsync(x => x.IdRenter == IdRenter);
            if (!ExistsRenter)
            {
                return NotFound("El Renter no existe");
            }
            var ExistsEstateInRenter = await context.Renters.FirstOrDefaultAsync(x => x.IdRenter == IdRenter && x.IdEstate == IdEstate);
            if (ExistsEstateInRenter == null)
            {
                return NotFound("El Renter no coincide con esta propiedad");
            }
            if (ExistsEstateInRenter.Active == false)
            {
                return BadRequest("No se puede agregar un contrato de arrendamiento a un Renter que no está activo");
            }
            var RentingContracts = mapper.Map<Rentingcontract>(postRentingContract);
            RentingContracts.IdEst = IdEstate;
            RentingContracts.IdRenter = IdRenter;
            context.Add(RentingContracts);
            await context.SaveChangesAsync();

            var RentingContractDTO = mapper.Map<GetRentingContractsDTO>(RentingContracts);
            return CreatedAtRoute("GetRentingContract", new { IdEstate = IdEstate, IdRenter = IdRenter, Id = RentingContracts.IdRentingContract }, RentingContractDTO);

        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id, int IdRenter, int IdEstate)
        {
            var IdUser = await getUser.GetId();
            var Estates = await context.Estates.AnyAsync(x => x.IdEstate == IdEstate && x.IdUser == IdUser);
            if (!Estates)
            {
                return NotFound("No existe dicha propiedad o no le pertenece");
            }
            var ExistsRenter = await context.Renters.AnyAsync(x => x.IdRenter == IdRenter);
            if (!ExistsRenter)
            {
                return NotFound("El Renter no existe");
            }
            var ExistsEstateInRenter = await context.Renters.AnyAsync(x => x.IdRenter == IdRenter && x.IdEstate == IdEstate);
            if (!ExistsEstateInRenter)
            {
                return NotFound("El Renter no coincide con esta propiedad");
            }
            var ExistsContract = await context.Rentingcontracts.AnyAsync(x => x.IdRentingContract == id);
            if (!ExistsContract)
            {
                return NotFound("No existe dicho contrato");
            }
            var RentingContracts = await context.Rentingcontracts.FirstOrDefaultAsync(x => x.IdRenter == IdRenter && x.IdEst == IdEstate && x.IdRentingContract == id);
            if (RentingContracts == null)
            {
                return NotFound("El contrato no está asociado a este renter");
            }
            context.Rentingcontracts.Remove(RentingContracts);
            await context.SaveChangesAsync();
            return Ok("Contrato eliminado");
        }

        [HttpPatch("{Id:int}")]
        public async Task<ActionResult> Patch(JsonPatchDocument<PatchRentingContractsDTO> jsonPatchDocument, [FromRoute] int IdEstate, [FromRoute] int IdRenter, [FromRoute] int Id)
        {
            if (jsonPatchDocument == null)
            {
                return BadRequest();
            }

            var IdUser = await getUser.GetId();
            var Estates = await context.Estates.AnyAsync(x => x.IdEstate == IdEstate && x.IdUser == IdUser);
            if (!Estates)
            {
                return NotFound("No existe dicha propiedad o no le pertenece");
            }
            var Renter = await context.Renters.AnyAsync(x => x.IdEstate == IdEstate && x.IdRenter == IdRenter);
            if (!Renter)
            {
                return NotFound("No existe el Renter o la propiedad no le pertenece");
            }
            var ExistsContract = await context.Rentingcontracts.AnyAsync(x => x.IdRentingContract == Id);
            if (!ExistsContract)
            {
                return NotFound("No existe dicho contrato");
            }
            var RentingContract = await context.Rentingcontracts.FirstOrDefaultAsync(x => x.IdRenter == IdRenter && x.IdEst == IdEstate && x.IdRentingContract == Id);
            if (RentingContract == null)
            {
                return NotFound("El contrato no está asociado a este renter");
            }


            var RentingContractDTO = mapper.Map<PatchRentingContractsDTO>(RentingContract);
            jsonPatchDocument.ApplyTo(RentingContractDTO, ModelState);
            bool esValido = TryValidateModel(RentingContractDTO);
            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(RentingContractDTO, RentingContract);
            await context.SaveChangesAsync();
            return NoContent();

        }



    }
}
