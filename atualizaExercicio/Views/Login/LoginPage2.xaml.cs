using atualizaExercicio.Models;
using atualizaExercicio.Services.Database;
using MySql.Data.MySqlClient;
using System.Data;

namespace atualizaExercicio.Views.Login
{
    public partial class LoginPage2 : ContentPage
    {
        private string _email;
        public LoginPage2(string email)
        {
            InitializeComponent();
            _email = email;
            EmailEntry.Text = email;
            EmailEntry.IsEnabled = false;
        }

        private async void LoginButton_Clicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text;
            string senha = SenhaEntry.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
            {
                await DisplayAlert("Erro", "Por favor, preencha todos os campos.", "OK");
                return;
            }

            // ✅ ALTERADO: Agora retorna o usuário completo
            var usuario = await ValidarUsuarioAsync(email, senha);

            if (usuario != null)
            {
                // ✅ NOVO: Salvar ID do usuário logado
                await SecureStorage.SetAsync("usuario_id", usuario.IdUsuario.ToString());

                await DisplayAlert("Sucesso", $"Bem-vindo, {usuario.Nome}!", "OK");
                await Navigation.PushAsync(new Home());
            }
            else
            {
                await DisplayAlert("Falha", "Email ou senha incorretos.", "Tentar novamente");
            }
        }

        // ✅ ALTERADO: Agora retorna Usuario em vez de bool
        private async Task<Usuario> ValidarUsuarioAsync(string email, string senha)
        {
            try
            {
                string normalizedEmail = email.Trim().ToLowerInvariant();
                string normalizedSenha = senha.Trim();

                var dbService = new MySqlDatabaseService();
                string hashedSenha = dbService.HashPassword(normalizedSenha);

                using (MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString))
                {
                    await conn.OpenAsync();

                    // ✅ ALTERADO: Busca dados completos do usuário
                    string query = @"
                        SELECT idUsuario, nome, email, telefone, genero, nomeUsuario, dataNascimento, dataCadastro
                        FROM usuario 
                        WHERE email = @Email AND senha = @Senha";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", normalizedEmail);
                        cmd.Parameters.AddWithValue("@Senha", hashedSenha);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new Usuario
                                {
                                    IdUsuario = reader.GetInt32("idUsuario"),
                                    Nome = reader.GetString("nome"),
                                    Email = reader.GetString("email"),
                                    Telefone = reader.GetString("telefone"),
                                    Genero = reader.GetString("genero"),
                                    NomeUsuario = reader.GetString("nomeUsuario"),
                                    DataNascimento = reader.GetDateTime("dataNascimento"),
                                    DataCadastro = reader.GetDateTime("dataCadastro")
                                };
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", "Falha ao conectar com o banco: " + ex.Message, "OK");
                return null;
            }
        }

        private void ToggleSenhaButton_Clicked_1(object sender, EventArgs e)
        {
            // Alternar visibilidade
            SenhaEntry.IsPassword = !SenhaEntry.IsPassword;

            // Trocar ícone
            ToggleSenhaButton.Source = SenhaEntry.IsPassword ? "eye_slash_icon.png" : "eye_icon.png";
        }
    }
}