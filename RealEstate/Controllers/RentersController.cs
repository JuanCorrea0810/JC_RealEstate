using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstate.DTO_s.RentersDTO_s;
using RealEstate.Models;
using RealEstate.Utilities;

namespace RealEstate.Controllers
{
    [ApiController]
    [Route("api/Estates/{IdEstate:int}/Renters")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RentersController : ControllerBase
    {
        private readonly RealEstateProjectContext context;
        private readonly IMapper mapper;
        private readonly IGetUserInfo getUser;

        public RentersController(RealEstateProjectContext context, IMapper mapper,
            IGetUserInfo getUser)
        {
            this.context = context;
            this.mapper = mapper;
            this.getUser = getUser;
        }
        [HttpGet("/api/Users/Renters", Name = "UsersAndTheirRenters")]
        public async Task<ActionResult<List<GetRentersDTO>>> GetUsersAndTheirRenters()
        {
            var IdUser = await getUser.GetId();
            var _estates = await context.Estates.Where(x => x.IdUser == IdUser).Select(x => x.IdEstate).ToListAsync();
            if (_estates.Count == 0)
            {
                return NotFound("El usuario no tiene ninguna propiedad registrada");
            }
            var Renters = await context.Renters.Where(x => _estates.Contains(x.IdEstate)).ToListAsync();

            if (Renters.Count == 0)
            {
                return NotFound("El usuario no tiene ningún Renter registrado");
            }
            return mapper.Map<List<GetRentersDTO>>(Renters);
        }
        [HttpGet]
        public async Task<ActionResult<List<GetRentersDTO>>> Get(int IdEstate)
        {
            var IdUser = await getUser.GetId();
            var Estates = await context.Estates.AnyAsync(x => x.IdEstate == IdEstate && x.IdUser == IdUser);
            if (!Estates)
            {
                return NotFound("No existe dicha propiedad o no le pertenece");
            }
            var Renters = await context.Renters.Where(x => x.IdEstate == IdEstate).ToListAsync();
            if (Renters.Count == 0)
            {
                return NotFound("La propiedad no está arrendada");
            }

            return mapper.Map<List<GetRentersDTO>>(Renters);
        }

        [HttpGet("{Id:int}", Name = "GetRenter")]
        public async Task<ActionResult<GetRentersDTO>> GetById(int IdEstate, int Id)
        {
            var IdUser = await getUser.GetId();
            var ExistsEstate = await context.Estates.AnyAsync(x => x.IdEstate == IdEstate && x.IdUser == IdUser);
            if (!ExistsEstate)
            {
                return NotFound("La propiedad no existe o no le pertenece");
            }
            var EstatesinRenters = await context.Renters.AnyAsync(x => x.IdEstate == IdEstate);
            if (!EstatesinRenters)
            {
                return NotFound("La propiedad no está arrendada");
            }
            var ExistsRenter = await context.Renters.AnyAsync(x => x.IdRenter == Id);
            if (!ExistsRenter)
            {
                return NotFound("No existe el Renter");
            }
            var Renter = await context.Renters.FirstOrDefaultAsync(x => x.IdRenter == Id && x.IdEstate == IdEstate);
            if (Renter == null)
            {
                return NotFound("No hay registros de que esta persona haya arrendado esta propiedad");
            }
            return mapper.Map<GetRentersDTO>(Renter);
        }




        [HttpPost]
        public async Task<ActionResult> Post(int IdEstate, PostRentersDTO postRenterDTO)
        {
            var IdUser = await getUser.GetId();
            var _Estate = context.Estates.FirstOrDefault(x => x.IdEstate == IdEstate && x.IdUser == IdUser);
            if (_Estate == null)
            {
                return BadRequest("Se debe ingresar una propiedad válida");
            }
            if (_Estate.Sold == true)
            {
                return BadRequest("No se puede alquilar una propiedad que ya ha sido vendida previamente");
            }
            var RenterWithSameDNI = await context.Renters.AnyAsync(x => x.IdEstate == IdEstate && x.Dni == postRenterDTO.Dni);
            if (RenterWithSameDNI)
            {
                return BadRequest($"Ya se tiene un Renter con el DNI: {postRenterDTO.Dni} ");
            }
            var _Renter = mapper.Map<Renter>(postRenterDTO);
            _Renter.IdEstate = IdEstate;
            context.Add(_Renter);
            await context.SaveChangesAsync();
            string query = $"update Renter set Active = 0 where Id_Estate = {_Renter.IdEstate} AND Id_Renter != {_Renter.IdRenter}";
            await context.Database.ExecuteSqlRawAsync(query);

            var RenterDTO = mapper.Map<GetRentersDTO>(_Renter);
            return CreatedAtRoute("GetRenter", new { IdEstate = IdEstate, Id = _Renter.IdRenter }, RenterDTO);
        }



        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int IdEstate, int id)
        {
            var IdUser = await getUser.GetId();
            var ExistsEstate = await context.Estates.AnyAsync(x => x.IdEstate == IdEstate && x.IdUser == IdUser);
            if (!ExistsEstate)
            {
                return NotFound("La propiedad no existe o no le pertenece");
            }
            var ExistsRenter = await context.Renters.AnyAsync(x => x.IdRenter == id);
            if (!ExistsRenter)
            {
                return NotFound("No existe registro del renter");
            }
            var ExistsEstateInRenters = await context.Renters.FirstOrDefaultAsync(x => x.IdRenter == id && x.IdEstate == IdEstate);
            if (ExistsEstateInRenters == null)
            {
                return NotFound($"No hay registros que coincidan con id de la propiedad: {IdEstate}, y el id del renter: {id}.");
            }
            context.Renters.Remove(ExistsEstateInRenters);
            await context.SaveChangesAsync();
            return Ok("Renter eliminado");
        }

        [HttpPatch("{IdRenter:int}")]
        public async Task<ActionResult> Patch(JsonPatchDocument<PatchRentersDTO> jsonPatchDocument, [FromRoute] int IdEstate, [FromRoute] int IdRenter)
        {
            if (jsonPatchDocument == null)
            {
                return BadRequest();
            }
            var IdUser = await getUser.GetId();

            var _Estate = await context.Estates.AnyAsync(x => x.IdEstate == IdEstate && x.IdUser == IdUser);
            if (!_Estate)
            {
                return NotFound("La propiedad no existe o no le pertenece");
            }
            var Renter = await context.Renters.FirstOrDefaultAsync(x => x.IdEstate == IdEstate && x.IdRenter == IdRenter);
            if (Renter == null)
            {
                return NotFound("No existe el Renter o la propiedad no le pertenece");
            }

            if (jsonPatchDocument.Operations[0].path == "/Active" && jsonPatchDocument.Operations[0].value.ToString() == "true")
            {
                string query = $"update Renter set Active = 0 where Id_Estate = {IdEstate} AND Id_Renter != {IdRenter}";
                await context.Database.ExecuteSqlRawAsync(query);
            }

            var RenterDTO = mapper.Map<PatchRentersDTO>(Renter);
            jsonPatchDocument.ApplyTo(RenterDTO, ModelState);
            bool esValido = TryValidateModel(RenterDTO);
            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(RenterDTO, Renter);
            await context.SaveChangesAsync();
            return NoContent();

        }
    }
}
