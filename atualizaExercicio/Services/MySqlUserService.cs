using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using atualizaExercicio.Models;
using System.Text.RegularExpressions;
using atualizaExercicio.Services.Database;

namespace atualizaExercicio.Services
{
    public class MySqlUserService : IUserService
    {
        private readonly MySqlDatabaseService _databaseService;

        public MySqlUserService()
        {
            _databaseService = new MySqlDatabaseService();
        }

        public async Task<bool> EmailExisteAsync(string email)
        {
            return await _databaseService.EmailExistsAsync(email);
        }

        public async Task<bool> NomeUsuarioExisteAsync(string nomeUsuario)
        {
            return await _databaseService.NomeUsuarioExistsAsync(nomeUsuario);
        }

        public async Task<bool> TelefoneExisteAsync(string telefone)
        {
            return await _databaseService.TelefoneExistsAsync(telefone);
        }

        public async Task<ValidationResult> CriarUsuarioAsync(UserRegistrationData userData)
        {
            try
            {
                // Criar objeto Usuario para o BD
                var usuario = new Usuario
                {
                    Nome = userData.Nome.Trim(),
                    Email = userData.Email.Trim().ToLowerInvariant(),
                    Senha = _databaseService.HashPassword(userData.Senha),
                    Telefone = userData.Telefone,
                    Genero = userData.Genero.Trim().ToLowerInvariant(),
                    NomeUsuario = userData.NomeUsuario.Trim().ToLowerInvariant(),
                    DataNascimento = DateTime.ParseExact(userData.DataNascimento, "dd/MM/yyyy", null),
                    DataCadastro = DateTime.Now
                };

                // Inserir no banco
                await _databaseService.CreateUsuarioAsync(usuario);

                return ValidationResult.Success("Usuário criado com sucesso!");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao criar usuário: {ex.Message}");
                return ValidationResult.Error($"Erro ao criar usuário: {ex.Message}");
            }
        }

        public async Task<ValidationResult> ValidarDadosUsuarioAsync(UserRegistrationData userData)
        {
            if (await EmailExisteAsync(userData.Email))
                return ValidationResult.Error("Email já cadastrado", "Email");

            if (await NomeUsuarioExisteAsync(userData.NomeUsuario))
                return ValidationResult.Error("Nome de usuário já existe", "NomeUsuario");

            if (await TelefoneExisteAsync(userData.Telefone))
                return ValidationResult.Error("Telefone já cadastrado", "Telefone");

            return ValidationResult.Success("Dados válidos");
        }
    }
}