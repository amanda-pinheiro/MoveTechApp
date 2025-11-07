using atualizaExercicio.Models;
using atualizaExercicio.Services;
using atualizaExercicio.Views.CriarTreino;
using atualizaExercicio.Controls;
using System.Collections.Generic;
using System.Linq;

namespace atualizaExercicio.Views.VisualizarTreino
{
    public partial class Visualizar_TreinoPage1 : ContentPage
    {
        private readonly ITreinoService _treinoService;
        private List<TreinoCardViewModel> _todosTreinos;
        private List<TreinoCardViewModel> _treinosFiltrados;

        public Visualizar_TreinoPage1()
        {
            InitializeComponent();

            // Passa a referência da página para o ContentView
            menuHamburguer.ParentPage = this;

            _treinoService = new MySqlTreinoService();
            _todosTreinos = new List<TreinoCardViewModel>();
            _treinosFiltrados = new List<TreinoCardViewModel>();

            CarregarTreinos();
        }

        private async void CarregarTreinos()
        {
            try
            {
                // ✅ OBTER ID DO USUÁRIO LOGADO
                var usuarioIdStr = await SecureStorage.GetAsync("usuario_id");
                if (string.IsNullOrEmpty(usuarioIdStr) || !int.TryParse(usuarioIdStr, out int usuarioId))
                {
                    await DisplayAlert("Erro", "Usuário não identificado. Faça login novamente.", "OK");
                    return;
                }

                // ✅ BUSCAR TREINOS DO BD
                _todosTreinos = await _treinoService.BuscarTreinosCardAsync(usuarioId);
                _treinosFiltrados = new List<TreinoCardViewModel>(_todosTreinos);

                // ✅ ATUALIZAR UI
                AtualizarListaTreinos();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao carregar treinos: {ex.Message}", "OK");
            }
        }

        private void AtualizarListaTreinos()
        {
            // ✅ DEBUG
            System.Diagnostics.Debug.WriteLine($"🎯 Atualizando UI: {_treinosFiltrados.Count} treinos");

            // ✅ ENCONTRAR O CONTAINER
            var container = this.FindByName<VerticalStackLayout>("TreinosContainer");

            if (container == null)
            {
                System.Diagnostics.Debug.WriteLine("❌ Container 'TreinosContainer' não encontrado!");
                return;
            }

            // ✅ LIMPAR TODOS OS CARDS EXISTENTES
            container.Children.Clear();
            System.Diagnostics.Debug.WriteLine("✅ Container limpo");

            // ✅ CRIAR CARDS DINÂMICOS
            int cardsCriados = 0;
            foreach (var treino in _treinosFiltrados)
            {
                var cardFrame = CriarCardTreino(treino);
                container.Children.Add(cardFrame);
                cardsCriados++;

                System.Diagnostics.Debug.WriteLine($"✅ Card criado: {treino.Titulo}");
            }

            System.Diagnostics.Debug.WriteLine($"🎯 Total: {cardsCriados} cards criados");

            // ✅ SE NÃO TEM TREINOS, MOSTRA MENSAGEM
            if (cardsCriados == 0)
            {
                var mensagemLabel = new Label
                {
                    Text = "Nenhum treino encontrado. Clique em '+' para criar um!",
                    TextColor = Colors.White,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 50, 0, 0)
                };
                container.Children.Add(mensagemLabel);
            }
        }

        private Frame CriarCardTreino(TreinoCardViewModel treino)
        {
            System.Diagnostics.Debug.WriteLine($"🎯 Criando card para: {treino.Titulo}");

            var frame = new Frame
            {
                CornerRadius = 12,
                Padding = 12,
                HasShadow = false,
                BackgroundColor = Color.FromArgb("#4C795CB3"),
                Margin = new Thickness(0, 0, 0, 12)
            };

            var grid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = new GridLength(120) }
                },
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                },
                ColumnSpacing = 10
            };

            // ✅ COLUNA ESQUERDA: INFORMAÇÕES
            var infoStack = new StackLayout { Spacing = 6 };

            var tituloLabel = new Label
            {
                Text = $"Treino: {treino.Titulo}",
                FontSize = 14,
                TextColor = Colors.White,
                FontFamily = "QuicksandRegular",
                Margin = new Thickness(10, 0)
            };

            var dataLabel = new Label
            {
                Text = $"Data: {treino.DataRegistro}",
                FontSize = 14,
                TextColor = Colors.White,
                FontFamily = "QuicksandLight",
                Margin = new Thickness(10, 0)
            };

            infoStack.Children.Add(tituloLabel);
            infoStack.Children.Add(dataLabel);

            grid.Add(infoStack, 0, 0);

            // ✅ COLUNA DIREITA: GRUPO MUSCULAR E IMAGEM
            var direitaStack = new VerticalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Spacing = 6
            };

            var grupoLabel1 = new Label
            {
                Text = "Grupo Muscular",
                FontSize = 12,
                TextColor = Colors.White,
                FontFamily = "QuicksandLight",
                HorizontalTextAlignment = TextAlignment.Center
            };

            var grupoLabel2 = new Label
            {
                Text = treino.GrupoMuscularPrincipal,
                FontSize = 12,
                TextColor = Colors.White,
                FontFamily = "QuicksandRegular",
                HorizontalTextAlignment = TextAlignment.Center
            };

            var imagemFrame = new Frame
            {
                WidthRequest = 84,
                HeightRequest = 84,
                CornerRadius = 42,
                Padding = 6,
                HasShadow = false,
                BackgroundColor = Colors.White
            };

            var imagem = new Image
            {
                Source = string.IsNullOrEmpty(treino.ImagemPrimeiroExercicio)
                    ? "placeholder_exercicio.png"
                    : treino.ImagemPrimeiroExercicio,
                Aspect = Aspect.AspectFit
            };

            imagemFrame.Content = imagem;

            direitaStack.Children.Add(grupoLabel1);
            direitaStack.Children.Add(grupoLabel2);
            direitaStack.Children.Add(imagemFrame);

            grid.Add(direitaStack, 1, 0);

            // ✅ BOTÃO INICIAR
            var botaoIniciar = new Button
            {
                Text = treino.TemRegistro ? "Continuar" : "Iniciar",
                CornerRadius = 20,
                HeightRequest = 44,
                Padding = new Thickness(10, 0),
                BackgroundColor = Color.FromArgb("#4E2CA0"),
                TextColor = Colors.White,
                FontFamily = "QuicksandSemibold",
                FontSize = 14,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.End,
                Margin = new Thickness(0, 10, 0, 0)
            };

            botaoIniciar.Clicked += (s, e) => BotaoIniciar_Clicked(treino);

            grid.Add(botaoIniciar, 0, 3);
            Grid.SetColumnSpan(botaoIniciar, 2);

            frame.Content = grid;
            return frame;
        }

        private async void BotaoIniciar_Clicked(TreinoCardViewModel treino)
        {
            try
            {
                // ✅ Navegar para tela de detalhes do treino
                await Navigation.PushAsync(new Visualizar_TreinoPage2(treino.TreinoId));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao abrir treino: {ex.Message}", "OK");
            }
        }

        // ✅ BUSCA/FILTRO
        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string textoBusca = e.NewTextValue?.Trim().ToLowerInvariant() ?? "";

                if (string.IsNullOrWhiteSpace(textoBusca))
                {
                    _treinosFiltrados = new List<TreinoCardViewModel>(_todosTreinos);
                }
                else
                {
                    _treinosFiltrados = _todosTreinos
                        .Where(t => t.Titulo.ToLowerInvariant().Contains(textoBusca) ||
                                   t.GrupoMuscularPrincipal.ToLowerInvariant().Contains(textoBusca))
                        .ToList();
                }

                AtualizarListaTreinos();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na busca: {ex.Message}");
            }
        }
                
        private async void btn_NovoTreino(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new CriarTreinoPage1());
            }
            catch
            {
                await DisplayAlert("Ops!", "Não foi possível voltar. Tente novamente", "Ok");
            }
        }
    }
}