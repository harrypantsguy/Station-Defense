namespace DanonsTools.ConsoleCommandSystem
{
    public sealed class ListCommandsCommand : IConsoleCommand
    {
        public string Keyword => "listcommands";
        public string Description => "Displays a list of all registered commands.";

        private readonly ICommandConsole _console;

        public ListCommandsCommand(in ICommandConsole console)
        {
            _console = console;
        }
        
        public void Execute(params string[] parameters)
        {
            _console.Log("All registered commands:", ConsoleLogType.Message);
            
            foreach (var command in _console.RegisteredCommands)
            {
                _console.Log($"  {command.Key} - {command.Value.Description}", ConsoleLogType.Message);
            }
        }
    }
}