using _584_server.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using SchoolModel;

namespace _584_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController(UserManager<SchoolModelUser> userManager, JwtHandler jwtHandler) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            SchoolModelUser? schoolUser = await userManager.FindByNameAsync(loginRequest.Username);
            if (schoolUser is null || !await userManager.CheckPasswordAsync(schoolUser, loginRequest.Password))
            {
               Response.StatusCode = StatusCodes.Status401Unauthorized;
                await Response.WriteAsync("Invalid username or password.");
                return Empty;
            }

            JwtSecurityToken token  = await jwtHandler.GenerateTokenAsync(schoolUser);
            string stringToken = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new LoginResponse
            {
                Success = true,
                Message = "Mom loves me",
                Token = stringToken
            });
        }
    
    }
}


