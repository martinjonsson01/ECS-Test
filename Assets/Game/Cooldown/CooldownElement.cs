using Unity.Entities;

namespace Game.Cooldown
{
public struct CooldownElement : IBufferElementData
{
    public CooldownType Type;
    public float Seconds;
}
}
