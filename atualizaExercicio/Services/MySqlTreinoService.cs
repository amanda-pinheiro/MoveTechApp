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

        // ✅ CORRIGIDO: Agora retorna int (ID do treino criado)
        public async Task<int> CriarTreinoAsync(CriarTreinoData treinoData)
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

                        var result = await cmd.ExecuteScalarAsync();
                        int treinoId = result != null ? Convert.ToInt32(result) : 0;

                        System.Diagnostics.Debug.WriteLine($"✅ Treino criado com ID: {treinoId}");
                        return treinoId;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao criar treino: {ex.Message}");
                return 0;
            }
        }

        // ✅ CORRIGIDO: Agora retorna bool (sucesso/falha)
        public async Task<bool> SalvarExercicioTreinoAsync(ExercicioParametros parametros)
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

                        System.Diagnostics.Debug.WriteLine($"✅ Exercício '{parametros.NomeExercicio}' salvo no treino {parametros.TreinoId}");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao salvar exercício no treino: {ex.Message}");
                return false;
            }
        }

        public async Task<List<TreinoCardViewModel>> BuscarTreinosCardAsync(int usuarioId)
        {
            var treinos = new List<TreinoCardViewModel>();

            try
            {
                using (var conn = new MySqlConnection(DatabaseConfig.ConnectionString))
                {
                    await conn.OpenAsync();

                    string query = @"
                SELECT 
                    t.idTreino,
                    t.tituloTreino,
                    COALESCE(
                        (SELECT MAX(rt2.data) 
                         FROM RegistroTreino rt2 
                         WHERE rt2.registro_Treino = t.idTreino),
                        t.dataInicio
                    ) as DataDisplay,
                    COALESCE(gm.nomeGrupo, 'Geral') as GrupoMuscularPrincipal,
                    COALESCE(e.imagem, 'placeholder_exercicio.png') as ImagemExercicio,
                    (SELECT COUNT(*) FROM RegistroTreino rt WHERE rt.registro_Treino = t.idTreino) > 0 as TemRegistro
                FROM Treino t
                LEFT JOIN TreinoExercicio te ON t.idTreino = te.idTreino
                LEFT JOIN Exercicio e ON te.idExercicio = e.idExercicio
                LEFT JOIN GrupoMuscular gm ON e.exercicioGrupMuscular = gm.idGrupoMuscular
                WHERE t.usuario_Treino = @UsuarioId
                GROUP BY t.idTreino
                ORDER BY DataDisplay DESC, t.idTreino DESC";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var dataDisplay = reader.GetDateTime("DataDisplay");
                                var treino = new TreinoCardViewModel
                                {
                                    TreinoId = reader.GetInt32("idTreino"),
                                    Titulo = reader.GetString("tituloTreino"),
                                    DataRegistro = dataDisplay.ToString("dd/MM/yyyy"),
                                    GrupoMuscularPrincipal = reader.GetString("GrupoMuscularPrincipal"),
                                    ImagemPrimeiroExercicio = reader.GetString("ImagemExercicio"),
                                    TemRegistro = reader.GetBoolean("TemRegistro")
                                };
                                treinos.Add(treino);
                            }
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine($"✅ Encontrados {treinos.Count} treinos para o usuário {usuarioId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao buscar treinos: {ex.Message}");
            }

            return treinos;
        }

        public async Task<TreinoDetalhesViewModel> BuscarDetalhesTreinoAsync(int treinoId)
        {
            try
            {
                using (var conn = new MySqlConnection(DatabaseConfig.ConnectionString))
                {
                    await conn.OpenAsync();

                    string query = @"
                SELECT 
                    t.idTreino,
                    t.tituloTreino,
                    t.objetivo,
                    t.dataInicio,
                    t.dataFim,
                    te.idTreinoExercicio,
                    te.serie,
                    te.reps,
                    te.intervalo,
                    te.carga,
                    e.idExercicio,
                    e.nomeExer,
                    e.imagem as imagemOriginal
                FROM Treino t
                INNER JOIN TreinoExercicio te ON t.idTreino = te.idTreino
                INNER JOIN Exercicio e ON te.idExercicio = e.idExercicio
                WHERE t.idTreino = @TreinoId
                ORDER BY te.idTreinoExercicio";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TreinoId", treinoId);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            var treinoDetalhes = new TreinoDetalhesViewModel();
                            bool primeiroRegistro = true;

                            while (await reader.ReadAsync())
                            {
                                if (primeiroRegistro)
                                {
                                    treinoDetalhes.TreinoId = reader.GetInt32("idTreino");
                                    treinoDetalhes.TituloTreino = reader.GetString("tituloTreino");
                                    treinoDetalhes.Objetivo = reader.GetString("objetivo");

                                    if (!reader.IsDBNull(reader.GetOrdinal("dataInicio")))
                                        treinoDetalhes.DataInicio = reader.GetDateTime("dataInicio");

                                    if (!reader.IsDBNull(reader.GetOrdinal("dataFim")))
                                        treinoDetalhes.DataFim = reader.GetDateTime("dataFim");

                                    primeiroRegistro = false;
                                }

                                // ✅ TRATAMENTO DA IMAGEM: Se estiver vazia, usa placeholder
                                string imagemExercicio = "placeholder_exercicio.png";
                                if (!reader.IsDBNull(reader.GetOrdinal("imagemOriginal")))
                                {
                                    var imagemOriginal = reader.GetString("imagemOriginal");
                                    if (!string.IsNullOrEmpty(imagemOriginal) && imagemOriginal != "placeholder_exercicio.png")
                                    {
                                        imagemExercicio = imagemOriginal;
                                    }
                                }

                                var exercicio = new ExercicioTreinoViewModel
                                {
                                    TreinoExercicioId = reader.GetInt32("idTreinoExercicio"),
                                    ExercicioId = reader.GetInt32("idExercicio"),
                                    NomeExercicio = reader.GetString("nomeExer"),
                                    ImagemExercicio = imagemExercicio, // ✅ Já tratado
                                    Serie = reader.IsDBNull(reader.GetOrdinal("serie")) ? 3 : reader.GetInt32("serie"),
                                    Reps = reader.IsDBNull(reader.GetOrdinal("reps")) ? 12 : reader.GetInt32("reps"),
                                    Carga = reader.IsDBNull(reader.GetOrdinal("carga")) ? 0 : reader.GetDouble("carga"),
                                    Intervalo = reader.IsDBNull(reader.GetOrdinal("intervalo"))
                                        ? TimeSpan.FromSeconds(60)
                                        : TimeSpan.FromSeconds(reader.GetInt32("intervalo"))
                                };

                                treinoDetalhes.Exercicios.Add(exercicio);
                            }

                            System.Diagnostics.Debug.WriteLine($"✅ Encontrados {treinoDetalhes.Exercicios.Count} exercícios para o treino {treinoId}");
                            return treinoDetalhes;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao buscar detalhes do treino: {ex.Message}");
                return new TreinoDetalhesViewModel();
            }
        }

        public async Task<bool> RegistrarTreinoConcluidoAsync(RegistroTreinoData registroData)
        {
            try
            {
                using (var conn = new MySqlConnection(DatabaseConfig.ConnectionString))
                {
                    await conn.OpenAsync();

                    string query = @"
                INSERT INTO RegistroTreino (data, carga, registro_usuario, registro_Treino)
                VALUES (@Data, @Carga, @UsuarioId, @TreinoId)";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Data", registroData.Data);
                        cmd.Parameters.AddWithValue("@Carga", registroData.CargaTotal);
                        cmd.Parameters.AddWithValue("@UsuarioId", registroData.UsuarioId);
                        cmd.Parameters.AddWithValue("@TreinoId", registroData.TreinoId);

                        await cmd.ExecuteNonQueryAsync();

                        System.Diagnostics.Debug.WriteLine($"✅ Registro salvo: Treino {registroData.TreinoId} concluído em {registroData.Data}");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao registrar treino: {ex.Message}");
                return false;
            }
        }

        public async Task<ExercicioParametros> BuscarParametrosAnterioresAsync(int exercicioId, int usuarioId, int treinoExercicioAtualId)
        {
            try
            {
                using (var conn = new MySqlConnection(DatabaseConfig.ConnectionString))
                {
                    await conn.OpenAsync();

                    // ✅ CORREÇÃO: Buscar o histórico do MESMO exercício no MESMO treino anterior
                    string query = @"
                SELECT te.serie, te.reps, te.carga, te.intervalo 
                FROM TreinoExercicio te
                INNER JOIN Treino t ON te.idTreino = t.idTreino
                WHERE te.idExercicio = @ExercicioId 
                  AND t.usuario_Treino = @UsuarioId
                  AND te.idTreinoExercicio < @TreinoExercicioAtualId  -- ✅ Busca apenas anteriores
                ORDER BY te.idTreinoExercicio DESC  -- ✅ Ordena pelo ID do treinoExercicio
                LIMIT 1";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ExercicioId", exercicioId);
                        cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@TreinoExercicioAtualId", treinoExercicioAtualId);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new ExercicioParametros
                                {
                                    Serie = reader.IsDBNull(reader.GetOrdinal("serie")) ? 3 : reader.GetInt32("serie"),
                                    Reps = reader.IsDBNull(reader.GetOrdinal("reps")) ? 12 : reader.GetInt32("reps"),
                                    Carga = reader.IsDBNull(reader.GetOrdinal("carga")) ? 0 : reader.GetDouble("carga"),
                                    Intervalo = reader.IsDBNull(reader.GetOrdinal("intervalo"))
                                        ? TimeSpan.FromSeconds(60)
                                        : TimeSpan.FromSeconds(reader.GetInt32("intervalo"))
                                };
                            }
                            else
                            {
                                // ✅ Se não encontrar histórico, retorna valores padrão
                                return new ExercicioParametros
                                {
                                    Serie = 3,
                                    Reps = 12,
                                    Carga = 0,
                                    Intervalo = TimeSpan.FromSeconds(60)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao buscar parâmetros anteriores: {ex.Message}");
                return new ExercicioParametros(); // Retorna vazio em caso de erro
            }
        }

        public async Task<bool> AtualizarExercicioTreinoAsync(ExercicioParametros parametros)
        {
            try
            {
                using (var conn = new MySqlConnection(DatabaseConfig.ConnectionString))
                {
                    await conn.OpenAsync();

                    // ✅ ATUALIZA o registro existente (não cria novo)
                    string query = @"
                UPDATE TreinoExercicio 
                SET serie = @Serie, 
                    reps = @Reps, 
                    intervalo = @Intervalo, 
                    carga = @Carga
                WHERE idTreinoExercicio = @TreinoExercicioId";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Serie", parametros.Serie);
                        cmd.Parameters.AddWithValue("@Reps", parametros.Reps);
                        cmd.Parameters.AddWithValue("@Intervalo", (int)parametros.Intervalo.TotalSeconds);
                        cmd.Parameters.AddWithValue("@Carga", parametros.Carga > 0 ? (object)parametros.Carga : DBNull.Value);
                        cmd.Parameters.AddWithValue("@TreinoExercicioId", parametros.TreinoExercicioId);

                        int linhasAfetadas = await cmd.ExecuteNonQueryAsync();

                        bool atualizou = linhasAfetadas > 0;
                        System.Diagnostics.Debug.WriteLine($"✅ Exercício atualizado: {atualizou} - ID: {parametros.TreinoExercicioId}");
                        return atualizou;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao atualizar exercício: {ex.Message}");
                return false;
            }
        }
    }
}