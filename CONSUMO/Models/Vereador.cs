namespace CONSUMO.Models
{
    public class Vereador : Politico
    {
        public string Estado { get; set; }
        public string Cidade { get; set; }
        public bool Processo { get; set; }
    }
}