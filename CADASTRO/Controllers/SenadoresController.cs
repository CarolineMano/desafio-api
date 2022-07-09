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
    /// Controlador responsável pelo CRUD de Senadores, acessável, por padrão, por admin.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class SenadoresController : ControllerBase
    {
        private readonly ApplicationDbContext _database;
        private readonly LinkHelper _hateoas;
        private readonly FileHelper _fileHelper;
        public SenadoresController(ApplicationDbContext database, IWebHostEnvironment hostEnvironment)
        {
            _database = database;
            _fileHelper = new FileHelper(hostEnvironment);
            _hateoas = new LinkHelper("localhost:5001/api/Senadores");
            _hateoas.AddActionById("EDITAR_SENADOR", "PATCH");
            _hateoas.AddActionById("INFO_SENADOR", "GET");
            _hateoas.AddActionById("DELETAR_SENADOR", "DELETE");
            _hateoas.AddGeneralActions("TODOS_SENADORES", "GET");
            _hateoas.AddGeneralActions("NOVO_SENADOR", "POST");
        }

        /// <summary>
        /// Método para pegar a lista de todos os senadores cadastrados. Disponível para todos, os dados sensíveis só são exibidos para admin.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetSenadores()
        {
            var adminLogado = HttpContext.User.IsInRole("Admin");
            List<Senador> senadoresBd = new List<Senador>();

            if(adminLogado)
            {
                senadoresBd = _database.Senadores.Include(senador => senador.Partido).Include(senador => senador.DadosSensiveis).ToList();
            }

            else
            {
                senadoresBd = _database.Senadores.Include(senador => senador.Partido).ToList();
            }

            List<SenadorContainer> senadoresHateoas = new List<SenadorContainer>();
            
            foreach (var senador in senadoresBd)
            {
                SenadorContainer senadorComLinks = new SenadorContainer();
                senadorComLinks.Senador = senador;
                senadorComLinks.Links = adminLogado ? _hateoas.GetActionsById(senador.Id.ToString()) : _hateoas.GetOpenActions(senador.Id.ToString());
                senadoresHateoas.Add(senadorComLinks);
            }

            return Ok(senadoresHateoas.ToArray());
        }

        /// <summary>
        /// Método para pegar um senador por id. Disponível para todos, os dados sensíveis só são exibidos para admin.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetSenadorPorId(int id)
        {
            try
            {
                var adminLogado = HttpContext.User.IsInRole("Admin");
                Senador senadorBd = new Senador();

                if(adminLogado)
                {
                    senadorBd = _database.Senadores.Include(senador => senador.Partido).Include(senador => senador.DadosSensiveis).First(senador => senador.Id == id);
                }
                else
                {
                    senadorBd = _database.Senadores.Include(senador => senador.Partido).First(senador => senador.Id == id);                    
                }               

                SenadorContainer senadorComLinks = new SenadorContainer();
                senadorComLinks.Senador = senadorBd;
                senadorComLinks.Links = adminLogado ? _hateoas.GetActionsById(id.ToString()) : _hateoas.GetOpenActions(id.ToString());

                return Ok(senadorComLinks);
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Senador não encontrado");
            }
        }

        /// <summary>
        /// Método para criar novo senador. Disponível apenas para admin.
        /// </summary>
        [HttpPost]
        public IActionResult NovoSenador(IFormFile imagem, [FromForm] PoliticoDto modeloPolitico, [FromForm] SenadorDto modeloSenador)
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
                    var partidoBd = _database.Partidos.First(p => p.NumeroPartido == modeloSenador.NumeroPartido);
                    DadoSensivel novoDadoSensivel = PoliticoHelper.NovoDadosSensiveis(modeloPolitico);

                    _database.DadosSensiveis.Add(novoDadoSensivel);

                    var nomeFoto = _fileHelper.UploadedFile(imagem, modeloPolitico.Nome);

                    Senador novoSenador = new Senador();
                    novoSenador = (Senador)PoliticoHelper.NovoPolitico(novoDadoSensivel, nomeFoto, partidoBd, modeloPolitico.Nome, modeloPolitico.ProjetosDeLei, "Senador");

                    novoSenador.Estado = modeloSenador.Estado;

                    _database.Senadores.Add(novoSenador);
                    _database.SaveChanges();

                    SenadorContainer senadorComLinks = new SenadorContainer();
                    senadorComLinks.Senador = novoSenador;
                    senadorComLinks.Links = _hateoas.GetActionsById(novoSenador.Id.ToString());
                    Response.StatusCode = 201;
                    return new ObjectResult(senadorComLinks);    
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
        /// Método para editar senador existente. Disponível apenas para admin.
        /// </summary>
        [HttpPatch("{id}")]
        public IActionResult EditarSenador(int id, IFormFile imagem, [FromForm] PoliticoPatchDto modeloPolitico, [FromForm] SenadorPatchDto modeloSenador)
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
                var senadorBd = _database.Senadores.Include(s => s.Partido).Include(s => s.DadosSensiveis).First(s => s.Id == id);
                PoliticoHelper.EditarDadosSensiveis(modeloPolitico, senadorBd.DadosSensiveis.Id, _database);

                var partidoBd = _database.Partidos.FirstOrDefault(p => p.NumeroPartido == modeloSenador.NumeroPartido);

                if(partidoBd == null && modeloSenador.NumeroPartido != 0 && modeloSenador.NumeroPartido != null)
                {
                    Response.StatusCode = 404;
                    return new ObjectResult("Não há esse partido cadastrado");
                }

                senadorBd.Nome = modeloPolitico.Nome != null ? modeloPolitico.Nome : senadorBd.Nome;
                senadorBd.ProjetosDeLei = modeloPolitico.ProjetosDeLei != null ? (int)modeloPolitico.ProjetosDeLei : senadorBd.ProjetosDeLei;
                senadorBd.Foto = imagem == null ? senadorBd.Foto : _fileHelper.UploadedFile(imagem, modeloPolitico.Nome);
                senadorBd.Estado = modeloSenador.Estado != null ? modeloSenador.Estado : senadorBd.Estado;

                if(partidoBd != null)
                {
                    senadorBd.PartidoId = partidoBd.Id;
                    senadorBd.Partido = partidoBd;
                }
            
                _database.SaveChanges();

                SenadorContainer senadorComLinks = new SenadorContainer();
                senadorComLinks.Senador = senadorBd;
                senadorComLinks.Links = _hateoas.GetActionsById(id.ToString());
               
                return Ok(senadorComLinks);
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Senador não encontrado");
            }
        }

        /// <summary>
        /// Método para excluir senador existente. Disponível apenas para admin.
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult DeletarSenador(int id)
        {
            try
            {
                var senadorBd = _database.Senadores.First(s => s.Id == id);
                var dadosBd = _database.DadosSensiveis.First(d => d.Id == senadorBd.DadosSensiveisId);
                _database.DadosSensiveis.Remove(dadosBd);                
                _database.Senadores.Remove(senadorBd);
                _database.SaveChanges();

                SenadorContainer senadorComLinks = new SenadorContainer();
                senadorComLinks.Senador = senadorBd;
                senadorComLinks.Links = _hateoas.GetGeneralActions();
         
                return Ok(new {msg = "Senador excluído", senador = senadorComLinks});         
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Senador não encontrado");
            }
        }
    }
}