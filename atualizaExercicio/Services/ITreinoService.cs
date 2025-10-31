using atualizaExercicio.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace atualizaExercicio.Services
{
    public interface ITreinoService
    {
        Task<List<Exercicio>> BuscarExerciciosAsync(ExercicioFiltro filtro = null);
        Task<int> CriarTreinoAsync(CriarTreinoData treinoData); // ✅ Retorna int
        Task<bool> SalvarExercicioTreinoAsync(ExercicioParametros parametros); // ✅ Retorna bool

    }

}