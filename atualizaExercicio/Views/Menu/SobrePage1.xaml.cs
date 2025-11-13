namespace atualizaExercicio.Views.Menu;


public partial class SobrePage1 : ContentPage
{
	public SobrePage1()
	{
		InitializeComponent();
	}

    private async void VoltarButton1_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PopAsync(); // PopAsync em vez de PushAsync
        }
        catch
        {
            await DisplayAlert("Ops!", "Não foi possível voltar. Tente novamente", "Ok");
        }
    }





}