using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atualizaExercicio.Models
{
    public class Exercicio
    {
        
        // Corresponde a: idExercicio INT AUTO_INCREMENT PRIMARY KEY
        public int IdExercicio { get; set; }

        // Corresponde a: nomeExer VARCHAR(70)
        public string NomeExer { get; set; } = "";

        // Corresponde a: exercicioGrupMuscular INT (FK para GrupoMuscular)
        public int ExercicioGrupMuscular { get; set; }

        // Corresponde a: imagem VARCHAR(70)
        public string Imagem { get; set; } = "";

        // Propriedade extra para exibir o nome do grupo muscular na UI
        // (não existe no BD, será preenchida via JOIN ou depois)
        public string NomeGrupoMuscular { get; set; } = "";
    }
}

