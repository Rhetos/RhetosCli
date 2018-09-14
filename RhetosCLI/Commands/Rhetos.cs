using RhetosCLI.Attributes;
using RhetosCLI.Helpers;
using RhetosCLI.Interfaces;
using System;
using System.Configuration;
using System.IO;
using System.Web.Configuration;

namespace RhetosCLI.Commands
{
    [ClicommandModuleAttribute("Rhetos", "General rhetos functions")]
    public class Rhetos : ICliCommand
    {
        public const string CONN_STRINGS_TEMPLATE_CONFIG = @"bin\Template.ConnectionStrings.config";
        public const string CONN_STRINGS_CONFIG = @"bin\ConnectionStrings.config";
        public const string DEPLOY_PACKAGES_CONFIG = @"bin\DeployPackages.exe.config";
        public const string DEPLOY_PACKAGES = @"bin\DeployPackages.exe";
        public const string RHETOS_PACKAGES_TEMPLATE_CONFIG = @"Template.RhetosPackages.config";
        public const string RHETOS_PACKAGES_CONFIG = @"RhetosPackages.config";
        public const string RHETOS_PACKAGES__SOURCE_TEMPLATE_CONFIG = @"Template.RhetosPackageSources.config";
        public const string RHETOS_PACKAGES__SORCE_CONFIG = @"RhetosPackageSources";
        public const string CONN_STRING_NAME = @"ServerConnectionString";

        public string RhetosVersion { get; set; }
        public string SiteName { get; set; }
        public string AppPoolName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DBServer { get; set; }
        public string DataBaseName { get; set; }
        public string DBUserName{ get; set; }
        public string DBPassword{ get; set; }
        public bool UseWindowsAuth { get; set; }


        private string RhetosPath { get; set; }

        public Rhetos()
        {

        }

        [CliCommand("Create", "Creates new rhetos app instance,setups database and starts web service")]
        public void Create()
        {
            Logging.LogInfo("Creating app {0} with rhetos {1}", AppPoolName, RhetosVersion);
            var enable32Bit = string.Compare(RhetosVersion, "v1.1") != 1;
            RhetosPath = Releases.DownloadRhetosRelease(RhetosVersion);
            IIS.CreateWebSite(SiteName, AppPoolName, UserName, Password, RhetosPath, enable32Bit, UseWindowsAuth);
            SetDatabase();
            CheckDbPermissions();
            Deploy();
            /*
            In the RhetosServer folder:
            Copy "Template.RhetosPackages.config" file to "RhetosPackages.config", if the target file does not already exist.
            Copy "Template.RhetosPackageSources.config" file to "RhetosPackageSources.config", if the target does not already exist.
            Verify that the RhetosServer is configured correctly by opening command prompt at RhetosServer\bin folder and running DeployPackages.exe.
            The last printed line should be "[Trace] DeployPackages: Done.".
            The output may include "[Error] DeploymentConfiguration: No packages" and "[Error] DeployPackages: WARNING: Empty assembly...", because no packages are provided in the "RhetosPackages.config".This is ok for now.
             */
            //AddAdmin
            //
        }
        [CliCommand("DeployPackages", "Run deploy packages")]
        public void Deploy()
        {
            var deployPackagesPath = Path.Combine(RhetosPath, DEPLOY_PACKAGES);
            MiscHelpers.StartExternalExe(deployPackagesPath);
        }

        [CliCommand("CheckDbPermissions", "Checks if user has needed DB permissions")]
        public void CheckDbPermissions()
        {
            var d = 0;
            d = d + 1;
        }

        [CliCommand("SetDatabase", "Updates database data in ConnectionStrings.config")]
        public void SetDatabase()
        {
            if (string.IsNullOrEmpty(DBServer)) throw new ArgumentException("DB server must be specified");
            if (string.IsNullOrEmpty(DataBaseName)) throw new ArgumentException("DataBase must be specified");
            
            if (!UseWindowsAuth && string.IsNullOrEmpty(DBUserName)) throw new ArgumentException("If not using windows auth Username and password must be specified");
            if (!UseWindowsAuth && string.IsNullOrEmpty(DBPassword)) throw new ArgumentException("If not using windows auth Username and password must be specified");
            if (string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(UserName)) throw new ArgumentException("If Use sspi is false, DBUserName and DBPassword are requred.");

            var connString = DataBase.GetConnectionString(DBServer, DataBaseName, UserName, Password, UseWindowsAuth);
            var config = GetConfiguration(CONN_STRINGS_CONFIG, CONN_STRINGS_TEMPLATE_CONFIG);
            config.ConnectionStrings.ConnectionStrings[CONN_STRING_NAME].ConnectionString=connString;
            config.Save();
        }

        private Configuration GetConfiguration(string config, string template)
        {
            var configPath = Path.Combine(RhetosPath, config);
            if (!File.Exists(configPath))
            {
                Logging.LogWarn("Config file {0} not found. Creating new from template {1}", config, template);
                var templatePath = Path.Combine(RhetosPath, template);
                CreateFromTemplate(configPath, templatePath);
            }

            VirtualDirectoryMapping vdm = new VirtualDirectoryMapping(RhetosPath, true);
            WebConfigurationFileMap wcfm = new WebConfigurationFileMap();
            wcfm.VirtualDirectories.Add("/", vdm);
            return  WebConfigurationManager.OpenMappedWebConfiguration(wcfm, "/");
        }

        private static void CreateFromTemplate(string target, string template)
        {
            if (!(File.Exists(target)))
            {
                File.Copy(template, target);
                Logging.LogWarn("Config file {0} created", target);
            }
        }

        public void Execute(CliCommand command)
        {
            switch (command.Command)
            {
                case "Create":
                    Create();
                        break;
                default:
                    break;
            }
        }

        public void ShowHelp(CliCommand command)
        {
            throw new System.NotImplementedException();
        }

        private void SetConnectionString()
        {
            var connString = DataBase.GetConnectionString(DBServer, DataBaseName, UserName, Password, UseWindowsAuth);
        }
        
    }
}