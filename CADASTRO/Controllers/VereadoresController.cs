using System;
using System.Collections.Generic;
using System.Linq;
using CADASTRO.Container;
using CADASTRO.Data;
using CADASTRO.DTO;
using CADASTRO.Helpers;
using CADASTRO.Models;
using CADASTRO.HATEOAS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CADASTRO.Controllers
{
    /// <summary>
    /// Controlador responsável pelo CRUD de Vereadores, acessável, por padrão, por admin.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class VereadoresController : ControllerBase
    {
        private readonly ApplicationDbContext _database;
        private readonly LinkHelper _hateoas;
        private readonly FileHelper _fileHelper;
        public VereadoresController(ApplicationDbContext database, IWebHostEnvironment hostEnvironment)
        {
            _database = database;
            _fileHelper = new FileHelper(hostEnvironment);
            _hateoas = new LinkHelper("localhost:5001/api/Vereadores");
            _hateoas.AddActionById("EDITAR_VEREADOR", "PATCH");
            _hateoas.AddActionById("INFO_VEREADOR", "GET");
            _hateoas.AddActionById("DELETAR_VEREADOR", "DELETE");
            _hateoas.AddGeneralActions("TODOS_VEREADORES", "GET");
            _hateoas.AddGeneralActions("NOVO_VEREADOR", "POST");
        }

        /// <summary>
        /// Método para pegar a lista de todos os vereadores cadastrados. Disponível para todos, os dados sensíveis só são exibidos para admin.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetVereadores()
        {
            var adminLogado = HttpContext.User.IsInRole("Admin");
            List<Vereador> vereadoresBd = new List<Vereador>();

            if(adminLogado)
            {
                vereadoresBd = _database.Vereadores.Include(vereador => vereador.Partido).Include(vereador => vereador.DadosSensiveis).ToList();
            }

            else
            {
                vereadoresBd = _database.Vereadores.Include(vereador => vereador.Partido).ToList();
            }

            List<VereadorContainer> vereadoresHateoas = new List<VereadorContainer>();
  
            foreach (var vereador in vereadoresBd)
            {
                VereadorContainer vereadorComLinks = new VereadorContainer();
                vereadorComLinks.Vereador = vereador;
                vereadorComLinks.Links = adminLogado ? _hateoas.GetActionsById(vereador.Id.ToString()) : _hateoas.GetOpenActions(vereador.Id.ToString());
                vereadoresHateoas.Add(vereadorComLinks);
            }

            return Ok(vereadoresHateoas.ToArray());
        }

        /// <summary>
        /// Método para pegar um vereador por id. Disponível para todos, os dados sensíveis só são exibidos para admin.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetVereadoresPorId(int id)
        {
            try
            {
                var adminLogado = HttpContext.User.IsInRole("Admin");
                Vereador vereadorBd = new Vereador();

                if(adminLogado)
                {
                    vereadorBd = _database.Vereadores.Include(vereador => vereador.Partido).Include(vereador => vereador.DadosSensiveis).First(vereador => vereador.Id == id);
                }
                else
                {
                    vereadorBd = _database.Vereadores.Include(vereador => vereador.Partido).First(vereador => vereador.Id == id);                    
                }        

                VereadorContainer vereadorComLinks = new VereadorContainer();
                vereadorComLinks.Vereador = vereadorBd;
                vereadorComLinks.Links = adminLogado ? _hateoas.GetActionsById(id.ToString()) : _hateoas.GetOpenActions(id.ToString());

                return Ok(vereadorComLinks);
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Vereador não encontrado");
            }
        }

        /// <summary>
        /// Método para criar novo vereador. Disponível apenas para admin.
        /// </summary>
        [HttpPost]
        public IActionResult NovoVereador(IFormFile imagem, [FromForm] PoliticoDto modeloPolitico, [FromForm] VereadorDto modeloVereador)
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
                    var partidoBd = _database.Partidos.First(p => p.NumeroPartido == modeloVereador.NumeroPartido);
                    DadoSensivel novoDadoSensivel = PoliticoHelper.NovoDadosSensiveis(modeloPolitico);

                    _database.DadosSensiveis.Add(novoDadoSensivel);

                    var nomeFoto = _fileHelper.UploadedFile(imagem, modeloPolitico.Nome);

                    Vereador novoVereador = new Vereador();
                    novoVereador = (Vereador)PoliticoHelper.NovoPolitico(novoDadoSensivel, nomeFoto, partidoBd, modeloPolitico.Nome, modeloPolitico.ProjetosDeLei, "Vereador");

                    novoVereador.Processo = modeloVereador.Processado;
                    novoVereador.Estado = modeloVereador.Estado;     
                    novoVereador.Cidade = modeloVereador.Cidade;               

                    _database.Vereadores.Add(novoVereador);
                    _database.SaveChanges();

                    VereadorContainer vereadorComLinks = new VereadorContainer();
                    vereadorComLinks.Vereador = novoVereador;
                    vereadorComLinks.Links = _hateoas.GetActionsById(novoVereador.Id.ToString());

                    Response.StatusCode = 201;
                    return new ObjectResult(vereadorComLinks);    
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
        /// Método para editar vereador existente. Disponível apenas para admin.
        /// </summary>
        [HttpPatch("{id}")]
        public IActionResult EditarVereador(int id, IFormFile imagem, [FromForm] PoliticoPatchDto modeloPolitico, [FromForm] VereadorPatchDto modeloVereador)
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
                var vereadorBd = _database.Vereadores.Include(v => v.Partido).Include(v => v.DadosSensiveis).First(v => v.Id == id);
                PoliticoHelper.EditarDadosSensiveis(modeloPolitico, vereadorBd.DadosSensiveis.Id, _database);

                var partidoBd = _database.Partidos.FirstOrDefault(p => p.NumeroPartido == modeloVereador.NumeroPartido);

                if(partidoBd == null && modeloVereador.NumeroPartido != 0 && modeloVereador.NumeroPartido != null)
                {
                    Response.StatusCode = 404;
                    return new ObjectResult("Não há esse partido cadastrado");
                }

                vereadorBd.Nome = modeloPolitico.Nome != null ? modeloPolitico.Nome : vereadorBd.Nome;
                vereadorBd.ProjetosDeLei = modeloPolitico.ProjetosDeLei != null ? (int)modeloPolitico.ProjetosDeLei : vereadorBd.ProjetosDeLei;
                vereadorBd.Foto = imagem == null ? vereadorBd.Foto : _fileHelper.UploadedFile(imagem, modeloPolitico.Nome);
                vereadorBd.Processo = modeloVereador.Processado != null ? (bool)modeloVereador.Processado : vereadorBd.Processo;
                vereadorBd.Cidade = modeloVereador.Cidade != null ? modeloVereador.Cidade : vereadorBd.Cidade;
                vereadorBd.Estado = modeloVereador.Estado != null ? modeloVereador.Estado : vereadorBd.Estado;

                if(partidoBd != null)
                {
                    vereadorBd.PartidoId = partidoBd.Id;
                    vereadorBd.Partido = partidoBd;
                }
            
                _database.SaveChanges();

                VereadorContainer vereadorComLinks = new VereadorContainer();
                vereadorComLinks.Vereador = vereadorBd;
                vereadorComLinks.Links = _hateoas.GetActionsById(id.ToString());
               
                return Ok(vereadorComLinks);
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Vereador não encontrado");
            }
        }

        /// <summary>
        /// Método para excluir vereador existente. Disponível apenas para admin.
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult DeletarVereador(int id)
        {
            try
            {
                var vereadorBd = _database.Vereadores.First(v => v.Id == id);
                var dadosBd = _database.DadosSensiveis.First(d => d.Id == vereadorBd.DadosSensiveisId);
                _database.DadosSensiveis.Remove(dadosBd);
                _database.Vereadores.Remove(vereadorBd);
                _database.SaveChanges();

                VereadorContainer vereadorComLinks = new VereadorContainer();
                vereadorComLinks.Vereador = vereadorBd;
                vereadorComLinks.Links = _hateoas.GetGeneralActions();
         
                return Ok(new {msg = "Vereador excluído", vereador = vereadorComLinks});         
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Vereador não encontrado");
            }
        }
    }
}