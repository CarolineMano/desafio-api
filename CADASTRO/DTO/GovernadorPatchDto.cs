using System.ComponentModel.DataAnnotations;

namespace CADASTRO.DTO
{
    public class GovernadorPatchDto
    {
        [RegularExpression("^[0-9]*$", ErrorMessage = "O número do partido só pode conter números")]
        public int? NumeroPartido { get; set; }
        public string Estado { get; set; }
        public bool? Processado { get; set; }   
    }
}