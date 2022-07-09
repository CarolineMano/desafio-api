using CONSUMO.HATEOAS;
using CONSUMO.Models;

namespace CONSUMO.Container
{
    public class DeputadoFederalContainer
    {
        public DeputadoFederal DeputadoFederal { get; set; }
        public Link[] Links { get; set; }        
    }
}