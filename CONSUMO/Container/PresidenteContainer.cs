using CONSUMO.HATEOAS;
using CONSUMO.Models;

namespace CONSUMO.Container
{
    public class PresidenteContainer
    {
        public Presidente Presidente { get; set; }
        public Link[] Links { get; set; }
    }
}