using Unity.Entities;
using Unity.Mathematics;

namespace Game.Weapon
{
[GenerateAuthoringComponent]
public struct FireLaser : IComponentData
{
    public float Damage { get; set; }
    public float3 Direction { get; set; }
}
}
