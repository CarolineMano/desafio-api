using System.Text.Json.Serialization;

namespace CADASTRO.Models
{
    public class Partido
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Sigla { get; set; }
        public int NumeroPartido { get; set; }
    }
}