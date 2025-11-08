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

            CarregarTreinosInicial();
        }

        // ✅ MÉTODO RENOMEADO para evitar conflito
        private async void CarregarTreinosInicial()
        {
            try
            {
                // ✅ Obter usuário logado
                var usuarioIdStr = await SecureStorage.GetAsync("usuario_id");
                if (string.IsNullOrEmpty(usuarioIdStr) || !int.TryParse(usuarioIdStr, out int usuarioId))
                {
                    await DisplayAlert("Erro", "Usuário não identificado.", "OK");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"🔍 Buscando treinos para usuário ID: {usuarioId}");

                // ✅ Carregar treinos do service
                _todosTreinos = await _treinoService.BuscarTreinosCardAsync(usuarioId);

                System.Diagnostics.Debug.WriteLine($"📊 Treinos retornados do service: {_todosTreinos?.Count ?? 0}");

                _treinosFiltrados = new List<TreinoCardViewModel>(_todosTreinos);

                // ✅ DEBUG: Mostrar cada treino retornado
                foreach (var treino in _todosTreinos)
                {
                    System.Diagnostics.Debug.WriteLine($"🏋️ Treino: {treino.Titulo}, Data: {treino.DataRegistro}, ID: {treino.TreinoId}");
                }

                // ✅ Atualizar a UI
                AtualizarListaTreinos();

                System.Diagnostics.Debug.WriteLine($"✅ Lista de treinos carregada: {_todosTreinos.Count} treinos");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao carregar treinos: {ex.Message}");
            }
        }

        // ✅ NOVO MÉTODO para recarregar após exclusão
        private async Task CarregarTreinos()
        {
            try
            {
                // ✅ Obter usuário logado
                var usuarioIdStr = await SecureStorage.GetAsync("usuario_id");
                if (string.IsNullOrEmpty(usuarioIdStr) || !int.TryParse(usuarioIdStr, out int usuarioId))
                {
                    await DisplayAlert("Erro", "Usuário não identificado.", "OK");
                    return;
                }

                // ✅ Recarregar treinos do service
                _todosTreinos = await _treinoService.BuscarTreinosCardAsync(usuarioId);
                _treinosFiltrados = new List<TreinoCardViewModel>(_todosTreinos);

                // ✅ Atualizar a UI
                AtualizarListaTreinos();

                System.Diagnostics.Debug.WriteLine($"✅ Lista de treinos recarregada: {_todosTreinos.Count} treinos");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao recarregar treinos: {ex.Message}");
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

            // ✅ BOTÃO INICIAR E BOTÃO EXCLUIR - AGORA EM UMA GRID LATERAL
            var botoesGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                },
                ColumnSpacing = 10,
                Margin = new Thickness(0, 10, 0, 0)
            };

            var botaoIniciar = new Button
            {
                Text = treino.TemRegistro ? "Iniciar" : "Iniciar",
                CornerRadius = 20,
                HeightRequest = 44,
                Padding = new Thickness(10, 0),
                BackgroundColor = Color.FromArgb("#4E2CA0"),
                TextColor = Colors.White,
                FontFamily = "QuicksandSemibold",
                FontSize = 14,
                HorizontalOptions = LayoutOptions.Fill
            };

            botaoIniciar.Clicked += (s, e) => BotaoIniciar_Clicked(treino);

            var botaoExcluir = new ImageButton
            {
                Source = "delete_icon.png", // Você precisa ter este ícone nos recursos
                BackgroundColor = Colors.Transparent,
                HeightRequest = 44,
                WidthRequest = 44,
                CornerRadius = 8,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            // ✅ ADICIONANDO O EVENTO CLICKED PARA EXCLUIR
            botaoExcluir.Clicked += async (s, e) => await BotaoExcluir_Clicked(treino);

            // ✅ ADICIONANDO OS BOTÕES NA GRID DE BOTÕES
            botoesGrid.Add(botaoIniciar, 0, 0);
            botoesGrid.Add(botaoExcluir, 1, 0);

            // ✅ ADICIONANDO A GRID DE BOTÕES NA GRID PRINCIPAL
            grid.Add(botoesGrid, 0, 3);
            Grid.SetColumnSpan(botoesGrid, 2);

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

        private async Task BotaoExcluir_Clicked(TreinoCardViewModel treino)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🗑️ Tentando excluir treino: {treino.Titulo} (ID: {treino.TreinoId})");

                // ✅ CONFIRMAÇÃO DO USUÁRIO
                bool confirmacao = await DisplayAlert(
                    "Confirmar Exclusão",
                    $"Tem certeza que deseja excluir o treino '{treino.Titulo}'?\n\nEsta ação não pode ser desfeita.",
                    "Sim, Excluir",
                    "Cancelar"
                );

                if (!confirmacao)
                {
                    System.Diagnostics.Debug.WriteLine("❌ Exclusão cancelada pelo usuário");
                    return;
                }

                // ✅ EXCLUIR TREINO COMPLETO
                bool exclusaoSucesso = await _treinoService.ExcluirTreinoCompletoAsync(treino.TreinoId);

                if (exclusaoSucesso)
                {
                    System.Diagnostics.Debug.WriteLine($"✅ Treino excluído com sucesso - ID: {treino.TreinoId}");

                    // ✅ ATUALIZAR A UI - Recarregar a lista de treinos
                    await CarregarTreinos();

                    await DisplayAlert(
                        "Sucesso",
                        $"Treino '{treino.Titulo}' excluído com sucesso!",
                        "OK"
                    );
                }
                else
                {
                    await DisplayAlert(
                        "Erro",
                        "Não foi possível excluir o treino. Tente novamente.",
                        "OK"
                    );
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao processar exclusão: {ex.Message}");
                await DisplayAlert("Erro", "Ocorreu um erro ao tentar excluir o treino.", "OK");
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                System.Diagnostics.Debug.WriteLine("🔄 Visualizar_TreinoPage1 - OnAppearing chamado");

                // ✅ RECARREGAR os treinos sempre que a página aparecer
                await CarregarTreinos();

                System.Diagnostics.Debug.WriteLine("✅ Dados recarregados no OnAppearing");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro no OnAppearing: {ex.Message}");
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