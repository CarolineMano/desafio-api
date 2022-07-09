using CADASTRO.Models;
using CADASTRO.HATEOAS;

namespace CADASTRO.Container
{
    public class PresidenteContainer
    {
        public Presidente Presidente { get; set; }
        public Link[] Links { get; set; }
    }
}