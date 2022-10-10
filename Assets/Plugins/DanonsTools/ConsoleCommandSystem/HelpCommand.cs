namespace DanonsTools.ConsoleCommandSystem
{
    public sealed class HelpCommand : IConsoleCommand
    {
        public string Keyword => "help";
        public string Description => "";

        private readonly ICommandConsole _console;
        
        public HelpCommand(in ICommandConsole console)
        {
            _console = console;
        }
        
        public void Execute(params string[] parameters)
        {
            _console.Log("Type 'listcommands' for a list of all commands.", ConsoleLogType.Message);
            _console.Log("Type 'help <commandName>' for more information about the usage of a specific command.", ConsoleLogType.Message);
        }
    }
}