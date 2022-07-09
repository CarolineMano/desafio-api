using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CADASTRO.DTO
{
    public class PoliticoDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "O nome deve conter entre {2} e {1} caracteres", MinimumLength = 2)]
        public string Nome { get; set; }
        [Required]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve conter {1} dígitos")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "O CPF só pode conter números")]
        public string Cpf { get; set; }
        [Required]
        public string Endereco { get; set; }
        [Required]
        [StringLength(11, MinimumLength = 10, ErrorMessage = "O telefone deve conter entre {2} e {1} dígitos")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "O telefone só pode conter números")]
        public string Telefone { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Valor mínimo para projetos de lei é {1}")]
        public int ProjetosDeLei { get; set; }
    }
}