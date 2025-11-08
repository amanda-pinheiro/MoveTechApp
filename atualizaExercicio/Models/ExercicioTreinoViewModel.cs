using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atualizaExercicio.Models
{
    public class ExercicioTreinoViewModel
    {
        public int ExercicioId { get; set; }
        public string NomeExercicio { get; set; } = "";
        public string ImagemExercicio { get; set; } = "";
        public int Serie { get; set; }
        public int Reps { get; set; }
        public double Carga { get; set; }
        public TimeSpan Intervalo { get; set; }

        // ✅ Propriedade calculada para mostrar no formato "3x12"
        public string SeriesReps => $"{Serie}x{Reps}";

        // ✅ Para o menu de ações
        public int TreinoExercicioId { get; set; }
    }
}
