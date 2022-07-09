using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    /// Controlador responsável pelo CRUD de Ministros de Estado, acessável, por padrão, por admin.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class MinistrosController : ControllerBase
    {
        private readonly ApplicationDbContext _database;
        private readonly LinkHelper _hateoas;
        private readonly FileHelper _fileHelper;      

        public MinistrosController(ApplicationDbContext database, IWebHostEnvironment hostEnvironment)
        {
            _database = database;
            _fileHelper = new FileHelper(hostEnvironment);
            _hateoas = new LinkHelper("localhost:5001/api/Ministros");
            _hateoas.AddActionById("EDITAR_MINISTRO", "PATCH");
            _hateoas.AddActionById("INFO_MINISTRO", "GET");
            _hateoas.AddActionById("DELETAR_MINISTRO", "DELETE");
            _hateoas.AddGeneralActions("TODOS_MINISTROS", "GET");
            _hateoas.AddGeneralActions("NOVO_MINISTRO", "POST");
        }

        /// <summary>
        /// Método para pegar a lista de todos os ministros de estado cadastrados. Disponível para todos, os dados sensíveis só são exibidos para admin.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetMinistros()
        {
            var adminLogado = HttpContext.User.IsInRole("Admin");
            List<MinistroDeEstado> ministrosBd = new List<MinistroDeEstado>();

            if(adminLogado)
            {
                ministrosBd = _database.MinistrosDeEstado.Include(ministro => ministro.Partido).Include(ministro => ministro.DadosSensiveis).ToList();
            }

            else
            {
                ministrosBd = _database.MinistrosDeEstado.Include(ministro => ministro.Partido).ToList();
            }

            List<MinistroContainer> ministroHateoas = new List<MinistroContainer>();
            
            foreach (var ministro in ministrosBd)
            {
                MinistroContainer ministroComLinks = new MinistroContainer();
                ministroComLinks.Ministro = ministro;
                ministroComLinks.Links = adminLogado ? _hateoas.GetActionsById(ministro.Id.ToString()) : _hateoas.GetOpenActions(ministro.Id.ToString());
                ministroHateoas.Add(ministroComLinks);
            }

            return Ok(ministroHateoas.ToArray());
        }

        /// <summary>
        /// Método para pegar um ministro de estado por id. Disponível para todos, os dados sensíveis só são exibidos para admin.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetMinistroPorId(int id)
        {
            try
            {
                var adminLogado = HttpContext.User.IsInRole("Admin");
                MinistroDeEstado ministroBd = new MinistroDeEstado();

                if(adminLogado)
                {
                    ministroBd = _database.MinistrosDeEstado.Include(ministro => ministro.Partido).Include(ministro => ministro.DadosSensiveis).First(ministro => ministro.Id == id);
                }
                else
                {
                    ministroBd = _database.MinistrosDeEstado.Include(ministro => ministro.Partido).First(ministro => ministro.Id == id);                    
                }               

                MinistroContainer ministroComLinks = new MinistroContainer();
                ministroComLinks.Ministro = ministroBd;
                ministroComLinks.Links = adminLogado ? _hateoas.GetActionsById(id.ToString()) : _hateoas.GetOpenActions(id.ToString());

                return Ok(ministroComLinks);
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Ministro de Estado não encontrado");
            }
        }

        /// <summary>
        /// Método para criar novo ministro de estado. Disponível apenas para admin.
        /// </summary>
       [HttpPost]
        public IActionResult NovoMinistro(IFormFile imagem, [FromForm] PoliticoDto modeloPolitico, [FromForm] MinistroDto modeloMinistro)
        {
            if(ModelState.IsValid)
            {
                if(!ValidaCpf.ValidaCpfExtension.Validate(modeloPolitico.Cpf))
                {
                    Response.StatusCode = 422;
                    return new ObjectResult("CPF inválido");
                }    

                var partidoBd = _database.Partidos.FirstOrDefault(p => p.NumeroPartido == modeloMinistro.NumeroPartido);
                DadoSensivel novoDadoSensivel = PoliticoHelper.NovoDadosSensiveis(modeloPolitico);

                if(partidoBd == null && modeloMinistro.NumeroPartido != null)
                {
                    Response.StatusCode = 404;
                    return new ObjectResult("Partido não cadastrado");
                }

                _database.DadosSensiveis.Add(novoDadoSensivel);

                var nomeFoto = _fileHelper.UploadedFile(imagem, modeloPolitico.Nome);

                MinistroDeEstado novoMinistro = new MinistroDeEstado();
                novoMinistro = (MinistroDeEstado)PoliticoHelper.NovoPolitico(novoDadoSensivel, nomeFoto, partidoBd, modeloPolitico.Nome, modeloPolitico.ProjetosDeLei, "Ministro de Estado");

                novoMinistro.Pasta = modeloMinistro.Pasta;

                _database.MinistrosDeEstado.Add(novoMinistro);
                _database.SaveChanges();

                MinistroContainer ministroComLinks = new MinistroContainer();
                ministroComLinks.Ministro = novoMinistro;
                ministroComLinks.Links = _hateoas.GetActionsById(novoMinistro.Id.ToString());
                Response.StatusCode = 201;
                return new ObjectResult(ministroComLinks);    
            }
            return BadRequest();            
        }

        /// <summary>
        /// Método para editar ministro de estado existente. Disponível apenas para admin.
        /// </summary>
        [HttpPatch("{id}")]
        public IActionResult EditarMinistro(int id, IFormFile imagem, [FromForm] PoliticoPatchDto modeloPolitico, [FromForm] MinistroPatchDto modeloMinistro)
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
                var ministroBd = _database.MinistrosDeEstado.Include(m => m.Partido).Include(m => m.DadosSensiveis).First(m => m.Id == id);
                PoliticoHelper.EditarDadosSensiveis(modeloPolitico, ministroBd.DadosSensiveis.Id, _database);

                var partidoBd = _database.Partidos.FirstOrDefault(p => p.NumeroPartido == modeloMinistro.NumeroPartido);

                if(partidoBd == null && modeloMinistro.NumeroPartido != 0 && modeloMinistro.NumeroPartido != null)
                {
                    Response.StatusCode = 404;
                    return new ObjectResult("Não há esse partido cadastrado");
                }

                ministroBd.Nome = modeloPolitico.Nome != null ? modeloPolitico.Nome : ministroBd.Nome;
                ministroBd.ProjetosDeLei = modeloPolitico.ProjetosDeLei != null ? (int)modeloPolitico.ProjetosDeLei : ministroBd.ProjetosDeLei;
                ministroBd.Foto = imagem == null ? ministroBd.Foto : _fileHelper.UploadedFile(imagem, modeloPolitico.Nome);
                ministroBd.Pasta = modeloMinistro.Pasta != null ? modeloMinistro.Pasta : ministroBd.Pasta;

                if(partidoBd != null)
                {
                    ministroBd.PartidoId = partidoBd.Id;
                    ministroBd.Partido = partidoBd;
                }
            
                _database.SaveChanges();

                MinistroContainer ministroComLinks = new MinistroContainer();
                ministroComLinks.Ministro = ministroBd;
                ministroComLinks.Links = _hateoas.GetActionsById(id.ToString());
               
                return Ok(ministroComLinks);
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Ministro não encontrado");
            }
        }

        /// <summary>
        /// Método para excluir ministro de estado existente. Disponível apenas para admin.
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult DeletarMinistro(int id)
        {
            try
            {
                var ministroBd = _database.MinistrosDeEstado.First(s => s.Id == id);
                var dadosBd = _database.DadosSensiveis.First(d => d.Id == ministroBd.DadosSensiveisId);
                _database.DadosSensiveis.Remove(dadosBd);                
                _database.MinistrosDeEstado.Remove(ministroBd);
                _database.SaveChanges();

                MinistroContainer ministroComLinks = new MinistroContainer();
                ministroComLinks.Ministro = ministroBd;
                ministroComLinks.Links = _hateoas.GetGeneralActions();
         
                return Ok(new {msg = "Ministro de Estado excluído", ministro = ministroComLinks});         
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Ministro de Estado não encontrado");
            }
        }

    }
}