using System;
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
    /// Controlador responsável pelo CRUD de Prefeitos, acessável, por padrão, por admin.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class PrefeitosController : ControllerBase
    {
        private readonly ApplicationDbContext _database;
        private readonly LinkHelper _hateoas;
        private readonly FileHelper _fileHelper;
        public PrefeitosController(ApplicationDbContext database, IWebHostEnvironment hostEnvironment)
        {
            _database = database;
            _fileHelper = new FileHelper(hostEnvironment);
            _hateoas = new LinkHelper("localhost:5001/api/Prefeitos");
            _hateoas.AddActionById("EDITAR_PREFEITOS", "PATCH");
            _hateoas.AddActionById("INFO_PREFEITOS", "GET");
            _hateoas.AddActionById("DELETAR_PREFEITOS", "DELETE");
            _hateoas.AddGeneralActions("TODOS_PREFEITOS", "GET");
            _hateoas.AddGeneralActions("NOVO_PREFEITOS", "POST");
        }

        /// <summary>
        /// Método para pegar a lista de todos os prefeitos cadastrados. Disponível para todos, os dados sensíveis só são exibidos para admin.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetPrefeitos()
        {
            var adminLogado = HttpContext.User.IsInRole("Admin");
            List<Prefeito> prefeitosBd = new List<Prefeito>();

            if(adminLogado)
            {
                prefeitosBd = _database.Prefeitos.Include(prefeito => prefeito.Partido).Include(prefeito => prefeito.DadosSensiveis).ToList();
            }

            else
            {
                prefeitosBd = _database.Prefeitos.Include(prefeito => prefeito.Partido).ToList();
            }

            List<PrefeitoContainer> prefeitosHateoas = new List<PrefeitoContainer>();
            
            foreach (var prefeito in prefeitosBd)
            {
                PrefeitoContainer prefeitoComLinks = new PrefeitoContainer();
                prefeitoComLinks.Prefeito = prefeito;
                prefeitoComLinks.Links = adminLogado ? _hateoas.GetActionsById(prefeito.Id.ToString()) : _hateoas.GetOpenActions(prefeito.Id.ToString());
                prefeitosHateoas.Add(prefeitoComLinks);
            }
            return Ok(prefeitosHateoas.ToArray());
        }        

        /// <summary>
        /// Método para pegar um prefeito por id. Disponível para todos, os dados sensíveis só são exibidos para admin.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetPrefeitosPorId(int id)
        {
            try
            {
                var adminLogado = HttpContext.User.IsInRole("Admin");
                Prefeito prefeitoBd = new Prefeito();

                if(adminLogado)
                {
                    prefeitoBd = _database.Prefeitos.Include(prefeito => prefeito.Partido).Include(prefeito => prefeito.DadosSensiveis).First(prefeito => prefeito.Id == id);
                }
                else
                {
                    prefeitoBd = _database.Prefeitos.Include(prefeito => prefeito.Partido).First(prefeito => prefeito.Id == id);                    
                }

                PrefeitoContainer prefeitoComLinks = new PrefeitoContainer();
                prefeitoComLinks.Prefeito = prefeitoBd;
                prefeitoComLinks.Links = adminLogado ? _hateoas.GetActionsById(id.ToString()) : _hateoas.GetOpenActions(id.ToString());

                return Ok(prefeitoComLinks);
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Prefeito não encontrado");
            }
        }

        /// <summary>
        /// Método para criar novo prefeito. Disponível apenas para admin.
        /// </summary>
        [HttpPost]
        public IActionResult NovoPrefeito(IFormFile imagem, [FromForm] PoliticoDto modeloPolitico, [FromForm] PrefeitoDto modeloPrefeito)
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
                    var partidoBd = _database.Partidos.First(p => p.NumeroPartido == modeloPrefeito.NumeroPartido);
                    DadoSensivel novoDadoSensivel = PoliticoHelper.NovoDadosSensiveis(modeloPolitico);

                    _database.DadosSensiveis.Add(novoDadoSensivel);

                    var nomeFoto = _fileHelper.UploadedFile(imagem, modeloPolitico.Nome);

                    Prefeito novoPrefeito = new Prefeito();
                    novoPrefeito = (Prefeito)PoliticoHelper.NovoPolitico(novoDadoSensivel, nomeFoto, partidoBd, modeloPolitico.Nome, modeloPolitico.ProjetosDeLei, "Prefeito");

                    novoPrefeito.Estado = modeloPrefeito.Estado;     
                    novoPrefeito.Cidade = modeloPrefeito.Cidade;               

                    _database.Prefeitos.Add(novoPrefeito);
                    _database.SaveChanges();

                    PrefeitoContainer prefeitoComLinks = new PrefeitoContainer();
                    prefeitoComLinks.Prefeito = novoPrefeito;
                    prefeitoComLinks.Links = _hateoas.GetActionsById(novoPrefeito.Id.ToString());

                    Response.StatusCode = 201;
                    return new ObjectResult(prefeitoComLinks);    
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
        /// Método para editar prefeito existente. Disponível apenas para admin.
        /// </summary>
        [HttpPatch("{id}")]
        public IActionResult EditarPrefeito(int id, IFormFile imagem, [FromForm] PoliticoPatchDto modeloPolitico, [FromForm] PrefeitoPatchDto modeloPrefeito)
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
                var prefeitoBd = _database.Prefeitos.Include(p => p.Partido).Include(p => p.DadosSensiveis).First(p => p.Id == id);
                PoliticoHelper.EditarDadosSensiveis(modeloPolitico, prefeitoBd.DadosSensiveis.Id, _database);

                var partidoBd = _database.Partidos.FirstOrDefault(p => p.NumeroPartido == modeloPrefeito.NumeroPartido);

                if(partidoBd == null && modeloPrefeito.NumeroPartido != 0 && modeloPrefeito.NumeroPartido != null)
                {
                    Response.StatusCode = 404;
                    return new ObjectResult("Não há esse partido cadastrado");
                }

                prefeitoBd.Nome = modeloPolitico.Nome != null ? modeloPolitico.Nome : prefeitoBd.Nome;
                prefeitoBd.ProjetosDeLei = modeloPolitico.ProjetosDeLei != null ? (int)modeloPolitico.ProjetosDeLei : prefeitoBd.ProjetosDeLei;
                prefeitoBd.Foto = imagem == null ? prefeitoBd.Foto : _fileHelper.UploadedFile(imagem, modeloPolitico.Nome);
                prefeitoBd.Cidade = modeloPrefeito.Cidade != null ? modeloPrefeito.Cidade : prefeitoBd.Cidade;
                prefeitoBd.Estado = modeloPrefeito.Estado != null ? modeloPrefeito.Estado : prefeitoBd.Estado;

                if(partidoBd != null)
                {
                    prefeitoBd.PartidoId = partidoBd.Id;
                    prefeitoBd.Partido = partidoBd;
                }
            
                _database.SaveChanges();

                PrefeitoContainer prefeitoComLinks = new PrefeitoContainer();
                prefeitoComLinks.Prefeito = prefeitoBd;
                prefeitoComLinks.Links = _hateoas.GetActionsById(id.ToString());
               
                return Ok(prefeitoComLinks);
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Prefeito não encontrado");
            }
        }

        /// <summary>
        /// Método para excluir prefeito existente. Disponível apenas para admin.
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult DeletarPrefeito(int id)
        {
            try
            {
                var prefeitoBd = _database.Prefeitos.First(v => v.Id == id);
                var dadosBd = _database.DadosSensiveis.First(d => d.Id == prefeitoBd.DadosSensiveisId);
                _database.DadosSensiveis.Remove(dadosBd);
                _database.Prefeitos.Remove(prefeitoBd);
                _database.SaveChanges();

                PrefeitoContainer prefeitoComLinks = new PrefeitoContainer();
                prefeitoComLinks.Prefeito = prefeitoBd;
                prefeitoComLinks.Links = _hateoas.GetGeneralActions();
         
                return Ok(new {msg = "Prefeito excluído", prefeito = prefeitoComLinks});         
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Prefeito não encontrado");
            }
        }
                    
    }
}