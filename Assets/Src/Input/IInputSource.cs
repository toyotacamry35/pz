using System.Collections.Generic;
using ColonyShared.SharedCode.Input;

namespace Src.Input
{
    public interface IInputSource
    {
        float Value { get; }
        
        InputSourceDef Def { get; }
    }
}
