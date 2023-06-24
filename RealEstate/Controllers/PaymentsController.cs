using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstate.DTO_s.PaymentsDTO_s;
using RealEstate.Models;
using RealEstate.Utilities;

namespace RealEstate.Controllers
{
    [ApiController]
    [Route("api/Estates/{IdEstate:int}/Mortgages/{IdMortgage:int}/Payments")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class PaymentsController : CustomBaseController
    {
        private readonly RealEstateProjectContext context;
        private readonly IMapper mapper;
        private readonly IGetUserInfo getUser;

        public PaymentsController(RealEstateProjectContext context, IMapper mapper,
            IGetUserInfo getUser) : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.getUser = getUser;

        }
        [HttpGet("/api/Users/Payments", Name = "UsersAndTheirPayments")]
        public async Task<ActionResult<List<GetPaymentsDTO>>> GetUsersAndPayments()
        {

            var IdUser = await getUser.GetId();
            var Mortgage = await context.Mortgages.Where(x => x.IdUser == IdUser).Select(x => x.IdMortgage).ToListAsync();
            if (Mortgage.Count == 0)
            {
                return NotFound("El usuario no tiene ninguna hipoteca");
            }

            var Payments = await context.Payments.Where(x => Mortgage.Contains(x.IdMortgage)).ToListAsync();
            if (Payments.Count == 0)
            {
                return NotFound("El usuario no ha registrado ningún pago hacia ninguna hipoteca");
            }

            return mapper.Map<List<GetPaymentsDTO>>(Payments);
        }


        [HttpGet]
        public async Task<ActionResult<List<GetPaymentsDTO>>> Get([FromRoute] int IdMortgage, [FromRoute] int IdEstate)
        {
            var IdUser = await getUser.GetId();
            var ExisteEstate = await SaberSiExistePropiedad(IdUser, IdEstate);
            if (!ExisteEstate.Value)
            { return ExisteEstate.Result; }

            var ExisteMortgage = await SaberSiExisteMortgage(IdEstate);
            if (!ExisteMortgage.Value)
            { return ExisteMortgage.Result; }

            var HayRelacion = await SaberSiHayRelacionEntreHipotecaYPropiedad(IdEstate, IdMortgage);
            if (!HayRelacion.Value)
            { return HayRelacion.Result; }

            var Payments = await context.Payments.Where(x => x.IdMortgage == IdMortgage).ToListAsync();
            if (Payments.Count == 0)
            {
                return NotFound("No se han registrado pagos a la hipoteca");
            }
            return mapper.Map<List<GetPaymentsDTO>>(Payments);
        }

        [HttpGet("{IdPayments:int}", Name = "GetPayment")]
        public async Task<ActionResult<GetPaymentsDTO>> GetById([FromRoute] int IdMortgage, [FromRoute] int IdEstate, [FromRoute] int IdPayments)
        {
            var IdUser = await getUser.GetId();
            var ExisteEstate = await SaberSiExistePropiedad(IdUser, IdEstate);
            if (!ExisteEstate.Value)
            { return ExisteEstate.Result; }

            var ExisteMortgage = await SaberSiExisteMortgage(IdEstate);
            if (!ExisteMortgage.Value)
            { return ExisteMortgage.Result; }

            var HayRelacion = await SaberSiHayRelacionEntreHipotecaYPropiedad(IdEstate, IdMortgage);
            if (!HayRelacion.Value)
            { return HayRelacion.Result; }

            var Payment = await context.Payments.FirstOrDefaultAsync(x => x.IdPayments == IdPayments && x.IdMortgage == IdMortgage);
            if (Payment == null)
            {
                return NotFound("Dicho pago no se ha registrado");
            }
            return mapper.Map<GetPaymentsDTO>(Payment);
        }

        [HttpPost]
        public async Task<ActionResult> Post(PostPaymentsDTO postPayment, [FromRoute] int IdMortgage, int IdEstate)
        {

            var IdUser = await getUser.GetId();
            var ExisteEstate = await SaberSiExistePropiedad(IdUser, IdEstate);
            if (!ExisteEstate.Value)
            { return ExisteEstate.Result; }

            var ExisteMortgage = await SaberSiExisteMortgage(IdEstate);
            if (!ExisteMortgage.Value)
            { return ExisteMortgage.Result; }

            var HayRelacion = await SaberSiHayRelacionEntreHipotecaYPropiedad(IdEstate, IdMortgage);
            if (!HayRelacion.Value)
            { return HayRelacion.Result; }

            var Payment = mapper.Map<Payment>(postPayment);
            Payment.IdMortgage = IdMortgage;
            context.Add(Payment);
            await context.SaveChangesAsync();
            var PaymentDTO = mapper.Map<GetPaymentsDTO>(Payment);
            return CreatedAtRoute("GetPayment", new { IdEstate = IdEstate, IdMortgage = IdMortgage, IdPayments = Payment.IdPayments }, PaymentDTO);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete([FromRoute] int IdMortgage, [FromRoute] int IdEstate, [FromRoute] int id)
        {
            var IdUser = await getUser.GetId();
            var ExisteEstate = await SaberSiExistePropiedad(IdUser, IdEstate);
            if (!ExisteEstate.Value)
            { return ExisteEstate.Result; }

            var ExisteMortgage = await SaberSiExisteMortgage(IdEstate);
            if (!ExisteMortgage.Value)
            { return ExisteMortgage.Result; }

            var HayRelacion = await SaberSiHayRelacionEntreHipotecaYPropiedad(IdEstate, IdMortgage);
            if (!HayRelacion.Value)
            { return HayRelacion.Result; }

            var Payment = await context.Payments.FirstOrDefaultAsync(x => x.IdPayments == id && x.IdMortgage == IdMortgage);
            if (Payment == null)
            {
                return NotFound("Dicho pago no se ha registrado");
            }
            context.Payments.Remove(Payment);
            await context.SaveChangesAsync();
            return Ok("Registro eliminado");
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(JsonPatchDocument<PatchPaymentsDTO> jsonPatchDocument, [FromRoute] int IdMortgage, [FromRoute] int IdEstate, [FromRoute] int id)
        {
            if (jsonPatchDocument == null)
            {
                return BadRequest();
            }

            var IdUser = await getUser.GetId();
            var ExisteEstate = await SaberSiExistePropiedad(IdUser, IdEstate);
            if (!ExisteEstate.Value)
            { return ExisteEstate.Result; }

            var ExisteMortgage = await SaberSiExisteMortgage(IdEstate);
            if (!ExisteMortgage.Value)
            { return ExisteMortgage.Result; }

            var HayRelacion = await SaberSiHayRelacionEntreHipotecaYPropiedad(IdEstate, IdMortgage);
            if (!HayRelacion.Value)
            { return HayRelacion.Result; }

            var Payment = await context.Payments.FirstOrDefaultAsync(x => x.IdPayments == id && x.IdMortgage == IdMortgage);
            if (Payment == null)
            {
                return NotFound("Dicho pago no se ha registrado");
            }

            var PaymentDTO = mapper.Map<PatchPaymentsDTO>(Payment);
            jsonPatchDocument.ApplyTo(PaymentDTO, ModelState);
            bool esValido = TryValidateModel(PaymentDTO);
            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(PaymentDTO, Payment);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
