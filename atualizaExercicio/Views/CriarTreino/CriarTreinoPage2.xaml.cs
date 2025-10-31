using atualizaExercicio.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace atualizaExercicio.Views.CriarTreino
{
    public partial class CriarTreinoPage2 : ContentPage
    {
        private readonly Exercicio _exercicio;
        private readonly List<Exercicio> _exerciciosSelecionados;
        private readonly Action<List<ExercicioParametros>> _onParametrosSalvos; // ✅ CALLBACK
        private readonly List<ExercicioParametros> _parametrosSalvos;

        // ✅ CONSTRUTOR ATUALIZADO: Recebe callback
        public CriarTreinoPage2(Exercicio exercicio, List<Exercicio> exerciciosSelecionados, Action<List<ExercicioParametros>> onParametrosSalvos)
        {
            InitializeComponent();

            _exercicio = exercicio;
            _exerciciosSelecionados = exerciciosSelecionados;
            _onParametrosSalvos = onParametrosSalvos; // ✅ GUARDA O CALLBACK
            _parametrosSalvos = new List<ExercicioParametros>();

            CarregarDadosExercicio();
        }

        private void CarregarDadosExercicio()
        {
            // ✅ PREENCHER NOME DO EXERCÍCIO
            var nomeExercicioLabel = this.FindByName<Label>("NomeExercicioLabel");
            if (nomeExercicioLabel != null)
            {
                nomeExercicioLabel.Text = _exercicio.NomeExer;
            }

            // ✅ PREENCHER IMAGEM DO EXERCÍCIO (se tiver)
            var imagemExercicio = this.FindByName<Image>("ImagemExercicio");
            if (imagemExercicio != null)
            {
                if (!string.IsNullOrEmpty(_exercicio.Imagem))
                {
                    imagemExercicio.Source = ImageSource.FromUri(new Uri(_exercicio.Imagem));
                    imagemExercicio.Aspect = Aspect.AspectFill;
                }
                else
                {
                    imagemExercicio.Source = "placeholder_exercicio.png";
                    imagemExercicio.Aspect = Aspect.AspectFill;
                }
            }
        }

        // ✅ BOTÃO SALVAR EXERCÍCIO (ATUALIZADO)
        private async void SalvarExercicio_Clicked(object sender, EventArgs e)
        {
            try
            {
                // ✅ CAPTURAR PARÂMETROS DOS CAMPOS
                var seriesEntry = this.FindByName<Entry>("SeriesEntry");
                var repsEntry = this.FindByName<Entry>("RepsEntry");
                var intervaloPicker = this.FindByName<Picker>("IntervaloPicker");

                if (seriesEntry == null || repsEntry == null || intervaloPicker == null)
                {
                    await DisplayAlert("Erro", "Campos não encontrados", "OK");
                    return;
                }

                // ✅ CRIAR OBJETO DE PARÂMETROS
                var parametros = new ExercicioParametros
                {
                    ExercicioId = _exercicio.IdExercicio,
                    NomeExercicio = _exercicio.NomeExer,
                    Serie = int.TryParse(seriesEntry.Text, out int series) ? series : 3,
                    Reps = int.TryParse(repsEntry.Text, out int reps) ? reps : 12,
                    Intervalo = ConverterIntervalo(intervaloPicker.SelectedItem?.ToString())
                };

                // ✅ SALVAR PARÂMETROS TEMPORARIAMENTE
                _parametrosSalvos.Add(parametros);

                // ✅ CHAMAR CALLBACK PARA VOLTAR DADOS PARA PAGE1
                _onParametrosSalvos?.Invoke(_parametrosSalvos);

                await DisplayAlert("Sucesso", $"Parâmetros salvos para {_exercicio.NomeExer}", "OK");

                // ✅ VOLTAR PARA LISTA
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao salvar exercício: {ex.Message}", "OK");
            }
        }

        private TimeSpan ConverterIntervalo(string intervalo)
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

        // ✅ BOTÃO MENU (VOLTAR)
        private async void MenuButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}