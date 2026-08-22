
namespace P3PPC.Expansion.TheAnswer.IFaces;

public interface IHookable
{
    void Hook();
}

public unsafe interface IP3PInfo : IHookable
{
    //bool* IsFeMC { get; }
    long* BossBtlCombatInfo { get; }
    long* BossBtlBEDFiles { get; }

}

