using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using atualizaExercicio.Models;
using System.Text.RegularExpressions;
namespace atualizaExercicio.Services
{
    public class EmailValidationService
    {
        private readonly IUserService _userService;  // MUDOU: agora usa IUserService

        private readonly string[] _dominiosPermitidos =
        {
            "gmail.com", "outlook.com", "hotmail.com",
            "yahoo.com", "live.com", "icloud.com"
        };

        // MUDOU: Construtor agora cria MySqlUserService
        public EmailValidationService()
        {
            _userService = new MySqlUserService();
        }

        // MUDOU: Agora consulta o MySQL de verdade
        public async Task<bool> EmailJaExisteAsync(string email)
        {
            email = email.ToLower().Trim();
            return await _userService.EmailExisteAsync(email);
        }

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;
            if (!email.Contains('@') || !email.Contains('.'))
                return false;
            if (!IsAllowedDomain(email))
                return false;
            return true;
        }

        // Validação completa (agora consulta BD real)
        public async Task<string> ValidateEmailCompleteAsync(string email)
        {
            // 1. Validações básicas primeiro
            if (!IsValidEmail(email))
            {
                return GetEmailErrorMessage(email);
            }

            // 2. Verificar se já existe no MySQL
            bool jaExiste = await EmailJaExisteAsync(email);
            if (jaExiste)
            {
                return "Este email já está cadastrado. Tente fazer login.";
            }

            // Se chegou aqui, está tudo OK
            return ""; // Sem erro
        }

        private bool IsAllowedDomain(string email)
        {
            var partes = email.Split('@');
            if (partes.Length != 2) return false;
            var dominio = partes[1].ToLower();
            return _dominiosPermitidos.Contains(dominio);
        }

        public string GetEmailErrorMessage(string email)
        {
            if (string.IsNullOrEmpty(email))
                return "Por favor, digite seu email";
            if (!email.Contains("@") || !email.Contains("."))
                return "Por favor, digite um email válido";
            if (!IsAllowedDomain(email))
                return "Use emails de Gmail, Outlook, Yahoo ou Hotmail";
            return "";
        }
    }
}
