using Unity.Entities;
using Unity.Mathematics;

namespace Game.Movement
{
[GenerateAuthoringComponent]
public struct Acceleration : IComponentData
{
    public float3 Value;

    public float Max;

    public override string ToString()
    {
        return $"{nameof(Acceleration)}(Seconds={Value.ToString()}, Max={Max.ToString()})";
    }
}
}
