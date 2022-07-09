using System;
using System.Collections.Generic;
using CADASTRO.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CADASTRO.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Partido> Partidos { get; set; }
        public DbSet<LiderPartido> Lideres { get; set; }
        public DbSet<DadoSensivel> DadosSensiveis { get; set; }
        public DbSet<Vereador> Vereadores { get; set; }
        public DbSet<Governador> Governadores { get; set; }
        public DbSet<Senador> Senadores { get; set; }
        public DbSet<Presidente> Presidentes { get; set; }
        public DbSet<Prefeito> Prefeitos { get; set; }
        public DbSet<MinistroDeEstado> MinistrosDeEstado { get; set; }
        public DbSet<DeputadoEstadual> DeputadosEstaduais { get; set; }
        public DbSet<DeputadoFederal> DeputadosFederais { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            this.SeedUser(builder);
            this.SeedPartidos(builder);
            this.SeedVereadores(builder);
            this.SeedGovernadores(builder);
            this.SeedSenadores(builder);
            this.SeedPresidente(builder);
            this.SeedPrefeitos(builder);
            this.SeedMinistros(builder);
            this.SeedDeputadosEstaduais(builder);
            this.SeedDeputadosFederais(builder);
        }

        private void SeedUser(ModelBuilder builder)
        {
            var hasher = new PasswordHasher<IdentityUser>();

            IdentityUser user1 = new IdentityUser()
            {
                Id = "58f4c99a-fabd-4946-bc52-34416a856353",
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                PasswordHash = hasher.HashPassword(null, "Gft2021")
            };

            builder.Entity<IdentityUser>().HasData(user1);

            IdentityRole role1 = new IdentityRole()
            {
                Name = "Admin",
                NormalizedName = "ADMIN"
            };  

            builder.Entity<IdentityRole>().HasData(role1);

            IdentityUserRole<string> userRole1 = new IdentityUserRole<string>()
            {
                RoleId = role1.Id,
                UserId = user1.Id
            };

            builder.Entity<IdentityUserRole<string>>().HasData(userRole1);
        }

        private void SeedPartidos(ModelBuilder builder)
        {
            List<int> idsDosPartidos = new List<int>() {1, 2, 3, 4, 5, 6, 7, 8};
            List<string> nomesDosPartidos = new List<string>() {"Movimento Democrático Brasileiro", "Partido dos Trabalhadores", "Partido da Social Democracia Brasileira", "Progressistas", "Partido Democrático Trabalhista", "Partido Trabalhista Brasileiro", "Democratas", "Partido Liberal"};
            List<string> siglasDosPartidos = new List<string>() {"MDB", "PT", "PSDB", "PP", "PDT", "PTB", "DEM", "PL"};
            List<int> numerosDosPartidos = new List<int>() {15, 13, 45, 11, 12, 14, 25, 22};
            List<string> lideresNaCamara = new List<string>() {"Isnaldo Bulhões Jr.", "Bohn Gass", "Rodrigo de Castro", "Cacá Leão", "Wolney Queiroz", "Nivaldo Albuquerque", "Efraim Filho", "Wellington Roberto"};

            for (int i = 0; i < nomesDosPartidos.Count; i++)
            {
                Partido novoPartido = new Partido
                {
                    Id = idsDosPartidos[i],
                    Nome = nomesDosPartidos[i],
                    Sigla = siglasDosPartidos[i],
                    NumeroPartido = numerosDosPartidos[i],
                };

                LiderPartido novoLider = new LiderPartido
                {
                    Id = idsDosPartidos[i],
                    Nome = lideresNaCamara[i]
                };

                builder.Entity<Partido>().HasData(novoPartido);
                builder.Entity<LiderPartido>().HasData(novoLider);
            }
        }
        private void SeedVereadores(ModelBuilder builder)
        {
            List<string> nomesDosVereadores = new List<string>() {"Adilson Amadeu", "Lindbergh Farias"};
            List<string> cpfDosVereadores = new List<string>() {"12345678900", "98765432100"};
            List<string> enderecoDosVereadores = new List<string>() {"Rua Inventada 1, nº 39, São Paulo", "Praça Floriano s/nº, Rio de Janeiro"};
            List<string> telefoneDosVereadores = new List<string>() {"1133964628", "2138142735"};
            List<int> idPartidosDosVereadores = new List<int>() {7, 2};
            List<string> fotosDosVereadores = new List<string>() {"AdilsonAmadeu.jpg", "LindberghFarias.jpg"};
            List<int> projetosDeLei = new List<int>() {15, 26};
            List<string> estadosDosVereadores = new List<string>() {"São Paulo", "Rio de Janeiro"};
            List<string> cidadesDosVereadores = new List<string>() {"São Paulo", "Rio de Janeiro"};
            List<bool> vereadorProcessado = new List<bool>() {true, true};

            for (int i = 0; i < nomesDosVereadores.Count; i++)
            {
                DadoSensivel novoDadoSensivel = new DadoSensivel
                {
                    Id = i + 1,
                    Cpf = cpfDosVereadores[i],
                    Endereco = enderecoDosVereadores[i],
                    Telefone = telefoneDosVereadores[i],
                };
                Vereador novoVereador = new Vereador
                {
                    Id = i + 1,
                    Nome = nomesDosVereadores[i],
                    DadosSensiveisId = novoDadoSensivel.Id,
                    PartidoId = idPartidosDosVereadores[i],
                    Foto = fotosDosVereadores[i],
                    ProjetosDeLei = projetosDeLei[i],
                    Estado = estadosDosVereadores[i],
                    Cidade = cidadesDosVereadores[i],
                    Processo = vereadorProcessado[i]
                };
                builder.Entity<DadoSensivel>().HasData(novoDadoSensivel);
                builder.Entity<Vereador>().HasData(novoVereador);
            }
        }
        private void SeedGovernadores(ModelBuilder builder)
        {
            List<string> nomesDosGovernadores = new List<string>() {"Renan Filho", "Reinaldo Azambuja"};
            List<string> cpfDosGovernadores = new List<string>() {"98765412345", "98765432100"};
            List<string> enderecoDosGovernadores = new List<string>() {"Rua Inventada 2, nº 50, Maceió", "Praça Sem Nome s/nº, Campo Grande"};
            List<string> telefoneDosGovernadores = new List<string>() {"8298741235", "6732156984"};
            List<int> idPartidosDosGovernadores = new List<int>() {1, 3};
            List<string> fotosDosGovernadores = new List<string>() {"RenanFilho.jpg", "ReinaldoAzambuja.jpg"};
            List<int> projetosDeLei = new List<int>() {11, 26};
            List<string> estadosDosGovernadores = new List<string>() {"Alagoas", "Mato Grosso do Sul"};
            List<bool> governadorProcessado = new List<bool>() {false, false};

            for (int i = 0; i < nomesDosGovernadores.Count; i++)
            {
                DadoSensivel novoDadoSensivel = new DadoSensivel
                {
                    Id = i + 3,
                    Cpf = cpfDosGovernadores[i],
                    Endereco = enderecoDosGovernadores[i],
                    Telefone = telefoneDosGovernadores[i],
                };
                Governador novoGovernador = new Governador
                {
                    Id = i + 1,
                    Nome = nomesDosGovernadores[i],
                    DadosSensiveisId = novoDadoSensivel.Id,
                    PartidoId = idPartidosDosGovernadores[i],
                    Foto = fotosDosGovernadores[i],
                    ProjetosDeLei = projetosDeLei[i],
                    Estado = estadosDosGovernadores[i],
                    Processo = governadorProcessado[i]
                };
                builder.Entity<DadoSensivel>().HasData(novoDadoSensivel);
                builder.Entity<Governador>().HasData(novoGovernador);
            }          
        }
        private void SeedSenadores(ModelBuilder builder)
        {
            List<string> nomesDosSenadores = new List<string>() {"Jaques Wagner", "Rose de Freitas"};
            List<string> cpfDosSenadores = new List<string>() {"85236974158", "74114736965"};
            List<string> enderecoDosSenadores = new List<string>() {"Av. L. Viana Filho, nº 6462, Salvador", "Av. João Baptista Parra, nº 673, Vitória"};
            List<string> telefoneDosSenadores = new List<string>() {"7133036390", "2733031156"};
            List<int> idPartidosDosSenadores = new List<int>() {2, 1};
            List<string> fotosDosSenadores = new List<string>() {"JaquesWagner.jpg", "RoseDeFreitas.jpg"};
            List<int> projetosDeLei = new List<int>() {23, 26};
            List<string> estadosDosSenadores = new List<string>() {"Bahia", "Espírito Santo"};

            for (int i = 0; i < nomesDosSenadores.Count; i++)
            {
                DadoSensivel novoDadoSensivel = new DadoSensivel
                {
                    Id = i + 5,
                    Cpf = cpfDosSenadores[i],
                    Endereco = enderecoDosSenadores[i],
                    Telefone = telefoneDosSenadores[i],
                };
                Senador novoSenador = new Senador
                {
                    Id = i + 1,
                    Nome = nomesDosSenadores[i],
                    DadosSensiveisId = novoDadoSensivel.Id,
                    PartidoId = idPartidosDosSenadores[i],
                    Foto = fotosDosSenadores[i],
                    ProjetosDeLei = projetosDeLei[i],
                    Estado = estadosDosSenadores[i]
                };
                builder.Entity<DadoSensivel>().HasData(novoDadoSensivel);
                builder.Entity<Senador>().HasData(novoSenador);
            }
        }
        private void SeedPresidente(ModelBuilder builder)
        {
            List<string> nomesDosPresidentes = new List<string>() {"Jair Bolsonaro", "Luiz Inácio Lula da Silva"};
            List<string> cpfDosPresidentes = new List<string>() {"89636974158", "74119126965"};
            List<string> enderecoDosPresidentes = new List<string>() {"Brasília", "São Bernardo do Campo"};
            List<string> telefoneDosPresidentes = new List<string>() {"6133036390", "1133031156"};
            List<int> idPartidosDosPresidentes = new List<int>() {8, 2};
            List<string> fotosDosPresidentes = new List<string>() {"JairBolsonaro.jpg", "LulaDaSilva.jpg"};
            List<int> projetosDeLei = new List<int>() {35, 36};

            for (int i = 0; i < nomesDosPresidentes.Count; i++)
            {
                DadoSensivel novoDadoSensivel = new DadoSensivel
                {
                    Id = i + 7,
                    Cpf = cpfDosPresidentes[i],
                    Endereco = enderecoDosPresidentes[i],
                    Telefone = telefoneDosPresidentes[i],
                };
                Presidente novoPresidente = new Presidente
                {
                    Id = i + 1,
                    Nome = nomesDosPresidentes[i],
                    DadosSensiveisId = novoDadoSensivel.Id,
                    PartidoId = idPartidosDosPresidentes[i],
                    Foto = fotosDosPresidentes[i],
                    ProjetosDeLei = projetosDeLei[i]
                };
                builder.Entity<DadoSensivel>().HasData(novoDadoSensivel);
                builder.Entity<Presidente>().HasData(novoPresidente);
            }
        }
        private void SeedPrefeitos(ModelBuilder builder)
        {
            List<string> nomesDosPrefeitos = new List<string>() {"Rodolfo Hessel Fanganiello", "Nelita Cristina Michel Franceschini"};
            List<string> cpfDosPrefeitos = new List<string>() {"26533296417", "96658821247"};
            List<string> enderecoDosPrefeitos = new List<string>() {"Rua Inventada 5, nº 10, São Paranapanema", "Praça X, nº 11, Iracemápolis"};
            List<string> telefoneDosPrefeitos = new List<string>() {"1169325478", "1745632857"};
            List<int> idPartidosDosPrefeitos = new List<int>() {4, 8};
            List<string> fotosDosPrefeitos = new List<string>() {"RodolfoFanganiello.jpg", "NelitaMichel.jpg"};
            List<int> projetosDeLei = new List<int>() {33, 26};
            List<string> estadosDosPrefeitos = new List<string>() {"São Paulo", "São Paulo"};
            List<string> cidadesDosPrefeitos = new List<string>() {"Paranapanema", "Iracemápolis"};

            for (int i = 0; i < nomesDosPrefeitos.Count; i++)
            {
                DadoSensivel novoDadoSensivel = new DadoSensivel
                {
                    Id = i + 9,
                    Cpf = cpfDosPrefeitos[i],
                    Endereco = enderecoDosPrefeitos[i],
                    Telefone = telefoneDosPrefeitos[i],
                };
                Prefeito novoPrefeito = new Prefeito
                {
                    Id = i + 1,
                    Nome = nomesDosPrefeitos[i],
                    DadosSensiveisId = novoDadoSensivel.Id,
                    PartidoId = idPartidosDosPrefeitos[i],
                    Foto = fotosDosPrefeitos[i],
                    ProjetosDeLei = projetosDeLei[i],
                    Estado = estadosDosPrefeitos[i],
                    Cidade = cidadesDosPrefeitos[i]
                };
                builder.Entity<DadoSensivel>().HasData(novoDadoSensivel);
                builder.Entity<Prefeito>().HasData(novoPrefeito);      
            }      
        }
        
        private void SeedMinistros(ModelBuilder builder)
        {
            List<string> nomesDosMinistros = new List<string>() {"Tereza Cristina", "Onyx Lorenzoni"};
            List<string> cpfDosMinistros = new List<string>() {"11236254987", "88563101270"};
            List<string> enderecoDosMinistros = new List<string>() {"Rua x, Brasília", "Rua Y, Brasília"};
            List<string> telefoneDosMinistros = new List<string>() {"6132574589", "6120254511"};
            List<int> idPartidosDosMinistros = new List<int>() {7, 7};
            List<string> fotosDosMinistros = new List<string>() {"TerezaCristina.jpg", "OnyxLorenzoni.jpg"};
            List<int> projetosDeLei = new List<int>() {22, 36};
            List<string> pasta = new List<string>() {"Agricultura, Pecuária e Abastecimento", "Trabalho e Previdência"};

            for (int i = 0; i < nomesDosMinistros.Count; i++)
            {
                DadoSensivel novoDadoSensivel = new DadoSensivel
                {
                    Id = i + 11,
                    Cpf = cpfDosMinistros[i],
                    Endereco = enderecoDosMinistros[i],
                    Telefone = telefoneDosMinistros[i],
                };
                MinistroDeEstado novoMinistro = new MinistroDeEstado
                {
                    Id = i + 1,
                    Nome = nomesDosMinistros[i],
                    DadosSensiveisId = novoDadoSensivel.Id,
                    PartidoId = idPartidosDosMinistros[i],
                    Foto = fotosDosMinistros[i],
                    ProjetosDeLei = projetosDeLei[i],
                    Pasta = pasta[i]
                };

                if(i == 1)
                {
                    novoMinistro.Partido = null;
                }
                builder.Entity<DadoSensivel>().HasData(novoDadoSensivel);
                builder.Entity<MinistroDeEstado>().HasData(novoMinistro);
            }            
        }

        private void SeedDeputadosEstaduais(ModelBuilder builder)
        {
            List<string> nomesDosDeputadosEstaduais = new List<string>() {"Analice Fernandes", "Coronel Telhada"};
            List<string> cpfDosDeputadosEstaduais = new List<string>() {"14744522369", "00215985512"};
            List<string> enderecoDosDeputadosEstaduais = new List<string>() {"Rua Inventada 10, nº 50, Taboão da Serra", "Praça Sem Nome s/nº, São Paulo"};
            List<string> telefoneDosDeputadosEstaduais = new List<string>() {"1122325247", "1125229897"};
            List<int> idPartidosDosDeputadosEstaduais = new List<int>() {3, 4};
            List<string> fotosDosDeputadosEstaduais = new List<string>() {"AnaliceFernandes.jpg", "CoronelTelhada.jpg"};
            List<int> projetosDeLei = new List<int>() {29, 31};
            List<string> estadosDosDeputadosEstaduais = new List<string>() {"São Paulo", "São Paulo"};
            List<bool> deputadoProcessado = new List<bool>() {false, false};

            for (int i = 0; i < nomesDosDeputadosEstaduais.Count; i++)
            {
                DadoSensivel novoDadoSensivel = new DadoSensivel
                {
                    Id = i + 13,
                    Cpf = cpfDosDeputadosEstaduais[i],
                    Endereco = enderecoDosDeputadosEstaduais[i],
                    Telefone = telefoneDosDeputadosEstaduais[i],
                };
                DeputadoEstadual novoDeputadoEstadual = new DeputadoEstadual
                {
                    Id = i + 1,
                    Nome = nomesDosDeputadosEstaduais[i],
                    DadosSensiveisId = novoDadoSensivel.Id,
                    PartidoId = idPartidosDosDeputadosEstaduais[i],
                    Foto = fotosDosDeputadosEstaduais[i],
                    ProjetosDeLei = projetosDeLei[i],
                    Estado = estadosDosDeputadosEstaduais[i],
                    Processo = deputadoProcessado[i]
                };
                builder.Entity<DadoSensivel>().HasData(novoDadoSensivel);
                builder.Entity<DeputadoEstadual>().HasData(novoDeputadoEstadual);
            }       
        }       
        private void SeedDeputadosFederais(ModelBuilder builder)
        {
            List<string> nomesDosDeputadosFederais = new List<string>() {"Geovania de Sá", "Aécio Neves da Cunha"};
            List<string> cpfDosDeputadosFederais = new List<string>() {"99658742312", "11235789611"};
            List<string> enderecoDosDeputadosFederais = new List<string>() {"Av. Inventada 10, nº 22, Criciúma", "Av. Y, nº 40, Belo Horizonte"};
            List<string> telefoneDosDeputadosFederais = new List<string>() {"4853326964", "6132155964"};
            List<int> idPartidosDosDeputadosFederais = new List<int>() {3, 3};
            List<string> fotosDosDeputadosFederais = new List<string>() {"GeovaniaDeSa.jpg", "AecioNeves.jpg"};
            List<int> projetosDeLei = new List<int>() {45, 36};
            List<string> estadosDosDeputadosFederais = new List<string>() {"Santa Catarina", "Minas Gerais"};
            List<bool> deputadoProcessado = new List<bool>() {false, true};

            for (int i = 0; i < nomesDosDeputadosFederais.Count; i++)
            {
                DadoSensivel novoDadoSensivel = new DadoSensivel
                {
                    Id = i + 15,
                    Cpf = cpfDosDeputadosFederais[i],
                    Endereco = enderecoDosDeputadosFederais[i],
                    Telefone = telefoneDosDeputadosFederais[i],
                };
                DeputadoFederal novoDeputadoFederal = new DeputadoFederal
                {
                    Id = i + 1,
                    Nome = nomesDosDeputadosFederais[i],
                    DadosSensiveisId = novoDadoSensivel.Id,
                    PartidoId = idPartidosDosDeputadosFederais[i],
                    Foto = fotosDosDeputadosFederais[i],
                    ProjetosDeLei = projetosDeLei[i],
                    Estado = estadosDosDeputadosFederais[i],
                    Processo = deputadoProcessado[i]
                };
                builder.Entity<DadoSensivel>().HasData(novoDadoSensivel);
                builder.Entity<DeputadoFederal>().HasData(novoDeputadoFederal);
            }       
        }
    }
}