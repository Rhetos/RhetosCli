namespace RhetosCLI.Interfaces
{
    public interface ICliCommand
    {
        bool Execute(CliCommandParams parameters);

        void ShowHelp();
    }
}