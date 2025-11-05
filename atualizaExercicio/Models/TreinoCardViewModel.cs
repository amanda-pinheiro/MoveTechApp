using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atualizaExercicio.Models
{
    public class TreinoCardViewModel
    {
        public int TreinoId { get; set; }
        public string Titulo { get; set; } = "";
        public string DataRegistro { get; set; } = ""; // Data do RegistroTreino
        public string GrupoMuscularPrincipal { get; set; } = "";
        public string ImagemPrimeiroExercicio { get; set; } = "";
        public bool TemRegistro { get; set; } // Se já foi executado

    }
}
