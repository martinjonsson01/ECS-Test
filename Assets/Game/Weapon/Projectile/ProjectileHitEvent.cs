using Unity.Entities;

namespace Game.Weapon.Projectile
{
public struct ProjectileHitEvent
{
    public Entity ProjectileEntity;
    public Entity HitEntity;
    public float Damage;
}
}
