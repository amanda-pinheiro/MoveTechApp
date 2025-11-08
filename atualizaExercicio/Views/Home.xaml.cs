using atualizaExercicio.Views.CriarTreino;
using atualizaExercicio.Views.VisualizarTreino;
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

    // ===== MENU HAMBURGUER =====
    private void OnMenuClicked(object sender, EventArgs e)
    {
        bool exibir = !MenuLateral.IsVisible;
        MenuLateral.IsVisible = exibir;
        OverlayFundo.IsVisible = exibir;
    }

    //Fecha o menu hamburguer se o usuário clicar fora
    private void OnOverlayTapped(object sender, EventArgs e)
    {
        MenuLateral.IsVisible = false;
        OverlayFundo.IsVisible = false;
    }

    private async void Sobre_Clicked(object sender, EventArgs e)
    {

        try
        { 
            await Navigation.PushAsync(new Menu.SobrePage1());
        }
        catch
        {
            await DisplayAlert("Ops!", "Não foi possível voltar. Tente novamente", "Ok");
        }


    }

    private async void Contato_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new Menu.ContatoPage1());
        }
        catch
        {
            await DisplayAlert("Ops!", "Não foi possível voltar. Tente novamente", "Ok");
        }
    }
    private async void Logout_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("Logout", "", "OK");
        MenuLateral.IsVisible = false;
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