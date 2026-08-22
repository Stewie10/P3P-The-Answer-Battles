using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3PPC.Expansion.TheAnswer.IFaces;

internal interface IAutoWrapper<TStruct,TEnum> 
    where TStruct : IAutoWrapper<TStruct,TEnum>
    where TEnum : Enum
{
    TEnum Value { get; }
    // Additional members and methods defined in an AutoWrapper struct do not need to be declared here, as they will be implemented in the struct itself.
    static abstract TStruct FromEnum(TEnum value);
}
