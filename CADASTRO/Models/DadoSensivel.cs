using System.Text.Json.Serialization;

namespace CADASTRO.Models
{
    public class DadoSensivel
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Cpf { get; set; }
        public string Endereco { get; set; }
        public string Telefone { get; set; }        
    }
}