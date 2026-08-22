namespace P3PPC.Expansion.TheAnswer.FunctionDelegates;

public static class ScanPresets
{
    private const string NonFullMoonBossBattleFunc_SIG = "40 53 48 83 EC ?? 48 8B D9 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 8B 90 ?? ?? ?? ?? 0F B7 42 ?? 05 ?? ?? ?? ?? 83 F8 ?? 0F 87";
    private const string BossBtlUnitAnim_SIG = "48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 48 8B D9 0F B7 FA 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B 15 ?? ?? ?? ?? 48 8B 82 ?? ?? ?? ?? 44 0F B7 40 ?? 41 8D 80 ?? ?? ?? ?? 83 F8 27";
    private const string BossPersonaBtlUnitSet_SIG = "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 48 89 CD 48 63 F2 48 8D 0D ?? ?? ?? ??";
    private const string BossBtlUnit_SIG = "48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 48 8B D9 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 8B 90 ?? ?? ?? ?? 0F B7 42 10 05 ?? ?? ?? ?? 83 F8 27";
    private const string BossBtlCombatInfo_SIG = "48 89 5C 24 ?? 57 48 83 EC 70 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 44 24 ?? 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B 0D ?? ?? ?? ?? 33 D2 48 81 C1 ?? ?? ?? ?? 41 B8 ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 33 FF";
    private const string BossBtlBEDFiles_SIG = "48 83 EC 28 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 8B 88 ?? ?? ?? ?? 0F B7 41 10 05 ?? ?? ?? ?? 83 F8 28";
    //private const string HermitCharge_SIG = "48 83 EC 28 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B 0D ?? ?? ?? ?? 48 8B 81 ?? ?? ?? ?? 0F B7 50 10 B8 A5 01 00 00";
    //private const string HangedManAnimSet_SIG = "48 89 5C 24 ?? 57 48 83 EC 20 48 89 CB 89 D7 48 8D 0D D3 10 C6 FB E8 ?? ?? ?? ?? 85 7B 0C B8 ?? ?? ?? ?? 48 8B 5C 24 ?? 0F 95 D0 48 83 C4 20 5F";
    //private const string NyxAvatarAnimSet_SIG = "48 89 5C 24 ?? 57 48 83 EC 20 48 89 CF 0F B6 DA 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 80 FB 16 72 14";
    //private const string IsFeMC_SIG = "48 8D 35 ?? ?? ?? ?? 0F 28 05 ?? ?? ?? ??";

    public static (string Signature, string ScanName) Get<TDelegate>() where TDelegate : Delegate
        => Get(typeof(TDelegate));
    public static (string Signature, string ScanName) Get(string functionName)
        => GetFromName(functionName);
    private static DelegateNameAttribute? GetDelegateNameAttribute(Type delegateType)
    {
        return (DelegateNameAttribute?)Attribute.GetCustomAttribute(delegateType, typeof(DelegateNameAttribute));
    }
    private static (string Signature, string ScanName) Get(Type delegateType)
    {
        var delegateNameAttribute = GetDelegateNameAttribute(delegateType);
        if (delegateNameAttribute == null)
        {
            throw new InvalidOperationException("Delegate type must have a DelegateNameAttribute.");
        }
        var delName = delegateType.Name;
        var sigName = $"{delName}_SIG";
        var signature = typeof(ScanPresets).GetField(sigName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)?.GetValue(null) as string ?? throw new InvalidOperationException($"Signature for delegate type '{delName}' not found.");
        return (signature, delegateNameAttribute.PtrName);
    }
    private static (string Signature, string ScanName) GetFromName(string functionName)
    {
        var sigName = $"{functionName}_SIG";
        var ptrName = functionName.PointerName();
        var signature = typeof(ScanPresets).GetField(sigName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)?.GetValue(null) as string ?? throw new InvalidOperationException($"Signature for function name '{functionName}' not found.");
        //
        return (signature, ptrName);
    }
    private static string PointerName(this string noSpaceName)
    {
        var spacePositions = GetSpacePositions(noSpaceName).ToList();
        if (!spacePositions.Any())
            return noSpaceName;
        var result = new System.Text.StringBuilder(noSpaceName);
        foreach (var pos in spacePositions)
        {
            result.Insert(pos, ' ');
        }
        return result.ToString();
    }
    private enum Case
    {
        None,
        Lower,
        Upper,
    }
    private static Case GetCase(this char c)
    {
        if (char.IsLower(c))
            return Case.Lower;
        else if (char.IsUpper(c))
            return Case.Upper;
        else
            return Case.None;
    }
    private static IEnumerable<int> GetSpacePositions(string noSpaceName)
    {
        int currentIndex = 0;
        int adjustedIndex = 0;
        Case prevPrevCase = Case.None;
        Case prevCase = Case.None;
        for (int i = 0; i < noSpaceName.Length; i++)
        {
            var currentCase = noSpaceName[i].GetCase();
            if (currentCase == Case.Upper && prevCase == Case.Lower && prevPrevCase != Case.Upper)
            {
                yield return adjustedIndex;
                adjustedIndex += 2; // Adjust for the space that will be added
            }
            else
            {
                adjustedIndex++;
            }
            prevPrevCase = prevCase;
            prevCase = currentCase;
            currentIndex++;
        }
    }
}
