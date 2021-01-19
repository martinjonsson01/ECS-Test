using Unity.Entities;
using Unity.Mathematics;

namespace Game.Movement
{
[GenerateAuthoringComponent]
public struct Acceleration : IComponentData
{
    public float3 Value;

    public float Max;
}
}
