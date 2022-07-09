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
    /// Controlador responsável pelas pesquisas de Deputados Estaduais, acessável apenas por usuário logado.
    /// </summary>    
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DeputadosEstaduaisController : ControllerBase
    {
        private readonly PoliticosServices _deputadosEstaduaisServices;        
        public DeputadosEstaduaisController() : base()
        {
            _deputadosEstaduaisServices = new PoliticosServices("DeputadosEstaduais");
        }

        /// <summary>
        /// Método responsável por pegar todos os Deputados Estaduais cadastrados na outra API.
        /// </summary>    
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var content = await _deputadosEstaduaisServices.GetTodos();
            var deputadosEstaduais = JsonConvert.DeserializeObject<List<DeputadoEstadualContainer>>(content); 
            
            if(deputadosEstaduais.Count() == 0)
            {
                return NoContent();
            }            
            
            return Ok(deputadosEstaduais);
        }    

        /// <summary>
        /// Método responsável por pegar um Deputado Estadual cadastrado na outra API.
        /// </summary>    
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var content = await _deputadosEstaduaisServices.GetPorId(id);
            try
            {
                var deputadoEstadual = JsonConvert.DeserializeObject<DeputadoEstadualContainer>(content);
                return Ok(deputadoEstadual);
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult(content);
            }  
        }

        /// <summary>
        /// Método responsável por pegar todos os Deputados Estaduais cadastrados na outra API em ordem ascendente de nome.
        /// </summary>            
        [HttpGet("asc")]
        public async Task<IActionResult> GetAsc()
        {
            var content = await _deputadosEstaduaisServices.GetTodos();
            var deputadosEstaduais = JsonConvert.DeserializeObject<List<DeputadoEstadualContainer>>(content);             
            
            if(deputadosEstaduais.Count() == 0)
            {
                return NoContent();
            }            
            
            var deputadosEstaduaisOrdenados = deputadosEstaduais.OrderBy(dE => dE.DeputadoEstadual.Nome);

            return Ok(deputadosEstaduaisOrdenados);
        }

        /// <summary>
        /// Método responsável por pegar todos os Deputados Estaduais cadastrados na outra API em ordem descendente de nome.
        /// </summary>           
        [HttpGet("desc")]
        public async Task<IActionResult> GetDesc()
        {
            var content = await _deputadosEstaduaisServices.GetTodos();
            var deputadosEstaduais = JsonConvert.DeserializeObject<List<DeputadoEstadualContainer>>(content); 

            if(deputadosEstaduais.Count() == 0)
            {
                return NoContent();
            }

            var deputadosEstaduaisOrdenados = deputadosEstaduais.OrderByDescending(dE => dE.DeputadoEstadual.Nome);

            return Ok(deputadosEstaduaisOrdenados);
        }

        /// <summary>
        /// Método responsável por pegar todos os Deputados Estaduais cadastrados na outra API que possuem um número mínimo de projetos de lei.
        /// </summary>  
        [HttpGet("leis/{qtde}")]
        public async Task<IActionResult> GetPorLeis(int qtde)
        {
            var content = await _deputadosEstaduaisServices.GetTodos();
            var deputadosEstaduais = JsonConvert.DeserializeObject<List<DeputadoEstadualContainer>>(content);
            var deputadosEstaduaisSelecionados = deputadosEstaduais.Where(dE => dE.DeputadoEstadual.ProjetosDeLei >= qtde);

            if(deputadosEstaduaisSelecionados.Count() == 0)
            {
                return NoContent();
            }

            return Ok(deputadosEstaduaisSelecionados);
        }
    }
}