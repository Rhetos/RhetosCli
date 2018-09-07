using System;
using System.Collections.Generic;

namespace RhetosCLI
{
    public class CliCommandParams
    {
        public String Command { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
    }
}