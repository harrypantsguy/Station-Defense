using System;

namespace DanonsTools.ConsoleCommandSystem
{
    public interface ICommandOverload
    {
        public Type[] ParameterTypes { get; }
        
        public void Execute(params string[] parameters);
    }
}