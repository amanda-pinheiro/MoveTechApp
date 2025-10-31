using atualizaExercicio.Models;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace atualizaExercicio.Services.Database
{
    public class MySqlDatabaseService
    {
        private readonly string _connectionString;

        public MySqlDatabaseService()
        {
            _connectionString = DatabaseConfig.ConnectionString;
        }

        // Testar se a conexão funciona
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro de conexão: {ex.Message}");
                return false;
            }
        }

        // Criar usuário no banco
        public async Task<int> CreateUsuarioAsync(Usuario usuario)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var query = @"INSERT INTO usuario 
                        (nome, email, senha, telefone, genero, nomeUsuario, dataNascimento, dataCadastro) 
                        VALUES 
                        (@Nome, @Email, @Senha, @Telefone, @Genero, @NomeUsuario, @DataNascimento, @DataCadastro)";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Nome", usuario.Nome);
                        command.Parameters.AddWithValue("@Email", usuario.Email);
                        command.Parameters.AddWithValue("@Senha", usuario.Senha);
                        command.Parameters.AddWithValue("@Telefone", usuario.Telefone);
                        command.Parameters.AddWithValue("@Genero", usuario.Genero);
                        command.Parameters.AddWithValue("@NomeUsuario", usuario.NomeUsuario);
                        command.Parameters.AddWithValue("@DataNascimento", usuario.DataNascimento);
                        command.Parameters.AddWithValue("@DataCadastro", usuario.DataCadastro);

                        return await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao criar usuário: {ex.Message}");
                throw;
            }
        }

        // Verificar se email existe
        public async Task<bool> EmailExistsAsync(string email)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var query = "SELECT COUNT(*) FROM usuario WHERE email = @Email";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        var count = Convert.ToInt32(await command.ExecuteScalarAsync());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao verificar email: {ex.Message}");
                return false;
            }
        }

        // Verificar se nome de usuário existe
        public async Task<bool> NomeUsuarioExistsAsync(string nomeUsuario)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var query = "SELECT COUNT(*) FROM usuario WHERE nomeUsuario = @NomeUsuario";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NomeUsuario", nomeUsuario);
                        var count = Convert.ToInt32(await command.ExecuteScalarAsync());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao verificar usuário: {ex.Message}");
                return false;
            }
        }

        // Verificar se telefone existe
        public async Task<bool> TelefoneExistsAsync(string telefone)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var query = "SELECT COUNT(*) FROM usuario WHERE telefone = @Telefone";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Telefone", telefone);
                        var count = Convert.ToInt32(await command.ExecuteScalarAsync());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao verificar telefone: {ex.Message}");
                return false;
            }
        }

        // Criptografar senha
        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                // GARANTIR o mesmo encoding em cadastro e login
                var bytes = Encoding.UTF8.GetBytes(password?.Trim() ?? "");
                var hashedBytes = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
