using atualizaExercicio.Models;
using atualizaExercicio.Services;
using System.Collections.ObjectModel;

namespace atualizaExercicio.Views.VisualizarTreino
{
    public partial class Visualizar_TreinoPage2 : ContentPage
    {
        private readonly ITreinoService _treinoService;
        private int _treinoId;
        private ObservableCollection<ExercicioTreinoViewModel> _exercicios;

        public Visualizar_TreinoPage2(int treinoId)
        {
            InitializeComponent();

            _treinoService = new MySqlTreinoService();
            _treinoId = treinoId;
            _exercicios = new ObservableCollection<ExercicioTreinoViewModel>();

            CarregarDetalhesTreino();
        }

        private async void CarregarDetalhesTreino()
        {
            try
            {
                var detalhesTreino = await _treinoService.BuscarDetalhesTreinoAsync(_treinoId);

                if (detalhesTreino != null && detalhesTreino.Exercicios.Any())
                {
                    // ✅ Atualizar título
                    TituloTreinoLabel.Text = detalhesTreino.TituloTreino;

                    // ✅ Preencher CollectionView
                    _exercicios.Clear();
                    foreach (var exercicio in detalhesTreino.Exercicios)
                    {
                        _exercicios.Add(exercicio);
                    }

                    ExerciciosCollectionView.ItemsSource = _exercicios;

                    System.Diagnostics.Debug.WriteLine($"✅ Tela carregada: {detalhesTreino.Exercicios.Count} exercícios");
                }
                else
                {
                    await DisplayAlert("Aviso", "Treino não encontrado ou sem exercícios.", "OK");
                    await Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao carregar treino: {ex.Message}", "OK");
                await Navigation.PopAsync();
            }
        }

        // ✅ EVENTOS DO MENU EXERCÍCIO
        private async void MenuExercicio_Clicked(object sender, EventArgs e)
        {
            var button = (ImageButton)sender;
            var exercicio = (ExercicioTreinoViewModel)button.BindingContext;

            if (exercicio == null) return;

            // ✅ Menu de ações para o exercício
            string action = await DisplayActionSheet(
                $"Ações: {exercicio.NomeExercicio}",
                "Cancelar",
                null,
                "Visualizar Imagem",
                "Editar Parâmetros",
                "Remover do Treino"
            );

            switch (action)
            {
                case "Visualizar Imagem":
                    await VisualizarImagemExercicio(exercicio);
                    break;
                case "Editar Parâmetros":
                    await EditarParametrosExercicio(exercicio);
                    break;
                case "Remover do Treino":
                    await RemoverExercicioTreino(exercicio);
                    break;
            }
        }

        private async Task VisualizarImagemExercicio(ExercicioTreinoViewModel exercicio)
        {
            try
            {
                // ✅ Navegar para tela de visualização da imagem
                await Navigation.PushAsync(new Visualizar_TreinoPage3(exercicio));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao abrir imagem: {ex.Message}", "OK");
            }
        }

        private async Task EditarParametrosExercicio(ExercicioTreinoViewModel exercicio)
        {
            try
            {
                // ✅ Obter usuário logado
                var usuarioIdStr = await SecureStorage.GetAsync("usuario_id");
                if (string.IsNullOrEmpty(usuarioIdStr) || !int.TryParse(usuarioIdStr, out int usuarioId))
                {
                    await DisplayAlert("Erro", "Usuário não identificado. Faça login novamente.", "OK");
                    return;
                }

                // ✅ Navegar para tela de edição de parâmetros com todos os dados necessários
                await Navigation.PushAsync(new Visualizar_TreinoPage4(
                    exercicio: exercicio,
                    usuarioId: usuarioId,
                    treinoExercicioId: exercicio.TreinoExercicioId
                ));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao editar parâmetros: {ex.Message}", "OK");
            }
        }

        private async Task RemoverExercicioTreino(ExercicioTreinoViewModel exercicio)
        {
            bool confirmado = await DisplayAlert(
                "Confirmar",
                $"Remover {exercicio.NomeExercicio} do treino?",
                "Sim", "Não"
            );

            if (confirmado)
            {
                // ✅ Futuro: Implementar remoção no BD
                _exercicios.Remove(exercicio);
                await DisplayAlert("Sucesso", $"{exercicio.NomeExercicio} removido do treino", "OK");
            }
        }

        // ✅ EVENTO CONCLUIR TREINO
        private async void ConcluirTreino_Clicked(object sender, EventArgs e)
        {
            try
            {
                // ✅ OBTER USUÁRIO LOGADO
                var usuarioIdStr = await SecureStorage.GetAsync("usuario_id");
                if (string.IsNullOrEmpty(usuarioIdStr) || !int.TryParse(usuarioIdStr, out int usuarioId))
                {
                    await DisplayAlert("Erro", "Usuário não identificado.", "OK");
                    return;
                }

                // ✅ CALCULAR CARGA TOTAL (soma das cargas dos exercícios)
                double cargaTotal = _exercicios.Sum(ex => ex.Carga);

                // ✅ CRIAR REGISTRO
                var registroData = new RegistroTreinoData
                {
                    TreinoId = _treinoId,
                    UsuarioId = usuarioId,
                    Data = DateTime.Now,
                    CargaTotal = cargaTotal
                };

                // ✅ SALVAR NO BD
                bool salvou = await _treinoService.RegistrarTreinoConcluidoAsync(registroData);

                if (salvou)
                {
                    await DisplayAlert("Sucesso", "Treino concluído e registrado com sucesso!", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Erro", "Falha ao registrar treino.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao concluir treino: {ex.Message}", "OK");
            }
        }

        // ✅ EVENTOS DO MENU LATERAL (padrão do projeto)
        private void MenuButton_Clicked(object sender, EventArgs e)
        {
            MenuLateral.IsVisible = true;
            OverlayFundo.IsVisible = true;
        }

        private void OverlayFundo_Tapped(object sender, TappedEventArgs e)
        {
            MenuLateral.IsVisible = false;
            OverlayFundo.IsVisible = false;
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

        private string GarantirImagemValida(string imagemOriginal)
        {
            if (string.IsNullOrEmpty(imagemOriginal) ||
                imagemOriginal == "placeholder_exercicio.png" ||
                !Uri.IsWellFormedUriString(imagemOriginal, UriKind.Absolute))
            {
                return "placeholder_exercicio.png";
            }

            return imagemOriginal;
        }
    }
}