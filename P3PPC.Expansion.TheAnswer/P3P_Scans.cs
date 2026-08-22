using P3PPC.Expansion.TheAnswer.FunctionDelegates;
using Reloaded.Hooks.Definitions;
using Reloaded.Memory.Sigscan.Definitions.Structs;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using RyoTune.Reloaded.Scans;
using SharedScans.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace P3PPC.Expansion.TheAnswer;

internal record struct ScanInfo(string Signature, string ScanName, nint Address, Type? DelegateType = null);

internal class ScanInfoHashSet : HashSet<ScanInfo>
{
    public ScanInfoHashSet() : base(new ScanInfoEqualityComparer())
    {
    }
    public bool ContainsByScanName(string scanName)
    {
        return this.Any(scan => scan.ScanName == scanName);
    }
    public bool ContainsBySig(string signature)
    {
        return this.Any(scan => scan.Signature == signature);
    }
    public bool ContainsByDelegateType(Type delegateType)
    {
        return this.Any(scan => scan.DelegateType == delegateType);
    }
}

internal class ScanInfoEqualityComparer : IEqualityComparer<ScanInfo>
{
    public bool Equals(ScanInfo x, ScanInfo y)
    {
        return x.Signature == y.Signature && x.ScanName == y.ScanName && x.Address == y.Address && x.DelegateType == y.DelegateType;
    }
    public int GetHashCode(ScanInfo obj)
    {
        return HashCode.Combine(obj.Signature, obj.ScanName, obj.Address, obj.DelegateType);
    }
}

internal static class ScanTracker
{
    private static readonly ScanInfoHashSet scans = new ScanInfoHashSet();
    private static readonly Lock _lock = new Lock();
    public static bool TrackScan(string signature, string scanName, nint address, Type? delegateType = null)
    {
        lock (_lock)
        {
            var scanInfo = new ScanInfo(signature, scanName, address, delegateType);
            if (!scans.Contains(scanInfo))
            {
                scans.Add(scanInfo);
                return true;
            }
            return false;
        }
    }
    public static bool HasScan(string scanName)
    {
        lock (_lock)
        {
            return scans.ContainsByScanName(scanName);
        }
    }
}

public static class P3P_Scans
{
    private static bool isInitialized = false;
    internal static nint BaseAddress { get; private set; }

    /// <summary>
    /// Initializes the base address from the current process's main module.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the main module's base address cannot be retrieved.</exception>
    private static void InitializeBaseAddress()
    {
        if (!isInitialized)
        {
            BaseAddress = System.Diagnostics.Process.GetCurrentProcess().MainModule?.BaseAddress ?? throw new InvalidOperationException("Failed to get base address of the main module.");
            isInitialized = true;
        }
    }

    /// <summary>
    /// Adds a delegate scan using preset signature patterns if not already registered.
    /// </summary>
    /// <typeparam name="TDelegate">The delegate type used to retrieve the preset scan configuration.</typeparam>
    /// <param name="scanner">The startup scanner instance.</param>
    /// <param name="onSuccess">Callback invoked with the found address when the scan succeeds.</param>
    /// <returns><c>true</c> if the scan was added; <c>false</c> if it already exists.</returns>
    public static bool AddDelegateScan<TDelegate>(this IStartupScanner scanner, Action<nint> onSuccess)
        where TDelegate : Delegate
    {
        var (sig, scanName) = ScanPresets.Get<TDelegate>();
        if (!ScanTracker.HasScan(scanName))
        {
            scanner.Scan(scanName, sig, onSuccess);
            return true;
        }
        return false;
    }
    /// <summary>
    /// Adds a memory scan using a preset signature if it hasn't been tracked yet.
    /// </summary>
    /// <param name="scanner">The startup scanner instance to add the scan to.</param>
    /// <param name="functionName">The name of the function to look up the preset signature for.</param>
    /// <param name="onSuccess">The callback to invoke when the scan succeeds, receiving the memory address.</param>
    /// <returns><c>true</c> if the scan was added; <c>false</c> if the scan was already tracked.</returns>
    public static bool AddArbitraryScan(this IStartupScanner scanner, string functionName, Action<nint> onSuccess)
    {
        var (signature, scanName) = ScanPresets.Get(functionName);
        if (!ScanTracker.HasScan(scanName))
        {
            scanner.Scan(scanName, signature, onSuccess);
            return true;
        }
        return false;
    }
    public static bool SetArbitraryScanWrapper<TReturn>(this IStartupScanner scanner, string functionName, IReloadedHooks hooks, ref Func<TReturn> wrapper, Action onFailure)
    {
        Func<TReturn>? tempWrapper = null;
        var (signature, scanName) = ScanPresets.Get(functionName);
        if (!ScanTracker.HasScan(scanName))
        {
            scanner.Scan(scanName, signature, (address) =>
            {
                tempWrapper = hooks.CreateWrapper<Func<TReturn>>(address, out _);
            }, onFailure);
            if (tempWrapper != null)
            {
                wrapper = tempWrapper;
                return true;
            }
            else
                return false;
        }
        return false;
    }
    /// <summary>
    /// Adds a delegate-based scan to the scanner if it has not already been registered.
    /// </summary>
    /// <typeparam name="TDelegate">The delegate type used to retrieve the scan preset configuration.</typeparam>
    /// <param name="scanner">The startup scanner instance.</param>
    /// <param name="onSuccess">Action invoked when the scan succeeds, receiving the address of the scanned result.</param>
    /// <param name="onFailure">Action invoked when the scan fails.</param>
    /// <returns><see langword="true"/> if the scan was added; <see langword="false"/> if it was already registered.</returns>
    public static bool AddDelegateScan<TDelegate>(this IStartupScanner scanner, Action<nint> onSuccess, Action onFailure)
        where TDelegate : Delegate
    {
        var (sig, scanName) = ScanPresets.Get<TDelegate>();
        if (!ScanTracker.HasScan(scanName))
        {
            scanner.Scan(scanName, sig, onSuccess, onFailure);
            return true;
        }
        return false;
    }
    //z
    public static bool AddArbitraryScan(this IStartupScanner scanner, string functionName, Action<nint> onSuccess, Action onFailure)
    {
        var (signature, scanName) = ScanPresets.Get(functionName);
        if (!ScanTracker.HasScan(scanName))
        {
            scanner.Scan(scanName, signature, onSuccess, onFailure);
            return true;
        }
        return false;
    }
    public static bool AddDelegateScanHook<TDelegate>(this IStartupScanner scanner, IReloadedHooks hooks, Action<nint, IReloadedHooks> onSuccess)
        where TDelegate : Delegate
    {
        void onSuccessWrapper(nint address)
        {
            onSuccess(address, hooks);
        }
        var (sig, scanName) = ScanPresets.Get<TDelegate>();
        if (!ScanTracker.HasScan(scanName))
        {
            scanner.Scan(scanName, sig, onSuccessWrapper);
            return true;
        }
        return false;
    }
    public static bool SetDelegateScanHook<TDelegate>(this IStartupScanner scanner, IReloadedHooks hooks, TDelegate function, ref IHook<TDelegate> hook)
        where TDelegate : Delegate
    {
        IHook<TDelegate>? tempHook = null;
        var (sig, scanName) = ScanPresets.Get<TDelegate>();
        if (!ScanTracker.HasScan(scanName))
        {
            scanner.Scan(scanName, sig, (address) =>
            {
                tempHook = hooks.CreateHook(function, address);
            });
            if (tempHook != null)
            {
                hook = tempHook.Activate();
                return true;
            }
            else
                return false;
        }
        return false;
    }
    public static bool SetDelegateScanWrapper<TDelegate>(this IStartupScanner scanner, IReloadedHooks hooks, ref TDelegate? wrapper, Action onFailure)
        where TDelegate : Delegate
    {
        TDelegate? tempWrapper = null;
        var (sig, scanName) = ScanPresets.Get<TDelegate>();
        if (!ScanTracker.HasScan(scanName))
        {
            scanner.Scan(scanName, sig, (address) =>
            {
                tempWrapper = hooks.CreateWrapper<TDelegate>(address, out _);
            }, onFailure);
            if (tempWrapper != null)
            {
                wrapper = tempWrapper;
                return true;
            }
            else
                return false;
        }
        return false;
    }
    public static bool AddArbitraryScanHook(this IStartupScanner scanner, string functionName, IReloadedHooks hooks, Action<nint, IReloadedHooks> onSuccess)
    {
        void onSuccessWrapper(nint address)
        {
            onSuccess(address, hooks);
        }
        var (signature, scanName) = ScanPresets.Get(functionName);
        if (!ScanTracker.HasScan(scanName))
        {
            scanner.Scan(scanName, signature, onSuccessWrapper);
            return true;
        }
        return false;
    }
    public static bool SetArbitraryScanHook(this IStartupScanner scanner, string functionName, IReloadedHooks hooks, Delegate function, ref IHook<Delegate> hook)
    {
        IHook<Delegate>? tempHook = null;
        var (signature, scanName) = ScanPresets.Get(functionName);
        if (!ScanTracker.HasScan(scanName))
        {
            scanner.Scan(scanName, signature, (address) =>
            {
                tempHook = hooks.CreateHook(function, address);
            });
            if (tempHook != null)
            {
                hook = tempHook.Activate();
                return true;
            }
            else
                return false;
        }
        return false;
    }

    public static bool AddDelegateScanHook<TDelegate>(this IStartupScanner scanner, IReloadedHooks hooks, Action<nint, IReloadedHooks> onSuccess, Action onFailure)
        where TDelegate : Delegate
    {
        void onSuccessWrapper(nint address)
        {
            onSuccess(address, hooks);
        }
        var (sig, scanName) = ScanPresets.Get<TDelegate>();
        if (!ScanTracker.HasScan(scanName))
        {
            scanner.Scan(scanName, sig, onSuccessWrapper, onFailure);
            return true;
        }
        return false;
    }
    public static bool SetDelegateScanHook<TDelegate>(this IStartupScanner scanner, IReloadedHooks hooks, TDelegate function, ref IHook<TDelegate> hook, Action onFailure)
        where TDelegate : Delegate
    {
        IHook<TDelegate>? tempHook = null;
        var (sig, scanName) = ScanPresets.Get<TDelegate>();
        if (!ScanTracker.HasScan(scanName))
        {
            scanner.Scan(scanName, sig, (address) =>
            {
                tempHook = hooks.CreateHook(function, address);
            }, onFailure);
            if (tempHook != null)
            {
                hook = tempHook.Activate();
                return true;
            }
            else
                return false;
        }
        return false;
    }
    public static bool AddArbitraryScanHook(this IStartupScanner scanner, string functionName, IReloadedHooks hooks, Action<nint, IReloadedHooks> onSuccess, Action onFailure)
    {
        void onSuccessWrapper(nint address)
        {
            onSuccess(address, hooks);
        }
        var (signature, scanName) = ScanPresets.Get(functionName);
        if (!ScanTracker.HasScan(scanName))
        {
            scanner.Scan(scanName, signature, onSuccessWrapper, onFailure);
            return true;
        }
        return false;
    }
    public static bool SetArbitraryScanHook(this IStartupScanner scanner, string functionName, IReloadedHooks hooks, Delegate function, ref IHook<Delegate> hook, Action onFailure)
    {
        IHook<Delegate>? tempHook = null;
        var (signature, scanName) = ScanPresets.Get(functionName);
        if (!ScanTracker.HasScan(scanName))
        {
            scanner.Scan(scanName, signature, (address) =>
            {
                tempHook = hooks.CreateHook(function, address);
            }, onFailure);
            if (tempHook != null)
            {
                hook = tempHook.Activate();
                return true;
            }
            else
                return false;
        }
        return false;
    }
}
public static class P3P_SharedScansExtensions
{
    private static readonly List<Type> _addedDelegateScans = new();
    private static void RecordAddedDelegateScan<TDelegate>() where TDelegate : Delegate
    {
        var delegateType = typeof(TDelegate);
        if (!_addedDelegateScans.Contains(delegateType))
        {
            _addedDelegateScans.Add(delegateType);

        }
    }
    private static bool ScanHasNotBeenAdded<TDelegate>() where TDelegate : Delegate
    {
        return !_addedDelegateScans.Contains(typeof(TDelegate));
    }
    public static bool AddDelegateScan<TDelegate>(this ISharedScans sharedScans, string modName, [NotNullWhen(true)] out WrapperContainer<TDelegate>? wrapperContainer)
        where TDelegate : Delegate
    {
        wrapperContainer = null;
        try
        {
            var (signature, scanName) = ScanPresets.Get<TDelegate>();
            sharedScans.AddScan<TDelegate>(signature);
            wrapperContainer = sharedScans.CreateWrapper<TDelegate>(modName);
            RecordAddedDelegateScan<TDelegate>();
            return true;
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to add shared scan for {typeof(TDelegate).Name}: {ex.Message}");
        }
        return false;
    }
    public static bool CreateDelegateListener<TDelegate>(this ISharedScans sharedScans, Action<nint> onCallback) where TDelegate : Delegate
    {
        try
        {
            if (!ScanHasNotBeenAdded<TDelegate>())
            {
                Log.Warning($"Delegate scan for {typeof(TDelegate).Name} has already been added. Skipping listener creation.");
                return false;
            }
            sharedScans.CreateListener<TDelegate>(onCallback);
            return true;
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to create delegate listener for {typeof(TDelegate).Name}: {ex.Message}");
        }
        return false;
    }
}

/// <summary>
/// Provides utility methods for generating x64 assembly code to save and restore XMM registers to and from the stack.
/// </summary>
/// <remarks>Used primarily in function hooking scenarios where preservation of SIMD register state is
/// required.</remarks>
public static class HookUtils
{
    // Pushes the value of an xmm register to the stack, saving it so it can be restored with PopXmm
    public static string PushXmm(int xmmNum)
    {
        return // Save an xmm register 
            $"sub rsp, 16\n" + // allocate space on stack
            $"movdqu dqword [rsp], xmm{xmmNum}\n";
    }

    // Pushes all xmm registers (0-15) to the stack, saving them to be restored with PopXmm
    public static string PushXmm()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < 16; i++)
        {
            sb.Append(PushXmm(i));
        }
        return sb.ToString();
    }

    // Pops the value of an xmm register to the stack, restoring it after being saved with PushXmm
    public static string PopXmm(int xmmNum)
    {
        return                 //Pop back the value from stack to xmm
            $"movdqu xmm{xmmNum}, dqword [rsp]\n" +
            $"add rsp, 16\n"; // re-align the stack
    }

    // Pops all xmm registers (0-7) from the stack, restoring them after being saved with PushXmm
    public static string PopXmm()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 7; i >= 0; i--)
        {
            sb.Append(PopXmm(i));
        }
        return sb.ToString();
    }
}