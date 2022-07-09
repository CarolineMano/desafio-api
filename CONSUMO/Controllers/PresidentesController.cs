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
    /// Controlador responsável pelas pesquisas de Presidentes, acessável apenas por usuário logado.
    /// </summary> 
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PresidentesController : ControllerBase
    {
        private readonly PoliticosServices _presidentesServices;        
        public PresidentesController() : base()
        {
            _presidentesServices = new PoliticosServices("Presidentes");
        }

        /// <summary>
        /// Método responsável por pegar todos os Presidentes cadastrados na outra API.
        /// </summary>    
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var content = await _presidentesServices.GetTodos();
            var presidentes = JsonConvert.DeserializeObject<List<PresidenteContainer>>(content); 

            if(presidentes.Count() == 0)
            {
                return NoContent();
            }  

            return Ok(presidentes);
        }    

        /// <summary>
        /// Método responsável por pegar um Presidente cadastrado na outra API.
        /// </summary>    
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var content = await _presidentesServices.GetPorId(id);
            try
            {
                var presidente = JsonConvert.DeserializeObject<PresidenteContainer>(content);
                return Ok(presidente);
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult(content);
            }  
        }  

        /// <summary>
        /// Método responsável por pegar todos os Presidentes cadastrados na outra API em ordem ascendente de nome.
        /// </summary>   
        [HttpGet("asc")]
        public async Task<IActionResult> GetAsc()
        {
            var content = await _presidentesServices.GetTodos();
            var presidentes = JsonConvert.DeserializeObject<List<PresidenteContainer>>(content);             

            if(presidentes.Count() == 0)
            {
                return NoContent();
            }  

            var presidentesOrdenados = presidentes.OrderBy(dE => dE.Presidente.Nome);

            return Ok(presidentesOrdenados);
        }

        /// <summary>
        /// Método responsável por pegar todos os Presidentes cadastrados na outra API em ordem descendente de nome.
        /// </summary>             
        [HttpGet("desc")]
        public async Task<IActionResult> GetDesc()
        {
            var content = await _presidentesServices.GetTodos();
            var presidentes = JsonConvert.DeserializeObject<List<PresidenteContainer>>(content);             

            if(presidentes.Count() == 0)
            {
                return NoContent();
            }  

            var presidentesOrdenados = presidentes.OrderByDescending(dE => dE.Presidente.Nome);

            return Ok(presidentesOrdenados);
        }

        /// <summary>
        /// Método responsável por pegar todos os Presidentes cadastrados na outra API que possuem um número mínimo de projetos de lei.
        /// </summary>  
        [HttpGet("leis/{qtde}")]
        public async Task<IActionResult> GetPorLeis(int qtde)
        {
            var content = await _presidentesServices.GetTodos();
            var presidentes = JsonConvert.DeserializeObject<List<PresidenteContainer>>(content);
            var presidentesSelecionados = presidentes.Where(p => p.Presidente.ProjetosDeLei >= qtde);

            if(presidentesSelecionados.Count() == 0)
            {
                return NoContent();
            }  

            return Ok(presidentesSelecionados);
        }
    }
}