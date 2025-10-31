using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using atualizaExercicio.Models;
using System.Text.RegularExpressions;


namespace atualizaExercicio.Services
{
    public class CadastroValidationService
    {
        // Validação do campo Nome (nome e sobrenome obrigatórios)
        public ValidationResult ValidateNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return ValidationResult.Error("Nome é obrigatório", "Nome");

            nome = nome.Trim();

            if (nome.Length < 3)
                return ValidationResult.Error("Nome deve ter pelo menos 3 caracteres", "Nome");

            if (nome.Length > 100)
                return ValidationResult.Error("Nome deve ter no máximo 100 caracteres", "Nome");

            // Regex: apenas letras, espaços, acentos e traços
            var nomeRegex = new Regex(@"^[a-zA-ZÀ-ÿ\s\-']+$");
            if (!nomeRegex.IsMatch(nome))
                return ValidationResult.Error("Nome deve conter apenas letras, espaços, acentos e traços", "Nome");

            // Verificar se tem pelo menos nome e sobrenome
            var partesNome = nome.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (partesNome.Length < 2)
                return ValidationResult.Error("Digite nome e sobrenome", "Nome");

            // Verificar se cada parte tem pelo menos 2 caracteres
            foreach (var parte in partesNome)
            {
                if (parte.Length < 2)
                    return ValidationResult.Error("Cada parte do nome deve ter pelo menos 2 caracteres", "Nome");
            }

            return ValidationResult.Success("Nome válido");
        }

        // Método para validar data de nascimento
        public ValidationResult ValidateDataNascimento(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return ValidationResult.Error("Data de nascimento é obrigatória", "DataNascimento");

            // Verificar formato DD/MM/AAAA
            if (!Regex.IsMatch(data, @"^\d{2}/\d{2}/\d{4}$"))
                return ValidationResult.Error("Use o formato DD/MM/AAAA", "DataNascimento");

            if (DateTime.TryParseExact(data, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataValida))
            {
                var idade = DateTime.Today.Year - dataValida.Year;
                if (dataValida.Date > DateTime.Today.AddYears(-idade)) idade--;

                if (idade < 13)
                    return ValidationResult.Error("Idade mínima: 13 anos", "DataNascimento");

                if (idade > 120)
                    return ValidationResult.Error("Data inválida", "DataNascimento");

                if (dataValida > DateTime.Today)
                {
                    return ValidationResult.Error("Data não pode ser futura", "DataNascimento");
                }

                return ValidationResult.Success("Data válida");

            }

            return ValidationResult.Error("Data inválida", "DataNascimento");
        }

        // Validação do nome de usuário
        public ValidationResult ValidateNomeUsuario(string nomeUsuario)
        {
            if (string.IsNullOrWhiteSpace(nomeUsuario))
                return ValidationResult.Error("Nome de usuário é obrigatório", "NomeUsuario");

            nomeUsuario = nomeUsuario.Trim().ToLowerInvariant();

            if (nomeUsuario.Length < 3)
                return ValidationResult.Error("Nome de usuário deve ter pelo menos 3 caracteres", "NomeUsuario");

            if (nomeUsuario.Length > 30)
                return ValidationResult.Error("Nome de usuário deve ter no máximo 30 caracteres", "NomeUsuario");

            // Regex: apenas letras minúsculas, números e caracteres especiais @_-.
            var usuarioRegex = new Regex(@"^[a-z0-9@_\-\.]+$");
            if (!usuarioRegex.IsMatch(nomeUsuario))
                return ValidationResult.Error("Use apenas letras minúsculas, números e os caracteres @_-.", "NomeUsuario");

            // Não pode começar ou terminar com caracteres especiais
            if (nomeUsuario.StartsWith("@") || nomeUsuario.StartsWith("_") ||
                nomeUsuario.StartsWith("-") || nomeUsuario.StartsWith(".") ||
                nomeUsuario.EndsWith("@") || nomeUsuario.EndsWith("_") ||
                nomeUsuario.EndsWith("-") || nomeUsuario.EndsWith("."))
                return ValidationResult.Error("Nome de usuário não pode começar ou terminar com @_-.", "NomeUsuario");

            return ValidationResult.Success("Nome de usuário válido");
        }

        public ValidationResult ValidateTelefone(string telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone))
                return ValidationResult.Error("Telefone é obrigatório", "Telefone");

            // Remover formatação para validar apenas números
            string numeroLimpo = telefone.Replace("(", "").Replace(")", "")
                                       .Replace("-", "").Replace(".", "").Replace(" ", "");

            if (!numeroLimpo.All(char.IsDigit))
                return ValidationResult.Error("Telefone deve conter apenas números", "Telefone");

            if (numeroLimpo.Length != 11)
                return ValidationResult.Error("Telefone deve ter 11 dígitos (DDD + 9 dígitos)", "Telefone");

            // Verificar se o primeiro dígito do celular é 9
            if (numeroLimpo[2] != '9')
                return ValidationResult.Error("Número de celular deve começar com 9", "Telefone");

            return ValidationResult.Success("Telefone válido");
        }

        // Validação da confirmação de email
        public ValidationResult ValidateConfirmacaoEmail(string emailConfirmacao, string emailOriginal)
        {
            if (string.IsNullOrWhiteSpace(emailConfirmacao))
                return ValidationResult.Error("Confirmação de email é obrigatória", "ConfirmacaoEmail");

            emailConfirmacao = emailConfirmacao.Trim().ToLowerInvariant();
            emailOriginal = emailOriginal.Trim().ToLowerInvariant();

            // Validar formato básico
            var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            if (!emailRegex.IsMatch(emailConfirmacao))
                return ValidationResult.Error("Email de confirmação deve ter formato válido", "ConfirmacaoEmail");

            // Verificar se é igual ao email original
            if (emailConfirmacao != emailOriginal)
                return ValidationResult.Error("Os emails não coincidem", "ConfirmacaoEmail");

            return ValidationResult.Success("Email confirmado com sucesso");
        }

        // Validação da senha
        public ValidationResult ValidateSenha(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha))
                return ValidationResult.Error("Senha é obrigatória", "Senha");

            if (senha.Length < 8)
                return ValidationResult.Error("Senha deve ter no mínimo 8 caracteres", "Senha");

            if (senha.Length > 50)
                return ValidationResult.Error("Senha deve ter no máximo 50 caracteres", "Senha");

            // Verificar se tem letra maiúscula
            if (!senha.Any(char.IsUpper))
                return ValidationResult.Error("Senha deve conter pelo menos uma letra maiúscula", "Senha");

            // Verificar se tem letra minúscula
            if (!senha.Any(char.IsLower))
                return ValidationResult.Error("Senha deve conter pelo menos uma letra minúscula", "Senha");

            // Verificar se tem número
            if (!senha.Any(char.IsDigit))
                return ValidationResult.Error("Senha deve conter pelo menos um número", "Senha");

            // Verificar se tem caractere especial
            if (!senha.Any(c => !char.IsLetterOrDigit(c)))
                return ValidationResult.Error("Senha deve conter pelo menos um caractere especial (@#$%&*)", "Senha");

            return ValidationResult.Success("Senha válida");
        }

        // Validação da confirmação de senha
        public ValidationResult ValidateConfirmacaoSenha(string senha, string confirmacao)
        {
            if (string.IsNullOrWhiteSpace(confirmacao))
                return ValidationResult.Error("Confirmação de senha é obrigatória", "ConfirmacaoSenha");

            if (senha != confirmacao)
                return ValidationResult.Error("As senhas não coincidem", "ConfirmacaoSenha");

            return ValidationResult.Success("Senhas coincidem");
        }
    }
}
