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
    /// Controlador responsável pelas pesquisas de Prefeitos, acessável apenas por usuário logado.
    /// </summary>       
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PrefeitosController : ControllerBase
    {
        private readonly PoliticosServices _prefeitosServices;        
        public PrefeitosController() : base()
        {
            _prefeitosServices = new PoliticosServices("Prefeitos");
        }

        /// <summary>
        /// Método responsável por pegar todos os Prefeitos cadastrados na outra API.
        /// </summary>   
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var content = await _prefeitosServices.GetTodos();
            var prefeitos = JsonConvert.DeserializeObject<List<PrefeitoContainer>>(content); 

            if(prefeitos.Count() == 0)
            {
                return NoContent();
            }   

            return Ok(prefeitos);
        }    

        /// <summary>
        /// Método responsável por pegar um Prefeito cadastrado na outra API.
        /// </summary>   
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var content = await _prefeitosServices.GetPorId(id);
            try
            {
                var prefeito = JsonConvert.DeserializeObject<PrefeitoContainer>(content);
                return Ok(prefeito);
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult(content);
            }  
        }    

        /// <summary>
        /// Método responsável por pegar todos os Prefeitos cadastrados na outra API em ordem ascendente de nome.
        /// </summary>          
        [HttpGet("asc")]
        public async Task<IActionResult> GetAsc()
        {
            var content = await _prefeitosServices.GetTodos();
            var prefeitos = JsonConvert.DeserializeObject<List<PrefeitoContainer>>(content);             

            if(prefeitos.Count() == 0)
            {
                return NoContent();
            }  

            var prefeitosOrdenados = prefeitos.OrderBy(p => p.Prefeito.Nome);

            return Ok(prefeitosOrdenados);
        }

        /// <summary>
        /// Método responsável por pegar todos os Prefeitos cadastrados na outra API em ordem descendente de nome.
        /// </summary>          
        [HttpGet("desc")]
        public async Task<IActionResult> GetDesc()
        {
            var content = await _prefeitosServices.GetTodos();
            var prefeitos = JsonConvert.DeserializeObject<List<PrefeitoContainer>>(content);             

            if(prefeitos.Count() == 0)
            {
                return NoContent();
            }  

            var prefeitosOrdenados = prefeitos.OrderByDescending(p => p.Prefeito.Nome);

            return Ok(prefeitosOrdenados);
        }

        /// <summary>
        /// Método responsável por pegar todos os Prefeitos cadastrados na outra API que possuem um número mínimo de projetos de lei.
        /// </summary>         
        [HttpGet("leis/{qtde}")]
        public async Task<IActionResult> GetPorLeis(int qtde)
        {
            var content = await _prefeitosServices.GetTodos();
            var prefeitos = JsonConvert.DeserializeObject<List<PrefeitoContainer>>(content);
            var prefeitosSelecionados = prefeitos.Where(p => p.Prefeito.ProjetosDeLei >= qtde);

            if(prefeitosSelecionados.Count() == 0)
            {
                return NoContent();
            }  

            return Ok(prefeitosSelecionados);
        }
    }
}