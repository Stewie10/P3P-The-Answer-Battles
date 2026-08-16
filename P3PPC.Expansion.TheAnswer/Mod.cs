using P3PPC.Expansion.TheAnswer.Configuration;
using P3PPC.Expansion.TheAnswer.Template;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Definitions.X64;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using Reloaded.Memory.Sources;
using Reloaded.Mod.Interfaces;
using IReloadedHooks = Reloaded.Hooks.ReloadedII.Interfaces.IReloadedHooks;
using static P3PPC.Expansion.TheAnswer.Models.Personas;
using static P3PPC.Expansion.TheAnswer.Models.BtlUnits;
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
        /// Provides access to this mod's configuration.
        /// </summary>
        private Config _configuration;

        /// <summary>
        /// The configuration of the currently executing mod.
        /// </summary>
        private readonly IModConfig _modConfig;

        private IMemory _memory;

        private IAsmHook _setAnswerBattlesCombatInfoHook;
        private IAsmHook _setAnswerBattlesAnimHook;
        private IAsmHook _setAnswerBattlesBtlUnitHook;
        private IAsmHook _setAnswerBattlesCharPosHook;
        private IAsmHook _setAnswerBattlesSomeAIEventHook;
        private IAsmHook _setAnswerBattlesBEDFileHook;
        private IHook<PersonaBtlUnitDelegate> _setPersonaBtlUnitHook;
        private IHook<MemorySetDelegate> _setMemorySetHook;

        private IReverseWrapper<AnswerBattlesCombatInfoDelegate> _answerBattlesCombatInfoReverseWrapper;
        private IReverseWrapper<AnswerBattlesBEDFileDelegate> _answerBattlesBEDFileReverseWrapper;
        private IReverseWrapper<AnswerBattlesAnimDelegate> _answerBattlesAnimReverseWrapper;
        private IReverseWrapper<AnswerBattlesBtlUnitDelegate> _answerBattlesBtlUnitReverseWrapper;
        private IReverseWrapper<MemorySetDelegate> _memorySetReverseWrapper;

        private uint _encounterID;
        private nint _param2;
        private int _btlUnitInt;
        private int _setBtlUnit;
        private nint _btlUnitInfo;
        private int _param3;
        private Persona _personaUnit;
        private BtlUnit _btlUnit;
        private bool _btlUnitSet;

        public Mod(ModContext context)
        {
            _modLoader = context.ModLoader;
            _hooks = context.Hooks;
            _logger = context.Logger;
            _owner = context.Owner;
            _modConfig = context.ModConfig;

            _memory = Memory.Instance;

            Utils.Initialise(_logger, _configuration);

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

            string memorySetCall = _hooks.Utilities.GetAbsoluteJumpMnemonics(MemorySet, out _memorySetReverseWrapper);

            /*startupScanner.AddMainModuleScan("48 81 C1 58 83 00 00 48 8B 05 ?? ?? ?? ?? 48 89 88 ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 81 48 ?? ?? ?? ?? ??", result =>
                {

                    string[] function =
                    {
                        "use64",
                        "add rcx, 0x8358",
                        "jmp combatInfoSet",
                        $"{_hooks.Utilities.GetAbsoluteJumpMnemonics(AnswerBattlesCombatInfo, out _answerBattlesCombatInfoReverseWrapper)}",
                        "label combatInfoSet",
                        "mov rax, [qword 0x1408CD418]",
                    };
                    _setAnswerBattlesCombatInfoHook = _hooks.CreateAsmHook(function, result.Offset + Utils.BaseAddress, AsmHookBehaviour.ExecuteFirst).Activate();
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
                        $"{_hooks.Utilities.GetAbsoluteJumpMnemonics(AnswerBattlesBEDFile, out _answerBattlesBEDFileReverseWrapper)}",
                        "label endLabel",
                        "mov edx, 1",
                    };
                _setAnswerBattlesBEDFileHook = _hooks.CreateAsmHook(function, result.Offset + Utils.BaseAddress, AsmHookBehaviour.ExecuteFirst).Activate();
            });*/

            startupScanner.AddMainModuleScan("48 89 5C 24 08 48 89 74 24 10 57 48 83 EC 20 48 8B D9 48 8D 0D 6A BE 64 03 E8 6A 41 3F 00", result =>
            {
                byte* memorySetAddress = (byte*)_memory.Allocate(4);
                nint memorySetAddressPtr = _hooks.Utilities.WritePointer((nint)memorySetAddress);
                *memorySetAddress = 1;

                string[] function =
                {
                        "use64",
                        "mov [rsp + 8], rbx",
                        "mov [rsp + 16], rsi",
                        "push rdi",
                        "sub rsp, 32",
                        "mov rbx, rcx",
                        $"lea rcx, [qword {memorySetAddressPtr}]",
                        $"{memorySetCall}",
                        $"{_hooks.Utilities.GetAbsoluteJumpMnemonics(AnswerBattlesBtlUnit, out _answerBattlesBtlUnitReverseWrapper)}",
                    };
                _setAnswerBattlesBtlUnitHook = _hooks.CreateAsmHook(function, result.Offset + Utils.BaseAddress, AsmHookBehaviour.ExecuteFirst).Activate();
            });

            startupScanner.AddMainModuleScan("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 48 89 CD 48 63 F2 48 8D 0D ?? ?? ?? ??", result =>
            {

                _setPersonaBtlUnitHook = _hooks.CreateHook<PersonaBtlUnitDelegate>(PersonaBtlUnit, Utils.BaseAddress + result.Offset).Activate();
            });

            startupScanner.AddMainModuleScan("48 89 5C 24 08 48 89 74 24 10 57 48 83 EC 20 48 8B D9 0F B7 FA 48 8D 0D F7 61 65 03 E8 F7 E4 3F 00", result =>
            {
                byte* memorySetAddress = (byte*)_memory.Allocate(4);
                nint memorySetAddressPtr = _hooks.Utilities.WritePointer((nint)memorySetAddress);
                *memorySetAddress = 1;

                string[] function =
                {
                        "use64",
                        "mov [rsp + 8], rbx",
                        "mov [rsp + 16], rsi",
                        "push rdi",
                        "sub rsp, 32",
                        "mov rbx, rcx",
                        "movzx edi, dx",
                        $"lea rcx, [qword {memorySetAddressPtr}]",
                        $"{memorySetCall}",
                        $"{_hooks.Utilities.GetAbsoluteJumpMnemonics(AnswerBattlesAnim, out _answerBattlesAnimReverseWrapper)}",
                    };
                _setAnswerBattlesAnimHook = _hooks.CreateAsmHook(function, result.Offset + Utils.BaseAddress, AsmHookBehaviour.ExecuteFirst).Activate();
            });

            startupScanner.AddMainModuleScan("48 89 4C 24 08 48 83 EC 38 48 8B 44 24 40 48 89 44 24 20 48 8B 44 24 40 0F B6 00", result =>
            {

                _setMemorySetHook = _hooks.CreateHook<MemorySetDelegate>(MemorySet, Utils.BaseAddress + result.Offset).Activate();
            });

        }

        /*private void AnswerBattlesCombatInfo(nint param1, uint encounterID)
        {
            long* combatInfoSetAddress = (long*)_memory.Allocate(4);
            nint combatInfoSetAddressPtr = _hooks.Utilities.WritePointer((nint)combatInfoSetAddress);
            *combatInfoSetAddress = 0x1408CD408;
            long* combatInfoAddress = (long*)_memory.Allocate(4);
            nint combatInfoAddressPtr = _hooks.Utilities.WritePointer((nint)combatInfoAddress);
            *combatInfoAddress = 0x1408CD418;
            encounterID = 446;
            {
            switch (encounterID) {
                    case 446:
                        combatInfoAddress + 12 = 0xfffeffff;
                        (uint*)combatInfoAddress + 12 = 0xfbffffff;
                        combatInfoAddress = 1;
                        (combatInfoAddress + 16) |= 2;
                        (combatInfoAddress + 16) |= 8;
                        (nint)combatInfoAddress + 16 |= 16;
                        param1 = (nint)combatInfoSetAddress + 35492;
                        break;
                }
            }
        }*/

        /*private void AnswerBattlesBEDFile(uint encounterID, string bedDirectory)
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

        private nint AnswerBattlesBtlUnit(nint param1)
        {
            long* combatInfoAddress = (long*)_memory.Allocate(4);
            nint combatInfoAddressPtr = _hooks.Utilities.WritePointer((nint)combatInfoAddress);
            *combatInfoAddress = 0x1408CD418;
            {
                switch (*(int*)*(nint*)(combatInfoAddressPtr + 4456) + 4)
                {
                    case 416:
                    case 431:
                        if (((byte)(param1 + 30) & 1) == 0)
                            return 1;
                        _btlUnitInfo = (param1 + 56);
                        if ((byte)(_btlUnitInfo + 162) != 1)
                            return 1;
                        _btlUnitInt = (int)(_btlUnitInfo + 164) - 243;
                        _btlUnitSet = (int)(_btlUnitInfo + 164) == 243;
                        goto BtlUnitSet;
                    BtlUnitSet:
                        if (!_btlUnitSet && _btlUnitInt != 13)
                            return 1;
                        *(int*)(_btlUnitInfo + 156) |= 0x540;
                        return 1;
                    case 446:
                            _param2 = (int)(param1 + 30);
                            if ((_param2 & 1) == 0)
                                return 1;
                            _btlUnitInfo = (param1 + 56);
                            if ((byte)(_btlUnitInfo + 162) != 1)
                                return 1;
                            _btlUnit = (BtlUnit)(_btlUnitInfo + 164);
                            if (_btlUnit >= BtlUnit.Akihiko && _btlUnit <= BtlUnit.Ken)
                            {
                                *(nint*)(param1 + 30) = _param2 | 0x210;
                                _param3 = (int)(_btlUnitInfo + 3296);
                                *(int*)(_btlUnitInfo + 156) |= 0x1C0;
                                _param3 |= 0x40;
                                _btlUnit = (BtlUnit)(_btlUnitInfo + 164);
                            }
                            if (_btlUnit == BtlUnit.Akihiko)
                            {
                                _setPersonaBtlUnitHook.OriginalFunction(_btlUnitInfo, Persona.Caesar);
                                return 1;
                            }
                            else
                            {
                                if (_btlUnit != BtlUnit.Ken)
                                    _setPersonaBtlUnitHook.OriginalFunction(_btlUnitInfo, Persona.KalaNemi);
                                return 1;
                            }
                    default:
                        return 0;
                        }
            }
        }

        private void PersonaBtlUnit(nint setBtlUnit, Persona personaID)
        {
            _setPersonaBtlUnitHook.OriginalFunction(setBtlUnit, personaID);
        }

        private nint AnswerBattlesAnim(nint btlUnitInfo, byte animArray)
        {
            long* animArrayAddress = (long*)_memory.Allocate(4);
            nint animArrayAddressPtr = _hooks.Utilities.WritePointer((nint)animArrayAddress);
            *animArrayAddress = 0x14071C390;
            long* combatInfoAddress = (long*)_memory.Allocate(4);
            nint combatInfoAddressPtr = _hooks.Utilities.WritePointer((nint)combatInfoAddress);
            *combatInfoAddress = 0x1408CD418;
            {
                switch (*(int*)*(nint*)(combatInfoAddressPtr + 4456) + 4)
                {
                    case 416:
                    case 431:
                        if ((byte)(btlUnitInfo + 162) != 1 || (BtlUnit)(btlUnitInfo + 164) != BtlUnit.PriestessRematch && (BtlUnit)(btlUnitInfo + 164) != BtlUnit.Priestess)
                        {
                            return -1;
                        }
                        return animArrayAddressPtr = animArray;
                    case 446:
                        if ((byte)(btlUnitInfo + 162) != 1)
                            return -1;
                        if ((BtlUnit)(btlUnitInfo + 164) == BtlUnit.Akihiko)
                            return animArrayAddressPtr = animArray;
                        if ((BtlUnit)(btlUnitInfo + 164) != BtlUnit.Ken)
                            return -1;
                        return animArrayAddressPtr = (animArray + 28);
                    default:
                        return 0;
                }
            }
        }

        private void MemorySet(byte* address)
        {
            _setMemorySetHook.OriginalFunction(address);
        }

        [Function(CallingConventions.Microsoft)]
        private delegate void AnswerBattlesCombatInfoDelegate(nint param1, uint encounterID);

        [Function(CallingConventions.Microsoft)]
        private delegate void AnswerBattlesBEDFileDelegate(uint encounterID, string bedDirectory);

        [Function(CallingConventions.Microsoft)]
        private delegate nint AnswerBattlesAnimDelegate(nint btlUnitInfo, byte animArray);

        [Function(CallingConventions.Microsoft)]
        private delegate nint AnswerBattlesBtlUnitDelegate(nint param1);

        [Function(CallingConventions.Microsoft)]
        private delegate void PersonaBtlUnitDelegate(nint setBtlUnit, Persona personaID);

        [Function(CallingConventions.Microsoft)]
        private delegate void MemorySetDelegate(byte* address);

        #region Standard Overrides
        #endregion

        #region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Mod() { }
#pragma warning restore CS8618
        #endregion
    }
}