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

        Task<List<TreinoCardViewModel>> BuscarTreinosCardAsync(int usuarioId);

        Task<TreinoDetalhesViewModel> BuscarDetalhesTreinoAsync(int treinoId);

        Task<bool> RegistrarTreinoConcluidoAsync(RegistroTreinoData registroData);

        Task<ExercicioParametros> BuscarParametrosAnterioresAsync(int exercicioId, int usuarioId, int treinoExercicioAtualId);

        Task<bool> AtualizarExercicioTreinoAsync(ExercicioParametros parametros);



    }

}