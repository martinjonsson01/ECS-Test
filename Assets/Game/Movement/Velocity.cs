using Unity.Entities;
using Unity.Mathematics;

namespace Game.Movement
{
[GenerateAuthoringComponent]
public struct Velocity : IComponentData
{
    /**
     * Contains both direction and speed in units per second.
     */
    public float3 Value;
}
}
