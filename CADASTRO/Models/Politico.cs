using System.Text.Json.Serialization;


namespace CADASTRO.Models
{
    public class Politico
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        [JsonIgnore]
        public int DadosSensiveisId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DadoSensivel DadosSensiveis { get; set; }
        [JsonIgnore]
        public int? PartidoId { get; set; }
        public Partido Partido { get; set; }
        public string Foto { get; set; }
        public int ProjetosDeLei { get; set; }

    }
}