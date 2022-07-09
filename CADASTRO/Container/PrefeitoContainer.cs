using CADASTRO.HATEOAS;
using CADASTRO.Models;

namespace CADASTRO.Container
{
    public class PrefeitoContainer
    {
        public Prefeito Prefeito { get; set; }
        public Link[] Links { get; set; }
    }
}