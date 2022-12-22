using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstate.DTO_s.BuyContractsDTO_s;
using RealEstate.Models;
using RealEstate.Utilities;

namespace RealEstate.Controllers
{

    [ApiController]
    [Route("api/Estates/{IdEstate:int}/Buyer/{IdBuyer:int}/BuyContracts")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BuyContractsController : CustomBaseController
    {
        private readonly RealEstateProjectContext context;
        private readonly IMapper mapper;
        private readonly IGetUserInfo getUser;

        public BuyContractsController(RealEstateProjectContext context, IMapper mapper,
            IGetUserInfo getUser) : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.getUser = getUser;
        }
        [HttpGet("/api/Users/BuyContracts", Name = "UsersAndTheirBuyContracts")]
        public async Task<ActionResult<List<GetBuyContractsDTo>>> GetBuyContractsAndUsers()
        {
            var IdUser = await getUser.GetId();
            var _estates = await context.Estates.Where(x => x.IdUser == IdUser).Select(x => x.IdEstate).ToListAsync();
            if (_estates.Count == 0)
            {
                return NotFound("El usuario no tiene ninguna propiedad registrada");
            }
            var Buyers = await context.Buyers.Where(x => _estates.Contains(x.IdEstate)).Select(x => x.IdBuyer).ToListAsync();

            if (Buyers.Count == 0)
            {
                return NotFound("El usuario no tiene ningún Comprador registrado");
            }
            var BuyContracts = await context.Buycontracts.Where(x => Buyers.Contains(x.IdBuyer)).ToListAsync();
            if (BuyContracts.Count == 0)
            {
                return NotFound("El usuario no tiene ningún Contrato de compra registrado");
            }
            return mapper.Map<List<GetBuyContractsDTo>>(BuyContracts);

        }

        [HttpGet(Name = "GetContract")]
        public async Task<ActionResult<GetBuyContractsDTo>> Get([FromRoute] int IdEstate, [FromRoute] int IdBuyer)
        {
            var IdUser = await getUser.GetId();
            var ExisteEstate = await SaberSiExistePropiedad(IdUser, IdEstate);
            if (ExisteEstate.Value)
            {
                var ExisteBuyer = await SaberSiExisteBuyer(IdBuyer);
                if (ExisteBuyer.Value)
                {
                    var HayRelacion = await SaberSiHayRelacionEntreBuyerYPropiedad(IdEstate, IdBuyer);
                    if (HayRelacion.Value)
                    {
                        var BuyContracts = await context.Buycontracts.FirstOrDefaultAsync(x => x.IdBuyer == IdBuyer && x.IdEst == IdEstate);
                        if (BuyContracts == null)
                        {
                            return NotFound("El usuario no ha registrado ningun contrato sobre dicha compra");
                        }
                        return mapper.Map<GetBuyContractsDTo>(BuyContracts);
                    }
                    return HayRelacion.Result;

                }
                return ExisteBuyer.Result;
            }
            return ExisteEstate.Result;

        }




        [HttpPost]
        public async Task<ActionResult> Post(PostBuyContractsDTO postBuyContract, [FromRoute] int IdEstate, [FromRoute] int IdBuyer)
        {
            var IdUser = await getUser.GetId();
            var ExisteEstate = await SaberSiExistePropiedad(IdUser, IdEstate);
            if (ExisteEstate.Value)
            {
                var ExisteBuyer = await SaberSiExisteBuyer(IdBuyer);
                if (ExisteBuyer.Value)
                {
                    var HayRelacion = await SaberSiHayRelacionEntreBuyerYPropiedad(IdEstate, IdBuyer);
                    if (HayRelacion.Value)
                    {
                        var ExisteBuyContract = await context.Buycontracts.AnyAsync(x=> x.IdBuyer == IdBuyer && x.IdEst == IdEstate);
                        if (ExisteBuyContract)
                        {
                            return BadRequest("Ya la propiedad ha sido previamente comprada");
                        }
                        var BuyContract = mapper.Map<Buycontract>(postBuyContract);
                        BuyContract.IdBuyer = IdBuyer;
                        BuyContract.IdEst = IdEstate;
                        context.Add(BuyContract);
                        await context.SaveChangesAsync();

                        var BuyContractDTO = mapper.Map<GetBuyContractsDTo>(BuyContract);
                        return CreatedAtRoute("GetContract", new { IdEstate = IdEstate, IdBuyer = IdBuyer }, BuyContractDTO);
                    }
                    return HayRelacion.Result;
                }
                return ExisteBuyer.Result;
            }
            return ExisteEstate.Result;
                        

        }

        [HttpDelete]
        public async Task<ActionResult> Delete([FromRoute] int IdEstate, [FromRoute] int IdBuyer)
        {
            var IdUser = await getUser.GetId();
            var ExisteEstate = await SaberSiExistePropiedad(IdUser, IdEstate);
            if (ExisteEstate.Value)
            {
                var ExisteBuyer = await SaberSiExisteBuyer(IdBuyer);
                if (ExisteBuyer.Value)
                {
                    var HayRelacion = await SaberSiHayRelacionEntreBuyerYPropiedad(IdEstate, IdBuyer);
                    if (HayRelacion.Value)
                    {
                        var BuyContract = await context.Buycontracts.FirstOrDefaultAsync(x => x.IdEst == IdEstate && x.IdBuyer == IdBuyer);
                        if (BuyContract == null)
                        {
                            return NotFound("No se ha registrado dicho contrato para esta compra");
                        }
                        context.Buycontracts.Remove(BuyContract);
                        await context.SaveChangesAsync();
                        return Ok("Contrato eliminado");
                    }
                    return HayRelacion.Result;
                }
                return ExisteBuyer.Result;
            }
            return ExisteEstate.Result;
                        
        }

        [HttpPatch]
        public async Task<ActionResult> Patch(JsonPatchDocument<PatchBuyContractsDTO> jsonPatchDocument, [FromRoute] int IdEstate, [FromRoute] int IdBuyer)
        {
            if (jsonPatchDocument == null)
            {
                return BadRequest();
            }
            var IdUser = await getUser.GetId();
            var ExisteEstate = await SaberSiExistePropiedad(IdUser, IdEstate);
            if (ExisteEstate.Value)
            {
                var ExisteBuyer = await SaberSiExisteBuyer(IdBuyer);
                if (ExisteBuyer.Value)
                {
                    var HayRelacion = await SaberSiHayRelacionEntreBuyerYPropiedad(IdEstate, IdBuyer);
                    if (HayRelacion.Value)
                    {
                        var BuyContract = await context.Buycontracts.FirstOrDefaultAsync(x => x.IdEst == IdEstate && x.IdBuyer == IdBuyer);
                        if (BuyContract == null)
                        {
                            return NotFound("El usuario no ha registrado dicho contrato de compra");
                        }

                        var BuyContractDTO = mapper.Map<PatchBuyContractsDTO>(BuyContract);
                        jsonPatchDocument.ApplyTo(BuyContractDTO, ModelState);
                        bool esValido = TryValidateModel(BuyContractDTO);
                        if (!esValido)
                        {
                            return BadRequest(ModelState);
                        }

                        mapper.Map(BuyContractDTO, BuyContract);
                        await context.SaveChangesAsync();
                        return NoContent();
                    }
                    return HayRelacion.Result;
                }
                return ExisteBuyer.Result;
            }
            return ExisteEstate.Result;
        }



    }
}
