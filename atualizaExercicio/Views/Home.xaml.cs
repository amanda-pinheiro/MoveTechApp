using atualizaExercicio.Views.CriarTreino;
namespace atualizaExercicio.Views;

public partial class Home : ContentPage
{
	public Home()
	{
		InitializeComponent();
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
}