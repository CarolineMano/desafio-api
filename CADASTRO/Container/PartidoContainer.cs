using CADASTRO.HATEOAS;
using CADASTRO.Models;
using Microsoft.AspNetCore.Mvc;

namespace CADASTRO.Container
{
    public class PartidoContainer
    {
        public Partido Partido { get; set; }
        public LiderPartido LiderPartido { get; set; }
        public Link[] Links { get; set; }
    }
}