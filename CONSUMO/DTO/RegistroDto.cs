using System.ComponentModel.DataAnnotations;

namespace CONSUMO.DTO
{
    public class RegistroDto
    {
        /// <summary>
        /// Senha do novo usuário.
        /// </summary>        
        [Required]
        [StringLength(50, ErrorMessage = "Deve ter entre {2} e {1} caracteres", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Senha { get; set; }
        /// <summary>
        /// Confirmação de senha do novo usuário.
        /// </summary> 
        [Required]
        [StringLength(50, ErrorMessage = "Deve ter entre {2} e {1} caracteres", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Compare("Senha")]
        public string ConfirmacaoSenha { get; set; }
        /// <summary>
        /// E-mail do novo usuário.
        /// </summary>         
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }        
    }
}