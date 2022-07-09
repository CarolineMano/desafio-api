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
    /// Controlador responsável pelas pesquisas de Ministros de Estado, acessável apenas por usuário logado.
    /// </summary>   
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MinistrosController : ControllerBase
    {
        private readonly PoliticosServices _ministrosServices;        
        public MinistrosController() : base()
        {
            _ministrosServices = new PoliticosServices("Ministros");
        }

        /// <summary>
        /// Método responsável por pegar todos os Ministros de Estado cadastrados na outra API.
        /// </summary>    
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var content = await _ministrosServices.GetTodos();
            var ministros = JsonConvert.DeserializeObject<List<MinistroContainer>>(content); 

            if(ministros.Count() == 0)
            {
                return NoContent();
            }      

            return Ok(ministros);
        }    

        /// <summary>
        /// Método responsável por pegar um Ministro de Estado cadastrado na outra API.
        /// </summary>    
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var content = await _ministrosServices.GetPorId(id);
            try
            {
                var ministro = JsonConvert.DeserializeObject<MinistroContainer>(content);
                return Ok(ministro);
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult(content);
            }  
        }    

        /// <summary>
        /// Método responsável por pegar todos os Ministros de Estado cadastrados na outra API em ordem ascendente de nome.
        /// </summary>   
        [HttpGet("asc")]
        public async Task<IActionResult> GetAsc()
        {
            var content = await _ministrosServices.GetTodos();
            var ministros = JsonConvert.DeserializeObject<List<MinistroContainer>>(content);             
            
            if(ministros.Count() == 0)
            {
                return NoContent();
            }   

            var ministrosOrdenados = ministros.OrderBy(m => m.Ministro.Nome);

            return Ok(ministrosOrdenados);
        }

        /// <summary>
        /// Método responsável por pegar todos os Ministros de Estado cadastrados na outra API em ordem descendente de nome.
        /// </summary>             
        [HttpGet("desc")]
        public async Task<IActionResult> GetDesc()
        {
            var content = await _ministrosServices.GetTodos();
            var ministros = JsonConvert.DeserializeObject<List<MinistroContainer>>(content);             

            if(ministros.Count() == 0)
            {
                return NoContent();
            }   

            var ministrosOrdenados = ministros.OrderByDescending(m => m.Ministro.Nome);

            return Ok(ministrosOrdenados);
        }

        /// <summary>
        /// Método responsável por pegar todos os Ministros de Estado cadastrados na outra API que possuem um número mínimo de projetos de lei.
        /// </summary>          
        [HttpGet("leis/{qtde}")]
        public async Task<IActionResult> GetPorLeis(int qtde)
        {
            var content = await _ministrosServices.GetTodos();
            var ministros = JsonConvert.DeserializeObject<List<MinistroContainer>>(content);
            var ministrosSelecionados = ministros.Where(m => m.Ministro.ProjetosDeLei >= qtde);

            if(ministrosSelecionados.Count() == 0)
            {
                return NoContent();
            }   

            return Ok(ministrosSelecionados);
        }
    }
}