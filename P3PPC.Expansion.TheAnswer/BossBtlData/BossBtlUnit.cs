using P3PPC.Expansion.TheAnswer.FunctionDelegates;
using P3PPC.Expansion.TheAnswer.IFaces;
using static P3PPC.Expansion.TheAnswer.Models.BtlUnits;
using static P3PPC.Expansion.TheAnswer.Models.Personas;
using System.Diagnostics.CodeAnalysis;

namespace P3PPC.Expansion.TheAnswer.BossBtlData;

    public unsafe static class BossBtlUnitHook
    {
        private static IP3PInfo? infoProvider;

        private static long _param2;
        private static int _btlUnitInt;
        private static long _btlUnitInfo;
        private static int _param3;
        private static BtlUnit _btlUnit;
        private static Persona _personaUnit;

        public static void HookInfoProvider(IP3PInfo provider)
        {
            provider.Hook();
            infoProvider = provider;
        }
        public static long BossBtlUnit(long param1)
        {
            long combatInfoAddress = 0x1408CD418;
            long encounter = *(int*)*(long*)(combatInfoAddress + 4456) + 16;
            {
                switch (encounter)
                {
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
                            *(long*)(param1 + 30) = _param2 | 0x210;
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
                    case 447:
                        _param2 = (int)(param1 + 30);
                        if ((_param2 & 1) == 0)
                            return 1;
                        _btlUnitInfo = (param1 + 56);
                        if ((byte)(_btlUnitInfo + 162) != 1)
                            return 1;
                        _btlUnit = (BtlUnit)(_btlUnitInfo + 164);
                        if (_btlUnit >= BtlUnit.Koromaru && _btlUnit <= BtlUnit.Junpei)
                        {
                            *(long*)(param1 + 30) = _param2 | 0x210;
                            _param3 = (int)(_btlUnitInfo + 3296);
                            *(int*)(_btlUnitInfo + 156) |= 0x1C0;
                            _param3 |= 0x40;
                            _btlUnit = (BtlUnit)(_btlUnitInfo + 164);
                        }
                        if (_btlUnit == BtlUnit.Koromaru)
                        {
                            _setPersonaBtlUnitHook.OriginalFunction(_btlUnitInfo, Persona.Cerberus);
                            return 1;
                        }
                        else
                        {
                            if (_btlUnit != BtlUnit.Junpei)
                                _setPersonaBtlUnitHook.OriginalFunction(_btlUnitInfo, Persona.Trismegistus);
                                return 1;
                        }
                    case 448:
                        _param2 = (int)(param1 + 30);
                        if ((_param2 & 1) == 0)
                            return 1;
                        _btlUnitInfo = (param1 + 56);
                        if ((byte)(_btlUnitInfo + 162) != 1)
                            return 1;
                        _btlUnit = (BtlUnit)(_btlUnitInfo + 164);
                        if (_btlUnit >= BtlUnit.Yukari && _btlUnit <= BtlUnit.Mitsuru)
                        {
                            *(long*)(param1 + 30) = _param2 | 0x210;
                            _param3 = (int)(_btlUnitInfo + 3296);
                            *(int*)(_btlUnitInfo + 156) |= 0x1C0;
                            _param3 |= 0x40;
                            _btlUnit = (BtlUnit)(_btlUnitInfo + 164);
                        }
                        if (_btlUnit == BtlUnit.Yukari)
                        {
                            _setPersonaBtlUnitHook.OriginalFunction(_btlUnitInfo, Persona.Isis);
                            return 1;
                        }
                        else
                        {
                            if (_btlUnit != BtlUnit.Mitsuru)
                                _setPersonaBtlUnitHook.OriginalFunction(_btlUnitInfo, Persona.Artemisia);
                                return 1;
                        }
                    default:
                        return 0;
                        }
            }
        }

        
}