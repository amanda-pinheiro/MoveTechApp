namespace atualizaExercicio.Views.Menu;

public partial class ContatoPage1 : ContentPage
{
	public ContatoPage1()
	{
		InitializeComponent();
	}


    private async void VoltarButton1_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new Home()); // PopAsync em vez de PushAsync
        }
        catch
        {
            await DisplayAlert("Ops!", "Não foi possível voltar. Tente novamente", "Ok");
        }
    }
}