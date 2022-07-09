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

namespace CADASTRO.Controllers
{
    /// <summary>
    /// Controlador responsável pelo CRUD de Deputados Estaduais, acessável, por padrão, por admin.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class DeputadosFederaisController : ControllerBase
    {
        private readonly ApplicationDbContext _database;
        private readonly LinkHelper _hateoas;
        private readonly FileHelper _fileHelper;
        public DeputadosFederaisController(ApplicationDbContext database, IWebHostEnvironment hostEnvironment)
        {
            _database = database;
            _fileHelper = new FileHelper(hostEnvironment);
            _hateoas = new LinkHelper("localhost:5001/api/DeputadosFederais");
            _hateoas.AddActionById("EDITAR_DEPUTADO-FEDERAL", "PATCH");
            _hateoas.AddActionById("INFO_DEPUTADO-FEDERAL", "GET");
            _hateoas.AddActionById("DELETAR_DEPUTADO-FEDERAL", "DELETE");
            _hateoas.AddGeneralActions("TODOS_DEPUTADO-FEDERAIS", "GET");
            _hateoas.AddGeneralActions("NOVO_DEPUTADO-FEDERAL", "POST");
        }        

        /// <summary>
        /// Método para pegar a lista de todos os deputados federais cadastrados. Disponível para todos, os dados sensíveis só são exibidos para admin.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetDeputadoFederais()
        {
            var adminLogado = HttpContext.User.IsInRole("Admin");
            List<DeputadoFederal> deputadosFederaisBd = new List<DeputadoFederal>();

            if(adminLogado)
            {
                deputadosFederaisBd = _database.DeputadosFederais.Include(deputadoFederal => deputadoFederal.Partido).Include(deputadoFederal => deputadoFederal.DadosSensiveis).ToList();
            }

            else
            {
                deputadosFederaisBd = _database.DeputadosFederais.Include(deputadoFederal => deputadoFederal.Partido).ToList();
            }
            
            List<DeputadoFederalContainer> deputadosFederaisHateoas = new List<DeputadoFederalContainer>();

            foreach (var deputadoFederal in deputadosFederaisBd)
            {
                deputadoFederal.Lider = _database.Lideres.First(lider => lider.Id == deputadoFederal.PartidoId).Nome;
                
                DeputadoFederalContainer deputadoFederalComLinks = new DeputadoFederalContainer();
                deputadoFederalComLinks.DeputadoFederal = deputadoFederal;
                deputadoFederalComLinks.Links = adminLogado ? _hateoas.GetActionsById(deputadoFederal.Id.ToString()) : _hateoas.GetOpenActions(deputadoFederal.Id.ToString());
                deputadosFederaisHateoas.Add(deputadoFederalComLinks);
            }

            return Ok(deputadosFederaisHateoas.ToArray());
        }        

        /// <summary>
        /// Método para pegar um deputado federal por id. Disponível para todos, os dados sensíveis só são exibidos para admin.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetDeputadoFederalPorId(int id)
        {
            try
            {
                var adminLogado = HttpContext.User.IsInRole("Admin");
                DeputadoFederal deputadoFederalBd = new DeputadoFederal();

                if(adminLogado)
                {
                    deputadoFederalBd = _database.DeputadosFederais.Include(deputadoFederal => deputadoFederal.Partido).Include(deputadoFederal => deputadoFederal.DadosSensiveis).First(deputadoFederal => deputadoFederal.Id == id);
                }
                else
                {
                    deputadoFederalBd = _database.DeputadosFederais.Include(deputadoFederal => deputadoFederal.Partido).First(deputadoFederal => deputadoFederal.Id == id);                    
                }                

                deputadoFederalBd.Lider = _database.Lideres.First(lider => lider.Id == deputadoFederalBd.PartidoId).Nome;

                DeputadoFederalContainer deputadoFederalComLinks = new DeputadoFederalContainer();
                deputadoFederalComLinks.DeputadoFederal = deputadoFederalBd;
                deputadoFederalComLinks.Links = adminLogado ? _hateoas.GetActionsById(id.ToString()) : _hateoas.GetOpenActions(id.ToString());

                return Ok(deputadoFederalComLinks);
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Deputado Federal não encontrado");
            }
        }

        /// <summary>
        /// Método para criar novo deputado federal. Disponível apenas para admin.
        /// </summary>
        [HttpPost]
        public IActionResult NovoDeputadoFederal(IFormFile imagem, [FromForm] PoliticoDto modeloPolitico, [FromForm] DeputadoDto modeloDeputadoFederal)
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
                    var partidoBd = _database.Partidos.First(p => p.NumeroPartido == modeloDeputadoFederal.NumeroPartido);
                    DadoSensivel novoDadoSensivel = PoliticoHelper.NovoDadosSensiveis(modeloPolitico);

                    _database.DadosSensiveis.Add(novoDadoSensivel);

                    var nomeFoto = _fileHelper.UploadedFile(imagem, modeloPolitico.Nome);

                    DeputadoFederal novoDeputadoFederal = new DeputadoFederal();
                    novoDeputadoFederal = (DeputadoFederal)PoliticoHelper.NovoPolitico(novoDadoSensivel, nomeFoto, partidoBd, modeloPolitico.Nome, modeloPolitico.ProjetosDeLei, "Deputado Federal");

                    novoDeputadoFederal.Processo = modeloDeputadoFederal.Processado;
                    novoDeputadoFederal.Estado = modeloDeputadoFederal.Estado;
                    novoDeputadoFederal.Lider = _database.Lideres.First(lider => lider.Id == partidoBd.Id).Nome;

                    _database.DeputadosFederais.Add(novoDeputadoFederal);
                    _database.SaveChanges();

                    DeputadoFederalContainer deputadoFederalComLinks = new DeputadoFederalContainer();
                    deputadoFederalComLinks.DeputadoFederal = novoDeputadoFederal;
                    deputadoFederalComLinks.Links = _hateoas.GetActionsById(novoDeputadoFederal.Id.ToString());

                    Response.StatusCode = 201;
                    return new ObjectResult(deputadoFederalComLinks);    
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
        /// Método para editar deputado federal existente. Disponível apenas para admin.
        /// </summary>
        [HttpPatch("{id}")]
        public IActionResult EditarDeputadoFederal(int id, IFormFile imagem, [FromForm] PoliticoPatchDto modeloPolitico, [FromForm] DeputadoPatchDto modeloDeputadoFederal)
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
                var deputadoFederalBd = _database.DeputadosFederais.Include(dF => dF.Partido).Include(dF => dF.DadosSensiveis).First(dF => dF.Id == id);
                PoliticoHelper.EditarDadosSensiveis(modeloPolitico, deputadoFederalBd.DadosSensiveis.Id, _database);

                var partidoBd = _database.Partidos.FirstOrDefault(p => p.NumeroPartido == modeloDeputadoFederal.NumeroPartido);

                if(partidoBd == null && modeloDeputadoFederal.NumeroPartido != 0 && modeloDeputadoFederal.NumeroPartido != null)
                {
                    Response.StatusCode = 404;
                    return new ObjectResult("Não há esse partido cadastrado");
                }

                deputadoFederalBd.Nome = modeloPolitico.Nome != null ? modeloPolitico.Nome : deputadoFederalBd.Nome;
                deputadoFederalBd.ProjetosDeLei = modeloPolitico.ProjetosDeLei != null ? (int)modeloPolitico.ProjetosDeLei : deputadoFederalBd.ProjetosDeLei;
                deputadoFederalBd.Foto = imagem == null ? deputadoFederalBd.Foto : _fileHelper.UploadedFile(imagem, modeloPolitico.Nome);
                deputadoFederalBd.Processo = modeloDeputadoFederal.Processado != null ? (bool)modeloDeputadoFederal.Processado : deputadoFederalBd.Processo;
                deputadoFederalBd.Estado = modeloDeputadoFederal.Estado != null ? modeloDeputadoFederal.Estado : deputadoFederalBd.Estado;
                deputadoFederalBd.Lider = _database.Lideres.First(lider => lider.Id == deputadoFederalBd.PartidoId).Nome;

                if(partidoBd != null)
                {
                    deputadoFederalBd.PartidoId = partidoBd.Id;
                    deputadoFederalBd.Partido = partidoBd;
                }
            
                _database.SaveChanges();

                DeputadoFederalContainer deputadoFederalComLinks = new DeputadoFederalContainer();
                deputadoFederalComLinks.DeputadoFederal = deputadoFederalBd;
                deputadoFederalComLinks.Links = _hateoas.GetActionsById(id.ToString());
               
                return Ok(deputadoFederalComLinks);
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Deputado Federal não encontrado");
            }
        }

        /// <summary>
        /// Método para excluir deputado federal existente. Disponível apenas para admin.
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult DeletarDeputadoFederal(int id)
        {
            try
            {
                var deputadoFederalBd = _database.DeputadosFederais.First(dF => dF.Id == id);
                var dadosBd = _database.DadosSensiveis.First(d => d.Id == deputadoFederalBd.DadosSensiveisId);

                deputadoFederalBd.Lider = _database.Lideres.First(lider => lider.Id == deputadoFederalBd.PartidoId).Nome;

                _database.DadosSensiveis.Remove(dadosBd);                
                _database.DeputadosFederais.Remove(deputadoFederalBd);
                _database.SaveChanges();

                DeputadoFederalContainer deputadoFederalComLinks = new DeputadoFederalContainer();
                deputadoFederalComLinks.DeputadoFederal = deputadoFederalBd;
                deputadoFederalComLinks.Links = _hateoas.GetGeneralActions();
         
                return Ok(new {msg = "Deputado Federal excluído", deputadoFederal = deputadoFederalComLinks});         
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Deputado Federal não encontrado");
            }
        }

    }
}