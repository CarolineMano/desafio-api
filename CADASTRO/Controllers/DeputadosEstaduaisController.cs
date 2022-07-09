using System.Collections.Generic;
using System.Linq;
using CADASTRO.Container;
using CADASTRO.Data;
using CADASTRO.DTO;
using CADASTRO.HATEOAS;
using CADASTRO.Helpers;
using CADASTRO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CADASTRO.Controllers
{
    /// <summary>
    /// Controlador responsável pelo CRUD de Deputados Estaduais, acessável, por padrão, por admin.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class DeputadosEstaduaisController : ControllerBase
    {
        private readonly ApplicationDbContext _database;
        private readonly LinkHelper _hateoas;
        private readonly FileHelper _fileHelper;
        public DeputadosEstaduaisController(ApplicationDbContext database, IWebHostEnvironment hostEnvironment)
        {
            _database = database;
            _fileHelper = new FileHelper(hostEnvironment);
            _hateoas = new LinkHelper("localhost:5001/api/DeputadosEstaduais");
            _hateoas.AddActionById("EDITAR_DEPUTADO-ESTADUAL", "PATCH");
            _hateoas.AddActionById("INFO_DEPUTADO-ESTADUAL", "GET");
            _hateoas.AddActionById("DELETAR_DEPUTADO-ESTADUAL", "DELETE");
            _hateoas.AddGeneralActions("TODOS_DEPUTADO-ESTADUAIS", "GET");
            _hateoas.AddGeneralActions("NOVO_DEPUTADO-ESTADUAL", "POST");
        }

        /// <summary>
        /// Método para pegar a lista de todos os deputados estaduais cadastrados. Disponível para todos, os dados sensíveis só são exibidos para admin.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetDeputadoEstaduais()
        {
            var adminLogado = HttpContext.User.IsInRole("Admin");
            List<DeputadoEstadual> deputadosEstaduaisBd = new List<DeputadoEstadual>();

            if(adminLogado)
            {
                deputadosEstaduaisBd = _database.DeputadosEstaduais.Include(deputadoEstadual => deputadoEstadual.Partido).Include(deputadoEstadual => deputadoEstadual.DadosSensiveis).ToList();
            }

            else
            {
                deputadosEstaduaisBd = _database.DeputadosEstaduais.Include(deputadoEstadual => deputadoEstadual.Partido).ToList();
            }
            
            List<DeputadoEstadualContainer> deputadosEstaduaisHateoas = new List<DeputadoEstadualContainer>();
            
            foreach (var deputadoEstadual in deputadosEstaduaisBd)
            {
                DeputadoEstadualContainer deputadoEstadualComLinks = new DeputadoEstadualContainer();
                deputadoEstadualComLinks.DeputadoEstadual = deputadoEstadual;
                deputadoEstadualComLinks.Links = adminLogado ? _hateoas.GetActionsById(deputadoEstadual.Id.ToString()) : _hateoas.GetOpenActions(deputadoEstadual.Id.ToString());
                deputadosEstaduaisHateoas.Add(deputadoEstadualComLinks);
            }

            return Ok(deputadosEstaduaisHateoas.ToArray());
        }        

        /// <summary>
        /// Método para pegar um deputado estadual por id. Disponível para todos, os dados sensíveis só são exibidos para admin.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetDeputadoEstadualPorId(int id)
        {
            try
            {
                var adminLogado = HttpContext.User.IsInRole("Admin");
                DeputadoEstadual deputadoEstadualBd = new DeputadoEstadual();

                if(adminLogado)
                {
                    deputadoEstadualBd = _database.DeputadosEstaduais.Include(deputadoEstadual => deputadoEstadual.Partido).Include(deputadoEstadual => deputadoEstadual.DadosSensiveis).First(deputadoEstadual => deputadoEstadual.Id == id);
                }
                else
                {
                    deputadoEstadualBd = _database.DeputadosEstaduais.Include(deputadoEstadual => deputadoEstadual.Partido).First(deputadoEstadual => deputadoEstadual.Id == id);                    
                }                

                DeputadoEstadualContainer deputadoEstadualComLinks = new DeputadoEstadualContainer();
                deputadoEstadualComLinks.DeputadoEstadual = deputadoEstadualBd;
                deputadoEstadualComLinks.Links = adminLogado ? _hateoas.GetActionsById(id.ToString()) : _hateoas.GetOpenActions(id.ToString());

                return Ok(deputadoEstadualComLinks);
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Deputado Estadual não encontrado");
            }
        }

        /// <summary>
        /// Método para criar novo deputado estadual. Disponível apenas para admin.
        /// </summary>
        [HttpPost]
        public IActionResult NovoDeputadoEstadual(IFormFile imagem, [FromForm] PoliticoDto modeloPolitico, [FromForm] DeputadoDto modeloDeputadoEstadual)
        {
            if(ModelState.IsValid)
            {
                if(!ValidaCpf.ValidaCpfExtension.Validate(modeloPolitico.Cpf))
                {
                    Response.StatusCode = 422;
                    return new ObjectResult("CPF inválido");
                }
                try
                {
                    var partidoBd = _database.Partidos.First(p => p.NumeroPartido == modeloDeputadoEstadual.NumeroPartido);
                    DadoSensivel novoDadoSensivel = PoliticoHelper.NovoDadosSensiveis(modeloPolitico);

                    _database.DadosSensiveis.Add(novoDadoSensivel);

                    var nomeFoto = _fileHelper.UploadedFile(imagem, modeloPolitico.Nome);

                    DeputadoEstadual novoDeputadoEstadual = new DeputadoEstadual();
                    novoDeputadoEstadual = (DeputadoEstadual)PoliticoHelper.NovoPolitico(novoDadoSensivel, nomeFoto, partidoBd, modeloPolitico.Nome, modeloPolitico.ProjetosDeLei, "Deputado Estadual");

                    novoDeputadoEstadual.Processo = modeloDeputadoEstadual.Processado;
                    novoDeputadoEstadual.Estado = modeloDeputadoEstadual.Estado;

                    _database.DeputadosEstaduais.Add(novoDeputadoEstadual);
                    _database.SaveChanges();

                    DeputadoEstadualContainer deputadoEstadualComLinks = new DeputadoEstadualContainer();
                    deputadoEstadualComLinks.DeputadoEstadual = novoDeputadoEstadual;
                    deputadoEstadualComLinks.Links = _hateoas.GetActionsById(novoDeputadoEstadual.Id.ToString());

                    Response.StatusCode = 201;
                    return new ObjectResult(deputadoEstadualComLinks);    
                }
                catch (System.Exception)
                {
                    Response.StatusCode = 404;
                    return new ObjectResult("Partido não cadastrado");
                }            
            }
            return BadRequest();            
        }

        /// <summary>
        /// Método para editar deputado estadual existente. Disponível apenas para admin.
        /// </summary>
        [HttpPatch("{id}")]
        public IActionResult EditarDeputadoEstadual(int id, IFormFile imagem, [FromForm] PoliticoPatchDto modeloPolitico, [FromForm] DeputadoPatchDto modeloDeputadoEstadual)
        {
            if(modeloPolitico.Cpf != null)
            {
                if(!ValidaCpf.ValidaCpfExtension.Validate(modeloPolitico.Cpf))
                {
                    Response.StatusCode = 422;
                    return new ObjectResult("CPF inválido");
                }  
            }
          
            try
            {
                var deputadoEstadualBd = _database.DeputadosEstaduais.Include(dE => dE.Partido).Include(dE => dE.DadosSensiveis).First(dE => dE.Id == id);
                PoliticoHelper.EditarDadosSensiveis(modeloPolitico, deputadoEstadualBd.DadosSensiveis.Id, _database);

                var partidoBd = _database.Partidos.FirstOrDefault(p => p.NumeroPartido == modeloDeputadoEstadual.NumeroPartido);

                if(partidoBd == null && modeloDeputadoEstadual.NumeroPartido != 0 && modeloDeputadoEstadual.NumeroPartido != null)
                {
                    Response.StatusCode = 404;
                    return new ObjectResult("Não há esse partido cadastrado");
                }

                deputadoEstadualBd.Nome = modeloPolitico.Nome != null ? modeloPolitico.Nome : deputadoEstadualBd.Nome;
                deputadoEstadualBd.ProjetosDeLei = modeloPolitico.ProjetosDeLei != null ? (int)modeloPolitico.ProjetosDeLei : deputadoEstadualBd.ProjetosDeLei;
                deputadoEstadualBd.Foto = imagem == null ? deputadoEstadualBd.Foto : _fileHelper.UploadedFile(imagem, modeloPolitico.Nome);
                deputadoEstadualBd.Processo = modeloDeputadoEstadual.Processado != null ? (bool)modeloDeputadoEstadual.Processado : deputadoEstadualBd.Processo;
                deputadoEstadualBd.Estado = modeloDeputadoEstadual.Estado != null ? modeloDeputadoEstadual.Estado : deputadoEstadualBd.Estado;

                if(partidoBd != null)
                {
                    deputadoEstadualBd.PartidoId = partidoBd.Id;
                    deputadoEstadualBd.Partido = partidoBd;
                }
            
                _database.SaveChanges();

                DeputadoEstadualContainer deputadoEstadualComLinks = new DeputadoEstadualContainer();
                deputadoEstadualComLinks.DeputadoEstadual = deputadoEstadualBd;
                deputadoEstadualComLinks.Links = _hateoas.GetActionsById(id.ToString());
               
                return Ok(deputadoEstadualComLinks);
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Deputado Estadual não encontrado");
            }
        }

        /// <summary>
        /// Método para excluir deputado estadual existente. Disponível apenas para admin.
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult DeletarDeputadoEstadual(int id)
        {
            try
            {
                var deputadoEstadualBd = _database.DeputadosEstaduais.First(dE => dE.Id == id);
                var dadosBd = _database.DadosSensiveis.First(d => d.Id == deputadoEstadualBd.DadosSensiveisId);
                _database.DadosSensiveis.Remove(dadosBd);                
                _database.DeputadosEstaduais.Remove(deputadoEstadualBd);
                _database.SaveChanges();

                DeputadoEstadualContainer deputadoEstadualComLinks = new DeputadoEstadualContainer();
                deputadoEstadualComLinks.DeputadoEstadual = deputadoEstadualBd;
                deputadoEstadualComLinks.Links = _hateoas.GetGeneralActions();
         
                return Ok(new {msg = "Deputado Estadual excluído", deputadoEstadual = deputadoEstadualComLinks});         
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Deputado Estadual não encontrado");
            }
        }
    }
}