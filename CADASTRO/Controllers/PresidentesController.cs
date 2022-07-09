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
    /// Controlador responsável pelo CRUD de Presidentes, acessável, por padrão, por admin.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class PresidentesController : ControllerBase
    {
        private readonly ApplicationDbContext _database;
        private readonly LinkHelper _hateoas;
        private readonly FileHelper _fileHelper;
        public PresidentesController(ApplicationDbContext database, IWebHostEnvironment hostEnvironment)
        {
            _database = database;
            _fileHelper = new FileHelper(hostEnvironment);
            _hateoas = new LinkHelper("localhost:5001/api/Presidentes");
            _hateoas.AddActionById("EDITAR_PRESIDENTE", "PATCH");
            _hateoas.AddActionById("INFO_PRESIDENTE", "GET");
            _hateoas.AddActionById("DELETAR_PRESIDENTE", "DELETE");
            _hateoas.AddGeneralActions("TODOS_PRESIDENTES", "GET");
            _hateoas.AddGeneralActions("NOVO_PRESIDENTE", "POST");
        }

        /// <summary>
        /// Método para pegar a lista de todos os presidentes cadastrados. Disponível para todos, os dados sensíveis só são exibidos para admin.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetPresidentes()
        {
            var adminLogado = HttpContext.User.IsInRole("Admin");
            List<Presidente> presidentesBd = new List<Presidente>();

            if(adminLogado)
            {
                presidentesBd = _database.Presidentes.Include(presidente => presidente.Partido).Include(presidente => presidente.DadosSensiveis).ToList();
            }

            else
            {
                presidentesBd = _database.Presidentes.Include(presidente => presidente.Partido).ToList();
            }

            List<PresidenteContainer> presidentesHateoas = new List<PresidenteContainer>();
            
            foreach (var presidente in presidentesBd)
            {
                PresidenteContainer presidenteComLinks = new PresidenteContainer();
                presidenteComLinks.Presidente = presidente;
                presidenteComLinks.Links = adminLogado ? _hateoas.GetActionsById(presidente.Id.ToString()) : _hateoas.GetOpenActions(presidente.Id.ToString());
                presidentesHateoas.Add(presidenteComLinks);
            }

            return Ok(presidentesHateoas.ToArray());
        }

        /// <summary>
        /// Método para pegar um presidente por id. Disponível para todos, os dados sensíveis só são exibidos para admin.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetPresidentePorId(int id)
        {
            try
            {
                var adminLogado = HttpContext.User.IsInRole("Admin");
                Presidente presidenteBd = new Presidente();

                if(adminLogado)
                {
                    presidenteBd = _database.Presidentes.Include(presidente => presidente.Partido).Include(presidente => presidente.DadosSensiveis).First(presidente => presidente.Id == id);
                }
                else
                {
                    presidenteBd = _database.Presidentes.Include(presidente => presidente.Partido).First(presidente => presidente.Id == id);
                }

                PresidenteContainer presidenteComLinks = new PresidenteContainer();
                presidenteComLinks.Presidente = presidenteBd;
                presidenteComLinks.Links = adminLogado ? _hateoas.GetActionsById(id.ToString()) : _hateoas.GetOpenActions(id.ToString());

                return Ok(presidenteComLinks);
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Presidente não encontrado");
            }
        }

        /// <summary>
        /// Método para criar novo presidente. Disponível apenas para admin.
        /// </summary>
        [HttpPost]
        public IActionResult NovoPresidente(IFormFile imagem, [FromForm] PoliticoDto modeloPolitico, [FromForm] PresidenteDto modeloPresidente)
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
                    var partidoBd = _database.Partidos.First(p => p.NumeroPartido == modeloPresidente.NumeroPartido);
                    DadoSensivel novoDadoSensivel = PoliticoHelper.NovoDadosSensiveis(modeloPolitico);

                    _database.DadosSensiveis.Add(novoDadoSensivel);

                    var nomeFoto = _fileHelper.UploadedFile(imagem, modeloPolitico.Nome);

                    Presidente novoPresidente = new Presidente();
                    novoPresidente = (Presidente)PoliticoHelper.NovoPolitico(novoDadoSensivel, nomeFoto, partidoBd, modeloPolitico.Nome, modeloPolitico.ProjetosDeLei, "Presidente");

                    _database.Presidentes.Add(novoPresidente);
                    _database.SaveChanges();

                    PresidenteContainer presidenteComLinks = new PresidenteContainer();
                    presidenteComLinks.Presidente = novoPresidente;
                    presidenteComLinks.Links = _hateoas.GetActionsById(novoPresidente.Id.ToString());
                    Response.StatusCode = 201;
                    return new ObjectResult(presidenteComLinks);    
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
        /// Método para editar presidente existente. Disponível apenas para admin.
        /// </summary>
       [HttpPatch("{id}")]
        public IActionResult EditarPresidente(int id, IFormFile imagem, [FromForm] PoliticoPatchDto modeloPolitico, [FromForm] PresidentePatchDto modeloPresidente)
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
                var presidenteBd = _database.Presidentes.Include(p => p.Partido).Include(p => p.DadosSensiveis).First(p => p.Id == id);
                PoliticoHelper.EditarDadosSensiveis(modeloPolitico, presidenteBd.DadosSensiveis.Id, _database);

                var partidoBd = _database.Partidos.FirstOrDefault(p => p.NumeroPartido == modeloPresidente.NumeroPartido);

                if(partidoBd == null && modeloPresidente.NumeroPartido != 0 && modeloPresidente.NumeroPartido != null)
                {
                    Response.StatusCode = 404;
                    return new ObjectResult("Não há esse partido cadastrado");
                }

                presidenteBd.Nome = modeloPolitico.Nome != null ? modeloPolitico.Nome : presidenteBd.Nome;
                presidenteBd.ProjetosDeLei = modeloPolitico.ProjetosDeLei != null ? (int)modeloPolitico.ProjetosDeLei : presidenteBd.ProjetosDeLei;
                presidenteBd.Foto = imagem == null ? presidenteBd.Foto : _fileHelper.UploadedFile(imagem, modeloPolitico.Nome);

                if(partidoBd != null)
                {
                    presidenteBd.PartidoId = partidoBd.Id;
                    presidenteBd.Partido = partidoBd;
                }
            
                _database.SaveChanges();

                PresidenteContainer presidenteComLinks = new PresidenteContainer();
                presidenteComLinks.Presidente = presidenteBd;
                presidenteComLinks.Links = _hateoas.GetActionsById(id.ToString());
               
                return Ok(presidenteComLinks);
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Presidente não encontrado");
            }
        } 

        /// <summary>
        /// Método para excluir presidente existente. Disponível apenas para admin.
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult DeletarPresidente(int id)
        {
            try
            {
                var presidenteBd = _database.Presidentes.First(s => s.Id == id);
                var dadosBd = _database.DadosSensiveis.First(d => d.Id == presidenteBd.DadosSensiveisId);
                _database.DadosSensiveis.Remove(dadosBd);
                _database.Presidentes.Remove(presidenteBd);                
                _database.SaveChanges();

                PresidenteContainer presidenteComLinks = new PresidenteContainer();
                presidenteComLinks.Presidente = presidenteBd;
                presidenteComLinks.Links = _hateoas.GetGeneralActions();
         
                return Ok(new {msg = "Presidente excluído", presidente = presidenteComLinks});         
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Presidente não encontrado");
            }
        } 
    }
}