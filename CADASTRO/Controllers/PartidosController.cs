using System.Collections.Generic;
using System.Linq;
using CADASTRO.Container;
using CADASTRO.Data;
using CADASTRO.DTO;
using CADASTRO.HATEOAS;
using CADASTRO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CADASTRO.Controllers
{
    /// <summary>
    /// Controlador responsável pelo CRUD de Partidos, acessável, por padrão, por admin.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class PartidosController : ControllerBase
    {
        private readonly ApplicationDbContext _database;
        private readonly LinkHelper _hateoas;     
        public PartidosController(ApplicationDbContext database)
        {
            _database = database;
            _hateoas = new LinkHelper("localhost:5001/api/Partidos");
            _hateoas.AddActionById("EDITAR_PARTIDO", "PATCH");
            _hateoas.AddActionById("INFO_PARTIDO", "GET");
            _hateoas.AddActionById("DELETAR_PARTIDO", "DELETE");
            _hateoas.AddGeneralActions("TODOS_PARTIDOS", "GET");
            _hateoas.AddGeneralActions("NOVO_PARTIDO", "POST");
        }   

        /// <summary>
        /// Método para pegar a lista de todos os partidos cadastrados. Disponível apenas para admin.
        /// </summary>
        [HttpGet]
        public IActionResult GetPartidos()
        {
            try
            {
                var partidosBd = _database.Partidos.ToList();
                var lideresBd = _database.Lideres.ToList();

                List<PartidoContainer> partidosHateoas = new List<PartidoContainer>();

                for(int i = 0; i < partidosBd.Count; i++)
                {
                    PartidoContainer partidoComLinks = new PartidoContainer();
                    partidoComLinks.Partido = partidosBd[i];
                    partidoComLinks.LiderPartido = lideresBd[i];
                    partidoComLinks.Links = _hateoas.GetActionsById(partidosBd[i].Id.ToString());
                    partidosHateoas.Add(partidoComLinks);
                }
                return Ok(partidosHateoas.ToArray());

            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new OkObjectResult("Não há partidos registrados");
            }
        }

        /// <summary>
        /// Método para pegar um partido por id. Disponível apenas para admin.
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetPartidoPorId(int id)
        {
            try
            {
                var partidoBd = _database.Partidos.First(partido => partido.Id == id);
                var liderBd = _database.Lideres.First(lider => lider.Id == id);

                PartidoContainer partidoComLinks = new PartidoContainer();
                partidoComLinks.Partido = partidoBd;
                partidoComLinks.LiderPartido = liderBd;
                partidoComLinks.Links = _hateoas.GetActionsById(partidoBd.Id.ToString());

                return Ok(partidoComLinks);
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new OkObjectResult("Partido não encontrado");
            }
        }

        /// <summary>
        /// Método para criar novo partido. Disponível apenas para admin.
        /// </summary>
        [HttpPost]
        public IActionResult NovoPartido([FromForm] PartidoDto modeloPartido)
        {
            if(ModelState.IsValid)
            {
                Partido novoPartido = new Partido();
                novoPartido.Nome = modeloPartido.NomePartido;
                novoPartido.Sigla = modeloPartido.Sigla;
                novoPartido.NumeroPartido = modeloPartido.NumeroPartido;

                _database.Partidos.Add(novoPartido);

                LiderPartido novoLider = new LiderPartido();
                novoLider.Id = novoPartido.Id;
                novoLider.Nome = modeloPartido.NomeLider;

                _database.Lideres.Add(novoLider);

                _database.SaveChanges();

                PartidoContainer partidoComLinks = new PartidoContainer();
                partidoComLinks.Partido = novoPartido;
                partidoComLinks.LiderPartido = novoLider;
                partidoComLinks.Links = _hateoas.GetActionsById(novoPartido.Id.ToString());

                return Ok(partidoComLinks);
            }
            return BadRequest();
        }

        /// <summary>
        /// Método para editar partido existente. Disponível apenas para admin.
        /// </summary>        
        [HttpPatch("{id}")]
        public IActionResult EditarPartido(int id, [FromForm] PartidoPatchDto modeloPartido)
        {
            try
            {
                var partidoBd = _database.Partidos.First(p => p.Id == id);
                var liderBd = _database.Lideres.First(l => l.Id == id);

                partidoBd.Nome = modeloPartido.NomePartido != null ? modeloPartido.NomePartido : partidoBd.Nome;
                partidoBd.Sigla = modeloPartido.Sigla != null ? modeloPartido.Sigla : partidoBd.Sigla;
                partidoBd.NumeroPartido = modeloPartido.NumeroPartido > 0 ? (int)modeloPartido.NumeroPartido : partidoBd.NumeroPartido;

                liderBd.Nome = modeloPartido.NomeLider != null ? modeloPartido.NomeLider : liderBd.Nome;

                _database.SaveChanges();

                PartidoContainer partidoComLinks = new PartidoContainer();
                partidoComLinks.Partido = partidoBd;
                partidoComLinks.LiderPartido = liderBd;
                partidoComLinks.Links = _hateoas.GetActionsById(partidoBd.Id.ToString());

                return Ok(partidoComLinks);
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new OkObjectResult("Partido não encontrado");
            }
        }

        /// <summary>
        /// Método para excluir partido existente. Disponível apenas para admin.
        /// </summary>        
        [HttpDelete("{id}")]
        public IActionResult DeletarPartido(int id)
        {
            var partidoBd = _database.Partidos.FirstOrDefault(p => p.Id == id);
            var liderBd = _database.Lideres.FirstOrDefault(l => l.Id == id);
            if(partidoBd == null)
            {
                Response.StatusCode = 404;
                return new OkObjectResult("Partido não encontrado");
            }

            try
            {
                _database.Partidos.Remove(partidoBd);
                _database.Lideres.Remove(liderBd);
                _database.SaveChanges();

                PartidoContainer partidoComLinks = new PartidoContainer();
                partidoComLinks.Partido = partidoBd;
                partidoComLinks.LiderPartido = liderBd;
                partidoComLinks.Links = _hateoas.GetGeneralActions();

                return Ok(new {msg = "Partido excluído", partido = partidoComLinks});

            }
            catch (System.Exception)
            {
                Response.StatusCode = 423;
                return new ObjectResult("O partido está associado a políticos cadastrados e não poderá ser excluído.");
            }
        }
    }
}