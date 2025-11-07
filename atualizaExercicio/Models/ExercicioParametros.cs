using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atualizaExercicio.Models
{
    public class ExercicioParametros
    {
        public int Serie { get; set; } = 3;
        public int Reps { get; set; } = 12;
        public TimeSpan Intervalo { get; set; } = TimeSpan.FromSeconds(60);

        public double Carga { get; set; }
        public int ExercicioId { get; set; }
        public string NomeExercicio { get; set; } = "";

        // ✅ Para salvar depois no BD
        public int TreinoId { get; set; }

        public int TreinoExercicioId { get; set; }
    }
}
