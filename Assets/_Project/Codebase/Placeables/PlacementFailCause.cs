using System;

namespace _Project.Codebase
{
    [Flags]
    public enum PlacementFailCause : short
    {
        None = 0,
        ImproperLocation = 1,
        InsufficientFunds = 2,
    }
}