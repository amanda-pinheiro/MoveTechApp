using System.Net.NetworkInformation;
using atualizaExercicio.Services;
using atualizaExercicio.Models;
using atualizaExercicio.Services.Database;
namespace atualizaExercicio.Views.Cadastro;

public partial class CadastroPage3 : ContentPage
{
    private CadastroValidationService _validationService;

    public CadastroPage3()
    {
        InitializeComponent();
        _validationService = new CadastroValidationService();
    }

    private async void VoltarButton3_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", "Não foi possível voltar.", "OK");
            System.Diagnostics.Debug.WriteLine($"Erro ao voltar: {ex.Message}");
        }
    }

    private async void ContinuarButton3_Clicked(object sender, EventArgs e)
    {
        try
        {
            ContinuarButton3.IsEnabled = false;
            ContinuarButton3.Text = "Salvando...";

            // Validar senha
            var senhaResult = _validationService.ValidateSenha(SenhaEntry.Text);
            if (!senhaResult.IsValid)
            {
                await DisplayAlert("Erro", senhaResult.Message, "OK");
                SenhaEntry.Focus();
                return;
            }

            // Validar confirmação de senha
            var confSenhaResult = _validationService.ValidateConfirmacaoSenha(
                SenhaEntry.Text,
                ConfSenhaEntry.Text);
            if (!confSenhaResult.IsValid)
            {
                await DisplayAlert("Erro", confSenhaResult.Message, "OK");
                ConfSenhaEntry.Focus();
                return;
            }

            // Salvar senha no UserRegistrationService
            var registrationService = new UserRegistrationService();
            var userData = registrationService.GetRegistrationData();
            userData.Senha = SenhaEntry.Text;

            // NOVO: Salvar no MySQL
            var mysqlService = new MySqlUserService();
            var resultado = await mysqlService.CriarUsuarioAsync(userData);

            if (resultado.IsValid)
            {
                // Limpar dados temporários
                registrationService.ClearRegistration();

                // Vai para a tela de cadastro 4
                await Navigation.PushAsync(new CadastroPage4());
            }
            else
            {
                await DisplayAlert("Erro", resultado.Message, "OK");
            }

        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro: {ex.Message}", "OK");
            System.Diagnostics.Debug.WriteLine($"Erro completo: {ex}");
        }
        finally
        {
            ContinuarButton3.IsEnabled = true;
            ContinuarButton3.Text = "Continuar";
        }
    }

    private void ToggleSenhaButton_Clicked(object sender, EventArgs e)
    {
        // Alternar visibilidade
        SenhaEntry.IsPassword = !SenhaEntry.IsPassword;

        // Trocar ícone: olho fechado quando IsPassword = true, olho aberto quando false
        ToggleSenhaButton.Source = SenhaEntry.IsPassword ? "eye_slash_icon.png" : "eye_icon.png";
    }

    private void ToggleConfSenhaButton_Clicked(object sender, EventArgs e)
    {
        // Alternar visibilidade
        ConfSenhaEntry.IsPassword = !ConfSenhaEntry.IsPassword;

        // Trocar ícone
        ToggleConfSenhaButton.Source = ConfSenhaEntry.IsPassword ? "eye_slash_icon.png" : "eye_icon.png";
    }
}