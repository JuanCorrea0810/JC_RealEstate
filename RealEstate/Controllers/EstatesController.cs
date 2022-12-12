using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstate.DTO_s.EstatesDTO_s;
using RealEstate.Models;
using RealEstate.Utilities;

namespace RealEstate.Controllers
{
    
    [ApiController]
    [Route("api/Estates")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EstatesController : ControllerBase
    {
            private readonly RealEstateProjectContext context;
            private readonly IMapper mapper;
            private readonly IGetUserInfo getUser;

        public EstatesController(RealEstateProjectContext context, IMapper mapper,
            IGetUserInfo getUser)
        {
            this.context = context;
            this.mapper = mapper; 
            this.getUser = getUser;
        }

        [AllowAnonymous]
        [HttpGet("ListOfEstates")]
        public async Task<ActionResult<List<GetEstatesDTO>>> GetAllEstates() 
        {
            var Estates = await context.Estates.ToListAsync();
            return mapper.Map<List<GetEstatesDTO>>(Estates);
        }

        [HttpGet(Name = "UserAndTheirEstates")]
        public async Task<ActionResult<List<GetEstatesDTO>>> GetUserAndTheirEstates()
        {
            
            var IdUser = await getUser.GetId();
            var Estates = await context.Estates.Where(x => x.IdUser == IdUser).ToListAsync();
            if (Estates.Count == 0)
            {
                return NotFound("El usuario no ha registrado ninguna propiedad");
            }
            return mapper.Map<List<GetEstatesDTO>>(Estates);
        }



        [HttpGet("{IdEstate:int}", Name = "GetEstate")]
        public async Task<ActionResult<GetEstatesDTO>> GetById([FromRoute] int IdEstate)
        {

            var IdUser = await getUser.GetId();
            var _Estate = await context.Estates.FirstOrDefaultAsync(x => x.IdEstate == IdEstate && x.IdUser == IdUser);
            if (_Estate == null)
            {
                return NotFound("La propiedad no existe o el usuario no es dueño de dicha propiedad");
            }
            return mapper.Map<GetEstatesDTO>(_Estate);
        }

        [HttpGet("{Alias}")]
        public async Task<ActionResult<GetEstatesDTO>> GetByAlias([FromRoute] string Alias)
        {
            var IdUser = await getUser.GetId();

            var db = await context.Estates.Where(x => x.Alias.Contains(Alias) && x.IdUser == IdUser).Select(x => x.Alias).FirstOrDefaultAsync();
            if (db == null)
            {
                return NotFound($"El usuario no ha registrado ninguna propiedad con el Alias: {Alias}");
            }
            var AliasArray = db.ToCharArray();
            var Char = Alias.ToCharArray();

            if (Char.Length != AliasArray.Length)
            {
                return BadRequest($"Ninguna de sus propiedades tiene el Alias: {Alias} , por favor revise que la información sea correcta.");
            }
            var Result = await context.Estates.FirstOrDefaultAsync(x => x.Alias.Contains(Alias) && x.IdUser == IdUser);

            return mapper.Map<GetEstatesDTO>(Result);
        }



        [HttpPost]
        public async Task<ActionResult> Post(PostEstatesDTO postEstateDto)
        {

            var IdUser = await getUser.GetId();

            var Alias = await context.Estates.FirstOrDefaultAsync(x => x.Alias.Contains(postEstateDto.Alias) && x.IdUser == IdUser);
            if (Alias != null)
            {
                var CharAlias = Alias.Alias.ToCharArray();
                var SameAlias = postEstateDto.Alias.ToCharArray();

                if (SameAlias.Length == CharAlias.Length)
                {
                    return BadRequest($"El usuario ya registró una propiedad previamente con el alias : {postEstateDto.Alias}");
                }
            }
            var Estate = mapper.Map<Estate>(postEstateDto);
            Estate.IdUser = IdUser;
            context.Add(Estate);
            await context.SaveChangesAsync();

            var EstateDTO = mapper.Map<GetEstatesDTO>(Estate);
            return CreatedAtRoute("GetEstate", new { IdEstate = Estate.IdEstate }, EstateDTO);
        }



        [HttpDelete("{IdEstate:int}")]
        public async Task<ActionResult> Delete([FromRoute] int IdEstate)
        {
            var IdUser = await getUser.GetId();

            var _Estate = await context.Estates.FirstOrDefaultAsync(x => x.IdEstate == IdEstate && x.IdUser == IdUser);
            if (_Estate == null)
            {
                return NotFound("La propiedad no existe o el usuario no es dueño de dicha propiedad");
            }
            context.Estates.Remove(_Estate);
            await context.SaveChangesAsync();
            return Ok("Propiedad eliminada");
        }

        [HttpPatch("{IdEstate:int}")]
        public async Task<ActionResult> Patch(JsonPatchDocument<PatchEstatesDTO> jsonPatchDocument, [FromRoute] int IdEstate)
        {
            if (jsonPatchDocument == null)
            {
                return BadRequest();
            }
            var IdUser = await getUser.GetId();

            var _Estate = await context.Estates.FirstOrDefaultAsync(x => x.IdEstate == IdEstate && x.IdUser == IdUser);
            if (_Estate == null)
            {
                return NotFound("La propiedad no existe o el usuario no es dueño de dicha propiedad");
            }

            //Saber si el usuario quiere actualizar el campos "Alias" y verificar que no repita nombre
            if (jsonPatchDocument.Operations[0].path == "/Alias" && jsonPatchDocument.Operations[0].op == "replace")
            {

                var Alias = await context.Estates.FirstOrDefaultAsync(x => x.Alias.Contains(jsonPatchDocument.Operations[0].value.ToString()) && x.IdUser == IdUser);


                if (Alias != null)
                {
                    var CharAlias = Alias.Alias.ToCharArray();
                    var SameAlias = jsonPatchDocument.Operations[0].value.ToString().ToCharArray();

                    if (SameAlias.Length == CharAlias.Length)
                    {
                        return BadRequest($"El usuario ya registró una propiedad previamente con el alias : {jsonPatchDocument.Operations[0].value.ToString()}");
                    }
                }

            }

            var EstateDTO = mapper.Map<PatchEstatesDTO>(_Estate);
            jsonPatchDocument.ApplyTo(EstateDTO, ModelState);

            bool esValido = TryValidateModel(EstateDTO);
            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(EstateDTO, _Estate);
            await context.SaveChangesAsync();
            return NoContent();

        }

  
    }
}
