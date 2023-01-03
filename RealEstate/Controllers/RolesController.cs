using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstate.DTO_s.RolesDTO_s;
using RealEstate.Models;
using System.Security.Claims;

namespace RealEstate.Controllers
{
    [ApiController]
    [Route("api/Roles")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles = "Owner,Admin")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<NewIdentityUser> userManager;
        private readonly IMapper mapper;

        public RolesController(RoleManager<IdentityRole> roleManager,
            UserManager<NewIdentityUser> userManager,
            IMapper mapper)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<RolesDTO>>> Roles()
        {
            var RolesDB = await roleManager.Roles.ToListAsync();
            return mapper.Map<List<RolesDTO>>(RolesDB);
        }

        [HttpPost("CreateRole")]
        public async Task<ActionResult> CreateRole([FromBody]string RoleName)
        {

            if (await roleManager.RoleExistsAsync(RoleName))
            {
                return BadRequest("Rol ya existe");
            }
            var NewRole = new IdentityRole { Name = RoleName };
            await roleManager.CreateAsync(NewRole);
            return NoContent();
        }

        [HttpPost("DeleteRole")]
        public async Task<ActionResult> DeleteRole([FromBody]string RoleName)
        {

            var rol = await roleManager.FindByNameAsync(RoleName);
            if (rol == null)
            {
                return NotFound();
            }
            await roleManager.DeleteAsync(rol);
            return NoContent();
        }

        [HttpPost("AddToRole")]
        public async Task<ActionResult> AddToRole([FromQuery] string IdUser, [FromBody] string RoleName)
        {
            var rol = await roleManager.FindByNameAsync(RoleName);
            if (rol == null)
            {
                return NotFound();
            }
            var user = await userManager.FindByIdAsync(IdUser);
            if (user == null)
            {
                return NotFound();
            }
            await userManager.AddToRoleAsync(user, RoleName);
            await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, RoleName));
            return Ok();
        }

        [HttpPost("DeleteFromRole")]
        public async Task<ActionResult> DeleteFromRole([FromQuery] string IdUser, [FromBody] string RoleName)
        {
            var rol = await roleManager.FindByNameAsync(RoleName);
            if (rol == null)
            {
                return NotFound();
            }
            var user = await userManager.FindByIdAsync(IdUser);
            if (user == null)
            {
                return NotFound();
            }
            await userManager.RemoveFromRoleAsync(user, RoleName);
            await userManager.RemoveClaimAsync(user, new Claim(ClaimTypes.Role, RoleName));
            return Ok();
        }

        [HttpPost("AddToRoles")]
        public async Task<ActionResult> AddToRoles([FromQuery] string IdUser, [FromBody] List<string> Roles)
        {
            foreach (var item in Roles)
            {
                if (!await roleManager.RoleExistsAsync(item))
                {
                    return BadRequest("Uno de los Roles no existe");
                }
            }
            var user = await userManager.FindByIdAsync(IdUser);
            if (user == null)
            {
                return NotFound();
            }
            await userManager.AddToRolesAsync(user, Roles);

            var list = new List<Claim>();
            foreach (var role in Roles) 
            {
                list.Add(new Claim(ClaimTypes.Role, role));
            }
            await userManager.AddClaimsAsync(user,list);
            
            return Ok();
        }

        [HttpPost("DeleteFromRoles")]
        public async Task<ActionResult> DeleteFromRoles([FromQuery] string IdUser, [FromBody] List<string> Roles)
        {
            foreach (var item in Roles)
            {
                if (!await roleManager.RoleExistsAsync(item))
                {
                    return BadRequest("Uno de los Roles no existe");
                }
            }
            var user = await userManager.FindByIdAsync(IdUser);
            if (user == null)
            {
                return NotFound();
            }
            await userManager.RemoveFromRolesAsync(user, Roles);
            var listClaims = await userManager.GetClaimsAsync(user);
            int Index = 0;
            for (int i = 0; i < listClaims.Count; )
            {
                if (Index >= Roles.Count)
                {
                    Index = 0;
                    i++;
                    
                }
                if (i <= listClaims.Count - 1)
                {
                    if (listClaims[i].Value == Roles[Index])
                    {
                        await userManager.RemoveClaimAsync(user, listClaims[i]);
                        Index++;
                    }
                    else
                    {
                        Index++;
                    }          
                }
                
            }        
            return Ok();
        }

    
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Owner")]
        [HttpPost("CreateAdmin")]
        public async Task<ActionResult> CreateAdmin([FromQuery] string IdUser)
        {
            var user = await userManager.FindByIdAsync(IdUser);
            if (user == null)
            {
                return NotFound();
            }
            await userManager.AddClaimAsync(user,new Claim(ClaimTypes.Role,"Admin"));
            await userManager.AddToRoleAsync(user, "Admin");
            return Ok();
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Owner")]
        [HttpPost("RemoveAdmin")]
        public async Task<ActionResult> RemoveAdmin([FromQuery] string IdUser)
        {
            var user = await userManager.FindByIdAsync(IdUser);
            if (user == null)
            {
                return NotFound();
            }
            await userManager.RemoveClaimAsync(user, new Claim(ClaimTypes.Role, "Admin"));
            await userManager.RemoveFromRoleAsync(user, "Admin");
            return Ok();
        }
    }
}
