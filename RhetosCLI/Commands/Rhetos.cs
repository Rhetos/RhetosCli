using RhetosCLI.Attributes;
using RhetosCLI.Helpers;
using RhetosCLI.Interfaces;

namespace RhetosCLI.Commands
{
    public class Rhetos : ICliCommand
    {
        [CliCommand("Create", "Creates new rhetos instance")]
        public static void Create(string rhetosVersion, string siteName, string appPoolName, string userName, string password, bool useWindowsAuth)
        {
            MiscHelpers.WriteLine("Creating app {0} with rhetos {1}", appPoolName, rhetosVersion);
            var enable32Bit = string.Compare(rhetosVersion, "v1.1") != 1;
            var path = Releases.DownloadRhetosRelease(rhetosVersion);
            IIS.CreateWebSite(siteName, appPoolName, userName, password, path, enable32Bit, useWindowsAuth);
            //Database.CreateDatabase
            //Rhetos.Deploy
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

        public bool Execute(CliCommandParams cmdParams)
        {
            throw new System.NotImplementedException();
        }

        public void ShowHelp()
        {
            throw new System.NotImplementedException();
        }
    }
}