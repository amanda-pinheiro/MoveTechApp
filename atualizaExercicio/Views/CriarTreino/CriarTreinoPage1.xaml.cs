using atualizaExercicio.Models;
using atualizaExercicio.Services;
using atualizaExercicio.Views.VisualizarTreino;
using System.Collections.Generic;
using System.Linq;
using System;

namespace atualizaExercicio.Views.CriarTreino
{
    public partial class CriarTreinoPage1 : ContentPage
    {
        private readonly ITreinoService _treinoService;
        private List<Exercicio>? _todosExercicios;
        private List<Exercicio> _exerciciosSelecionados;

        // ✅ NOVA LISTA: Parâmetros temporários
        private List<ExercicioParametros> _parametrosTemporarios = new List<ExercicioParametros>();

        public CriarTreinoPage1()
        {
            InitializeComponent();

            // ✅ BD REAL para buscar exercícios
            _treinoService = new MySqlTreinoService();
            _exerciciosSelecionados = new List<Exercicio>();

            CarregarExercicios();

            // ✅ INICIALIZAR ESTADO DO BOTÃO
            AtualizarBotaoSalvar();
        }

        private async void CarregarExercicios()
        {
            try
            {
                // ✅ Busca do BD real
                _todosExercicios = await _treinoService.BuscarExerciciosAsync(new ExercicioFiltro());
                AtualizarListaExercicios();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao carregar exercícios: {ex.Message}", "OK");
            }
        }

        private void AtualizarListaExercicios()
        {
            if (_todosExercicios == null) return;

            List<Exercicio> exerciciosParaMostrar;

            if (_exerciciosSelecionados.Count > 0)
            {
                exerciciosParaMostrar = _exerciciosSelecionados;
            }
            else
            {
                exerciciosParaMostrar = _todosExercicios;
            }

            CriarCardsExercicios(exerciciosParaMostrar);

            // ✅ ATUALIZAR BOTÃO APÓS MUDANÇAS NA LISTA
            AtualizarBotaoSalvar();
        }

        private void CriarCardsExercicios(List<Exercicio> exercicios)
        {
            var container = this.FindByName<VerticalStackLayout>("ListaExerciciosContainer");
            if (container == null) return;

            container.Clear();

            foreach (var exercicio in exercicios)
            {
                bool estaSelecionado = _exerciciosSelecionados.Any(ex => ex.IdExercicio == exercicio.IdExercicio);

                // ✅ VERIFICAR SE TEM PARÂMETROS SALVOS
                var parametros = _parametrosTemporarios.FirstOrDefault(p => p.ExercicioId == exercicio.IdExercicio);
                bool temParametros = parametros != null;

                var frame = new Frame
                {
                    BackgroundColor = estaSelecionado ? Color.FromArgb("#4D2D8C") : Color.FromArgb("#2C2C3E"),
                    CornerRadius = 12,
                    Padding = 12,
                    HasShadow = false,
                    Margin = new Thickness(0, 0, 0, 8)
                };

                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (s, e) => CardExercicio_Tapped(exercicio);
                frame.GestureRecognizers.Add(tapGesture);

                var grid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(60) },
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = new GridLength(40) }
                    },
                    ColumnSpacing = 12
                };

                // Imagem do exercício
                var imagemFrame = new Frame
                {
                    WidthRequest = 50,
                    HeightRequest = 50,
                    CornerRadius = 8,
                    Padding = 4,
                    HasShadow = false,
                    BackgroundColor = Colors.White,
                    VerticalOptions = LayoutOptions.Center
                };

                var imagem = new Image
                {
                    Source = string.IsNullOrEmpty(exercicio.Imagem)
                        ? "placeholder_exercicio.png"
                        : ImageSource.FromUri(new Uri(exercicio.Imagem)),
                    Aspect = Aspect.AspectFit,
                };

                imagemFrame.Content = imagem;
                grid.Add(imagemFrame, 0, 0);

                // Informações do exercício
                var infoStack = new VerticalStackLayout
                {
                    Spacing = 4,
                    VerticalOptions = LayoutOptions.Center
                };

                var nomeLabel = new Label
                {
                    Text = exercicio.NomeExer,
                    TextColor = Colors.White,
                    FontFamily = "QuicksandRegular",
                    FontSize = 14
                };

                var grupoLabel = new Label
                {
                    Text = $"ID: {exercicio.IdExercicio}",
                    TextColor = Color.FromArgb("#BBBBBB"),
                    FontFamily = "QuicksandLight",
                    FontSize = 11
                };

                infoStack.Add(nomeLabel);
                infoStack.Add(grupoLabel);

                // ✅ ADICIONAR LINHA COM PARÂMETROS SE EXISTIR
                if (temParametros)
                {
                    string textoCarga = parametros.Carga > 0 ? $" • {parametros.Carga}kg" : "";
                    var parametrosLabel = new Label
                    {
                        Text = $"{parametros.Serie}x{parametros.Reps}{textoCarga}",
                        TextColor = Color.FromArgb("#90EE90"), // Verde claro
                        FontFamily = "QuicksandRegular",
                        FontSize = 11,
                        Margin = new Thickness(0, 2, 0, 0)
                    };
                    infoStack.Add(parametrosLabel);
                }

                grid.Add(infoStack, 1, 0);

                // ✅ BOTÃO "+" SÓ PARA SELECIONADOS
                if (estaSelecionado)
                {
                    var botaoAdicionar = new Button
                    {
                        Text = "+",
                        BackgroundColor = Color.FromArgb("#4D2D8C"),
                        TextColor = Color.FromArgb("#E6E6FA"),
                        FontFamily = "QuicksandSemibold",
                        FontSize = 24,
                        CornerRadius = 20,
                        WidthRequest = 40,
                        HeightRequest = 40,
                        Padding = 0,
                        VerticalOptions = LayoutOptions.Center
                    };

                    botaoAdicionar.Clicked += (s, e) => BotaoConfigurar_Clicked(exercicio);
                    grid.Add(botaoAdicionar, 2, 0);
                }

                frame.Content = grid;
                container.Add(frame);
            }
        }

        private void CardExercicio_Tapped(Exercicio exercicio)
        {
            try
            {
                if (_exerciciosSelecionados.Any(ex => ex.IdExercicio == exercicio.IdExercicio))
                {
                    _exerciciosSelecionados.RemoveAll(ex => ex.IdExercicio == exercicio.IdExercicio);
                }
                else
                {
                    _exerciciosSelecionados.Add(exercicio);
                }

                AtualizarListaExercicios();
                System.Diagnostics.Debug.WriteLine($"{_exerciciosSelecionados.Count} exercícios selecionados");

                // ✅ ATUALIZAR BOTÃO APÓS SELECIONAR/DESSELECIONAR
                AtualizarBotaoSalvar();
            }
            catch (Exception ex)
            {
                DisplayAlert("Erro", $"Erro ao selecionar: {ex.Message}", "OK");
            }
        }

        // ✅ ATUALIZADO: Método do Botão "+" com CALLBACK
        private async void BotaoConfigurar_Clicked(Exercicio exercicio)
        {
            try
            {
                // ✅ NAVEGA PARA TELA DE PARÂMETROS COM CALLBACK
                await Navigation.PushAsync(new CriarTreinoPage2(
                    exercicio: exercicio,
                    exerciciosSelecionados: _exerciciosSelecionados,
                    onParametrosSalvos: (parametrosSalvos) =>
                    {
                        // ✅ CALLBACK: Quando parâmetros forem salvos na Page2
                        OnParametrosSalvos(parametrosSalvos);
                    }
                ));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao configurar exercício: {ex.Message}", "OK");
            }
        }

        // ✅ NOVO MÉTODO: Processar parâmetros salvos
        private void OnParametrosSalvos(List<ExercicioParametros> parametrosSalvos)
        {
            try
            {
                // ✅ ATUALIZAR LISTA DE PARÂMETROS TEMPORÁRIOS
                foreach (var parametro in parametrosSalvos)
                {
                    // Remover se já existir (atualização)
                    _parametrosTemporarios.RemoveAll(p => p.ExercicioId == parametro.ExercicioId);
                    // Adicionar novo
                    _parametrosTemporarios.Add(parametro);
                }

                // ✅ DEBUG: Mostrar quantos parâmetros temos
                System.Diagnostics.Debug.WriteLine($"✅ {_parametrosTemporarios.Count} exercícios com parâmetros salvos");

                // ✅ ATUALIZAR UI PARA MOSTRAR PARÂMETROS
                AtualizarListaExercicios();

                // ✅ ATUALIZAR BOTÃO APÓS SALVAR PARÂMETROS
                AtualizarBotaoSalvar();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao processar parâmetros: {ex.Message}");
            }
        }

        // ✅ MÉTODO: Verificar se pode salvar treino
        private bool PodeSalvarTreino()
        {
            // Se não tem exercícios selecionados, não pode salvar
            if (_exerciciosSelecionados.Count == 0)
                return false;

            // Verificar se TODOS os exercícios selecionados têm parâmetros
            foreach (var exercicio in _exerciciosSelecionados)
            {
                var parametros = _parametrosTemporarios.FirstOrDefault(p => p.ExercicioId == exercicio.IdExercicio);
                if (parametros == null)
                    return false; // ❌ Este exercício não tem parâmetros
            }

            return true; // ✅ Todos têm parâmetros
        }

        // ✅ MÉTODO: Atualizar estado do botão
        private void AtualizarBotaoSalvar()
        {
            if (SalvarTreinoButton != null)
            {
                bool podeSalvar = PodeSalvarTreino();
                SalvarTreinoButton.IsEnabled = podeSalvar;
                SalvarTreinoButton.BackgroundColor = podeSalvar ? Color.FromArgb("#7C3AED") : Color.FromArgb("#555555");

                // ✅ DEBUG
                System.Diagnostics.Debug.WriteLine($"🎯 Botão salvar: {(podeSalvar ? "HABILITADO" : "DESABILITADO")}");
            }
        }

        // ✅ MÉTODO COMPLETO: Salvar treino no BD (SIMPLIFICADO)
        private async void SalvarButton2_Clicked(object sender, EventArgs e)
        {
            try
            {
                // ✅ VALIDAR NOVAMENTE (segurança extra)
                if (!PodeSalvarTreino())
                {
                    await DisplayAlert("Aviso", "Configure os parâmetros de todos os exercícios selecionados antes de salvar o treino.", "OK");
                    return;
                }

                // ✅ SOLICITAR TÍTULO DO TREINO
                string tituloTreino = await DisplayPromptAsync(
                    "Salvar Treino",
                    "Digite um nome para o treino:",
                    "Salvar",
                    "Cancelar",
                    "Meu Treino",
                    maxLength: 45);

                if (string.IsNullOrWhiteSpace(tituloTreino))
                {
                    await DisplayAlert("Aviso", "É necessário informar um nome para o treino.", "OK");
                    return;
                }

                // ✅ OBTER ID DO USUÁRIO LOGADO
                var usuarioIdStr = await SecureStorage.GetAsync("usuario_id");
                if (string.IsNullOrEmpty(usuarioIdStr) || !int.TryParse(usuarioIdStr, out int usuarioId))
                {
                    await DisplayAlert("Erro", "Usuário não identificado. Faça login novamente.", "OK");
                    return;
                }

                // ✅ CRIAR DADOS DO TREINO
                var treinoData = new CriarTreinoData
                {
                    TituloTreino = tituloTreino.Trim(),
                    Objetivo = "Treino personalizado",
                    DataInicio = DateTime.Now.Date,
                    DataFim = null,
                    UsuarioId = usuarioId
                    
                };

                // ✅ SALVAR TREINO NO BD (SEM VALIDAÇÃO COMPLEXA)
                await _treinoService.CriarTreinoAsync(treinoData);

                // ✅ OBTER ID DO TREINO CRIADO
                int treinoId = await ObterTreinoIdCriado(treinoData.TituloTreino, usuarioId);

                if (treinoId == 0)
                {
                    await DisplayAlert("Erro", "Não foi possível identificar o treino criado.", "OK");
                    return;
                }

                // ✅ SALVAR EXERCÍCIOS DO TREINO (SEM VALIDAÇÃO COMPLEXA)
                int exerciciosSalvos = 0;
                foreach (var parametro in _parametrosTemporarios)
                {
                    // Só salvar os parâmetros dos exercícios selecionados
                    if (_exerciciosSelecionados.Any(ex => ex.IdExercicio == parametro.ExercicioId))
                    {
                        parametro.TreinoId = treinoId;

                        // ✅ CHAMA DIRETO SEM VERIFICAR RESULTADO
                        await _treinoService.SalvarExercicioTreinoAsync(parametro);
                        exerciciosSalvos++;

                        System.Diagnostics.Debug.WriteLine($"✅ Exercício {parametro.NomeExercicio} salvo");
                    }
                }

                // ✅ FEEDBACK FINAL
                if (exerciciosSalvos > 0)
                {
                    await DisplayAlert("Sucesso", $"Treino '{tituloTreino}' salvo com {exerciciosSalvos} exercícios!", "OK");

                    // ✅ LIMPAR DADOS E VOLTAR
                    _exerciciosSelecionados.Clear();
                    _parametrosTemporarios.Clear();
                    await Navigation.PushAsync(new Visualizar_TreinoPage1());
                }
                else
                {
                    await DisplayAlert("Erro", "Treino criado, mas nenhum exercício foi salvo.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao salvar treino: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"❌ Erro completo: {ex}");
            }
        }

        // ✅ MÉTODO AUXILIAR: Obter ID do treino recém-criado
        private async Task<int> ObterTreinoIdCriado(string tituloTreino, int usuarioId)
        {
            try
            {
                // Buscar o treino mais recente com este título e usuário
                using (var conn = new MySql.Data.MySqlClient.MySqlConnection(Services.Database.DatabaseConfig.ConnectionString))
                {
                    await conn.OpenAsync();

                    string query = @"
                        SELECT idTreino 
                        FROM Treino 
                        WHERE tituloTreino = @TituloTreino 
                        AND usuario_Treino = @UsuarioId 
                        ORDER BY idTreino DESC 
                        LIMIT 1";

                    using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TituloTreino", tituloTreino);
                        cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                        var result = await cmd.ExecuteScalarAsync();
                        return result != null ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao obter ID do treino: {ex.Message}");
                return 0;
            }
        }

        private void SearchBar_TextChanged(object? sender, TextChangedEventArgs e)
        {
            try
            {
                if (_todosExercicios == null) return;

                string textoBusca = e.NewTextValue?.Trim().ToLowerInvariant() ?? "";

                if (string.IsNullOrWhiteSpace(textoBusca))
                {
                    AtualizarListaExercicios();
                }
                else
                {
                    var exerciciosFiltrados = _todosExercicios
                        .Where(ex => ex.NomeExer.ToLowerInvariant().Contains(textoBusca))
                        .ToList();

                    CriarCardsExercicios(exerciciosFiltrados);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na busca: {ex.Message}");
            }
        }

        // ===== MENU HAMBURGUER =====
        private void OnMenuClicked(object sender, EventArgs e)
        {
            bool exibir = !MenuLateral.IsVisible;
            MenuLateral.IsVisible = exibir;
            OverlayFundo.IsVisible = exibir;
        }

        //Fecha o menu hamburguer se o usuário clicar fora
        private void OnOverlayTapped(object sender, EventArgs e)
        {
            MenuLateral.IsVisible = false;
            OverlayFundo.IsVisible = false;
        }

        private async void Home_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Home", "Você clicou em Home", "OK");
            MenuLateral.IsVisible = false;
        }

        private async void Sobre_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Sobre", "Você clicou em Sobre", "OK");
            MenuLateral.IsVisible = false;
        }

        private async void Contato_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Contato", "Você clicou em Contato", "OK");
            MenuLateral.IsVisible = false;
        }
        private async void Logout_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Logout", "", "OK");
            MenuLateral.IsVisible = false;
        }

        private async void MenuButton_Clicked(object? sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}