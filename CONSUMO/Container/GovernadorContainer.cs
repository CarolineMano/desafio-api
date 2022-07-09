using CONSUMO.HATEOAS;
using CONSUMO.Models;

namespace CONSUMO.Container
{
    public class GovernadorContainer
    {
        public Governador Governador { get; set; }
        public Link[] Links { get; set; }
    }
}