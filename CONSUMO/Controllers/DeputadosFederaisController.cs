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
    /// Controlador responsável pelas pesquisas de Deputados Federais, acessável apenas por usuário logado.
    /// </summary>       
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DeputadosFederaisController : ControllerBase
    {
        private readonly PoliticosServices _deputadosFederaisServices;        
        public DeputadosFederaisController() : base()
        {
            _deputadosFederaisServices = new PoliticosServices("DeputadosFederais");
        }

        /// <summary>
        /// Método responsável por pegar todos os Deputados Federais cadastrados na outra API.
        /// </summary>    
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var content = await _deputadosFederaisServices.GetTodos();
            var deputadosFederais = JsonConvert.DeserializeObject<List<DeputadoFederalContainer>>(content); 
            
            if(deputadosFederais.Count() == 0)
            {
                return NoContent();
            }                
            
            return Ok(deputadosFederais);
        }    

        /// <summary>
        /// Método responsável por pegar um Deputado Federal cadastrado na outra API.
        /// </summary>  
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var content = await _deputadosFederaisServices.GetPorId(id);
            try
            {
                var deputadoFederal = JsonConvert.DeserializeObject<DeputadoFederalContainer>(content);
                return Ok(deputadoFederal);
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult(content);
            }  
        }    

        /// <summary>
        /// Método responsável por pegar todos os Deputados Federais cadastrados na outra API em ordem ascendente de nome.
        /// </summary>         
        [HttpGet("asc")]
        public async Task<IActionResult> GetAsc()
        {
            var content = await _deputadosFederaisServices.GetTodos();
            var deputadosFederais = JsonConvert.DeserializeObject<List<DeputadoFederalContainer>>(content);             

            if(deputadosFederais.Count() == 0)
            {
                return NoContent();
            }     

            var deputadosFederaisOrdenados = deputadosFederais.OrderBy(dF => dF.DeputadoFederal.Nome);

            return Ok(deputadosFederaisOrdenados);
        }

        /// <summary>
        /// Método responsável por pegar todos os Deputados Federais cadastrados na outra API em ordem descendente de nome.
        /// </summary>  
        [HttpGet("desc")]
        public async Task<IActionResult> GetDesc()
        {
            var content = await _deputadosFederaisServices.GetTodos();
            var deputadosFederais = JsonConvert.DeserializeObject<List<DeputadoFederalContainer>>(content);             

            if(deputadosFederais.Count() == 0)
            {
                return NoContent();
            }     

            var deputadosFederaisOrdenados = deputadosFederais.OrderByDescending(dF => dF.DeputadoFederal.Nome);

            return Ok(deputadosFederaisOrdenados);
        }

        /// <summary>
        /// Método responsável por pegar todos os Deputados Federais cadastrados na outra API que possuem um número mínimo de projetos de lei.
        /// </summary> 
        [HttpGet("leis/{qtde}")]
        public async Task<IActionResult> GetPorLeis(int qtde)
        {
            var content = await _deputadosFederaisServices.GetTodos();
            var deputadosFederais = JsonConvert.DeserializeObject<List<DeputadoFederalContainer>>(content);
            var deputadosFederaisSelecionados = deputadosFederais.Where(dF => dF.DeputadoFederal.ProjetosDeLei >= qtde);

            if(deputadosFederaisSelecionados.Count() == 0)
            {
                return NoContent();
            }     

            return Ok(deputadosFederaisSelecionados);
        }
    }
}