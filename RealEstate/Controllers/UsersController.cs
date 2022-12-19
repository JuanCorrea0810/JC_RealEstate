using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstate.DTO_s.UsersDTO_s;
using RealEstate.Models;
using RealEstate.Utilities;
using System.Data;

namespace RealEstate.Controllers
{
    [ApiController]
    [Route("api/Users")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Owner,Admin")]
    public class UsersController : ControllerBase
    {
        
        private readonly UserManager<NewIdentityUser> userManager;
        private readonly IMapper mapper;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly RealEstateProjectContext context;

        public UsersController(UserManager<NewIdentityUser> userManager, 
            IMapper mapper,
            RoleManager<IdentityRole> roleManager,
            RealEstateProjectContext context)
        {
            
            this.userManager = userManager;
            this.mapper = mapper;
            this.roleManager = roleManager;
            this.context = context;
        }

        [HttpGet("GetUsers")]
        //GetRentersOfEstate All the users but using pagination to improve the performance
        public async Task<ActionResult<List<GetUsersDTO>>> GetUsers([FromQuery]PaginacionDTO paginacionDTO) 
        {
            var Query = userManager.Users.AsQueryable();
            await HttpContext.InsertarParametrosPaginacion(Query,paginacionDTO.CantidadRegistrosPorPagina);
            var Users = await Query.Paginar(paginacionDTO).ToListAsync();
            return mapper.Map<List<GetUsersDTO>>(Users);
        }

        [HttpGet("GetAdmins")]
        public async Task<ActionResult<List<GetUsersDTO>>> GetAdmins([FromQuery] PaginacionDTO paginacionDTO)
        {
            
            var QueryRol = roleManager.Roles.Where(x=> x.Name == "Admin").Select(x=> x.Id).AsQueryable();
            var QueryUserRoles = context.UserRoles.Where(x=> QueryRol.Contains(x.RoleId)).Select(x=> x.UserId).AsQueryable();
            var QueryAdmins = userManager.Users.Where(x=> QueryUserRoles.Contains(x.Id)).AsQueryable();
            await HttpContext.InsertarParametrosPaginacion(QueryAdmins, paginacionDTO.CantidadRegistrosPorPagina);
            var Users = await QueryAdmins.Paginar(paginacionDTO).ToListAsync();
            return mapper.Map<List<GetUsersDTO>>(Users);
        }

        [HttpPut("LockOut")]
        public async Task<ActionResult> LockOutUser(string IdUser)
        {
            var user = await userManager.FindByIdAsync(IdUser);
            if (user == null)
            {
                return NotFound();
            }
            await userManager.SetLockoutEndDateAsync(user, DateTime.Now.AddMonths(3));
            return Ok();

        }

        [HttpPut("UnLock")]
        public async Task<ActionResult> UnLock(string IdUser)
        {
            var user = await userManager.FindByIdAsync(IdUser);
            if (user == null)
            {
                return NotFound();
            }
            await userManager.SetLockoutEndDateAsync(user,DateTime.Now);
            return Ok();

        }
        [HttpDelete("DeleteUser")]
        public async Task<ActionResult> DeleteUser(string IdUser)
        {
            var user = await userManager.FindByIdAsync(IdUser);
            if (user == null)
            {
                return NotFound();
            }
            await userManager.DeleteAsync(user);
            return Ok();
        }


    }
}
