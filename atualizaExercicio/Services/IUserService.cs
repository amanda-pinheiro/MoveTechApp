using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using atualizaExercicio.Models;
using System.Text.RegularExpressions;

namespace atualizaExercicio.Services
{
    // Interface que define o contrato para operações de usuário
    public interface IUserService
    {
        Task<bool> EmailExisteAsync(string email);
        Task<bool> NomeUsuarioExisteAsync(string nomeUsuario);
        Task<bool> TelefoneExisteAsync(string telefone);
        Task<ValidationResult> CriarUsuarioAsync(UserRegistrationData userData);
        Task<ValidationResult> ValidarDadosUsuarioAsync(UserRegistrationData userData);
    }

    // Implementação mock (temporária, sem BD)
    public class MockUserService : IUserService //quando tiver o BD trocar por SQLUserService
    {
        private readonly HashSet<string> _emailsExistentes = new()
        {
            "admin@gmail.com", "teste@outlook.com", "user@hotmail.com"
        };

        private readonly HashSet<string> _usuariosExistentes = new()
        {
            "admin", "teste", "user123", "movtech_oficial"
        };

        private readonly HashSet<string> _telefonesExistentes = new()
        {
            "(11) 9.9999-9999", "(21) 8.8888-8888"
        };

        public async Task<bool> EmailExisteAsync(string email)
        {
            await Task.Delay(300);
            return _emailsExistentes.Contains(email.ToLowerInvariant());
        }

        public async Task<bool> NomeUsuarioExisteAsync(string nomeUsuario)
        {
            await Task.Delay(400);
            return _usuariosExistentes.Contains(nomeUsuario.ToLowerInvariant());
        }

        public async Task<bool> TelefoneExisteAsync(string telefone)
        {
            await Task.Delay(350);
            return _telefonesExistentes.Contains(telefone);
        }

        public async Task<ValidationResult> CriarUsuarioAsync(UserRegistrationData userData)
        {
            await Task.Delay(800);
            // Simular criação - por enquanto sempre sucesso
            return ValidationResult.Success("Usuário criado com sucesso!");
        }

        public async Task<ValidationResult> ValidarDadosUsuarioAsync(UserRegistrationData userData)
        {
            await Task.Delay(500);
            // Validações que precisariam de BD
            return ValidationResult.Success("Dados válidos");
        }
    }
}