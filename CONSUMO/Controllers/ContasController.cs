using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CONSUMO.Data;
using CONSUMO.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CONSUMO.Controllers
{
    /// <summary>
    /// Controlador responsável pelo registro e login de usuários.
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
        /// Método responsável registrar um novo usuário no sistema.
        /// </summary>    
        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] RegistroDto modelo)
        {
            if(ModelState.IsValid)
            {
                var usuarioBd = await _userManager.FindByEmailAsync(modelo.Email);
                if(usuarioBd == null)
                {

                    IdentityUser novoUsuario = new IdentityUser();
                    novoUsuario.UserName = modelo.Email;
                    novoUsuario.Email = modelo.Email;
                    novoUsuario.EmailConfirmed = true;

                    IdentityResult result = _userManager.CreateAsync(novoUsuario, modelo.Senha).Result;

                    return Created("", new{userId = novoUsuario.Id, userName = modelo.Email});
                }
                return BadRequest("Email já registrado");
            }
            return BadRequest();
        }       

        /// <summary>
        /// Método responsável logar e validar um usuário registrado.
        /// </summary>    
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto modelo)
        {
            if(ModelState.IsValid)
            {
                var usuarioBd = await _userManager.FindByEmailAsync(modelo.Username);
                if(usuarioBd != null)
                {
                    var testeSenha = await _signInManager.CheckPasswordSignInAsync(usuarioBd, modelo.Senha, false);
                    if(testeSenha.Succeeded)
                    {
                        var token = GerarToken(usuarioBd);
                        return Ok(token);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
            }
            return BadRequest();
        } 

        private string GerarToken(IdentityUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var claims = new List<Claim>();
            claims.Add(new Claim("id", user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserName));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));

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