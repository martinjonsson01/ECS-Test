using Unity.Entities;
using Unity.Mathematics;

namespace Game.Enemy
{
[GenerateAuthoringComponent]
public struct Target : IComponentData
{
    public float3 Position;
    public float3 Velocity;
    public float3 Acceleration;
    public float StopDistanceSq;
}
}
