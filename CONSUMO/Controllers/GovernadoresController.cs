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
    /// Controlador responsável pelas pesquisas de Governadores, acessável apenas por usuário logado.
    /// </summary>       
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GovernadoresController : ControllerBase
    {
        private readonly PoliticosServices _governadoresServices;        
        public GovernadoresController() : base()
        {
            _governadoresServices = new PoliticosServices("Governadores");
        }

        /// <summary>
        /// Método responsável por pegar todos os Governadores cadastrados na outra API.
        /// </summary>   
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var content = await _governadoresServices.GetTodos();
            var governadores = JsonConvert.DeserializeObject<List<GovernadorContainer>>(content); 
            
            if(governadores.Count() == 0)
            {
                return NoContent();
            }                 
            
            return Ok(governadores);
        }    

        /// <summary>
        /// Método responsável por pegar um Governador cadastrado na outra API.
        /// </summary>  
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var content = await _governadoresServices.GetPorId(id);
            try
            {
                var governador = JsonConvert.DeserializeObject<GovernadorContainer>(content);
                return Ok(governador);
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult(content);
            }  
        }    

        /// <summary>
        /// Método responsável por pegar todos os Governadores cadastrados na outra API em ordem ascendente de nome.
        /// </summary> 
        [HttpGet("asc")]
        public async Task<IActionResult> GetAsc()
        {
            var content = await _governadoresServices.GetTodos();
            var governadores = JsonConvert.DeserializeObject<List<GovernadorContainer>>(content);             

            if(governadores.Count() == 0)
            {
                return NoContent();
            }      

            var governadoresOrdenados = governadores.OrderBy(g => g.Governador.Nome);

            return Ok(governadoresOrdenados);
        }

        /// <summary>
        /// Método responsável por pegar todos os Governadores cadastrados na outra API em ordem descendente de nome.
        /// </summary>            
        [HttpGet("desc")]
        public async Task<IActionResult> GetDesc()
        {
            var content = await _governadoresServices.GetTodos();
            var governadores = JsonConvert.DeserializeObject<List<GovernadorContainer>>(content);             

            if(governadores.Count() == 0)
            {
                return NoContent();
            }      

            var governadoresOrdenados = governadores.OrderByDescending(g => g.Governador.Nome);

            return Ok(governadoresOrdenados);
        }

        /// <summary>
        /// Método responsável por pegar todos os Governadores cadastrados na outra API que possuem um número mínimo de projetos de lei.
        /// </summary>  
        [HttpGet("leis/{qtde}")]
        public async Task<IActionResult> GetPorLeis(int qtde)
        {
            var content = await _governadoresServices.GetTodos();
            var governadores = JsonConvert.DeserializeObject<List<GovernadorContainer>>(content);
            var governadoresSelecionados = governadores.Where(g => g.Governador.ProjetosDeLei >= qtde);

            if(governadoresSelecionados.Count() == 0)
            {
                return NoContent();
            }      

            return Ok(governadoresSelecionados);
        }
    }
}