using atualizaExercicio.Models;
using atualizaExercicio.Services;
using atualizaExercicio.Views;
using System;

namespace atualizaExercicio.Views.VisualizarTreino
{
    public partial class Visualizar_TreinoPage4 : ContentPage
    {
        private readonly ITreinoService _treinoService;
        private readonly ExercicioTreinoViewModel _exercicioAtual;
        private readonly int _usuarioId;
        private readonly int _treinoExercicioId;

        public Visualizar_TreinoPage4(ExercicioTreinoViewModel exercicio, int usuarioId, int treinoExercicioId)
        {
            InitializeComponent();

            _treinoService = new MySqlTreinoService();
            _exercicioAtual = exercicio;
            _usuarioId = usuarioId;
            _treinoExercicioId = treinoExercicioId;

            CarregarDadosExercicio();
            CarregarHistorico();
        }

        private void CarregarDadosExercicio()
        {
            try
            {
                // Preencher dados atuais
                ExercicioNomeLabel.Text = _exercicioAtual.NomeExercicio;

                if (!string.IsNullOrEmpty(_exercicioAtual.ImagemExercicio) &&
                    _exercicioAtual.ImagemExercicio != "placeholder_exercicio.png")
                {
                    ExercicioImagem.Source = _exercicioAtual.ImagemExercicio;
                }

                SeriesAtualEntry.Text = _exercicioAtual.Serie.ToString();
                RepsAtualEntry.Text = _exercicioAtual.Reps.ToString();
                CargaAtualEntry.Text = _exercicioAtual.Carga > 0 ? _exercicioAtual.Carga.ToString() : "";

                ConfigurarIntervaloPicker(_exercicioAtual.Intervalo);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao carregar dados do exercício: {ex.Message}");
            }
        }

        private async void CarregarHistorico()
        {
            try
            {
                // ✅ O "histórico anterior" são os valores ATUAIS salvos no BD
                // que estão no _exercicioAtual (que veio do banco)

                SeriesAnteriorLabel.Text = _exercicioAtual.Serie.ToString();
                RepsAnteriorLabel.Text = _exercicioAtual.Reps.ToString();
                CargaAnteriorLabel.Text = _exercicioAtual.Carga > 0 ? _exercicioAtual.Carga.ToString() : "0";
                IntervaloAnteriorLabel.Text = ConverterIntervaloParaTexto(_exercicioAtual.Intervalo);

                System.Diagnostics.Debug.WriteLine($"✅ Histórico (valores atuais BD) carregado: {_exercicioAtual.Serie}x{_exercicioAtual.Reps}, Carga={_exercicioAtual.Carga}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao carregar histórico: {ex.Message}");

                // Fallback
                SeriesAnteriorLabel.Text = "3";
                RepsAnteriorLabel.Text = "12";
                CargaAnteriorLabel.Text = "0";
                IntervaloAnteriorLabel.Text = "1 min";
            }
        }

        private void ConfigurarIntervaloPicker(TimeSpan intervalo)
        {
            string intervaloTexto = ConverterIntervaloParaTexto(intervalo);
            var index = IntervaloAtualPicker.Items.IndexOf(intervaloTexto);
            IntervaloAtualPicker.SelectedIndex = index >= 0 ? index : 2;
        }

        private string ConverterIntervaloParaTexto(TimeSpan intervalo)
        {
            return intervalo.TotalSeconds switch
            {
                30 => "30 seg",
                60 => "1 min",
                90 => "1 min 30 seg",
                120 => "2 min",
                _ => "1 min 30 seg"
            };
        }

        private TimeSpan ConverterTextoParaIntervalo(string intervalo)
        {
            return intervalo switch
            {
                "30 seg" => TimeSpan.FromSeconds(30),
                "1 min" => TimeSpan.FromMinutes(1),
                "1 min 30 seg" => TimeSpan.FromSeconds(90),
                "2 min" => TimeSpan.FromMinutes(2),
                _ => TimeSpan.FromSeconds(60)
            };
        }

        private async void AtualizarButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(SeriesAtualEntry.Text, out int series) || series <= 0)
                {
                    await DisplayAlert("Erro", "Séries devem ser um número positivo", "OK");
                    return;
                }

                if (!int.TryParse(RepsAtualEntry.Text, out int reps) || reps <= 0)
                {
                    await DisplayAlert("Erro", "Repetições devem ser um número positivo", "OK");
                    return;
                }

                var parametrosAtualizados = new ExercicioParametros
                {
                    TreinoExercicioId = _treinoExercicioId,
                    ExercicioId = _exercicioAtual.ExercicioId,
                    NomeExercicio = _exercicioAtual.NomeExercicio,
                    Serie = series,
                    Reps = reps,
                    Carga = double.TryParse(CargaAtualEntry.Text, out double carga) ? carga : 0,
                    Intervalo = ConverterTextoParaIntervalo(IntervaloAtualPicker.SelectedItem?.ToString())
                };

                bool atualizou = await _treinoService.AtualizarExercicioTreinoAsync(parametrosAtualizados);

                if (atualizou)
                {
                    await DisplayAlert("Sucesso", $"Parâmetros de {_exercicioAtual.NomeExercicio} atualizados!", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Erro", "Falha ao atualizar parâmetros", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao atualizar exercício: {ex.Message}", "OK");
            }
        }

        // Eventos do menu lateral
        private void MenuButton_Clicked(object sender, EventArgs e)
        {
            MenuLateral.IsVisible = true;
            OverlayFundo.IsVisible = true;
        }

        private void OverlayFundo_Tapped(object sender, EventArgs e)
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
    }
}