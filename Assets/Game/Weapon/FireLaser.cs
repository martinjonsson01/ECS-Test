using Unity.Entities;
using Unity.Mathematics;

namespace Game.Weapon
{
[GenerateAuthoringComponent]
public struct FireLaser : IComponentData
{
    public float Damage { get; set; }
    public float3 Direction { get; set; }

    public override string ToString()
    {
        return $"{nameof(FireLaser)}(Direction={Direction.ToString()}, Damage={Damage.ToString()})";
    }
}
}
