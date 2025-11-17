/* using atualizaExercicio.Controls;
using atualizaExercicio.Views.CriarTreino;
using atualizaExercicio.Views.VisualizarTreino;
namespace atualizaExercicio.Views;

public partial class Home : ContentPage
{
	public Home()
	{
		InitializeComponent();
        // Passa a referência da página para o ContentView
        menuHamburguer.ParentPage = this;
    }

    private async void novoTreino_Clicked(object sender, EventArgs e)
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

    private async void Button_Clicked_VerTreinos(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new Visualizar_TreinoPage1());
        }
        catch
        {
            await DisplayAlert("Ops!", "Não foi possível voltar. Tente novamente", "Ok");

        }
    }
} */

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using atualizaExercicio.Views.CriarTreino;
using atualizaExercicio.Views.VisualizarTreino;
using atualizaExercicio.Services.Database;
using atualizaExercicio.Models;
using System.Collections.Generic;
using atualizaExercicio.Services;
using Microsoft.Maui.ApplicationModel;

namespace atualizaExercicio.Views
{
    public partial class Home : ContentPage
    {
        //private readonly ExercicioService _exercicioService;
        private readonly ITreinoService _treinoService;
        private CancellationTokenSource? _searchCts;

        public ObservableCollection<Exercicio> SearchResults { get; } = new();
        private ObservableCollection<TreinoCardViewModel> _treinosEncontrados = new();
        private TreinoCardViewModel? _proximoTreino;

        public Home()
        {
            InitializeComponent();
            //_exercicioService = new ExercicioService();
            _treinoService = new MySqlTreinoService();

            // Vincula ItemsSource da CollectionView (se presente)
            try
            {
                var cv = this.FindByName<CollectionView>("SearchResultsView");
                if (cv != null)
                    cv.ItemsSource = _treinosEncontrados;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SearchResultsView n o encontrado: {ex.Message}");
            }

            // Carregar pr ximo treino e condicional
            _ = CarregarProximoTreinoAsync();
        }

        //public async void NovoTreinoLabel( é necessário?
            
        private async Task CarregarProximoTreinoAsync()
        {
            try
            {
                var usuarioIdStr = await SecureStorage.GetAsync("usuario_id");
                if (string.IsNullOrEmpty(usuarioIdStr) || !int.TryParse(usuarioIdStr, out int usuarioId))
                {
                    // Sem usuário logado: mostrar mensagem padrão
                   NovoTreinoLabel.IsVisible = true;
                    ProximoTreinoFrame.IsVisible = false;
                    return;
                }

                var treinos = await _treinoService.BuscarTreinosCardAsync(usuarioId);

                if (treinos == null || treinos.Count == 0)
                {
                    NovoTreinoLabel.IsVisible = true;
                    ProximoTreinoFrame.IsVisible = false;
                }
                else
                {
                    NovoTreinoLabel.IsVisible = false;
                    ProximoTreinoFrame.IsVisible = true;

                    // escolher o treino mais pr ximo (aqui pega o primeiro)
                    _proximoTreino = treinos[0];

                    NextTreinoTituloLabel.Text = $"Treino: {_proximoTreino.Titulo}";
                    NextTreinoDataLabel.Text = $"Data: {_proximoTreino.DataRegistro}";
                    NextTreinoGrupoLabel.Text = $"Grupo Muscular: {_proximoTreino.GrupoMuscularPrincipal}";

                    // associar o click do botão para abrir Visualizar_TreinoPage2 com o id
                    BtnIniciar.Clicked -= OnIniciarClicked;
                    BtnIniciar.Clicked += async (s, e) =>
                    {
                        try
                        {
                            if (_proximoTreino != null)
                                await Navigation.PushAsync(new Visualizar_TreinoPage2(_proximoTreino.TreinoId));
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Erro ao abrir treino: {ex}");
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao carregar pr ximo treino: {ex}");
                NovoTreinoLabel.IsVisible = true;
                ProximoTreinoFrame.IsVisible = false;
            }
        }

        /* ===== MENU HAMBURGUER =====
        private void OnMenuClicked(object sender, EventArgs e)
        {
            bool exibir = !MenuLateral.IsVisible;
            MenuLateral.IsVisible = exibir;
            OverlayFundo.IsVisible = exibir;
        }

        private void OnOverlayTapped(object sender, EventArgs e)
        {
            MenuLateral.IsVisible = false;
            OverlayFundo.IsVisible = false;
        }

        private async void Sobre_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Sobre", "Aplicativo MoveTech - Treinos personalizados e controle de desempenho.", "OK");
            MenuLateral.IsVisible = false;
        }

        private async void Contato_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Contato", "E-mail: suporte@movetech.com", "OK");
            MenuLateral.IsVisible = false;
        }

        private async void Logout_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Logout", "Sess o encerrada.", "OK");
            MenuLateral.IsVisible = false;
        } */

        // ===== BUSCA (Entry.TextChanged) =====
        // Agora busca treinos do usuário
        private async void ActivitySearchEntry_TextChanged(object? sender, TextChangedEventArgs e)
        {
            // cancel previous pending search
            _searchCts?.Cancel();
            _searchCts?.Dispose();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            string query = e.NewTextValue?.Trim() ?? string.Empty;

            Debug.WriteLine($"[Home] Search text changed: '{query}'");

            if (string.IsNullOrWhiteSpace(query))
            {
                MainThread.BeginInvokeOnMainThread(() => _treinosEncontrados.Clear());
                var cvEmpty = this.FindByName<CollectionView>("SearchResultsView");
                if (cvEmpty != null)
                    cvEmpty.IsVisible = false;
                return;
            }

            // debounce
            try
            {
                await Task.Delay(300, token);
            }
            catch (TaskCanceledException)
            {
                Debug.WriteLine("[Home] Search cancelled due to new input");
                return; // novo caractere
            }

            try
            {
                var usuarioIdStr = await SecureStorage.GetAsync("usuario_id");
                if (string.IsNullOrEmpty(usuarioIdStr) || !int.TryParse(usuarioIdStr, out int usuarioId))
                {
                    Debug.WriteLine("[Home] Usuário não identificado ao buscar treinos");
                    await DisplayAlert("Erro", "Usuário não identificado. Faça login.", "OK");
                    return;
                }

                var list = await _treinoService.BuscarTreinosCardAsync(usuarioId);

                Debug.WriteLine($"[Home] Treinos retornados do serviço: {list?.Count ?? 0}");

                if (token.IsCancellationRequested) return;

                MainThread.BeginInvokeOnMainThread(() => _treinosEncontrados.Clear());

                if (!string.IsNullOrWhiteSpace(query) && list != null)
                {
                    foreach (var t in list)
                    {
                        if ((t.Titulo ?? string.Empty).Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            (t.GrupoMuscularPrincipal ?? string.Empty).Contains(query, StringComparison.OrdinalIgnoreCase))
                        {
                            MainThread.BeginInvokeOnMainThread(() => _treinosEncontrados.Add(t));
                        }
                    }
                }

                Debug.WriteLine($"[Home] Treinos filtrados: {_treinosEncontrados.Count}");

                var cv = this.FindByName<CollectionView>("SearchResultsView");
                if (cv != null)
                    cv.IsVisible = _treinosEncontrados.Count > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro buscando treinos: {ex}");
                try
                {
                    await DisplayAlert("Erro", "Falha ao buscar treinos. Verifique a conex o.", "OK");
                }
                catch { }
            }
        }

        // ===== Ao selecionar um resultado de busca =====
        private async void OnSearchResultSelected(object? sender, SelectionChangedEventArgs e)
        {
            var selected = (e.CurrentSelection?.Count > 0) ? e.CurrentSelection[0] as TreinoCardViewModel : null;
            if (selected == null) return;

            try
            {
                await Navigation.PushAsync(new Visualizar_TreinoPage2(selected.TreinoId));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro navegando para detalhe: {ex}");
            }

            // limpa seleção
            if (sender is CollectionView cv)
                cv.SelectedItem = null;

            var cvHide = this.FindByName<CollectionView>("SearchResultsView");
            if (cvHide != null)
                cvHide.IsVisible = false;
        }

        // ===== BOT ES PRINCIPAIS =====

        // Mantive exatamente a a  o do projeto novo: CriarTreinoPage1
        private async void novoTreino_Clicked(object? sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new CriarTreinoPage1());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro abrindo CriarTreinoPage1: {ex}");
                await DisplayAlert("Erro", "N o foi poss vel abrir a tela Novo Treino.", "OK");
            }
        }

        // Visualizar_TreinoPage1
        private async void Button_Clicked_VerTreinos(object? sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new Visualizar_TreinoPage1());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro abrindo Visualizar_TreinoPage1: {ex}");
                await DisplayAlert("Erro", "Não foi possível abrir a tela de Treinos.", "OK");
            }
        }

        // Funções antigas preservadas
        private async void OnIniciarClicked(object? sender, EventArgs e)
        {
            try
            {
                // Caso padrão: se tiver treino definido, abrir com id
                if (_proximoTreino != null)
                {
                    await Navigation.PushAsync(new Visualizar_TreinoPage2(_proximoTreino.TreinoId));
                }
                else
                {
                    await DisplayAlert("Aviso", "Nenhum treino configurado.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro abrindo ProximoTreino: {ex}");
                await DisplayAlert("Erro", "N o foi poss vel abrir Pr ximo Treino.", "OK");
            }
        }

        private async void OnFazerDeNovoClicked(object? sender, EventArgs e)
        {
            try
            {
                // Preferir _proximoTreino se existir
                if (_proximoTreino != null)
                {
                    await Navigation.PushAsync(new Visualizar_TreinoPage2(_proximoTreino.TreinoId));
                    return;
                }

                // Caso não tenha _proximoTreino, usar o primeiro item filtrado (se houver)
                if (_treinosEncontrados != null && _treinosEncontrados.Count > 0)
                {
                    var primeiro = _treinosEncontrados[0];
                    await Navigation.PushAsync(new Visualizar_TreinoPage2(primeiro.TreinoId));
                    return;
                }

                // Nenhum treino disponível
                await DisplayAlert("Aviso", "Nenhum treino disponível para repetir.", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro abrindo RepetirTreino: {ex}");
                await DisplayAlert("Erro", "Não foi possível abrir Repetir Treino.", "OK");
            }
        }

        private async void OnSeusTreinosClicked(object? sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new Visualizar_TreinoPage1());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro abrindo SeusTreinos: {ex}");
                await DisplayAlert("Erro", "N o foi poss vel abrir Seus Treinos.", "OK");
            }
        }
    }
}