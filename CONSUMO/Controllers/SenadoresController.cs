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
    /// Controlador responsável pelas pesquisas de Senadores, acessável apenas por usuário logado.
    /// </summary>  
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SenadoresController : ControllerBase
    {
        private readonly PoliticosServices _senadoresServices;        
        public SenadoresController() : base()
        {
            _senadoresServices = new PoliticosServices("Senadores");
        }

        /// <summary>
        /// Método responsável por pegar todos os Senadores cadastrados na outra API.
        /// </summary>    
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var content = await _senadoresServices.GetTodos();
            var senadores = JsonConvert.DeserializeObject<List<SenadorContainer>>(content); 

            if(senadores.Count() == 0)
            {
                return NoContent();
            }  

            return Ok(senadores);
        }    

        /// <summary>
        /// Método responsável por pegar um Senador cadastrado na outra API.
        /// </summary>   
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var content = await _senadoresServices.GetPorId(id);
            try
            {
                var senador = JsonConvert.DeserializeObject<SenadorContainer>(content);
                return Ok(senador);
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult(content);
            }  
        }   

        /// <summary>
        /// Método responsável por pegar todos os Senadores cadastrados na outra API em ordem ascendente de nome.
        /// </summary>   
        [HttpGet("asc")]
        public async Task<IActionResult> GetAsc()
        {
            var content = await _senadoresServices.GetTodos();
            var senadores = JsonConvert.DeserializeObject<List<SenadorContainer>>(content);             

            if(senadores.Count() == 0)
            {
                return NoContent();
            }  

            var senadoresOrdenados = senadores.OrderBy(s => s.Senador.Nome);

            return Ok(senadoresOrdenados);
        }

        /// <summary>
        /// Método responsável por pegar todos os Senadores cadastrados na outra API em ordem descendente de nome.
        /// </summary>           
        [HttpGet("desc")]
        public async Task<IActionResult> GetDesc()
        {
            var content = await _senadoresServices.GetTodos();
            var senadores = JsonConvert.DeserializeObject<List<SenadorContainer>>(content);             

            if(senadores.Count() == 0)
            {
                return NoContent();
            }  

            var senadoresOrdenados = senadores.OrderByDescending(s => s.Senador.Nome);

            return Ok(senadoresOrdenados);
        }

        /// <summary>
        /// Método responsável por pegar todos os Senadores cadastrados na outra API que possuem um número mínimo de projetos de lei.
        /// </summary>        
        [HttpGet("leis/{qtde}")]
        public async Task<IActionResult> GetPorLeis(int qtde)
        {
            var content = await _senadoresServices.GetTodos();
            var senadores = JsonConvert.DeserializeObject<List<SenadorContainer>>(content);
            var senadoresSelecionados = senadores.Where(s => s.Senador.ProjetosDeLei >= qtde);

            if(senadoresSelecionados.Count() == 0)
            {
                return NoContent();
            }  

            return Ok(senadoresSelecionados);
        }
    }
}