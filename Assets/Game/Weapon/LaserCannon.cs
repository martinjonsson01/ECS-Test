using Unity.Entities;

namespace Game.Weapon
{
public struct LaserCannon : IComponentData
{
    public float Damage;
    public float Cooldown;
}
}
