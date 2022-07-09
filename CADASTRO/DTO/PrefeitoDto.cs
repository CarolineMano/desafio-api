using System.ComponentModel.DataAnnotations;

namespace CADASTRO.DTO
{
    public class PrefeitoDto
    {
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "O número do partido só pode conter números")]
        public int NumeroPartido { get; set; }
        [Required]
        public string Estado { get; set; }
        [Required]
        public string Cidade { get; set; }  
    }
}