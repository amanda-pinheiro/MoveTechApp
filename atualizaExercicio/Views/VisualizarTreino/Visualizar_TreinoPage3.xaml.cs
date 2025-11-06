using atualizaExercicio.Models;

namespace atualizaExercicio.Views.VisualizarTreino;

public partial class Visualizar_TreinoPage3 : ContentPage
{
    private readonly ExercicioTreinoViewModel _exercicio;

    // ✅ Construtor recebe apenas o exercício (simplificado)
    public Visualizar_TreinoPage3(ExercicioTreinoViewModel exercicio)
    {
        InitializeComponent();

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
            TituloLabel.Text = _exercicio.NomeExercicio;

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

    // ✅ EVENTOS DO MENU LATERAL
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

    private async void Voltar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
