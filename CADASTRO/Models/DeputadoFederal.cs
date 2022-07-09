using System.ComponentModel.DataAnnotations.Schema;

namespace CADASTRO.Models
{
    public class DeputadoFederal : PoliticoProcessavel
    {
        public string Estado { get; set; }
        [NotMapped]
        public string Lider { get; set; }

    }
}