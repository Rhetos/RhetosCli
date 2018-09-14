namespace RhetosCLI.Interfaces
{
    public interface ICliCommand
    {
        void Execute(CliCommand command);
        void ShowHelp(CliCommand command);
    }
}