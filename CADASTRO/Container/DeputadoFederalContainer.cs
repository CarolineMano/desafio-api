using CADASTRO.HATEOAS;
using CADASTRO.Models;

namespace CADASTRO.Container
{
    public class DeputadoFederalContainer
    {
        public DeputadoFederal DeputadoFederal { get; set; }
        public Link[] Links { get; set; }
    }
}