using P3PPC.Expansion.TheAnswer.FunctionDelegates;
using P3PPC.Expansion.TheAnswer.IFaces;
using static P3PPC.Expansion.TheAnswer.Models.BtlUnits;
using System.Diagnostics.CodeAnalysis;

namespace P3PPC.Expansion.TheAnswer.BossBtlData;

public unsafe static class BossBtlUnitAnimHook
{
    private static IP3PInfo? infoProvider;

    private static BtlUnit _btlUnit;
    private static int _btlUnitHermit;
    private static nint _hermitChargeParam4;
    private static nint _combatInfoAddressNyxAvatar;

    public static void HookInfoProvider(IP3PInfo provider)
    {
        provider.Hook();
        infoProvider = provider;
    }
        public static long BossBtlUnitAnim(long btlUnitInfo, ushort animArraySet)
        {
            uint animArray = animArraySet;
            long combatInfoAddress = 0x1408CD418;
            long encounter = *(int*)*(long*)(combatInfoAddress + 4456) + 16;
            //long result;
            //int param1 = 521;
            //_combatInfoAddressNyxAvatar = (nint)combatInfoAddress;
            {
                switch (encounter)
                {
                    case 446:
                        if ((byte)(btlUnitInfo + 162) != 1)
                            return -1;
                        if ((BtlUnit)(btlUnitInfo + 164) == BtlUnit.Akihiko)
                            return ((byte*)0x14071C390)[animArray];
                        if ((BtlUnit)(btlUnitInfo + 164) != BtlUnit.Ken)
                            return -1;
                        return ((byte*)0x14071C390)[animArray + 28];
                    case 447:
                        if ((byte)(btlUnitInfo + 162) != 1)
                            return -1;
                        if ((BtlUnit)(btlUnitInfo + 164) == BtlUnit.Koromaru)
                            return ((byte*)0x14071C390)[animArray + 56];
                        if ((BtlUnit)(btlUnitInfo + 164) != BtlUnit.Junpei)
                            return -1;
                        return ((byte*)0x14071C390)[animArray + 84];
                    case 448:
                        if ((byte)(btlUnitInfo + 162) != 1)
                            return -1;
                        if ((BtlUnit)(btlUnitInfo + 164) == BtlUnit.Yukari)
                            return ((byte*)0x14071C390)[animArray + 112];
                        if ((BtlUnit)(btlUnitInfo + 164) != BtlUnit.Mitsuru)
                            return -1;
                        return ((byte*)0x14071C390)[animArray + 140];
                    default:
                        return -1;
                    /*case 416:
                    case 431:
                        if ((byte)(btlUnitInfo + 162) != 1 || (BtlUnit)(btlUnitInfo + 164) != BtlUnit.PriestessRematch && (BtlUnit)(btlUnitInfo + 164) != BtlUnit.Priestess)
                        {
                            return -1;
                        }
                        return ((byte*)0x14071C390)[animArray];
                    case 417:
                    case 432:
                        if ((byte)(btlUnitInfo + 162) != 1)
                            return -1;
                        if ((BtlUnit)(btlUnitInfo + 164) == BtlUnit.EmpressRematch)
                            return ((byte*)0x14071C390)[animArray];
                        if ((BtlUnit)(btlUnitInfo + 164) == BtlUnit.EmperorRematch)
                            return ((byte*)0x14071C390)[animArray + 28];
                        if ((BtlUnit)(btlUnitInfo + 164) != BtlUnit.Empress)
                        {
                            if ((BtlUnit)(btlUnitInfo + 164) != BtlUnit.Emperor)
                                return -1;
                            return ((byte*)0x14071C390)[animArray + 28];
                        }
                        return ((byte*)0x14071C390)[animArray];
                    case 418:
                    case 433:
                        if ((byte)(btlUnitInfo + 162) != 1 || (BtlUnit)(btlUnitInfo + 164) != BtlUnit.HierophantRematch && (BtlUnit)(btlUnitInfo + 164) != BtlUnit.Hierophant)
                        {
                            return -1;
                        }
                        return ((byte*)0x14071C390)[animArray + 32];
                    case 419:
                    case 434:
                        if ((byte)(btlUnitInfo + 162) != 1)
                            return -1;
                        _btlUnit = (BtlUnit)(btlUnitInfo + 164);
                        if (_btlUnit != BtlUnit.Lovers && _btlUnit != BtlUnit.LoversRematch)
                            return -1;
                        return ((byte*)0x14071C390)[animArray + 64];
                    case 420:
                    case 435:
                        if ((byte)(btlUnitInfo + 162) != 1)
                            return -1;
                        switch ((BtlUnit)(btlUnitInfo + 164))
                        {
                            case BtlUnit.ChariotRematch:
                            case BtlUnit.Chariot:
                                result = ((byte*)0x14071C390)[animArray + 96];
                                break;
                            case BtlUnit.JusticeRematch:
                            case BtlUnit.Justice:
                                result = ((byte*)0x14071C390)[animArray + 124];
                                break;
                            case BtlUnit.ChariotJusticeRematchDummy:
                            case BtlUnit.ChariotJusticeDummy:
                                result = ((byte*)0x14071C390)[animArray + 152];
                                break;
                            default:
                                return -1;
                        }
                        return result;
                    case 421:
                    case 436:
                        if ((byte)(btlUnitInfo + 162) != 1)
                            return -1;
                        _btlUnit = (BtlUnit)(btlUnitInfo + 164);
                        _btlUnitHermit = 263;
                        if (_btlUnit != BtlUnit.Hermit)
                        {
                            _btlUnitHermit = 250;
                            if (_btlUnit != BtlUnit.HermitRematch)
                                return -1;
                        }
                        if (*(bool*)animArray == true || animArray == 3 || animArray == 17 && encounter == 421 || encounter == 436 && (*(bool*)(combatInfoAddress + 4264) == true))
                        {
                            *(int*)(combatInfoAddress + 4272) = 1;
                            return 15;
                        }
                        else
                        {
                            if (*(bool*)HermitCharge(_btlUnitHermit, combatInfoAddress, (int)encounter, _hermitChargeParam4))
                                *(int*)(combatInfoAddress + 4272) = 0;
                            return ((byte*)0x14071C390)[animArray + 184];
                        }
                    case 422:
                    case 437:
                        if ((byte)(btlUnitInfo + 162) != 1)
                            return -1;
                        switch ((BtlUnit)(btlUnitInfo + 164))
                        {
                            case BtlUnit.FortuneRematch:
                            case BtlUnit.Fortune:
                                if (((ushort)animArray <= 9 && (BitTest(param1, animArray)) || animArray == 17) && *(int*)(combatInfoAddress + 4288) == 1)
                                {
                                    result = 6;
                                }
                                else
                                {
                                    result = ((byte*)0x14071C390)[animArray + 216];
                                }
                                break;
                            case BtlUnit.StrengthRematch:
                            case BtlUnit.Strength:
                                result = ((byte*)0x14071C390)[animArray + 244];
                                break;
                            default:
                                return -1;
                        }
                        return result;
                    case 423:
                        if ((byte)(btlUnitInfo + 162) != 1)
                            return -1;
                        if ((BtlUnit)(btlUnitInfo + 164) == BtlUnit.Takaya)
                            return ((byte*)0x14071C390)[animArray];
                        if ((BtlUnit)(btlUnitInfo + 164) != BtlUnit.Jin)
                            return -1;
                        return ((byte*)0x14071C390)[animArray + 28];
                    case 424:
                    case 438:
                        if ((byte)(btlUnitInfo + 162) != 1)
                            return -1;
                        switch ((BtlUnit)(btlUnitInfo + 164))
                        {
                            case BtlUnit.StatueRematch:
                            case BtlUnit.Statue2Rematch:
                            case BtlUnit.Statue3Rematch:
                            case BtlUnit.Statue:
                            case BtlUnit.Statue2:
                            case BtlUnit.Statue3:
                                result = ((byte*)0x14071C390)[animArray + 120];
                                break;
                            case BtlUnit.HangedManDeviousMayaRematch:
                            case BtlUnit.HangedManDeviousMaya2Rematch:
                            case BtlUnit.HangedManDeviousMaya:
                            case BtlUnit.HangedManDeviousMaya2:
                                if (animArray == 3 && HangedManAnimSet((btlUnitInfo + 3296), 2048))
                                    return 15;
                                result = ((byte*)0x14071C390)[animArray + 148];
                                break;
                            case BtlUnit.HangedManRematch:
                            case BtlUnit.HangedMan:
                                if (*(bool*)(combatInfoAddress + 4292) == true)
                                    result = ((byte*)0x14071C390)[animArray + 92];
                                else
                                    result = ((byte*)0x14071C390)[animArray + 64];
                                break;
                            default:
                                return -1;
                        }
                        return result;
                    case 425:
                        if ((byte)(btlUnitInfo + 162) != 1 || (BtlUnit)(btlUnitInfo + 164) != BtlUnit.Chidori)
                            return -1;
                        return ((byte*)0x14071C390)[animArray];
                    case 426:
                        if ((byte)(btlUnitInfo + 162) != 1 || (BtlUnit)(btlUnitInfo + 164) != BtlUnit.Jin2)
                            return -1;
                        return ((byte*)0x14071C390)[animArray + 32];
                    case 427:
                        if ((byte)(btlUnitInfo + 162) != 1 || (BtlUnit)(btlUnitInfo + 164) != BtlUnit.Takaya2)
                            return -1;
                        return ((byte*)0x14071C390)[animArray + 64];
                    case 428:
                        if ((byte)(btlUnitInfo + 162) != 1)
                            return -1;
                        if (*(bool*)animArray == true || animArray == 3 || animArray == 17 && *(byte*)_combatInfoAddressNyxAvatar == 11 || (NyxAvatarAnimSet((btlUnitInfo + 3296), *(ushort*)_combatInfoAddressNyxAvatar) > 0))
                        {
                            *(int*)(combatInfoAddress + 4280) = 1;
                            return 15;
                        }
                        else
                        {
                            *(byte*)_combatInfoAddressNyxAvatar = 11;
                            if (*(bool*)NyxAvatarAnimSet(btlUnitInfo + 3296, *(ushort*)_combatInfoAddressNyxAvatar))
                                *(int*)(combatInfoAddress + 4280) = 0;
                            return ((byte*)0x14071C390)[animArray + 96];
                        }
                    case 429:
                        if ((byte)(btlUnitInfo + 162) != 1 || (BtlUnit)(btlUnitInfo + 164) != BtlUnit.Nyx)
                            return -1;
                        return ((byte*)0x14071C390)[animArray];
                    case 430:
                        if ((byte)(btlUnitInfo + 162) != 1 || (BtlUnit)(btlUnitInfo + 164) != BtlUnit.Elizabeth)
                            return -1;
                        return ((byte*)0x14071C390)[animArray];
                    case 446:
                        if ((byte)(btlUnitInfo + 162) != 1)
                            return -1;
                        if ((BtlUnit)(btlUnitInfo + 164) == BtlUnit.Akihiko)
                            return ((byte*)0x14071C390)[animArray];
                        if ((BtlUnit)(btlUnitInfo + 164) != BtlUnit.Ken)
                            return -1;
                        return ((byte*)0x14071C390)[animArray + 28];
                    /*case 453:
                        if ((byte)(btlUnitInfo + 162) != 1 || (BtlUnit)(btlUnitInfo + 164) != BtlUnit.Margaret)
                            return -1;
                        return ((byte*)0x14071C390)[animArray + 64];
                    case 454:
                        if ((byte)(btlUnitInfo + 162) != 1 || (BtlUnit)(btlUnitInfo + 164) != BtlUnit.Magician)
                            return -1;
                        return ((byte*)0x14071C390)[animArray];
                    case 455:
                        if ((byte)(btlUnitInfo + 162) != 1 || (BtlUnit)(btlUnitInfo + 164) != BtlUnit.Teo)
                            return -1;
                        return ((byte*)0x14071C390)[animArray + 32];
                    default:
                        return -1;*/
                }
            }
        }
}