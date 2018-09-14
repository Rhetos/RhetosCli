using System;

namespace RhetosCLI.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ClicommandModuleAttribute : Attribute
    {
        public string ModuleName{ get; set; }
        public string ShortDescription { get; set; }

        public ClicommandModuleAttribute(string moduleName, string shortDescription)
        {
            ModuleName = moduleName;
            ShortDescription = shortDescription;
        }
    }
}