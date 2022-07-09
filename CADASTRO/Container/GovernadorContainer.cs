using CADASTRO.HATEOAS;
using CADASTRO.Models;

namespace CADASTRO.Container
{
    public class GovernadorContainer
    {
        public Governador Governador { get; set; }
        public Link[] Links { get; set; }        
    }
}