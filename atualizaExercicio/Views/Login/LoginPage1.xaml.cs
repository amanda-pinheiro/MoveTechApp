using atualizaExercicio.Services;
using atualizaExercicio.Services.Database;
using MySql.Data.MySqlClient;
using System.Runtime.CompilerServices;

namespace atualizaExercicio.Views.Login;


public partial class LoginPage1 : ContentPage
{
    private readonly MySqlDatabaseService _databaseService;
    public LoginPage1()
    {
        InitializeComponent();
        _databaseService = new MySqlDatabaseService();
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

            string email = EmailEntry.Text?.Trim();

            if (string.IsNullOrEmpty(email))
            {
                await DisplayAlert("Aviso", "Por favor, digite seu e-mail.", "OK");
                return;
            }

            // 🔍 Verifica se o e-mail existe no banco
            bool emailExiste = await _databaseService.EmailExistsAsync(email);

            if (emailExiste)
            {
                // E-mail existe → vai para próxima tela (LoginPage2)
                await Navigation.PushAsync(new LoginPage2(email));
            }
            else
            {
                await DisplayAlert("Aviso", "E-mail não encontrado. Verifique e tente novamente.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", "Ocorreu um erro ao validar o e-mail.", "OK");
            System.Diagnostics.Debug.WriteLine($"Erro: {ex.Message}");
        }
        finally
        {
            ProximoButton1.IsEnabled = true;
            ProximoButton1.Text = "Prosseguir";
        }
    }

}