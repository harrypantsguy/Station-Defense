using System;
using System.Collections.Generic;

namespace DanonsTools.ConsoleCommandSystem
{
    public static class ConsoleUtilities
    {
        public static bool TryGetOverloadWithParameterTypes(in Type[] parameterTypes, 
            in IEnumerable<ICommandOverload> overloads, out ICommandOverload overload)
        {
            foreach (var o in overloads)
            {
                if (o.ParameterTypes.Length != parameterTypes.Length) continue;

                var paramLength = o.ParameterTypes.Length;
                
                for (var i = 0; i < paramLength; i++)
                {
                    var desiredParam = parameterTypes[i];
                    var param = o.ParameterTypes[i];
                    
                    if (param != desiredParam) break;
                    if (i != paramLength - 1 || param != desiredParam) continue;
                    
                    overload = o;
                    return true;
                }
            }

            overload = default;
            return false;
        }
    }
}