using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using atualizaExercicio.Models;
using System.Text.RegularExpressions;

namespace atualizaExercicio.Services
{
    public class UserRegistrationService
    {
        // Dados temporários do cadastro em andamento
        private static UserRegistrationData _currentRegistration = new();

        public void SaveEmail(string email)
        {
            _currentRegistration.Email = email.Trim().ToLowerInvariant();
            _currentRegistration.DataInicio = DateTime.Now;
        }

        public string GetEmail()
        {
            return _currentRegistration.Email;
        }

        public void SaveUserData(string nome, string senha)
        {
            _currentRegistration.Nome = nome;
            _currentRegistration.Senha = senha; // Depois será hasheada
        }

        public UserRegistrationData GetRegistrationData()
        {
            return _currentRegistration;
        }

        public void ClearRegistration()
        {
            _currentRegistration = new UserRegistrationData();
        }
    }
}
