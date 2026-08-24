using P3PPC.Expansion.TheAnswer.FunctionDelegates;
using P3PPC.Expansion.TheAnswer.IFaces;
using System.Diagnostics.CodeAnalysis;

namespace P3PPC.Expansion.TheAnswer.BtlData;

public unsafe static class AllOutAttackAcceptVoiceHook
{
    private static IP3PInfo? infoProvider;

    public static void HookInfoProvider(IP3PInfo provider)
    {
        provider.Hook();
        infoProvider = provider;
    }

        public static void AllOutAttackAcceptVoice()
        {
            //not added yet
        }
    }
