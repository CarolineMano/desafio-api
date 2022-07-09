namespace CONSUMO.Models
{
    public class DeputadoFederal : Politico
    {
        public bool Processo { get; set; }
        public string Estado { get; set; }
        public string Lider { get; set; }
    }
}