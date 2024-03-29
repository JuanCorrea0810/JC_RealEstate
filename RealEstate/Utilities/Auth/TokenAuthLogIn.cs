﻿using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Utilities.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RealEstate.Models.Auth
{
    
    public class TokenAuthLogIn: IAuthLog_In
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<NewIdentityUser> userManager;

        public TokenAuthLogIn(IConfiguration configuration, UserManager<NewIdentityUser> userManager)
        {
            this.configuration = configuration;
            this.userManager = userManager;
        }
        public async Task<ResponseAuth> Token(LoginAuth credentials)
        {
            var user = await userManager.FindByEmailAsync(credentials.Email);
            var claims = await userManager.GetClaimsAsync(user);

            var MyClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, credentials.Email),
            };
            MyClaims.AddRange(claims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMonths(6);

            var MyToken = new JwtSecurityToken(issuer: null, audience: null, claims: MyClaims, expires: expires, signingCredentials: creds);

            return new ResponseAuth { Token = new JwtSecurityTokenHandler().WriteToken(MyToken), Expiration = expires };
        }
    }
}
