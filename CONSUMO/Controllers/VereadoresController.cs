using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CONSUMO.Container;
using CONSUMO.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CONSUMO.Controllers
{
    /// <summary>
    /// Controlador responsável pelas pesquisas de Vereadores, acessável apenas por usuário logado.
    /// </summary>      
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VereadoresController : ControllerBase
    {
        private readonly PoliticosServices _vereadoresServices;        
        public VereadoresController() : base()
        {
            _vereadoresServices = new PoliticosServices("Vereadores");
        }

        /// <summary>
        /// Método responsável por pegar todos os Vereadores cadastrados na outra API.
        /// </summary>    
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var content = await _vereadoresServices.GetTodos();
            var vereadores = JsonConvert.DeserializeObject<List<VereadorContainer>>(content); 

            if(vereadores.Count() == 0)
            {
                return NoContent();
            }  

            return Ok(vereadores);
        }    

        /// <summary>
        /// Método responsável por pegar um Vereador cadastrado na outra API.
        /// </summary>    
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var content = await _vereadoresServices.GetPorId(id);
            try
            {
                var vereador = JsonConvert.DeserializeObject<VereadorContainer>(content);
                return Ok(vereador);
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult(content);
            }  
        }    

        /// <summary>
        /// Método responsável por pegar todos os Vereadores cadastrados na outra API em ordem ascendente de nome.
        /// </summary>           
        [HttpGet("asc")]
        public async Task<IActionResult> GetAsc()
        {
            var content = await _vereadoresServices.GetTodos();
            var vereadores = JsonConvert.DeserializeObject<List<VereadorContainer>>(content);             

            if(vereadores.Count() == 0)
            {
                return NoContent();
            }  

            var vereadoresOrdenados = vereadores.OrderBy(v => v.Vereador.Nome);

            return Ok(vereadoresOrdenados);
        }

        /// <summary>
        /// Método responsável por pegar todos os Vereadores cadastrados na outra API em ordem descendente de nome.
        /// </summary>              
        [HttpGet("desc")]
        public async Task<IActionResult> GetDesc()
        {
            var content = await _vereadoresServices.GetTodos();
            var vereadores = JsonConvert.DeserializeObject<List<VereadorContainer>>(content);             

            if(vereadores.Count() == 0)
            {
                return NoContent();
            }  

            var vereadoresOrdenados = vereadores.OrderByDescending(v => v.Vereador.Nome);

            return Ok(vereadoresOrdenados);
        }

        /// <summary>
        /// Método responsável por pegar todos os Vereadores cadastrados na outra API que possuem um número mínimo de projetos de lei.
        /// </summary>         
        [HttpGet("leis/{qtde}")]
        public async Task<IActionResult> GetPorLeis(int qtde)
        {
            var content = await _vereadoresServices.GetTodos();
            var vereadores = JsonConvert.DeserializeObject<List<VereadorContainer>>(content);
            var vereadoresSelecionados = vereadores.Where(v => v.Vereador.ProjetosDeLei >= qtde);

            if(vereadoresSelecionados.Count() == 0)
            {
                return NoContent();
            }  

            return Ok(vereadoresSelecionados);
        }
    }
}