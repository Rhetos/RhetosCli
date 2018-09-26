using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
using RhetosCLI.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RhetosCLI.Helpers.Tests
{
    [TestClass()]
    public class MiscHelpersTests
    {
        [TestInitialize()]
        public void CreateLogger()
        {
            Logging.Logger = LogManager.GetCurrentClassLogger();
        }

        [TestMethod()]
        public void SetParamsTest()
        {
            var testCommand = PrepareTestCommand();
            var commands = MiscHelpers.LoadAllCommands();
            MiscHelpers.ResolveCommand(commands, testCommand);
            var cmd = Activator.CreateInstance(testCommand.Type);
            MiscHelpers.SetParams(cmd, testCommand);
            var expected = new Rhetos
            {
                AppPoolName = "vlado_test",
                DataBaseName = "vlado_test_vladimir",
                DBPassword = "Password",
                DBServer = "ADSQL2008R2\\ADSQL2012",
                DBUserName = "vlado",
                Password = "Password",
                RhetosVersion = "v2.7.0",
                SiteName = "vlado_test",
                UserName = "os\vladimir",
                UseWindowsAuth = true
            };
            cmd.Should().BeEquivalentTo(expected);
        }

        [TestMethod()]
        public void ParseCommandLineTest()
        {
            var expected = PrepareTestCommand();
            string[] args = { "Rhetos", "Create", "RhetosVersion = v2.7.0", "SiteName = vlado_test", "AppPoolName = vlado_test", "userName = os\vladimir", "password = Password", "UseWindowsAuth = true", "DBServer = ADSQL2008R2\\ADSQL2012", "DataBaseName = vlado_test_vladimir", "DBUserName = vlado", "DBPassword = Password" };
            var command = MiscHelpers.ParseCommandLine(args);
            command.Should().BeEquivalentTo(expected);
        }

        private CliCommand PrepareTestCommand()
        {
            var testCommand = new CliCommand
            {
                Command = "create",
                Module = "rhetos"
            };
            var parametrers = new Dictionary<string, string>
            {
                { "rhetosversion", "v2.7.0" },
                { "sitename", "vlado_test" },
                { "apppoolname", "vlado_test" },
                { "username", "os\vladimir" },
                { "password", "Password" },
                { "usewindowsauth", "true" },
                { "dbserver", "ADSQL2008R2\\ADSQL2012" },
                { "databasename", "vlado_test_vladimir" },
                { "dbusername", "vlado" },
                { "dbpassword", "Password" }
            };
            testCommand.Parameters = parametrers;
            return testCommand;
        }

        [TestMethod()]
        public void LoadAllCommandsTest()
        {
            var commands = MiscHelpers.LoadAllCommands();
            commands.Count.Should().Equals(10);
            commands.Select(c => c.Module).Distinct().Count().Should().Equals(3);
        }

        [TestMethod()]
        public void ResolveCommandTest()
        {
            var testCommand = PrepareTestCommand();
            var commands = MiscHelpers.LoadAllCommands();
            MiscHelpers.ResolveCommand(commands, testCommand);
            testCommand.Module.Should().Equals("rhetos");
            testCommand.Type.ToString().Should().Equals(typeof(Rhetos).ToString());
        }
    }
}