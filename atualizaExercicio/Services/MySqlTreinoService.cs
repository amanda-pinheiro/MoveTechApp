using atualizaExercicio.Models;
using atualizaExercicio.Services.Database;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace atualizaExercicio.Services
{
    public class MySqlTreinoService : ITreinoService
    {
        public async Task<List<Exercicio>> BuscarExerciciosAsync(ExercicioFiltro filtro = null)
        {
            var exercicios = new List<Exercicio>();

            try
            {
                using (var conn = new MySqlConnection(DatabaseConfig.ConnectionString))
                {
                    await conn.OpenAsync();

                    // ✅ APENAS busca por nome do exercício
                    string query = "SELECT idExercicio, nomeExer, exercicioGrupMuscular, imagem FROM exercicio";

                    if (filtro != null && !string.IsNullOrEmpty(filtro.Nome))
                    {
                        query += " WHERE nomeExer LIKE @Nome";
                    }

                    query += " ORDER BY nomeExer";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        if (filtro != null && !string.IsNullOrEmpty(filtro.Nome))
                            cmd.Parameters.AddWithValue("@Nome", $"%{filtro.Nome}%");

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var exercicio = new Exercicio
                                {
                                    IdExercicio = reader.GetInt32("idExercicio"),
                                    NomeExer = reader.GetString("nomeExer"),
                                    ExercicioGrupMuscular = reader.GetInt32("exercicioGrupMuscular"),
                                    Imagem = reader.IsDBNull(reader.GetOrdinal("imagem")) ? "" : reader.GetString("imagem")
                                };
                                exercicios.Add(exercicio);
                            }
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine($"✅ Encontrados {exercicios.Count} exercícios no BD");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao buscar exercícios: {ex.Message}");
            }

            return exercicios;
        }

        public async Task<ValidationResult> CriarTreinoAsync(CriarTreinoData treinoData)
        {
            try
            {
                using (var conn = new MySqlConnection(DatabaseConfig.ConnectionString))
                {
                    await conn.OpenAsync();

                    string query = @"
                INSERT INTO Treino (tituloTreino, objetivo, dataInicio, dataFim, usuario_Treino)
                VALUES (@TituloTreino, @Objetivo, @DataInicio, @DataFim, @UsuarioId);
                SELECT LAST_INSERT_ID();";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TituloTreino", treinoData.TituloTreino);
                        cmd.Parameters.AddWithValue("@Objetivo", treinoData.Objetivo);
                        cmd.Parameters.AddWithValue("@DataInicio", treinoData.DataInicio ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DataFim", treinoData.DataFim ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@UsuarioId", treinoData.UsuarioId);

                        var treinoId = await cmd.ExecuteScalarAsync();

                        return ValidationResult.Success($"Treino criado com ID: {treinoId}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao criar treino: {ex.Message}");
                return ValidationResult.Error($"Erro ao criar treino: {ex.Message}");
            }
        }

        public async Task<ValidationResult> SalvarExercicioTreinoAsync(ExercicioParametros parametros)
        {
            try
            {
                using (var conn = new MySqlConnection(DatabaseConfig.ConnectionString))
                {
                    await conn.OpenAsync();

                    string query = @"
                INSERT INTO TreinoExercicio (serie, reps, intervalo, carga, idTreino, idExercicio)
                VALUES (@Serie, @Reps, @Intervalo, @Carga, @TreinoId, @ExercicioId)";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Serie", parametros.Serie);
                        cmd.Parameters.AddWithValue("@Reps", parametros.Reps);
                        cmd.Parameters.AddWithValue("@Intervalo", (int)parametros.Intervalo.TotalSeconds);
                        cmd.Parameters.AddWithValue("@Carga", parametros.Carga > 0 ? (object)parametros.Carga : DBNull.Value);
                        cmd.Parameters.AddWithValue("@TreinoId", parametros.TreinoId);
                        cmd.Parameters.AddWithValue("@ExercicioId", parametros.ExercicioId);

                        await cmd.ExecuteNonQueryAsync();

                        return ValidationResult.Success($"Exercício '{parametros.NomeExercicio}' salvo no treino");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao salvar exercício no treino: {ex.Message}");
                return ValidationResult.Error($"Erro ao salvar exercício: {ex.Message}");
            }
        }




    }
}