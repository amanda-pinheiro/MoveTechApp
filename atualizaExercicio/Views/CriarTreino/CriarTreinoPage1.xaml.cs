using atualizaExercicio.Models;
using atualizaExercicio.Services;
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao processar parâmetros: {ex.Message}");
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

        private async void MenuButton_Clicked(object? sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}