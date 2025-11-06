using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atualizaExercicio.Models
{
    public class RegistroTreinoData
    {
        public int TreinoId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public double CargaTotal { get; set; } // Soma das cargas dos exercícios
    }
}
