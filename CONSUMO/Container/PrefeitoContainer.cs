using CONSUMO.HATEOAS;
using CONSUMO.Models;

namespace CONSUMO.Container
{
    public class PrefeitoContainer
    {
        public Prefeito Prefeito { get; set; }
        public Link[] Links { get; set; }
    }
}