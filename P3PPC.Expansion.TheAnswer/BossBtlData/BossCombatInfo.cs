using P3PPC.Expansion.TheAnswer.FunctionDelegates;
using P3PPC.Expansion.TheAnswer.IFaces;
using System.Diagnostics.CodeAnalysis;

namespace P3PPC.Expansion.TheAnswer.BossBtlData;

public unsafe static class BossBtlCombatInfoHook
{
    private static IP3PInfo? infoProvider;

    public static void HookInfoProvider(IP3PInfo provider)
    {
        provider.Hook();
        infoProvider = provider;
    }

        public static long BossBtlCombatInfo()
        {
            long param1;
            long result;
            long combatInfoAddress = 0x1408CD418;
            long encounter = *(int*)*(long*)(combatInfoAddress + 4456) + 16;
            {
                switch (encounter) 
                {
                    case 446:
                        *(long*)(combatInfoAddress + 12) = 0xfffeffff;
                        *(long*)(combatInfoAddress + 12) = 0xfbffffff;
                        *(int*)combatInfoAddress = 1;
                        *(int*)(combatInfoAddress + 16) |= 2;
                        *(int*)(combatInfoAddress + 16) |= 8;
                        *(int*)(combatInfoAddress + 16) |= 16;
                        param1 = *(long*)(combatInfoAddress + 35492);
                        break;
                    default:
                    break;
                }
                result = combatInfoAddress;
                return result;
            }
        }
    }