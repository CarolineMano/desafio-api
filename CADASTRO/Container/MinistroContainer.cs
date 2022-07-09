using CADASTRO.HATEOAS;
using CADASTRO.Models;

namespace CADASTRO.Container
{
    public class MinistroContainer
    {
        public MinistroDeEstado Ministro { get; set; }
        public Link[] Links { get; set; }
    }
}