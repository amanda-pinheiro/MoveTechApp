using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atualizaExercicio.Models
{
    public class UserRegistrationData
    {
        public string Email { get; set; } = "";
        public string Nome { get; set; } = "";
        public string DataNascimento { get; set; } = "";
        public string Genero { get; set; } = "";
        public string NomeUsuario { get; set; } = "";
        public string Telefone { get; set; } = "";
        public string EmailConfirmacao { get; set; } = "";
        public string Senha { get; set; } = "";  // Esta linha era a que estava faltando
        public DateTime DataInicio { get; set; } = DateTime.Now;
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = "";
        public string FieldName { get; set; } = "";

        public static ValidationResult Success(string message = "")
            => new() { IsValid = true, Message = message };

        public static ValidationResult Error(string message, string fieldName = "")
            => new() { IsValid = false, Message = message, FieldName = fieldName };
    }
}
