using atualizaExercicio.Controls;
using atualizaExercicio.Models;

namespace atualizaExercicio.Views.VisualizarTreino;

public partial class Visualizar_TreinoPage3 : ContentPage
{
    private readonly ExercicioTreinoViewModel _exercicio;

    // ✅ Construtor recebe apenas o exercício (simplificado)
    public Visualizar_TreinoPage3(ExercicioTreinoViewModel exercicio)
    {
        InitializeComponent();
        // Passa a referência da página para o ContentView
        menuHamburguer.ParentPage = this;

        _exercicio = exercicio;

        // ✅ Configurar UI
        CarregarExercicio();
    }

    private void CarregarExercicio()
    {
        try
        {
            // ✅ Atualizar labels
            NomeExercicioLabel.Text = _exercicio.NomeExercicio;
            

            // ✅ Carregar GIF/imagem
            CarregarGif();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erro ao carregar exercício: {ex.Message}");
        }
    }

    // ✅ Carrega GIF com WebView
    private void CarregarGif()
    {
        if (!string.IsNullOrEmpty(_exercicio.ImagemExercicio))
        {
            string imagemUrl = _exercicio.ImagemExercicio;

            // ✅ Se for placeholder local, não usa WebView
            if (imagemUrl == "placeholder_exercicio.png" || !imagemUrl.StartsWith("http"))
            {
                // ✅ Futuro: Podemos mostrar uma imagem local aqui
                System.Diagnostics.Debug.WriteLine($"ℹ️ Imagem local: {imagemUrl}");
                return;
            }

            // ✅ HTML para garantir que o GIF seja mostrado corretamente
            string html = $@" 
                    <html>
                      <body style='margin:0;padding:0;display:flex;justify-content:center;align-items:center;background:transparent;'>
                        <img src='{imagemUrl}' style='width:100%;height:auto;object-fit:contain;' />
                      </body>
                    </html>";

            GifViewer.Source = new HtmlWebViewSource { Html = html };

            System.Diagnostics.Debug.WriteLine($"✅ GIF carregado: {imagemUrl}");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("ℹ️ Exercício sem imagem");
        }
    }

    private async void Voltar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
