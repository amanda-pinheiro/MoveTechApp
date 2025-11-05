using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atualizaExercicio.Models
{
    public class CriarTreinoData
    {
        public string TituloTreino { get; set; } = "";
        public string Objetivo { get; set; } = "";
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public int UsuarioId { get; set; } // ✅ USUÁRIO REAL (será preenchido dinamicamente)
    }


}
