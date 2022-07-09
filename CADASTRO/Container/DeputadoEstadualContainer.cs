using CADASTRO.HATEOAS;
using CADASTRO.Models;

namespace CADASTRO.Container
{
    public class DeputadoEstadualContainer
    {
        public DeputadoEstadual DeputadoEstadual { get; set; }
        public Link[] Links { get; set; }
    }
}