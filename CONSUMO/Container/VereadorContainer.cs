using CONSUMO.HATEOAS;
using CONSUMO.Models;

namespace CONSUMO.Container
{
    public class VereadorContainer
    {
        public Vereador Vereador { get; set; }
        public Link[] Links { get; set; }
    }
}