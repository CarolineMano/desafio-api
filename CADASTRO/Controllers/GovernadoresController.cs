using System;
using System.Collections.Generic;
using System.Linq;
using CADASTRO.Container;
using CADASTRO.Data;
using CADASTRO.DTO;
using CADASTRO.Helpers;
using CADASTRO.Models;
using CADASTRO.HATEOAS;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace CADASTRO.Controllers
{
    /// <summary>
    /// Controlador responsável pelo CRUD de Governadores, acessável, por padrão, por admin.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class GovernadoresController : ControllerBase
    {
        private readonly ApplicationDbContext _database;
        private readonly LinkHelper _hateoas;
        private readonly FileHelper _fileHelper;
        public GovernadoresController(ApplicationDbContext database, IWebHostEnvironment hostEnvironment)
        {
            _database = database;
            _fileHelper = new FileHelper(hostEnvironment);
            _hateoas = new LinkHelper("localhost:5001/api/Governadores");
            _hateoas.AddActionById("EDITAR_GOVERNADOR", "PATCH");
            _hateoas.AddActionById("INFO_GOVERNADOR", "GET");
            _hateoas.AddActionById("DELETAR_GOVERNADOR", "DELETE");
            _hateoas.AddGeneralActions("TODOS_GOVERNADORES", "GET");
            _hateoas.AddGeneralActions("NOVO_GOVERNADOR", "POST");
        }

        /// <summary>
        /// Método para pegar a lista de todos os governadores cadastrados. Disponível para todos, dados sensíveis só são exibidos para admin.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetGovernadores()
        {
            var adminLogado = HttpContext.User.IsInRole("Admin");
            List<Governador> governadoresBd = new List<Governador>();

            if(adminLogado)
            {
                governadoresBd = _database.Governadores.Include(governador => governador.Partido).Include(governador => governador.DadosSensiveis).ToList();
            }

            else
            {
                governadoresBd = _database.Governadores.Include(governador => governador.Partido).ToList();
            }
            
            List<GovernadorContainer> governadoresHateoas = new List<GovernadorContainer>();
            
            foreach (var governador in governadoresBd)
            {
                GovernadorContainer governadorComLinks = new GovernadorContainer();
                governadorComLinks.Governador = governador;
                governadorComLinks.Links = adminLogado ? _hateoas.GetActionsById(governador.Id.ToString()) : _hateoas.GetOpenActions(governador.Id.ToString());
                governadoresHateoas.Add(governadorComLinks);
            }

            return Ok(governadoresHateoas.ToArray());
        }

        /// <summary>
        /// Método para pegar um governador por id. Disponível para todos, os dados sensíveis só são exibidos para admin.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetGovernadorPorId(int id)
        {
            try
            {
                var adminLogado = HttpContext.User.IsInRole("Admin");
                Governador governadorBd = new Governador();

                if(adminLogado)
                {
                    governadorBd = _database.Governadores.Include(governador => governador.Partido).Include(governador => governador.DadosSensiveis).First(governador => governador.Id == id);
                }
                else
                {
                    governadorBd = _database.Governadores.Include(governador => governador.Partido).First(governador => governador.Id == id);                    
                }                

                GovernadorContainer governadorComLinks = new GovernadorContainer();
                governadorComLinks.Governador = governadorBd;
                governadorComLinks.Links = adminLogado ? _hateoas.GetActionsById(id.ToString()) : _hateoas.GetOpenActions(id.ToString());

                return Ok(governadorComLinks);
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Governador não encontrado");
            }
        }

        /// <summary>
        /// Método para criar novo governador. Disponível apenas para admin.
        /// </summary>
        [HttpPost]
        public IActionResult NovoGovernador(IFormFile imagem, [FromForm] PoliticoDto modeloPolitico, [FromForm] GovernadorDto modeloGovernador)
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
                    var partidoBd = _database.Partidos.First(p => p.NumeroPartido == modeloGovernador.NumeroPartido);
                    DadoSensivel novoDadoSensivel = PoliticoHelper.NovoDadosSensiveis(modeloPolitico);

                    _database.DadosSensiveis.Add(novoDadoSensivel);

                    var nomeFoto = _fileHelper.UploadedFile(imagem, modeloPolitico.Nome);

                    Governador novoGovernador = new Governador();
                    novoGovernador = (Governador)PoliticoHelper.NovoPolitico(novoDadoSensivel, nomeFoto, partidoBd, modeloPolitico.Nome, modeloPolitico.ProjetosDeLei, "Governador");

                    novoGovernador.Processo = modeloGovernador.Processado;
                    novoGovernador.Estado = modeloGovernador.Estado;

                    _database.Governadores.Add(novoGovernador);
                    _database.SaveChanges();

                    GovernadorContainer governadorComLinks = new GovernadorContainer();
                    governadorComLinks.Governador = novoGovernador;
                    governadorComLinks.Links = _hateoas.GetActionsById(novoGovernador.Id.ToString());

                    Response.StatusCode = 201;
                    return new ObjectResult(governadorComLinks);    
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
        /// Método para editar governador existente. Disponível apenas para admin.
        /// </summary>
        [HttpPatch("{id}")]
        public IActionResult EditarGovernador(int id, IFormFile imagem, [FromForm] PoliticoPatchDto modeloPolitico, [FromForm] GovernadorPatchDto modeloGovernador)
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
                var governadorBd = _database.Governadores.Include(g => g.Partido).Include(g => g.DadosSensiveis).First(g => g.Id == id);
                PoliticoHelper.EditarDadosSensiveis(modeloPolitico, governadorBd.DadosSensiveis.Id, _database);

                var partidoBd = _database.Partidos.FirstOrDefault(p => p.NumeroPartido == modeloGovernador.NumeroPartido);

                if(partidoBd == null && modeloGovernador.NumeroPartido != 0 && modeloGovernador.NumeroPartido != null)
                {
                    Response.StatusCode = 404;
                    return new ObjectResult("Não há esse partido cadastrado");
                }

                governadorBd.Nome = modeloPolitico.Nome != null ? modeloPolitico.Nome : governadorBd.Nome;
                governadorBd.ProjetosDeLei = modeloPolitico.ProjetosDeLei != null ? (int)modeloPolitico.ProjetosDeLei : governadorBd.ProjetosDeLei;
                governadorBd.Foto = imagem == null ? governadorBd.Foto : _fileHelper.UploadedFile(imagem, modeloPolitico.Nome);
                governadorBd.Processo = modeloGovernador.Processado != null ? (bool)modeloGovernador.Processado : governadorBd.Processo;
                governadorBd.Estado = modeloGovernador.Estado != null ? modeloGovernador.Estado : governadorBd.Estado;

                if(partidoBd != null)
                {
                    governadorBd.PartidoId = partidoBd.Id;
                    governadorBd.Partido = partidoBd;
                }
            
                _database.SaveChanges();

                GovernadorContainer governadorComLinks = new GovernadorContainer();
                governadorComLinks.Governador = governadorBd;
                governadorComLinks.Links = _hateoas.GetActionsById(id.ToString());
               
                return Ok(governadorComLinks);
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Governador não encontrado");
            }
        }
        
        /// <summary>
        /// Método para excluir governador existente. Disponível apenas para admin.
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult DeletarGovernador(int id)
        {
            try
            {
                var governadorBd = _database.Governadores.First(g => g.Id == id);
                var dadosBd = _database.DadosSensiveis.First(d => d.Id == governadorBd.DadosSensiveisId);
                _database.DadosSensiveis.Remove(dadosBd);                
                _database.Governadores.Remove(governadorBd);
                _database.SaveChanges();

                GovernadorContainer governadorComLinks = new GovernadorContainer();
                governadorComLinks.Governador = governadorBd;
                governadorComLinks.Links = _hateoas.GetGeneralActions();
         
                return Ok(new {msg = "Governador excluído", governador = governadorComLinks});         
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Governador não encontrado");
            }
        }
    }
}