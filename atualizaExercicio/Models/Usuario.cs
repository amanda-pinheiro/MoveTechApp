using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atualizaExercicio.Models
{
    public class Usuario
    {
        // Corresponde a: idUsuario INT AUTO_INCREMENT PRIMARY KEY
        public int IdUsuario { get; set; }

        // Corresponde a: nome VARCHAR(45)
        public string Nome { get; set; } = "";

        // Corresponde a: email VARCHAR(45)
        public string Email { get; set; } = "";

        // Corresponde a: senha VARCHAR(8) - mas vamos salvar hash maior
        public string Senha { get; set; } = "";

        // Corresponde a: telefone VARCHAR(45)
        public string Telefone { get; set; } = "";

        // Corresponde a: genero VARCHAR(45)
        public string Genero { get; set; } = "";

        // Corresponde a: nomeUsuario VARCHAR(45)
        public string NomeUsuario { get; set; } = "";

        // Corresponde a: dataNascimento DATE
        public DateTime DataNascimento { get; set; }

        // Corresponde a: dataCadastro DATE
        public DateTime DataCadastro { get; set; }
    }
}
