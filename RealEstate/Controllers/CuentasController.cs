using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstate.DTO_s.UsersDTO_s;
using RealEstate.Models;
using RealEstate.Models.Auth;
using RealEstate.Utilities;
using RealEstate.Utilities.Auth;
using System.Security.Claims;

namespace RealEstate.Controllers

{
    [ApiController]
    [Route("api/Users")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CuentasController: ControllerBase
    {
        private readonly UserManager<NewIdentityUser> userManager;
        private readonly RealEstateProjectContext context;
        private readonly SignInManager<NewIdentityUser> signInManager;
        private readonly IAuthLog_In authgLogIn;
        private readonly IAuthSign_Up authSignUp;
        
        public IGetUserInfo GetUserInfo { get; set; }
        private readonly IMapper mapper;
        private readonly IEmailSender emailSender;

        public CuentasController(UserManager<NewIdentityUser> userManager, 
            RealEstateProjectContext context,
            SignInManager<NewIdentityUser> signInManager,
            IAuthLog_In AuthgLogIn,
            IAuthSign_Up AuthSignUp,
            IGetUserInfo getUserInfo,
            IMapper mapper,
            IEmailSender emailSender
            
            )
        {
            this.userManager = userManager;
            this.context = context;
            this.signInManager = signInManager;
            authgLogIn = AuthgLogIn;
            authSignUp = AuthSignUp;
            GetUserInfo = getUserInfo;
            this.mapper = mapper;
            this.emailSender = emailSender;
        }

        [AllowAnonymous]
        [HttpPost("SignUp")]
        public async Task<ActionResult<ResponseAuth>> Sign_Up([FromBody] InfoUser credentials) 
        {
            if (ModelState.IsValid)
            {
                var User = new NewIdentityUser { UserName = credentials.Email, Email = credentials.Email };
                var result = await userManager.CreateAsync(User, credentials.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(User,"User");
                    await userManager.AddClaimAsync(User, new Claim(ClaimTypes.Role, "User"));
                   
                    //Implementación de confirmación de email en el registro
                    var code = await userManager.GenerateEmailConfirmationTokenAsync(User);
                    var urlRetorno = Url.Action("ConfirmEmail", "Cuentas", new { IdUser = User.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    await emailSender.SendEmailAsync(credentials.Email, "Confirmar su cuenta - Juan Correa",
                    "Por favor confirme su cuenta dando click aquí: <a href=\"" + urlRetorno + "\">enlace</a>");
                    
                    //Token JWT-Authentication
                    return await authSignUp.Token(credentials);
                }
                ValidarErrores(result);
            }
            return BadRequest(ModelState);  
            
        }

        [AllowAnonymous]
        [HttpPost("LogIn")]
        
        public async Task<ActionResult<ResponseAuth>> LogIn([FromBody]LoginAuth credentials)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(credentials.Email, credentials.Password, credentials.RememberMe,
                                                               lockoutOnFailure: true);
                var user = await userManager.FindByEmailAsync(credentials.Email);
                if (user != null)
                {
                    var LockedOut = await userManager.IsLockedOutAsync(user);
                    if (LockedOut)
                    {
                        return BadRequest("Cuenta bloqueda temporalmente. Intente más tarde");
                    }

                    if (result.Succeeded)
                    {
                        await userManager.ResetAccessFailedCountAsync(user);
                        return await authgLogIn.Token(credentials);
                    }
                    else
                    {
                        var FailedCount = await userManager.GetAccessFailedCountAsync(user);
                        if (FailedCount < 4)
                        {
                            return BadRequest("Datos Incorrectos. Vuelva a intentarlo.");
                        }
                        else if (FailedCount == 4)
                        {
                            return BadRequest("Solo le queda un intento, si vuelve a fallar se bloqueará la cuenta temporalmente");
                        }
                        else
                        {
                            return BadRequest("Cuenta bloqueda temporalmente. Intente más tarde");
                        }

                    }
                }
                else
                {
                    return NotFound("El usuario no existe");
                }       
            }
            return BadRequest(ModelState);
           
        }

        [HttpPost("LogOut")]
        public async Task<ActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return NoContent();
        }

        [HttpPut("CompleteProfile")]
        public async Task<ActionResult> CompleteProfile([FromBody] PutUsersDTO putUsersDTO) 
        {
            var IdUser = await GetUserInfo.GetId();
            var User = await userManager.FindByIdAsync(IdUser);

            if (User == null)
            {
                return NotFound();
            }

            //Si es el mismo DNI entonces el usuario se actualiza sin problemas
            if (User.Dni == putUsersDTO.Dni)
            {
                User = mapper.Map(putUsersDTO, User);
                await context.SaveChangesAsync();
                return Ok();
            }

            //Sino es el mismo entonces se busca en la base de datos para saber que no haya otro usuario con ese nuevo DNI
            var ExistsDNI = await userManager.Users.AnyAsync(x => x.Dni == putUsersDTO.Dni);
            if (ExistsDNI)
            {
                return BadRequest("DNI no aceptado");
            }
            
            User = mapper.Map(putUsersDTO,User);
            await context.SaveChangesAsync();
            return Ok();
        }

        

        [AllowAnonymous]
        [HttpPost("ForgetPassword")]
        public async Task<ActionResult<string>> ForgetPassword([FromBody]ForgetPassword emailDTO) 
        {
            if (ModelState.IsValid)
            {
                var usuario = await userManager.FindByEmailAsync(emailDTO.Email);
                if (usuario == null)
                {
                    return NotFound();
                }

                var codigo = await userManager.GeneratePasswordResetTokenAsync(usuario);
                var urlRetorno = Url.Action("ResetPassword", "User", new { userId = usuario.Id, code = codigo }, protocol: HttpContext.Request.Scheme);

                await emailSender.SendEmailAsync(emailDTO.Email, "Recuperar contraseña - JuanCorrea",
                    "Por favor recupere su contraseña haciendo una petición POST aquí: <a href=\"" + urlRetorno + "\">enlace</a>");
                return "Se ha enviado un correo de recuperación de contraseña, por favor revise su bandeja de entrada o su carpeta de spam";
            }

            return BadRequest(ModelState);

        }
        
        
        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        
        public async Task<ActionResult> ResetPassword([FromBody]ResetPassword resetPassword, [FromQuery] string code) 
        {
            if (ModelState.IsValid)
            {
                var usuario = await userManager.FindByEmailAsync(resetPassword.Email);
                if (usuario == null)
                {
                    return NotFound();
                }

                var resultado = await userManager.ResetPasswordAsync(usuario, code, resetPassword.NewPassword);
                if (resultado.Succeeded)
                {
                    return Ok();
                }
                ValidarErrores(resultado);
            }
            return BadRequest(ModelState);         
        }

        [HttpPost("ConfirmEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromQuery]string IdUser, [FromQuery] string code)
        {

            if (IdUser == null || code == null)
            {
                return BadRequest();
            }

            var usuario = await userManager.FindByIdAsync(IdUser);
            if (usuario == null)
            {
                return NotFound();
            }

            var resultado = await userManager.ConfirmEmailAsync(usuario, code);
            ValidarErrores(resultado);
            return resultado.Succeeded ? Ok() : BadRequest(ModelState);
        }

        
        [HttpPost("ChangePassword")]
        public async Task<ActionResult> ChangePassword(ChangePassword changePassword)
        {
            if (ModelState.IsValid)
            {
                var IdUser = await GetUserInfo.GetId();
                var usuario = await userManager.FindByIdAsync(IdUser);
                if (usuario == null)
                {
                    return NotFound();
                }
                
                var resultado = await userManager.ChangePasswordAsync(usuario, changePassword.CurrentPassword, changePassword.NewPassword);
                
                if (resultado.Succeeded)
                {
                    return Ok();
                }
                ValidarErrores(resultado);
            }
            return BadRequest(ModelState);
        }

        private void ValidarErrores(IdentityResult resultado)
        {
            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError(String.Empty, error.Description);
            }
        }

    }
}
