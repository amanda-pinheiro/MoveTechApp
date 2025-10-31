using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atualizaExercicio.Services.Database
{
    class DatabaseConfig
    {
        // String de conexão com o MySQL
        public static string ConnectionString =
            "Server=localhost;" +              // Endereço do servidor MySQL
            "Port=3306;" +                     // Porta padrão do MySQL
            "Database=mockmovetech;" +             // Nome do seu banco
            "User=root;" +                     // Seu usuário MySQL
            "Password=;";      // ⚠️ TROQUE pela sua senha
            
    }
}
