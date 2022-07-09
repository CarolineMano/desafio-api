using System.Linq;
using CADASTRO.Data;
using CADASTRO.DTO;
using CADASTRO.Models;
using Microsoft.AspNetCore.Http;

namespace CADASTRO.Helpers
{
    public static class PoliticoHelper
    {
        public static DadoSensivel NovoDadosSensiveis(PoliticoDto modeloPolitico)
        {
            DadoSensivel novoDadoSensivel = new DadoSensivel();
            novoDadoSensivel.Cpf = modeloPolitico.Cpf;
            novoDadoSensivel.Endereco = modeloPolitico.Endereco;
            novoDadoSensivel.Telefone = modeloPolitico.Telefone;

            return novoDadoSensivel;
        }

        public static void EditarDadosSensiveis(PoliticoPatchDto modeloPolitico, int id, ApplicationDbContext database)
        {
            var dadoDb = database.DadosSensiveis.First(dado => dado.Id == id);
            dadoDb.Cpf = modeloPolitico.Cpf != null ? modeloPolitico.Cpf : dadoDb.Cpf;
            dadoDb.Endereco = modeloPolitico.Endereco != null ? modeloPolitico.Endereco : dadoDb.Endereco;
            dadoDb.Telefone = modeloPolitico.Telefone != null ? modeloPolitico.Telefone : dadoDb.Telefone;

            database.SaveChanges();
        }
        public static Politico NovoPolitico(DadoSensivel dadosPolitico, string nomeFoto, Partido partidoBd, string nomePolitico, int projetosDeLei, string tipoPolitico)
        {
            Politico novoPolitico;
            switch (tipoPolitico)
            {
                case "Vereador":
                    novoPolitico = new Vereador();
                    break;
                case "Governador":
                    novoPolitico = new Governador();
                    break;
                case "Deputado Federal":
                    novoPolitico = new DeputadoFederal();
                    break;
                case "Deputado Estadual":
                    novoPolitico = new DeputadoEstadual();
                    break;
                case "Senador":
                    novoPolitico = new Senador();
                    break;
                case "Ministro de Estado":
                    novoPolitico = new MinistroDeEstado();
                    break;
                case "Prefeito":
                    novoPolitico = new Prefeito();
                    break;
                case "Presidente":
                    novoPolitico = new Presidente();
                    break;
                default:
                    novoPolitico = new Politico();
                    break;
            }

            novoPolitico.Nome = nomePolitico;
            novoPolitico.DadosSensiveisId = dadosPolitico.Id;
            novoPolitico.DadosSensiveis = dadosPolitico;
            novoPolitico.Foto = nomeFoto;
            novoPolitico.ProjetosDeLei = projetosDeLei;
            novoPolitico.PartidoId = partidoBd == null ? (int?)null : partidoBd.Id;
            novoPolitico.Partido = partidoBd == null ? null : partidoBd;

            return novoPolitico;
        } 
    }
}