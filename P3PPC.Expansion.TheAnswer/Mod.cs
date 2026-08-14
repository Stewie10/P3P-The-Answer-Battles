using P3PPC.Expansion.TheAnswer.Template;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Definitions.X64;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using Reloaded.Memory.Sources;
using Reloaded.Mod.Interfaces;
using IReloadedHooks = Reloaded.Hooks.ReloadedII.Interfaces.IReloadedHooks;
using CriFs.V2.Hook.Interfaces;
using Reloaded.Universal.Localisation.Framework.Interfaces;
using Reloaded.Memory;
using System.Security.Cryptography.X509Certificates;
using static P3PPC.Expansion.TheAnswer.InternalUtils;
using static P3PPC.Expansion.TheAnswer.Anim;
using System.Drawing;
using System.ComponentModel.Design;

namespace P3PPC.Expansion.TheAnswer
{
    /// <summary>
    /// Your mod logic goes here.
    /// </summary>
    public unsafe class Mod : ModBase // <= Do not Remove.
    {
        /// <summary>
        /// Provides access to the mod loader API.
        /// </summary>
        private readonly IModLoader _modLoader;

        /// <summary>
        /// Provides access to the Reloaded.Hooks API.
        /// </summary>
        /// <remarks>This is null if you remove dependency on Reloaded.SharedLib.Hooks in your mod.</remarks>
        private readonly IReloadedHooks? _hooks;

        /// <summary>
        /// Provides access to the Reloaded logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Entry point into the mod, instance that created this class.
        /// </summary>
        private readonly IMod _owner;

        /// <summary>
        /// The configuration of the currently executing mod.
        /// </summary>
        private readonly IModConfig _modConfig;

        private Memory _memory;

        private IAsmHook _setAkihikoKenBattleCombatInfoHook;
        private IAsmHook _setAkihikoKenBattleAnimHook;
        private IAsmHook _setAkihikoKenBattleBtlUnitHook;
        private IAsmHook _setAkihikoKenBattleCharPosHook;
        private IAsmHook _setAkihikoKenBattleSomeAIEventHook;
        private IAsmHook _setAkihikoKenBattleBEDFileHook;
        private IHook<PersonaBtlUnitDelegate> _setPersonaBtlUnitHook;

        private IReverseWrapper<AkihikoKenBattleCombatInfoDelegate> _akihikoKenBattleCombatInfoReverseWrapper;
        private IReverseWrapper<AkihikoKenBattleBEDFileDelegate> _akihikoKenBattleBEDFileReverseWrapper;
        private IReverseWrapper<AkihikoKenBattleAnimDelegate> _akihikoKenBattleAnimReverseWrapper;
        private IReverseWrapper<AkihikoKenBattleBtlUnitDelegate> _akihikoKenBattleBtlUnitReverseWrapper;

        private TimeSpan movementDelay = TimeSpan.FromMilliseconds(100);
        private TimeSpan movementInitialDelay = TimeSpan.FromMilliseconds(230);

        private uint _encounterID;
        private int _btlUnit;
        private nint _param2;
        private int _setBtlUnit;
        private nint _btlUnitInfo;
        private int _param3;

        private Language _language;

        public Mod(ModContext context)
        {
            _modLoader = context.ModLoader;
            _hooks = context.Hooks;
            _logger = context.Logger;
            _owner = context.Owner;
            _modConfig = context.ModConfig;

            _memory = Memory.Instance;

            var startupScannerController = _modLoader.GetController<IStartupScanner>();
            if (startupScannerController == null || !startupScannerController.TryGetTarget(out var startupScanner))
            {
                Utils.LogError($"Unable to get controller for Reloaded SigScan Library, stuff won't work :(");
                return;
            }

            var criFsController = _modLoader.GetController<ICriFsRedirectorApi>();
            if (criFsController == null || !criFsController.TryGetTarget(out var criFsApi))
            {
                Utils.LogError($"Unable to get controller for CriFs Lib, things will not work :(");
                return;
            }

            var localisationFrameworkController = _modLoader.GetController<ILocalisationFramework>();
            if (localisationFrameworkController == null || !localisationFrameworkController.TryGetTarget(out var localisationFrameworkApi))
            {
                Utils.LogError($"Unable to get controller for Localisation Framework, things will not work :(");
                return;
            }

            if (!localisationFrameworkApi.TryGetLanguage(out _language))
            {
                Utils.LogError("Failed to get the language from localisation framework. Things might look funny...");
                _language = Language.English;
            }

            /*startupScanner.AddMainModuleScan("48 81 C1 58 83 00 00 48 8B 05 ?? ?? ?? ?? 48 89 88 ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 81 48 ?? ?? ?? ?? ??", result =>
                {

                    string[] function =
                    {
                        "use64",
                        "add rcx, 0x8358",
                        "jmp combatInfoSet",
                        $"{_hooks.Utilities.GetAbsoluteJumpMnemonics(AkihikoKenBattleCombatInfo, out _akihikoKenBattleCombatInfoReverseWrapper)}",
                        "label combatInfoSet",
                        "mov rax, [qword 0x1408CD418]",
                    };
                    _setAkihikoKenBattleCombatInfoHook = _hooks.CreateAsmHook(function, result.Offset + Utils.BaseAddress, AsmHookBehaviour.ExecuteFirst).Activate();
                });*/

            /*startupScanner.AddMainModuleScan("48 8D 0D 57 FB 96 FC EB 29 48 8D 0D ?? ?? ?? ?? EB 20 48 8D 0D ?? ?? ?? ?? EB 17 E8 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ??", result =>
            {
                string* bedFile = (string*)_memory.Allocate(4);
                nint bedFilePtr = _hooks.Utilities.WritePointer((nint)bedFile);
                *bedFile = "battle/boss/e1AE.bin";

                string[] function =
                {
                        "use64",
                        $"lea rcx, [qword {bedFilePtr}]",
                        "jmp endLabel",
                        $"{_hooks.Utilities.GetAbsoluteJumpMnemonics(AkihikoKenBattleBEDFile, out _akihikoKenBattleBEDFileReverseWrapper)}",
                        "label endLabel",
                        "mov edx, 1",
                    };
                _setAkihikoKenBattleBEDFileHook = _hooks.CreateAsmHook(function, result.Offset + Utils.BaseAddress, AsmHookBehaviour.ExecuteFirst).Activate();
            });*/

            startupScanner.AddMainModuleScan("66 3B C6 E9 ?? ?? ?? ?? 0F B7 4B 1E F6 C1 01 0F 84 ?? ?? ?? ??", result =>
            {

                string[] function =
                {
                        "use64",
                        "cmp ax, si",
                        "jmp endLabel",
                        $"{_hooks.Utilities.GetAbsoluteJumpMnemonics(AkihikoKenBattleBtlUnit, out _akihikoKenBattleBtlUnitReverseWrapper)}",
                        "label endLabel",
                        "mov rbx, [rsp + 48]",
                    };
                _setAkihikoKenBattleBtlUnitHook = _hooks.CreateAsmHook(function, result.Offset + Utils.BaseAddress, AsmHookBehaviour.ExecuteFirst).Activate();
            });

            startupScanner.AddMainModuleScan("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 48 89 CD 48 63 F2 48 8D 0D ?? ?? ?? ??", result =>
            {

                if (!result.Found)
                {
                    Utils.LogError($"Unable to find PersonaBtlUnit, stuff won't work :(");
                    return;
                }
                Utils.LogDebug($"Found PersonaBtlUnit at 0x{result.Offset + Utils.BaseAddress:X}");

                _setPersonaBtlUnitHook = _hooks.CreateHook<PersonaBtlUnitDelegate>(PersonaBtlUnit, Utils.BaseAddress + result.Offset).Activate();
            });

            startupScanner.AddMainModuleScan("48 83 C4 20 5F C3 80 BB A2 00 00 00 01 0F 85 8D 00 00 00", result =>
            {

                string[] function =
                {
                        "use64",
                        "add rsp, 32",
                        "pop rdi",
                        "ret",
                        $"{_hooks.Utilities.GetAbsoluteJumpMnemonics(AkihikoKenBattleAnim, out _akihikoKenBattleAnimReverseWrapper)}",
                    };
                _setAkihikoKenBattleAnimHook = _hooks.CreateAsmHook(function, result.Offset + Utils.BaseAddress, AsmHookBehaviour.ExecuteFirst).Activate();
            });

        }

        /*private void AkihikoKenBattleCombatInfo(nint param1, uint encounterID)
        {
            long combatInfoSetAddress = 0x1408CD408;
            long combatInfoAddress = 0x1408CD418;
            uint encounter = encounterID;
            encounter = 446;
            {
            switch (encounter) {
                    case 446:
                        _inAkihikoKenBattleCombatInfo = 0xfffeffff;
                        _inAkihikoKenBattleCombatInfo = 0xfbffffff;
                        _inAkihikoKenBattleCombatInfo = 1;
                        (combatInfoAddress + 16) |= 2;
                        (combatInfoAddress + 16) |= 8;
                        (nint)combatInfoAddress + 16 |= 16;
                        param1 = (nint)combatInfoSetAddress + 35492;
                        break;
                }
            }
        }*/

        /*private void AkihikoKenBattleBEDFile(uint encounterID, string bedDirectory)
        {
            uint encounter = encounterID;
            encounter = 446;
            {
                switch (encounter)
                {
                    case 446:
                        bedDirectory = "battle/boss/e1BE.bin";
                        break;
                }
            }
        }*/

        private void AkihikoKenBattleBtlUnit(nint param1)
        {
            _encounterID = 446;
            {
                switch (_encounterID)
                {
                    case 446:
                        _param2 = (int)(param1 + 30);
                        if ((_param2 & 1) == 0)
                            return;
                        _btlUnitInfo = (param1 + 56);
                        if ((byte)(_btlUnitInfo + 162) != 1)
                            return;
                        _setBtlUnit = (int)(_btlUnitInfo + 164);
                        _btlUnit = _setBtlUnit;
                        if (_setBtlUnit >= 235 && _setBtlUnit <= 236)
                        {
                            _param2 = (int)(param1 + 30) | 0x210;
                            _param3 = (int)(_btlUnitInfo + 3296);
                            _param2 = (int)(_btlUnitInfo + 156) | 0x1C0;
                            _param3 |= 0x40;
                            _btlUnit = (int)(_btlUnitInfo + 164);
                        }
                        if (_btlUnit == 235)
                        {
                            _setPersonaBtlUnitHook.OriginalFunction(_btlUnitInfo, 203);
                            return;
                        }
                        else
                        {
                            if (_btlUnit != 236)
                            _setPersonaBtlUnitHook.OriginalFunction(_btlUnitInfo, 205);
                            return;
                        }
                }
            }
        }

        private void PersonaBtlUnit(nint setBtlUnit, int personaID)
        {
            _setPersonaBtlUnitHook.OriginalFunction(setBtlUnit, personaID);
        }

        private void AkihikoKenBattleAnim(nint btlUnitInfo, uint animArray)
        {
            long* animArrayAddress = (long*)_memory.Allocate(4);
            nint animArrayAddressPtr = _hooks.Utilities.WritePointer((nint)animArrayAddress);
            *animArrayAddress = 0x14071C390;
            _encounterID = 446;
            {
                switch (_encounterID)
                {
                    case 446:
                        if ((byte)(btlUnitInfo + 162) != 1)
                            return;
                        if ((int)(btlUnitInfo + 164) == 235)
                            return;
                            animArrayAddressPtr = (nint)(animArray);
                        if ((int)(btlUnitInfo + 164) != 236)
                            return;
                        animArrayAddressPtr = (nint)(animArray + 28);
                        return;
                }
            }
        }

        [Function(CallingConventions.Microsoft)]
        private delegate void AkihikoKenBattleCombatInfoDelegate(nint param1, uint encounterID);

        [Function(CallingConventions.Microsoft)]
        private delegate void AkihikoKenBattleBEDFileDelegate(uint encounterID, string bedDirectory);

        [Function(CallingConventions.Microsoft)]
        private delegate void AkihikoKenBattleAnimDelegate(nint btlUnitInfo, uint animArray);

        [Function(CallingConventions.Microsoft)]
        private delegate void AkihikoKenBattleBtlUnitDelegate(nint param1);

        [Function(CallingConventions.Microsoft)]
        private delegate void PersonaBtlUnitDelegate(nint setBtlUnit, int personaID);

        #region Standard Overrides
        #endregion

        #region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Mod() { }
#pragma warning restore CS8618
        #endregion
    }
}