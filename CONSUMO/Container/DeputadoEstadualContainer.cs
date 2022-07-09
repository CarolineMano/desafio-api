using CONSUMO.HATEOAS;
using CONSUMO.Models;

namespace CONSUMO.Container
{
    public class DeputadoEstadualContainer
    {
        public DeputadoEstadual DeputadoEstadual { get; set; }
        public Link[] Links { get; set; }
    }
}