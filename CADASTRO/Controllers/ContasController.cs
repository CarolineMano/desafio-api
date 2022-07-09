using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CADASTRO.Data;
using CADASTRO.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CADASTRO.Controllers
{
    /// <summary>
    /// Controlador responsável por efetuar o login de usuário.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ContasController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _database;

        public ContasController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IConfiguration config, ApplicationDbContext database)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
            _database = database;
        }

        /// <summary>
        /// Método para login do usuário. Só há um login para usuário administrador.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if(ModelState.IsValid)
            {
                var userFromDb = await _userManager.FindByNameAsync(model.Username);
                if(userFromDb != null)
                {
                    var passwordCheck = await _signInManager.CheckPasswordSignInAsync(userFromDb, model.Password, false);
                    if(passwordCheck.Succeeded)
                    {
                        var token = Generate(userFromDb);
                        return Ok(token);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            return BadRequest();
        }

        private string Generate(IdentityUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            string roleId = _database.UserRoles.First(role => role.UserId == user.Id).RoleId;   

            var claims = new List<Claim>();
            claims.Add(new Claim("id", user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserName));
            claims.Add(new Claim(ClaimTypes.Role, _database.Roles.First(role => role.Id == roleId).ToString()));

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return (new JwtSecurityTokenHandler().WriteToken(token));
        }   
    }
}