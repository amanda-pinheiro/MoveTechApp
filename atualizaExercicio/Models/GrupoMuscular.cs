using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atualizaExercicio.Models
{
    public class GrupoMuscular
    {
        // Corresponde a: idGrupoMuscular INT AUTO_INCREMENT PRIMARY KEY
        public int IdGrupoMuscular { get; set; }

        // Corresponde a: nomeGrupo VARCHAR(45)
        public string NomeGrupo { get; set; } = "";
    }
}
