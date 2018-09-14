using System;
using System.Collections.Generic;

namespace RhetosCLI
{
    public class CliCommand
    {
        public String Command { get; set; }
        public Dictionary<string, string> Parameters { get; set; }

        public CliCommand()
        {
            Parameters = new Dictionary<string,string>();
        }
    }
}