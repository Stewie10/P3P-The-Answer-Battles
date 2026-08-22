using Reloaded.Hooks.Definitions;
using Reloaded.Memory;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;

namespace P3PPC.Expansion.TheAnswer;

/// <summary>
/// Provides a base class for services that hook functions and manage unmanaged memory allocations.
/// </summary>
/// <remarks>Derived classes must implement the <see cref="Hook"/> method to define their hooking behavior. The
/// class provides utility methods for allocating memory for unmanaged structures and pointer arrays.<br/><br/>See also <seealso cref="IFaces.IHookable"/> for the base interface which corresponds to a P3P service.</remarks>
public unsafe abstract class BaseService
{
    /// <summary>
    /// Maps <see cref="Delegate"/> types to their corresponding function addresses.
    /// </summary>
    protected readonly Dictionary<Type, nint> functionAddresses;
    /// <summary>
    /// Allows signature scanning.
    /// </summary>
    protected IStartupScanner startup;
    /// <summary>
    /// Service to create hooks.
    /// </summary>
    protected IReloadedHooks hooks;
    /// <summary>
    /// Memory management system.
    /// </summary>
    protected Memory memory;
    protected BaseService(IStartupScanner startup, IReloadedHooks hooks, Memory memory)
    {
        functionAddresses = new Dictionary<Type, nint>();
        ArgumentNullException.ThrowIfNull(startup);
        ArgumentNullException.ThrowIfNull(hooks);
        this.startup = startup;
        this.hooks = hooks;
        this.memory = memory;
    }
    private nuint AllocateMemory(nuint size)
        => memory.Allocate(size).Address;
    /// <summary>
    /// Allocates memory for an unmanaged struct of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The unmanaged struct type to allocate memory for.</typeparam>
    /// <returns>A pointer to the allocated memory.</returns>
    protected T* AllocateMemoryForStruct<T>() where T : unmanaged
    {
        var size = (nuint)sizeof(T);
        var address = AllocateMemory(size);
        return (T*)address;
    }
    /// <summary>
    /// Allocates memory for an array of pointers to unmanaged type instances.
    /// </summary>
    /// <typeparam name="T">The unmanaged type that the pointers will reference.</typeparam>
    /// <param name="count">The number of pointers in the array.</param>
    /// <returns>A pointer to the allocated memory for the pointer array.</returns>
    protected T** AllocateMemoryForPointerArray<T>(int count) where T : unmanaged
    {
        var size = (nuint)(count * sizeof(nint));
        var address = AllocateMemory(size);
        return (T**)address;
    }
    /// <summary>
    /// Custom midlife initialization method for derived classes to implement their logic for signature scanning and hooking functions. This method is called after the constructor and is intended to be overridden by subclasses to define their specific hooking behavior.
    /// </summary>
    public abstract void Hook();
}
