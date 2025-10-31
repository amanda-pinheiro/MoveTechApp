using atualizaExercicio.Services;
using atualizaExercicio.Services.Database;
namespace atualizaExercicio.Views.Cadastro;


public partial class CadastroPage1 : ContentPage
{
    // Criando uma instância do service (classe - validação geral para emails)
    private EmailValidationService _emailService;
    private UserRegistrationService _registrationService;

    public CadastroPage1()
    {
        InitializeComponent();
        _emailService = new EmailValidationService();
        _registrationService = new UserRegistrationService();

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

    private async void ContinuarButton1_Clicked(object sender, EventArgs e)
    {
        try
        {
            ContinuarButton1.IsEnabled = false;
            ContinuarButton1.Text = "Validando...";

            // 1. Pegar o email digitado
            string email = EmailEntry.Text?.Trim();

            // 2. ÚNICA validação necessária (faz tudo)
            string errorMessage = await _emailService.ValidateEmailCompleteAsync(email);

            // 3. Verificar se há erro
            if (!string.IsNullOrEmpty(errorMessage))
            {
                await DisplayAlert("Aviso", errorMessage, "OK");
                EmailEntry.Focus();
                return;
            }

            _registrationService.SaveEmail(email);


            // 4. Navegar para próxima tela (quando criar)
            await Navigation.PushAsync(new CadastroPage2());

        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", "Ocorreu um erro. Tente novamente.", "OK");
            System.Diagnostics.Debug.WriteLine($"Erro: {ex.Message}");
        }
        finally
        {
            ContinuarButton1.IsEnabled = true;
            ContinuarButton1.Text = "Continuar";
        }
    }

    // TESTE TEMPORÁRIO - Remover depois
    private async void TestDatabaseConnection()
    {
        try
        {
            var dbService = new MySqlDatabaseService();
            bool conectado = await dbService.TestConnectionAsync();

            if (conectado)
            {
                await DisplayAlert("✅ Sucesso", "Conectado ao MySQL com sucesso!", "OK");
            }
            else
            {
                await DisplayAlert("❌ Erro", "Falha ao conectar com MySQL. Verifique a string de conexão.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("❌ Erro", $"Erro ao conectar: {ex.Message}", "OK");
            System.Diagnostics.Debug.WriteLine($"Erro completo: {ex}");
        }
    }
}