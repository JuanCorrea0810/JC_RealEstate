using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstate.DTO_s.MortgagesDTO_s;
using RealEstate.Models;
using RealEstate.Utilities;

namespace RealEstate.Controllers
{
    [ApiController]
    [Route("api/Estates/{IdEstate:int}/Mortgage")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MortgagesController : CustomBaseController
    {
        private readonly RealEstateProjectContext context;
        private readonly IMapper mapper;
        private readonly IGetUserInfo getUser;

        public MortgagesController(RealEstateProjectContext context, IMapper mapper,
            IGetUserInfo getUser):base(context,mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.getUser = getUser;
        }
        [HttpGet("/api/Users/Mortgages", Name = "UsersAndTheirMortgages")]
        public async Task<ActionResult<List<GetMorgagesDTO>>> GetUserAndTheirMortgages()
        {
            var IdUser = await getUser.GetId();
            var Mortgages = await context.Mortgages.Where(x => x.IdUser == IdUser).ToListAsync();
            if (Mortgages.Count == 0)
            {
                return NotFound("El usuario no ha registrado ninguna hipoteca");
            }
            return mapper.Map<List<GetMorgagesDTO>>(Mortgages);
        }

        [HttpGet(Name = "GetMortgage")]
        public async Task<ActionResult<GetMorgagesDTO>> Get([FromRoute] int IdEstate)
        {
            var IdUser = await getUser.GetId();
            var ExisteEstate = await SaberSiExistePropiedad(IdUser, IdEstate);
            if (ExisteEstate.Value)
            {
                var Mortgage = await context.Mortgages.FirstOrDefaultAsync(x => x.IdEstate == IdEstate);
                if (Mortgage == null)
                {
                    return NotFound("La propiedad no tiene ninguna hipoteca registrada");
                }
                return mapper.Map<GetMorgagesDTO>(Mortgage);
            }
            return ExisteEstate.Result;      
        }



        [HttpPost]
        public async Task<ActionResult> Post(PostMortgagesDTO postMortgage, [FromRoute] int IdEstate)
        {
            var IdUser = await getUser.GetId();
            var ExisteEstate = await SaberSiExistePropiedad(IdUser, IdEstate);
            if (ExisteEstate.Value)
            {
                var ExistsMortgage = await context.Mortgages.AnyAsync(x => x.IdEstate == IdEstate);
                if (ExistsMortgage)
                {
                    return BadRequest("Ya dicha propiedad tiene una hipoteca registrada");
                }
                var Mortgage = mapper.Map<Mortgage>(postMortgage);
                Mortgage.IdEstate = IdEstate;
                Mortgage.IdUser = IdUser;
                context.Add(Mortgage);
                await context.SaveChangesAsync();

                var MortgageDTO = mapper.Map<GetMorgagesDTO>(Mortgage);
                return CreatedAtRoute("GetMortgage", new { IdEstate = IdEstate }, MortgageDTO);
            }
            return ExisteEstate.Result;

                
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int IdEstate)
        {
            var IdUser = await getUser.GetId();
            var ExisteEstate = await SaberSiExistePropiedad(IdUser, IdEstate);
            if (ExisteEstate.Value)
            {
                var ExistsMortgage = await context.Mortgages.FirstOrDefaultAsync(x => x.IdEstate == IdEstate);
                if (ExistsMortgage == null)
                {
                    return BadRequest("La propiedad no tiene una hipoteca registrada");
                }
                context.Mortgages.Remove(ExistsMortgage);
                await context.SaveChangesAsync();
                return Ok("Registro eliminado");
            }
            return ExisteEstate.Result;
                
        }

        [HttpPatch]
        public async Task<ActionResult> Patch(JsonPatchDocument<PatchMortgagesDTO> jsonPatchDocument, [FromRoute] int IdEstate)
        {
            if (jsonPatchDocument == null)
            {
                return BadRequest();
            }

            var IdUser = await getUser.GetId();
            var ExisteEstate = await SaberSiExistePropiedad(IdUser, IdEstate);
            if (ExisteEstate.Value)
            {
                var Mortgage = await context.Mortgages.FirstOrDefaultAsync(x => x.IdEstate == IdEstate);
                if (Mortgage == null)
                {
                    return NotFound("La propiedad no tiene ninuna hipoteca asociada");
                }

                var MortgageDTO = mapper.Map<PatchMortgagesDTO>(Mortgage);
                jsonPatchDocument.ApplyTo(MortgageDTO, ModelState);
                bool esValido = TryValidateModel(MortgageDTO);
                if (!esValido)
                {
                    return BadRequest(ModelState);
                }

                mapper.Map(MortgageDTO, Mortgage);
                await context.SaveChangesAsync();
                return NoContent();
            }
            return ExisteEstate.Result;
        }
    }
}
