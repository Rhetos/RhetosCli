using System;
using System.Collections.Generic;

namespace RhetosCLI
{
    public class CliCommand
    {
        public Type Type { get; set; }
        public string Module { get; set; }
        public string Command { get; set; }
        public string Method { get; set; }

        public Dictionary<string, string> Parameters { get; set; }

        public CliCommand()
        {
            Parameters = new Dictionary<string,string>();
        }
    }
}