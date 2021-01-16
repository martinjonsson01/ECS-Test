using Unity.Entities;

namespace Game.Weapon
{
[GenerateAuthoringComponent]
public struct Damage : IComponentData
{
    public float Value;
}
}
