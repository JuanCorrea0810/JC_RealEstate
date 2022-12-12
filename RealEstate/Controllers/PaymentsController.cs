using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstate.DTO_s.PaymentsDTO_s;
using RealEstate.Models;
using RealEstate.Utilities;
using System.Diagnostics;

namespace RealEstate.Controllers
{
    [ApiController]
    [Route("api/Estates/{IdEstate:int}/Mortgages/{IdMortgage:int}/Payments")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    
    public class PaymentsController : ControllerBase
    {
        private readonly RealEstateProjectContext context;
        private readonly IMapper mapper;
        private readonly IGetUserInfo getUser;

        public PaymentsController(RealEstateProjectContext context, IMapper mapper,
            IGetUserInfo getUser)
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
            var ExistsEstate = await context.Estates.AnyAsync(x => x.IdEstate == IdEstate && x.IdUser == IdUser);
            if (!ExistsEstate)
            {
                return NotFound("La propiedad no existe o no le pertenece");
            }
            var Mortgage = await context.Mortgages.AnyAsync(x => x.IdEstate == IdEstate);
            if (!Mortgage)
            {
                return NotFound("La propiedad no tiene hipotecas registradas");
            }
            var ExistsEstateInMortgage = await context.Mortgages.AnyAsync(x => x.IdEstate == IdEstate && x.IdMortgage == IdMortgage);
            if (!ExistsEstateInMortgage)
            {
                return NotFound("La hipoteca no coincide con la propiedad");
            }
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
            var ExistsEstate = await context.Estates.AnyAsync(x => x.IdEstate == IdEstate && x.IdUser == IdUser);
            if (!ExistsEstate)
            {
                return NotFound("La propiedad no existe o no le pertenece");
            }
            var Mortgage = await context.Mortgages.AnyAsync(x => x.IdEstate == IdEstate);
            if (!Mortgage)
            {
                return NotFound("La propiedad no tiene ninguna hipoteca registrada");
            }
            var ExistsEstateInMortgage = await context.Mortgages.AnyAsync(x => x.IdEstate == IdEstate && x.IdMortgage == IdMortgage);
            if (!ExistsEstateInMortgage)
            {
                return NotFound("La hipoteca no coincide con la propiedad");
            }
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
            var ExistsEstate = await context.Estates.AnyAsync(x => x.IdEstate == IdEstate && x.IdUser == IdUser);
            if (!ExistsEstate)
            {
                return NotFound("La propiedad no existe o no le pertenece");
            }
            var Mortgage = await context.Mortgages.AnyAsync(x => x.IdEstate == IdEstate);
            if (!Mortgage)
            {
                return NotFound("La propiedad no tiene ninguna hipoteca registrada, no se puede registrar ningún pago");
            }
            var ExistsEstateInMortgage = await context.Mortgages.AnyAsync(x => x.IdEstate == IdEstate && x.IdMortgage == IdMortgage);
            if (!ExistsEstateInMortgage)
            {
                return NotFound("La hipoteca no coincide con la propiedad");
            }
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
            var ExistsEstate = await context.Estates.AnyAsync(x => x.IdEstate == IdEstate && x.IdUser == IdUser);
            if (!ExistsEstate)
            {
                return NotFound("La propiedad no existe o no le pertenece");
            }
            var ExistsMortgage = await context.Mortgages.AnyAsync(x => x.IdEstate == IdEstate);
            if (!ExistsMortgage)
            {
                return NotFound("La propiedad no tiene ninguna hipoteca registrada");
            }
            var ExistsEstateInMortgage = await context.Mortgages.AnyAsync(x => x.IdEstate == IdEstate && x.IdMortgage == IdMortgage);
            if (!ExistsEstateInMortgage)
            {
                return NotFound("La hipoteca no coincide con la propiedad");
            }
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
            var ExistsEstate = await context.Estates.AnyAsync(x => x.IdEstate == IdEstate && x.IdUser == IdUser);
            if (!ExistsEstate)
            {
                return NotFound("La propiedad no existe o no le pertenece");
            }
            var Mortgage = await context.Mortgages.AnyAsync(x => x.IdEstate == IdEstate && x.IdMortgage == IdMortgage);
            if (!Mortgage)
            {
                return NotFound("La propiedad no tiene ninuna hipoteca o los registros no coinciden");
            }
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
