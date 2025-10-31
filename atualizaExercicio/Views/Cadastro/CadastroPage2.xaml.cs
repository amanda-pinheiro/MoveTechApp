using atualizaExercicio.Models;
using atualizaExercicio.Services;
using System.ComponentModel.DataAnnotations;
namespace atualizaExercicio.Views.Cadastro;

public partial class CadastroPage2 : ContentPage
{
    private CadastroValidationService _validationService;

    private string _emailOriginal;

    public CadastroPage2(string emailOriginal = "")
    {
        //Validações necessárias para inicialização do componente
        InitializeComponent();

        _validationService = new CadastroValidationService();
        _emailOriginal = emailOriginal;

        // No construtor, se não recebeu email, buscar do service
        if (string.IsNullOrEmpty(_emailOriginal))
        {
            var registrationService = new UserRegistrationService();
            _emailOriginal = registrationService.GetEmail();
        }

        // Aplicação de máscaras
        DataNascimentoEntry.TextChanged += OnDataTextChanged;
        DataNascimentoEntry.Unfocused += OnDataUnfocused;
        TelefoneEntry.TextChanged += OnTelefoneTextChanged;
    }

    private void OnDataTextChanged(object sender, TextChangedEventArgs e)
    {
        var entry = sender as Entry;
        string text = e.NewTextValue?.Replace("/", "");

        if (string.IsNullOrEmpty(text))
        {
            entry.TextColor = Colors.Black;
            return;
        }

        // Permitir apenas números
        if (!text.All(char.IsDigit))
        {
            entry.Text = e.OldTextValue;
            return;
        }

        // Limitar a 8 dígitos (DDMMAAAA)
        if (text.Length > 8)
        {
            entry.Text = e.OldTextValue;
            return;
        }

        // Aplicar máscara DD/MM/AAAA
        string maskedText = "";
        if (text.Length <= 2)
        {
            maskedText = text;
        }
        else if (text.Length <= 4)
        {
            maskedText = $"{text.Substring(0, 2)}/{text.Substring(2)}";
        }
        else
        {
            maskedText = $"{text.Substring(0, 2)}/{text.Substring(2, 2)}/{text.Substring(4)}";
        }

        entry.Text = maskedText;
        entry.CursorPosition = maskedText.Length;
    }

    // Validar quando o usuário sair do campo
    private void OnDataUnfocused(object sender, FocusEventArgs e)
    {
        var entry = sender as Entry;
        var result = _validationService.ValidateDataNascimento(entry.Text);

        if (!result.IsValid)
        {
            entry.TextColor = Colors.Red;
        }
        else
        {
            entry.TextColor = Colors.Green;
        }
    }

    // Método para testar validação do nome (pode remover depois)
    private async void TestValidateNome()
    {
        var result = _validationService.ValidateNome(NomeEntry.Text);
        if (!result.IsValid)
        {
            await DisplayAlert("Erro", result.Message, "OK");
        }
        else
        {
            await DisplayAlert("Sucesso", "Nome válido!", "OK");
        }
    }

    //Método para validar o telefone
    private void OnTelefoneTextChanged(object sender, TextChangedEventArgs e)
    {
        var entry = sender as Entry;
        string? text = e.NewTextValue?.Replace("(", "").Replace(")", "")
                                     .Replace("-", "").Replace(".", "").Replace(" ", "");

        if (string.IsNullOrEmpty(text))
        {
            entry.TextColor = Colors.Black;
            return;
        }

        if (!text.All(char.IsDigit))
        {
            entry.Text = e.OldTextValue;
            return;
        }

        // Limitar a 11 dígitos
        if (text.Length > 11)
        {
            entry.Text = e.OldTextValue;
            return;
        }

        // Aplicar máscara (11) 9.9999-9999
        string maskedText = "";
        if (text.Length <= 2)
        {
            maskedText = $"({text}";
        }
        else if (text.Length <= 3)
        {
            maskedText = $"({text.Substring(0, 2)}) {text.Substring(2)}";
        }
        else if (text.Length <= 7)
        {
            maskedText = $"({text.Substring(0, 2)}) {text.Substring(2, 1)}.{text.Substring(3)}";
        }
        else
        {
            maskedText = $"({text.Substring(0, 2)}) {text.Substring(2, 1)}.{text.Substring(3, 4)}-{text.Substring(7)}";
        }

        entry.Text = maskedText;
        entry.CursorPosition = maskedText.Length;
    }

    private async void ContinuarButton2_Clicked(object sender, EventArgs e)
    {
        try
        {
            ContinuarButton2.IsEnabled = false;
            ContinuarButton2.Text = "Validando...";

            // Validar nome
            var nomeResult = _validationService.ValidateNome(NomeEntry.Text);
            if (!nomeResult.IsValid)
            {
                await DisplayAlert("Erro", nomeResult.Message, "OK");
                NomeEntry.Focus();
                return;
            }

            // Validar data
            var dataResult = _validationService.ValidateDataNascimento(DataNascimentoEntry.Text);
            if (!dataResult.IsValid)
            {
                await DisplayAlert("Erro", dataResult.Message, "OK");
                DataNascimentoEntry.Focus();
                return;
            }

            // Validar nome de usuário
            var usuarioResult = _validationService.ValidateNomeUsuario(UsuarioEntry.Text);
            if (!usuarioResult.IsValid)
            {
                await DisplayAlert("Erro", usuarioResult.Message, "OK");
                UsuarioEntry.Focus();
                return;
            }

            // Validar telefone
            var telefoneResult = _validationService.ValidateTelefone(TelefoneEntry.Text);
            if (!telefoneResult.IsValid)
            {
                await DisplayAlert("Erro", telefoneResult.Message, "OK");
                TelefoneEntry.Focus();
                return;
            }

            // Validar confirmação de email
            var emailConfResult = _validationService.ValidateConfirmacaoEmail(ConfEmailEntry.Text,
                _emailOriginal);
            if (!emailConfResult.IsValid)
            {
                await DisplayAlert("Erro", emailConfResult.Message, "OK");
                ConfEmailEntry.Focus();
                return;
            }

            // Se chegou aqui, todos os campos são válidos - salvar os dados
            var registrationService = new UserRegistrationService();
            var userData = registrationService.GetRegistrationData();

            // Salvar todos os dados validados
            userData.Nome = NomeEntry.Text.Trim();
            userData.DataNascimento = DataNascimentoEntry.Text;
            userData.Genero = GeneroPicker.SelectedItem.ToString();
            userData.NomeUsuario = UsuarioEntry.Text.Trim().ToLowerInvariant();
            userData.Telefone = TelefoneEntry.Text;
            userData.EmailConfirmacao = ConfEmailEntry.Text.Trim().ToLowerInvariant();

            await Navigation.PushAsync(new CadastroPage3());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", "Ocorreu um erro inesperado. Tente novamente.", "OK");
            System.Diagnostics.Debug.WriteLine($"Erro no cadastro: {ex.Message}");
        }
        finally
        {
            ContinuarButton2.IsEnabled = true;
            ContinuarButton2.Text = "Continuar";
        }

    }

    private async void VoltarButton2_Clicked(object sender, EventArgs e)
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