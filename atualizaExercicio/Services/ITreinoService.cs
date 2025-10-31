using atualizaExercicio.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace atualizaExercicio.Services
{
    public interface ITreinoService
    {
        // ✅ APENAS ESTE MÉTODO PARA A LISTA FUNCIONAR
        Task<List<Exercicio>> BuscarExerciciosAsync(ExercicioFiltro filtro = null);

        Task<ValidationResult> CriarTreinoAsync(CriarTreinoData treinoData);
        Task<ValidationResult> SalvarExercicioTreinoAsync(ExercicioParametros parametros);

    }

}