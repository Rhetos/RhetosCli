using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhetosCLI.Commands
{
    public class DataBase
    {
        public const string CONN_STRING_TEMPLATE = "Data Source=<SERVER_NAME>; Initial Catalog=<DATABASE_NAME>;";
        public const string CONN_SECURITY_TEMPLATE_SSPI = "Integrated Security=SSPI;";
        public const string CONN_SECURITY_TEMPLATE_NO_SSPI = "Username=<USER_NAME>; Password=<PASSWORD>;";


        public DataBase()
        {
        }

        public static string GetConnectionString(string serverName, string dataBase, string username, string password, bool useSSPI)
        {
            var connString = CONN_STRING_TEMPLATE.Replace("<SERVER_NAME>", serverName).Replace("<DATABASE_NAME>", dataBase); 
            if (useSSPI)
            {
                connString = connString + CONN_SECURITY_TEMPLATE_SSPI;
            }
            else
            {
                connString= connString + CONN_SECURITY_TEMPLATE_NO_SSPI.Replace("<USER_NAME>",username).Replace("<PASSWORD>",password);
            }
            return connString;
        }
    }
}
