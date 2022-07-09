namespace CONSUMO.Models
{
    public class Politico
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public Partido Partido { get; set; }
        public string Foto { get; set; }
        public int ProjetosDeLei { get; set; }
    }
}