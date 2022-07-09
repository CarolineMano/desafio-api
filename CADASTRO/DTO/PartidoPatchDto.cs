using System.ComponentModel.DataAnnotations;

namespace CADASTRO.DTO
{
    public class PartidoPatchDto
    {
        [StringLength(100, ErrorMessage = "O nome deve conter entre {2} e {1} caracteres", MinimumLength = 2)]
        public string NomePartido { get; set; }
        public string Sigla { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessage = "O número do partido só pode conter números")]
        public int? NumeroPartido { get; set; }    
        public string NomeLider { get; set; }             
    }
}