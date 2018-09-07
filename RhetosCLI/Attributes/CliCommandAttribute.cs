using System;

namespace RhetosCLI.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CliCommandAttribute : Attribute
    {
        public string CommandName { get; set; }
        public string ShortDescription { get; set; }

        public CliCommandAttribute(string commandName, string shortDescription)
        {
            CommandName = commandName;
            ShortDescription = shortDescription;
        }
    }
}