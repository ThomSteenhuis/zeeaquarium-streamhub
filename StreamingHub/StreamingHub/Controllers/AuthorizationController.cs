using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StreamingHub.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StreamingHub.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthorizationController : Controller
    {
        [HttpPost, Route("login")]
        public IActionResult Login([FromBody] LoginModel user)
        {
            if (user == null)
            {
                return BadRequest("Invalid client request");
            }

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super@Secret@Key@789!"));

            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            if (user.UserName == "dikkie" && user.Password == "Buurman_37!")
            {
                var tokenOptions = new JwtSecurityToken(
                    issuer: "https://zeeaquarium-streaminghub.azurewebsites.net",
                    audience: "https://zeeaquarium-streaminghub.azurewebsites.net",
                    claims: new List<Claim>()
                    {
                        new Claim(ClaimTypes.Role, Role.Streamer)
                    },
                    expires: DateTime.Now.AddDays(365),
                    signingCredentials: signingCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

                return Ok(new { Token = tokenString });
            }

            if (user.UserName == "schattie" && user.Password == "Buurman_38?")
            {
                var tokenOptions = new JwtSecurityToken(
                    issuer: "https://zeeaquarium-streaminghub.azurewebsites.net",
                    audience: "https://zeeaquarium-streaminghub.azurewebsites.net",
                    claims: new List<Claim>()
                    {
                        new Claim(ClaimTypes.Role, Role.Viewer)
                    },
                    expires: DateTime.Now.AddDays(365),
                    signingCredentials: signingCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

                return Ok(new { Token = tokenString });
            }

            if (user.UserName == "wortel" && user.Password == "Buurman_39&")
            {
                var tokenOptions = new JwtSecurityToken(
                    issuer: "https://zeeaquarium-streaminghub.azurewebsites.net",
                    audience: "https://zeeaquarium-streaminghub.azurewebsites.net",
                    claims: new List<Claim>()
                    {
                        new Claim(ClaimTypes.Role, Role.User)
                    },
                    expires: DateTime.Now.AddDays(365),
                    signingCredentials: signingCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

                return Ok(new { Token = tokenString });
            }

            return Unauthorized("Invalid username and/or password");
        }
    }
}
