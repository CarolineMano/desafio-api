using CADASTRO.Models;
using CADASTRO.HATEOAS;

namespace CADASTRO.Container
{
    public class SenadorContainer
    {
        public Senador Senador { get; set; }
        public Link[] Links { get; set; }
    }
}