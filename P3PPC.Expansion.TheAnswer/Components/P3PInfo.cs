using P3PPC.Expansion.TheAnswer.FunctionDelegates;
using P3PPC.Expansion.TheAnswer.IFaces;
using Reloaded.Hooks.Definitions;
using Reloaded.Memory;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;

namespace P3PPC.Expansion.TheAnswer.Components;

public unsafe class P3PInfo : BaseService, IP3PInfo
{
    private IsFeMCFunc? isFeMCFunc;
    private BossBtlCombatInfoFunc? bossBtlCombatInfoFunc;
    private PersonaBtlUnitSetFunc? personaBtlUnitSetFunc;
    private BossBtlUnitAnimFunc? bossBtlUnitAnimFunc;
    private BossBtlUnitFunc? bossBtlUnitFunc;
    private BossBtlBEDFilesFunc? bossBtlBEDFilesFunc;
    private HermitChargeFunc? hermitChargeFunc;
    private HangedManAnimSetFunc? hangedManAnimSetFunc;
    private NyxAvatarAnimSetFunc? nyxAvatarAnimSetFunc;
    public P3PInfo(IStartupScanner startup, IReloadedHooks hooks, Memory memory) : base(startup, hooks, memory)
    {
    }
    public override void Hook()
    {
        /*startup.SetDelegateScanWrapper(hooks, ref isFeMCFunc, () =>
        {
            Log.Error("Failed to hook IsFeMCFunc");
        });*/

        startup.SetDelegateScanWrapper(hooks, ref bossBtlCombatInfoFunc, () =>
        {
            Log.Error("Failed to hook BossBtlCombatInfoFunc");
        });

        startup.SetDelegateScanWrapper(hooks, ref personaBtlUnitSetFunc, () =>
        {
            Log.Error("Failed to hook PersonaBtlUnitSetFunc");
        });

        startup.SetDelegateScanWrapper(hooks, ref bossBtlUnitAnimFunc, () =>
        {
            Log.Error("Failed to hook BossBtlUnitAnimFunc");
        });

        startup.SetDelegateScanWrapper(hooks, ref bossBtlUnitFunc, () =>
        {
            Log.Error("Failed to hook BossBtlUnitFunc");
        });

        startup.SetDelegateScanWrapper(hooks, ref bossBtlBEDFilesFunc, () =>
        {
            Log.Error("Failed to hook BossBtlBEDFilesFunc");
        });

        /*startup.SetDelegateScanWrapper(hooks, ref hermitChargeFunc, () =>
        {
            Log.Error("Failed to hook HermitChargeFunc");
        });

        startup.SetDelegateScanWrapper(hooks, ref hangedManAnimSetFunc, () =>
        {
            Log.Error("Failed to hook HangedManAnimSetFunc");
        });

        startup.SetDelegateScanWrapper(hooks, ref nyxAvatarAnimSetFunc, () =>
        {
            Log.Error("Failed to hook NyxAvatarAnimSetFunc");
        });*/
    }

    #region Pointer Properties
    //public bool* IsFeMC => isFeMCFunc?.Invoke();

    public long* BossBtlCombatInfo => bossBtlCombatInfoFunc?.Invoke();

    public long PersonaBtlUnitSet(long setBtlUnit, short personaID)
    {
        if (personaBtlUnitSetFunc == null)
        {
            Log.Error("PersonaBtlUnitSetFunc is not hooked.");
            return 0;
        }
        long* resultPtr = (long*)personaBtlUnitSetFunc.Invoke(setBtlUnit, personaID);
        if (resultPtr == null)
        {
            Log.Error("PersonaBtlUnitSetFunc returned a null pointer.");
            return 0;
        }
        return *resultPtr;
    }

    public long BossBtlUnitAnim(long btlUnitInfo, ushort animArray)
    {
        if (bossBtlUnitAnimFunc == null)
        {
            Log.Error("BossBtlUnitAnimFunc is not hooked.");
            return 0;
        }
        long* resultPtr = (long*)bossBtlUnitAnimFunc.Invoke(btlUnitInfo, animArray);
        if (resultPtr == null)
        {
            Log.Error("BossBtlUnitAnimFunc returned a null pointer.");
            return 0;
        }
        return *resultPtr;
    }

    public long BossBtlUnit(long param1)
    {
        if (bossBtlUnitFunc == null)
        {
            Log.Error("BossBtlUnitFunc is not hooked.");
            return 0;
        }
        long* resultPtr = (long*)bossBtlUnitFunc.Invoke(param1);
        if (resultPtr == null)
        {
            Log.Error("BossBtlUnitFunc returned a null pointer.");
            return 0;
        }
        return *resultPtr;
    }
    public long* BossBtlBEDFiles => bossBtlBEDFilesFunc?.Invoke();

    /*public long HermitCharge(int btlUnit, long combatInfoAddress, long encounter, nint param4)
    {
        if (hermitChargeFunc == null)
        {
            Log.Error("HermitChargeFunc is not hooked.");
            return 0;
        }
        long* resultPtr = (long*)hermitChargeFunc.Invoke(btlUnit, combatInfoAddress, encounter, param4);
        if (resultPtr == null)
        {
            Log.Error("HermitChargeFunc returned a null pointer.");
            return 0;
        }
        return *resultPtr;
    }

    public bool HangedManAnimSet(long btlUnitInfo, int param2)
    {
        if (hangedManAnimSetFunc == null)
        {
            Log.Error("HangedManAnimSetFunc is not hooked.");
            return false;
        }
        bool* resultPtr = hangedManAnimSetFunc.Invoke(btlUnitInfo, param2);
        if (resultPtr == null)
        {
            Log.Error("HangedManAnimSetFunc returned a null pointer.");
            return false;
        }
        return *resultPtr;
    }

    public ushort NyxAvatarAnimSet(long btlUnitInfo, ushort param2)
    {
        if (nyxAvatarAnimSetFunc == null)
        {
            Log.Error("NyxAvatarAnimSetFunc is not hooked.");
            return 0;
        }
        ushort* resultPtr = (ushort*)nyxAvatarAnimSetFunc.Invoke(btlUnitInfo, param2);
        if (resultPtr == null)
        {
            Log.Error("NyxAvatarAnimSetFunc returned a null pointer.");
            return 0;
        }
        return *resultPtr;
    }*/
    #endregion
    #region Functions

    #endregion

}
