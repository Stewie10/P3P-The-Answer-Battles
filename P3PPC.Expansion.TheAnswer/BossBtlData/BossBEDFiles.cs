using P3PPC.Expansion.TheAnswer.FunctionDelegates;
using P3PPC.Expansion.TheAnswer.IFaces;
using System.Diagnostics.CodeAnalysis;

namespace P3PPC.Expansion.TheAnswer.BossBtlData;

public unsafe static class BossBtlBEDFilesHook
{
    private static IP3PInfo? infoProvider;

    public static void HookInfoProvider(IP3PInfo provider)
    {
        provider.Hook();
        infoProvider = provider;
    }

        public static long BossBtlBEDFiles()
        {
            long result;
            string bedDirectory;
            long combatInfoAddress = 0x1408CD418;
            result = *(int*)*((long*)(combatInfoAddress + 4456) + 16) - 444;
            long encounter = *(int*)*(long*)(combatInfoAddress + 4456) + 16;
            {
                switch (encounter)
                {
                    case 444:
                        bedDirectory = "battle/boss/e1BC.bin";
                        break;
                    case 445:
                        bedDirectory = "battle/boss/e1BD.bin";
                        break;
                    case 446:
                        bedDirectory = "battle/boss/e1BE.bin";
                        break;
                    case 447:
                        bedDirectory = "battle/boss/e1BF.bin";
                        break;
                    case 448:
                        bedDirectory = "battle/boss/e1C0.bin";
                        break;
                    case 449:
                        bedDirectory = "battle/boss/e1C1.bin";
                        break;
                    case 450:
                        bedDirectory = "battle/boss/e1C2.bin";
                        break;
                }
            return result;
            }
        }
    }
