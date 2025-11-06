using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atualizaExercicio.Models
{
    public class TreinoDetalhesViewModel
    {
        public int TreinoId { get; set; }
        public string TituloTreino { get; set; } = "";
        public string Objetivo { get; set; } = "";
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public List<ExercicioTreinoViewModel> Exercicios { get; set; } = new();
    }
}
