using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstate.DTO_s.BuyersDTO_s;
using RealEstate.Models;
using RealEstate.Utilities;

namespace RealEstate.Controllers
{
    [ApiController]
    [Route("api/Estates/{IdEstate:int}/Buyer")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BuyersController : CustomBaseController
    {
        private readonly RealEstateProjectContext context;
        private readonly IMapper mapper;
        private readonly IGetUserInfo getUser;

        public BuyersController(RealEstateProjectContext context, IMapper mapper,
            IGetUserInfo getUser) : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.getUser = getUser;
        }

        [HttpGet("/api/Users/Buyers", Name = "UsersAndTheirBuyers")]
        public async Task<ActionResult<List<GetBuyersDTO>>> GetUsersAndTheirBuyers()
        {
            var IdUser = await getUser.GetId();

            var _estates = await context.Estates.Where(x => x.IdUser == IdUser).Select(x => x.IdEstate).ToListAsync();
            if (_estates.Count == 0)
            {
                return NotFound("El usuario no tiene ninguna propiedad registrada");
            }
            var Buyers = await context.Buyers.Where(x => _estates.Contains(x.IdEstate)).ToListAsync();

            if (Buyers.Count == 0)
            {
                return NotFound("El usuario no tiene ningún comprador registrado");
            }
            return mapper.Map<List<GetBuyersDTO>>(Buyers);

        }


        [HttpGet(Name = "GetBuyer")]
        public async Task<ActionResult<GetBuyersDTO>> GetById([FromRoute] int IdEstate)
        {
            var IdUser = await getUser.GetId();
            var ExisteEstate = await SaberSiExistePropiedad(IdUser, IdEstate);
            if (ExisteEstate.Value)
            {
                var Buyer = await context.Buyers.FirstOrDefaultAsync(x => x.IdEstate == IdEstate);
                if (Buyer == null)
                {
                    return NotFound("La propiedad no ha sido comprada aún");
                }
                return mapper.Map<GetBuyersDTO>(Buyer);

            }
            return ExisteEstate.Result;

        }



        [HttpPost]
        public async Task<ActionResult> Post([FromRoute] int IdEstate, PostBuyersDTO postBuyerDTO)
        {
            var IdUser = await getUser.GetId();
            var ExisteEstate = await SaberSiExistePropiedad(IdUser, IdEstate);
            if (ExisteEstate.Value)
            {
                var ExistsBuyer = await context.Buyers.AnyAsync(x => x.IdEstate == IdEstate);
                if (ExistsBuyer)
                {
                    return BadRequest("Ya dicha propiedad ha sido comprada previamente");
                }

                var Buyer = mapper.Map<Buyer>(postBuyerDTO);
                Buyer.IdEstate = IdEstate;

                context.Add(Buyer);
                await context.SaveChangesAsync();

                var BuyerDTO = mapper.Map<GetBuyersDTO>(Buyer);
                return CreatedAtRoute("GetBuyer", new { IdEstate = IdEstate }, BuyerDTO);
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
                var ExistsBuyer = await context.Buyers.FirstOrDefaultAsync(x => x.IdEstate == IdEstate);
                if (ExistsBuyer == null)
                {
                    return NotFound("No se ha registrado ninguna compra para esta propiedad");
                }
                context.Buyers.Remove(ExistsBuyer);
                await context.SaveChangesAsync();
                return Ok("Comprador eliminado");
            }
            return ExisteEstate.Result;

        }

        [HttpPatch]
        public async Task<ActionResult> Patch(JsonPatchDocument<PatchBuyersDTO> jsonPatchDocument, [FromRoute] int IdEstate)
        {
            if (jsonPatchDocument == null)
            {
                return BadRequest();
            }
            var IdUser = await getUser.GetId();
            var ExisteEstate = await SaberSiExistePropiedad(IdUser, IdEstate);
            if (ExisteEstate.Value)
            {
                var Buyer = await context.Buyers.FirstOrDefaultAsync(x => x.IdEstate == IdEstate);
                if (Buyer == null)
                {
                    return NotFound("No existe Comprador para esta propiedad");
                }

                var BuyerDTO = mapper.Map<PatchBuyersDTO>(Buyer);
                jsonPatchDocument.ApplyTo(BuyerDTO, ModelState);
                bool esValido = TryValidateModel(BuyerDTO);
                if (!esValido)
                {
                    return BadRequest(ModelState);
                }

                mapper.Map(BuyerDTO, Buyer);
                await context.SaveChangesAsync();
                return NoContent();
            }
            return ExisteEstate.Result;

        }
    }
}

