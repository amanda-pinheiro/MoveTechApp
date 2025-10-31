using atualizaExercicio.Services;
using atualizaExercicio.Services.Database;
using MySql.Data.MySqlClient;
using System.Runtime.CompilerServices;

namespace atualizaExercicio.Views.Login;


public partial class LoginPage1 : ContentPage
{
    public LoginPage1()
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



    private async void ProximoButton1_Clicked(object sender, EventArgs e)
    {
        try
        {
            ProximoButton1.IsEnabled = false;
            ProximoButton1.Text = "Validando...";

            // 1. Pegar o email digitado
            string email = EmailEntry.Text?.Trim();



            // 4. Navegar para próxima tela (quando criar)
            await Navigation.PushAsync(new LoginPage2());

        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", "Ocorreu um erro. Tente novamente.", "OK");
            System.Diagnostics.Debug.WriteLine($"Erro: {ex.Message}");
        }
        finally
        {
            ProximoButton1.IsEnabled = true;
            ProximoButton1.Text = "Prosseguindo...";
        }

    }

}