using RhetosCLI.Attributes;

namespace RhetosCLI.Commands
{
    [ClicommandModuleAttribute("Database", "General database function")]
    public class DataBase
    {
        public const string CONN_STRING_TEMPLATE = "Data Source=<SERVER_NAME>; Initial Catalog=<DATABASE_NAME>;";
        public const string CONN_SECURITY_TEMPLATE_SSPI = "Integrated Security=SSPI;";
        public const string CONN_SECURITY_TEMPLATE_NO_SSPI = "Username=<USER_NAME>; Password=<PASSWORD>;";

        public string ServerName { get; set; }
        public string DataBaseName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool UseSSPI { get; set; }

        public DataBase()
        {
        }

        public DataBase(string serverName, string dataBaseName, string userName, string password, bool useSSPI)
        {
            ServerName = serverName;
            DataBaseName = dataBaseName;
            UserName = userName;
            Password = password;
            UseSSPI = useSSPI;
        }

        [CliCommand("CheckPermissions", "Check if user has needed db permissions")]
        public bool CheckDbPermissions()
        {
            var retValue = true;
            return retValue;
        }

        public string GetConnectionString()
        {
            var connString = CONN_STRING_TEMPLATE.Replace("<SERVER_NAME>", ServerName).Replace("<DATABASE_NAME>", DataBaseName);
            if (UseSSPI)
            {
                connString = connString + CONN_SECURITY_TEMPLATE_SSPI;
            }
            else
            {
                connString = connString + CONN_SECURITY_TEMPLATE_NO_SSPI.Replace("<USER_NAME>", UserName).Replace("<PASSWORD>", Password);
            }
            return connString;
        }

        [CliCommand("Help", "Shows help for module commands")]
        public void ShowHelp(CliCommand command)
        {
            throw new System.NotImplementedException();
        }
    }
}