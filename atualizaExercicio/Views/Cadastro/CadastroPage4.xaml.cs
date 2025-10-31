namespace atualizaExercicio.Views.Cadastro;

public partial class CadastroPage4 : ContentPage
{
    public CadastroPage4()
    {
        InitializeComponent();
    }

    private async void VoltarButton4_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new MainPage());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", "Não foi possível voltar.", "OK");
            System.Diagnostics.Debug.WriteLine($"Erro ao voltar: {ex.Message}");
        }
    }

    private async void ComecarButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new Home());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", "Não foi possível voltar.", "OK");
            System.Diagnostics.Debug.WriteLine($"Erro ao voltar: {ex.Message}");
        }

    }
}