using Microsoft.Web.Administration;
using System;
using System.Linq;
using System.Security.Principal;

namespace RhetosCLI.Helpers
{
    public static class IIS
    {
        private const string DEFAULT_WEB_SITE_NAME = "Default Web Site";
        private const string WEB_SERVER_ANONYMOUS_AUTH_SECTION = "system.webServer/security/authentication/anonymousAuthentication";
        private const string WEB_SERVER_WINDOWS_AUTH_SECTION = "system.webServer/security/authentication/windowsAuthentication";
        private const string APP_AUTH_SECTION = "system.web/authentication";

        public enum AuthenticationMode
        {
            Windows = 1,
            Forms = 3
        }

        private static string GetIISAppName(string appName)
        {
            return "/" + appName;
        }

        private static bool IsUserAdmin()
        {
            
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            
        }
        

        public static void CreateWebSite(string appName, string appPoolName, string userName, string password, string path, bool enable32bit, bool enableWindowsAuth)
        {
            if (!IsUserAdmin())
            {
                throw new InvalidOperationException("This command needs elevated status to access IIS configuration data. You need to run it as admin!");
            }

            if (string.IsNullOrEmpty(appName)) throw new ArgumentException("App name can't be empty");
            if (string.IsNullOrEmpty(appPoolName)) throw new ArgumentException("App pool name can't be empty");
            if (string.IsNullOrEmpty(path)) throw new ArgumentException("Path can't be empty");
            if (string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password)) throw new ArgumentException("If user name is provided password can't be empty");
            if (string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(userName)) throw new ArgumentException("If password is provided user name can't be empty");

            var serverManager = new ServerManager();
            var iisAppName = GetIISAppName(appName);

            if (serverManager.Sites != null)
            {
                CreateAppPool(appPoolName, userName, password, enable32bit, serverManager);
                var defaultSite = serverManager.Sites.FirstOrDefault(s => s.Name == DEFAULT_WEB_SITE_NAME);
                if (defaultSite != null)
                {
                    CreateApplication(appName, appPoolName, path, enableWindowsAuth, serverManager, iisAppName, defaultSite);
                }
                else
                {
                    throw new InvalidOperationException("Can't find default site, aborting!");
                }
            }
        }

        private static void CreateApplication(string appName, string appPoolName, string path, bool enableWindowsAuth, ServerManager serverManager, string iisAppName, Site defaultSite)
        {
            var appExists = defaultSite.Applications[iisAppName] != null;
            if (!appExists)
            {
                var app = defaultSite.Applications.Add(iisAppName, path);
                app.ApplicationPoolName = appPoolName;
                //must commit before changing security mode
                serverManager.CommitChanges();
                SetAuthMode(enableWindowsAuth, serverManager, iisAppName, app);
                Logging.LogInfo("App {0} created, it is using {1} app pool.",appName, appPoolName);
            }
            else
            {
                //throw new InvalidOperationException(string.Format("Application {0} already exists, aborting.", appName));
            }
        }

        private static void CreateAppPool(string appPoolName, string userName, string password, bool enable32bit, ServerManager serverManager)
        {
            bool poolExists = serverManager.ApplicationPools.FirstOrDefault(p => p.Name == appPoolName) != null;
            if (!poolExists)
            {
                var appPool = serverManager.ApplicationPools.Add(appPoolName);
                appPool.ProcessModel.IdentityType = ProcessModelIdentityType.SpecificUser;
                appPool.Enable32BitAppOnWin64 = enable32bit;
                appPool.ProcessModel.UserName = userName;
                appPool.ProcessModel.Password = password;

                ///TODO If is is not possible to use Windows domain account, the Rhetos service can be set up to use ApplicationPoolIdentity in a development environment:
                ///TODO Skip the following steps if you are using a Windows domain account.

                ///TODO Modify the RhetosAppPool to use built-in account "ApplicationPoolIdentity", instead of the developers domain account(Advanced Settings => Identity). This is the default user for a new app pool.
            }
            else
            {
                Logging.LogWarn("App pool {0} exists, it wil be reused.", appPoolName);
            }
        }

        private static void SetAuthMode(bool enableWindowsAuth, ServerManager serverManager, string iisAppName, Application app)
        {
            Configuration config = serverManager.GetApplicationHostConfiguration();
            if (enableWindowsAuth)
            {
                EnableWindowsSecurity(config, DEFAULT_WEB_SITE_NAME + iisAppName, app);
            }
            else
            {
                EnableFormsSecurity(config, DEFAULT_WEB_SITE_NAME + iisAppName, app);
            }
            serverManager.CommitChanges();
        }

        private static void ResetSecurity(Configuration config, string appKey)
        {
            config.GetSection(WEB_SERVER_ANONYMOUS_AUTH_SECTION, appKey)["enabled"] = false;
            config.GetSection(WEB_SERVER_WINDOWS_AUTH_SECTION, appKey)["enabled"] = false;
        }

        private static void EnableWindowsSecurity(Configuration config, string appKey, Application app)
        {
            ResetSecurity(config, appKey);
            config.GetSection(WEB_SERVER_WINDOWS_AUTH_SECTION, appKey)["enabled"] = true;
            app.GetWebConfiguration().GetSection(APP_AUTH_SECTION)["mode"] = AuthenticationMode.Windows;
        }

        private static void EnableFormsSecurity(Configuration config, string appKey, Application app)
        {
            ResetSecurity(config, appKey);
            app.GetWebConfiguration().GetSection(APP_AUTH_SECTION)["mode"] = AuthenticationMode.Forms;
        }
    }
}