using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstate.DTO_s.GuarantorDTO_s;
using RealEstate.Models;
using RealEstate.Utilities;

namespace RealEstate.Controllers
{
    [ApiController]
    [Route("api/Estates/{IdEstate:int}/Renters/{IdRenter:int}/Guarantors")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GuarantorController : CustomBaseController
    {
        private readonly RealEstateProjectContext context;
        private readonly IMapper mapper;
        private readonly IGetUserInfo getUser;

        public GuarantorController(RealEstateProjectContext context, IMapper mapper,
            IGetUserInfo getUser) : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.getUser = getUser;
        }
        [HttpGet("/api/Users/Guarantors", Name = "UsersAndTheirGuarantors")]
        public async Task<ActionResult<List<GetGuarantorWithRenterDTO>>> GetUsersAndTheirGuarantors()
        {
            var IdUser = await getUser.GetId();
            var _estates = await context.Estates.Where(x => x.IdUser == IdUser).Select(x => x.IdEstate).ToListAsync();
            if (_estates.Count == 0)
            {
                return NotFound("El usuario no tiene ninguna propiedad registrada");
            }
            var Renters = await context.Renters.Where(x => _estates.Contains(x.IdEstate)).Select(x => x.IdRenter).ToListAsync();

            if (Renters.Count == 0)
            {
                return NotFound("El usuario no tiene ningún Renter registrado");
            }
            var Guarantors = await context.Guarantors.Where(x => Renters.Contains(x.IdRenter)).ToListAsync();
            if (Guarantors.Count == 0)
            {
                return NotFound("El usuario no tiene Fiadores registrados");
            }
            return mapper.Map<List<GetGuarantorWithRenterDTO>>(Guarantors);
        }

        [HttpGet]
        public async Task<ActionResult<List<GetGuarantorDTO>>> GetGuarantorsOfRenter([FromRoute] int IdRenter, [FromRoute] int IdEstate)
        {
            var IdUser = await getUser.GetId();
            var ExisteEstate = await SaberSiExistePropiedad(IdUser, IdEstate);
            if (!ExisteEstate.Value)
            { return ExisteEstate.Result; }

            var ExisteRenter = await SaberSiExisteRenter(IdRenter);
            if (!ExisteRenter.Value)
            { return ExisteRenter.Result; }

            var HayRelacion = await SaberSiHayRelacionEntreRenterYEstate(IdEstate, IdRenter);
            if (!HayRelacion.Value)
            { return HayRelacion.Result; }

            var Guarantors = await context.Guarantors.Where(x => x.IdRenter == IdRenter).ToListAsync();
            if (Guarantors.Count == 0)
            {
                return NotFound("El Renter no tiene Guarantors registrados");
            }
            return mapper.Map<List<GetGuarantorDTO>>(Guarantors);

        }

        [HttpGet("{Id:int}", Name = "GetGuarantor")]
        public async Task<ActionResult<GetGuarantorDTO>> GetById([FromRoute] int Id, [FromRoute] int IdRenter, [FromRoute] int IdEstate)
        {
            var IdUser = await getUser.GetId();
            var ExisteEstate = await SaberSiExistePropiedad(IdUser, IdEstate);
            if (!ExisteEstate.Value)
            { return ExisteEstate.Result; }

            var ExisteRenter = await SaberSiExisteRenter(IdRenter);
            if (!ExisteRenter.Value)
            { return ExisteRenter.Result; }

            var HayRelacion = await SaberSiHayRelacionEntreRenterYEstate(IdEstate, IdRenter);
            if (!HayRelacion.Value)
            { return HayRelacion.Result; }

            var ExisteGuarantor = await SaberSiExisteGuarantor(Id);
            if (!ExisteGuarantor.Value)
            { return ExisteGuarantor.Result; }

            var Guarantor_Renter = await context.Guarantors.FirstOrDefaultAsync(x => x.IdGuarantor == Id && x.IdRenter == IdRenter);
            if (Guarantor_Renter == null)
            {
                return NotFound("No existe relacion entre este fiador y este Renter");
            }
            return mapper.Map<GetGuarantorDTO>(Guarantor_Renter);

        }

        [HttpPost]
        public async Task<ActionResult> Post(PostGuarantorDTO postGuarantorDTO, [FromRoute] int IdRenter, [FromRoute] int IdEstate)
        {
            var IdUser = await getUser.GetId();
            var ExisteEstate = await SaberSiExistePropiedad(IdUser, IdEstate);
            if (!ExisteEstate.Value)
            { return ExisteEstate.Result; }

            var ExisteRenter = await SaberSiExisteRenter(IdRenter);
            if (!ExisteRenter.Value)
            {
                return ExisteRenter.Result;
            }

            var HayRelacion = await SaberSiHayRelacionEntreRenterYEstate(IdEstate, IdRenter);
            if (!HayRelacion.Value)
            { return HayRelacion.Result; }

            var ExistsGuarantorWithSameDNI = await context.Guarantors.AnyAsync(x => x.IdRenter == IdRenter && x.Dni == postGuarantorDTO.Dni);
            if (ExistsGuarantorWithSameDNI)
            {
                return BadRequest($"Ya existe un Fiador para este renter con el DNI: {postGuarantorDTO.Dni}");
            }
            var guarantor = mapper.Map<Guarantor>(postGuarantorDTO);
            guarantor.IdRenter = IdRenter;
            context.Add(guarantor);
            await context.SaveChangesAsync();

            var GuarantorDTO = mapper.Map<GetGuarantorDTO>(guarantor);
            return CreatedAtRoute("GetGuarantor", new { IdEstate = IdEstate, IdRenter = IdRenter, Id = guarantor.IdGuarantor }, GuarantorDTO);

        }



        [HttpDelete("{Id:int}")]
        public async Task<ActionResult> Delete([FromRoute] int Id, [FromRoute] int IdRenter, [FromRoute] int IdEstate)
        {
            var IdUser = await getUser.GetId();
            var ExisteEstate = await SaberSiExistePropiedad(IdUser, IdEstate);
            if (!ExisteEstate.Value)
            { return ExisteEstate.Result; }

            var ExisteRenter = await SaberSiExisteRenter(IdRenter);
            if (!ExisteRenter.Value)
            { return ExisteRenter.Result; }

            var HayRelacion = await SaberSiHayRelacionEntreRenterYEstate(IdEstate, IdRenter);
            if (!HayRelacion.Value)
            { return HayRelacion.Result; }

            var ExisteGuarantor = await SaberSiExisteGuarantor(Id);
            if (!ExisteGuarantor.Value)
            { return ExisteGuarantor.Result; }

            var ExistsRenter_Guarantor = await context.Guarantors.FirstOrDefaultAsync(x => x.IdGuarantor == Id && x.IdRenter == IdRenter);
            if (ExistsRenter_Guarantor == null)
            {
                return NotFound("No existe registro que vincule a este fiador con este Renter");
            }

            context.Guarantors.Remove(ExistsRenter_Guarantor);
            await context.SaveChangesAsync();
            return Ok("Fiador eliminado");
        }

        [HttpPatch("{Id:int}")]
        public async Task<ActionResult> Patch(JsonPatchDocument<PatchGuarantorsDTO> jsonPatchDocument, [FromRoute] int IdEstate, [FromRoute] int IdRenter, [FromRoute] int Id)
        {
            if (jsonPatchDocument == null)
            {
                return BadRequest();
            }
            var IdUser = await getUser.GetId();
            var ExisteEstate = await SaberSiExistePropiedad(IdUser, IdEstate);
            if (!ExisteEstate.Value)
            { return ExisteEstate.Result; }

            var ExisteRenter = await SaberSiExisteRenter(IdRenter);
            if (!ExisteRenter.Value)
            { return ExisteRenter.Result; }

            var ExisteGuarantor = await SaberSiExisteGuarantor(Id);
            if (!ExisteGuarantor.Value)
            { return ExisteGuarantor.Result; }

            var Guarantor_Renter = await context.Guarantors.FirstOrDefaultAsync(x => x.IdGuarantor == Id && x.IdRenter == IdRenter);
            if (Guarantor_Renter == null)
            {
                return NotFound("No existe relacion entre este fiador y este Renter");
            }
            var GuarantorDTO = mapper.Map<PatchGuarantorsDTO>(Guarantor_Renter);
            jsonPatchDocument.ApplyTo(GuarantorDTO, ModelState);
            bool esValido = TryValidateModel(GuarantorDTO);
            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(GuarantorDTO, Guarantor_Renter);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
