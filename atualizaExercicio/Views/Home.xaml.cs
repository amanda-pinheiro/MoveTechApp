using atualizaExercicio.Controls;
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
}