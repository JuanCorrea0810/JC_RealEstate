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
    public class EstatesController : CustomBaseController
    {
        private readonly RealEstateProjectContext context;
        private readonly IMapper mapper;
        private readonly IGetUserInfo getUser;

        public EstatesController(RealEstateProjectContext context, IMapper mapper,
            IGetUserInfo getUser) : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.getUser = getUser;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Owner,Admin")]
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
            var Estates = await ElUsuarioTienePropiedades(IdUser);
            return Estates;
        }



        [HttpGet("{IdEstate:int}", Name = "GetEstate")]
        public async Task<ActionResult<GetEstatesDTO>> GetById([FromRoute] int IdEstate)
        {

            var IdUser = await getUser.GetId();
            var _Estate = await DevolverPropiedad(IdUser, IdEstate);
            return _Estate;
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
            var AliasDB = db.ToCharArray();
            var AliasChar = Alias.ToCharArray();

            if (AliasChar.Length != AliasDB.Length)
            {
                return NotFound($"Ninguna de sus propiedades tiene el Alias: {Alias} , por favor revise que la información sea correcta.");
            }
            var Result = await context.Estates.FirstOrDefaultAsync(x => x.Alias.Contains(Alias) && x.IdUser == IdUser);

            return mapper.Map<GetEstatesDTO>(Result);
        }



        [HttpPost]
        public async Task<ActionResult> Post(PostEstatesDTO postEstateDto)
        {

            var IdUser = await getUser.GetId();

            var User = await context.Estates.FirstOrDefaultAsync(x => x.Alias.Contains(postEstateDto.Alias) && x.IdUser == IdUser);
            if (User != null)
            {
                var AliasDB = User.Alias.ToCharArray();
                var SameAlias = postEstateDto.Alias.ToCharArray();

                if (SameAlias.Length == AliasDB.Length)
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

            var Propiedad = await DevolverPropiedad(IdUser, IdEstate);

            //Si la Propiedad no existe entonces el método de arriba nos manda un ActionResult con el código 404 NotFound
            var respuestaActionResult = Propiedad.Result;
            if (respuestaActionResult == null)
            {
                //Si no manda codigo 404 significa que nos devolvió una propiedad y podemos trabajar con ella
                var EstateDTO = Propiedad.Value;
                var Estate = mapper.Map<Estate>(EstateDTO);
                context.Remove(Estate);
                await context.SaveChangesAsync();
                return Ok("Propiedad eliminada");
            }
            return respuestaActionResult;

        }

        [HttpPatch("{IdEstate:int}")]
        public async Task<ActionResult> Patch(JsonPatchDocument<PatchEstatesDTO> jsonPatchDocument, [FromRoute] int IdEstate)
        {
            if (jsonPatchDocument == null)
            {
                return BadRequest();
            }
            var IdUser = await getUser.GetId();

            //Si propiedad no existe se retorna 404
            var Propiedad = await DevolverPropiedad(IdUser, IdEstate);
            var EstateDTO = Propiedad.Value;
            if (EstateDTO != null)
            {
                var Estate = mapper.Map<Estate>(EstateDTO);


                var CampoActualizar = jsonPatchDocument.Operations[0].path;
                var Operacion = jsonPatchDocument.Operations[0].op;
                var Valor = jsonPatchDocument.Operations[0].value.ToString();

                //Saber si el usuario quiere actualizar es el campo "Alias" y verificar que no se repita 
                if (CampoActualizar == "/Alias" && Operacion == "replace")
                {

                    var Entidad = await context.Estates.FirstOrDefaultAsync(x => x.Alias.Contains(Valor) && x.IdUser == IdUser);


                    if (Entidad != null)
                    {
                        var AliasDB = Entidad.Alias.ToCharArray();
                        var SameAlias = Valor.ToCharArray();

                        if (SameAlias.Length == AliasDB.Length)
                        {
                            return BadRequest($"El usuario ya registró una propiedad previamente con el alias : {Valor}");
                        }
                    }

                }

                var EstatesDTO = mapper.Map<PatchEstatesDTO>(Estate);
                jsonPatchDocument.ApplyTo(EstatesDTO, ModelState);

                bool esValido = TryValidateModel(EstateDTO);
                if (!esValido)
                {
                    return BadRequest(ModelState);
                }

                mapper.Map(EstatesDTO, Estate);
                context.Entry(Estate).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return NoContent();

            }
            return Propiedad.Result;

        }
        

    }
}
