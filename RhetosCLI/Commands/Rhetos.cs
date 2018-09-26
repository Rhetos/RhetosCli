using RhetosCLI.Attributes;
using RhetosCLI.Helpers;
using System;
using System.Configuration;
using System.IO;
using System.Web.Configuration;

namespace RhetosCLI.Commands
{
    [ClicommandModuleAttribute("Rhetos", "General rhetos functions")]
    public class Rhetos
    {
        public const string CONN_STRINGS_TEMPLATE_CONFIG = @"bin\Template.ConnectionStrings.config";
        public const string CONN_STRINGS_CONFIG = @"bin\ConnectionStrings.config";
        public const string DEPLOY_PACKAGES_CONFIG = @"bin\DeployPackages.exe.config";
        public const string DEPLOY_PACKAGES = @"bin\DeployPackages.exe";
        public const string RHETOS_PACKAGES_TEMPLATE_CONFIG = @"Template.RhetosPackages.config";
        public const string RHETOS_PACKAGES_CONFIG = @"RhetosPackages.config";
        public const string RHETOS_PACKAGES_SOURCE_TEMPLATE_CONFIG = @"Template.RhetosPackageSources.config";
        public const string RHETOS_PACKAGES_SOURCE_CONFIG = @"RhetosPackageSources.config";
        public const string CONN_STRING_NAME = @"ServerConnectionString";

        public string RhetosVersion { get; set; }
        public string SiteName { get; set; }
        public string AppPoolName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DBServer { get; set; }
        public string DataBaseName { get; set; }
        public string DBUserName { get; set; }
        public string DBPassword { get; set; }
        public bool UseWindowsAuth { get; set; }

        private string RhetosPath { get; set; }

        public Rhetos()
        {
            //TODO implement detection of rhetos and setting path variable.
        }

        [CliCommand("Create", "Creates new rhetos app instance,setups database and starts web service")]
        public void Create()
        {
            ValidateSetDatabaseParams();

            Logging.LogInfo("Creating app {0} with rhetos {1}", AppPoolName, RhetosVersion);
            var enable32Bit = string.Compare(RhetosVersion, "v1.1") != 1;
            var releases = new Releases();

            RhetosPath = releases.DownloadRhetosRelease(RhetosVersion);
            IIS.CreateWebSite(SiteName, AppPoolName, UserName, Password, RhetosPath, enable32Bit, UseWindowsAuth);
            CheckForRhetosConfiguration();
            SetDatabase();
            CheckDbPermissions();
            Deploy();
            //TODO AddAdmin command
        }

        [CliCommand("Deploy", "Run deploy packages")]
        public void Deploy()
        {
            //TODO how to get rhetospath
            var deployPackagesPath = Path.Combine(RhetosPath, DEPLOY_PACKAGES);
            MiscHelpers.StartExternalExe(deployPackagesPath);
        }

        private void CheckForRhetosConfiguration()
        {
            var packagesConfigPath = Path.Combine(RhetosPath, RHETOS_PACKAGES_CONFIG);
            if (!File.Exists(packagesConfigPath))
            {
                var templatePath = Path.Combine(RhetosPath, RHETOS_PACKAGES_TEMPLATE_CONFIG);
                CreateFromTemplate(packagesConfigPath, templatePath);
            }

            var dbConfigPath = Path.Combine(RhetosPath, CONN_STRINGS_CONFIG);
            if (!File.Exists(dbConfigPath))
            {
                var templatePath = Path.Combine(RhetosPath, CONN_STRINGS_TEMPLATE_CONFIG);
                CreateFromTemplate(dbConfigPath, templatePath);
            }

            var packageSourcesConfig = Path.Combine(RhetosPath, RHETOS_PACKAGES_SOURCE_CONFIG);
            if (!File.Exists(packageSourcesConfig))
            {
                var templatePath = Path.Combine(RhetosPath, RHETOS_PACKAGES_SOURCE_TEMPLATE_CONFIG);
                CreateFromTemplate(packageSourcesConfig, templatePath);
            }
        }

        [CliCommand("CheckDbPermissions", "Checks if user has needed DB permissions")]
        public void CheckDbPermissions()
        {
            //TODO
        }

        [CliCommand("SetDatabase", "Updates database data in ConnectionStrings.config")]
        public void SetDatabase()
        {
            ValidateSetDatabaseParams();

            var db = new DataBase(DBServer, DataBaseName, UserName, Password, UseWindowsAuth);
            var connString = db.GetConnectionString();
            var config = GetDBConfiguration();
            config.ConnectionStrings.ConnectionStrings[CONN_STRING_NAME].ConnectionString = connString;
            config.Save();
        }

        private void ValidateSetDatabaseParams()
        {
            if (string.IsNullOrEmpty(DBServer)) throw new ArgumentException("DB server must be specified");
            if (string.IsNullOrEmpty(DataBaseName)) throw new ArgumentException("DataBase must be specified");

            if (!UseWindowsAuth && string.IsNullOrEmpty(DBUserName)) throw new ArgumentException("If not using windows auth Username and password must be specified");
            if (!UseWindowsAuth && string.IsNullOrEmpty(DBPassword)) throw new ArgumentException("If not using windows auth Username and password must be specified");
            if (string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(UserName)) throw new ArgumentException("If Use sspi is false, DBUserName and DBPassword are requred.");
        }

        private Configuration GetDBConfiguration()
        {
            VirtualDirectoryMapping vdm = new VirtualDirectoryMapping(RhetosPath, true);
            WebConfigurationFileMap wcfm = new WebConfigurationFileMap();
            wcfm.VirtualDirectories.Add("/", vdm);
            return WebConfigurationManager.OpenMappedWebConfiguration(wcfm, "/");
        }

        private static void CreateFromTemplate(string target, string template)
        {
            if (!(File.Exists(target)))
            {
                File.Copy(template, target);
                Logging.LogWarn("Config file {0} created from template {1}", target, template);
            }
        }

        [CliCommand("Help", "Shows help for module commands")]
        public void ShowHelp()
        {
            throw new System.NotImplementedException();
        }
    }
}