using System.ComponentModel.DataAnnotations;

namespace CONSUMO.DTO
{
    public class LoginDto
    {
        /// <summary>
        /// Username (email) do usuário registrado.
        /// </summary>
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }
        /// <summary>
        /// Senha do usuário registrado.
        /// </summary>
        [Required]
        public string Senha { get; set; }        
    }
}