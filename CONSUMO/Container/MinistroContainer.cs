using CONSUMO.HATEOAS;
using CONSUMO.Models;

namespace CONSUMO.Container
{
    public class MinistroContainer
    {
        public MinistroDeEstado Ministro { get; set; }
        public Link[] Links { get; set; }
    }
}