using CONSUMO.HATEOAS;
using CONSUMO.Models;

namespace CONSUMO.Container
{
    public class SenadorContainer
    {
        public Senador Senador { get; set; }
        public Link[] Links { get; set; }
    }
}