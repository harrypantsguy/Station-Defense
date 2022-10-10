using System;
using System.Collections.Generic;

namespace DanonsTools.ConsoleCommandSystem
{
    public sealed class DefaultCommandConsole : ICommandConsole
    {
        public Action<string, ConsoleLogType> ConsoleLogEvent { get; set; }
        public Dictionary<string, IConsoleCommand> RegisteredCommands { get; } = new Dictionary<string, IConsoleCommand>();

        public void RegisterCommand(in IConsoleCommand command)
        {
            RegisteredCommands.TryAdd(command.Keyword, command);
        }

        public void ProcessInput(in string input)
        {
            if (input.Equals(string.Empty)) return;

            var parsedInput = input.ToLower();
            
            var inputParams = parsedInput.Split(' ');

            if (!RegisteredCommands.TryGetValue(inputParams[0], out var command))
            {
                Log($"No command '{inputParams[0]}' found. Type 'help' for documentation.", ConsoleLogType.Error);
                return;
            }
            
            command.Execute(inputParams);
        }

        public void Log(in string message, in ConsoleLogType logType)
        {
            ConsoleLogEvent?.Invoke(message, logType);
        }
    }
}
