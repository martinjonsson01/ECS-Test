using Unity.Entities;

namespace Game.Cooldown
{
public struct CooldownElement : IBufferElementData
{
    public CooldownType Type;
    public float Seconds;

    public bool Equals(CooldownElement other)
    {
        return Type == other.Type && Seconds.Equals(other.Seconds);
    }

    public override bool Equals(object obj)
    {
        return obj is CooldownElement other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return ((int) Type * 397) ^ Seconds.GetHashCode();
        }
    }
}
}
