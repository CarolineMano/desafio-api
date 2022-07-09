using CADASTRO.Models;
using CADASTRO.HATEOAS;

namespace CADASTRO.Container
{
    public class VereadorContainer
    {
        public Vereador Vereador { get; set; }
        public Link[] Links { get; set; }
    }
}