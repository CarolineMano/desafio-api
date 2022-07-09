using System.ComponentModel.DataAnnotations;

namespace CADASTRO.DTO
{
    public class MinistroPatchDto
    {
        [RegularExpression("^[0-9]*$", ErrorMessage = "O número do partido só pode conter números")]
        public int? NumeroPartido { get; set; }
        public string Pasta { get; set; }             
    }
}