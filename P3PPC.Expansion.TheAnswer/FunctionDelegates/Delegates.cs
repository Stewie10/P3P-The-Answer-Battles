using Reloaded.Hooks.Definitions.Structs;
using Reloaded.Hooks.Definitions.X64;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3PPC.Expansion.TheAnswer.FunctionDelegates;

[System.AttributeUsage(AttributeTargets.Delegate, Inherited = false, AllowMultiple = true)]
sealed class DelegateNameAttribute : Attribute
{
    readonly string name;

    public DelegateNameAttribute(string name)
    {
        this.name = name;
    }

    public string Name => name;
    public string PtrName => name + " Ptr";
}

public class InterpolatedDelegate<TInterp,TInner,TOuter>
    where TInterp : InterpolatedDelegate<TInterp, TInner, TOuter>
    where TInner : Delegate
    where TOuter : Delegate
{
    private readonly TInner _innerDelegate;
    public InterpolatedDelegate(TInner innerDelegate)
    {
        _innerDelegate = innerDelegate;
    }
    public TOuter GetOuterDelegate()
    {
        return (TOuter)(object)this;
    }
    protected TInner GetInnerDelegate()
    {
        return _innerDelegate;
    }
}

[DelegateName("Non-Full Moon Boss Battle")]
public unsafe delegate nuint* NonFullMoonBossBattleFunc(long param1);

[DelegateName("Boss Battle Unit Animations"), Function(CallingConventions.Microsoft)]
public delegate long BossBtlUnitAnimFunc(long btlUnitInfo, ushort animArraySet);

[DelegateName("Boss Persona Battle Unit Set"),Function(CallingConventions.Microsoft)]
public delegate long PersonaBtlUnitSetFunc(long setBtlUnit, short personaID);

[DelegateName("Boss Battle Unit Set"), Function(CallingConventions.Microsoft)]
public delegate long BossBtlUnitFunc(long param1);

[DelegateName("Boss Battle Combat Info"), Function(CallingConventions.Microsoft)]
public unsafe delegate long* BossBtlCombatInfoFunc();

[DelegateName("Boss Battle BED Files"), Function(CallingConventions.Microsoft)]
public unsafe delegate long* BossBtlBEDFilesFunc();

[DelegateName("Hermit Charge"), Function(CallingConventions.Microsoft)]
public delegate long HermitChargeFunc(int btlUnit, long combatInfoAddress, long encounter, nint param4);

[DelegateName("Hanged Man Animations Set"), Function(CallingConventions.Microsoft)]
public unsafe delegate bool* HangedManAnimSetFunc(long btlUnitInfo, int param2);

[DelegateName("Nyx Avatar Animations Set"), Function(CallingConventions.Microsoft)]
public delegate ushort NyxAvatarAnimSetFunc(long btlUnitInfo, ushort param2);

[DelegateName("Check FeMC"), Function(CallingConventions.Microsoft)]
public unsafe delegate bool* IsFeMCFunc();