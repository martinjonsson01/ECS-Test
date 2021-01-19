using BovineLabs.Event.Containers;

using Game.Life;
using Game.Weapon;
using Game.Weapon.Projectile;

using NUnit.Framework;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

using static NUnit.Framework.Assert;

using Velocity = Game.Movement.Velocity;

namespace Tests.Weapon.Projectile
{
public class ProjectileHitDetectionSystemTests :
    EventSystemTestBase<ProjectileHitDetectionSystem, ProjectileHitEvent>
{
    private Entity _projectile;
    private Entity _target;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _projectile = m_Manager.CreateEntity(
            typeof(ProjectileTag),
            typeof(Translation),
            typeof(Velocity),
            typeof(Damage));
        _target = m_Manager.CreateEntity(
            typeof(Translation),
            typeof(Rotation),
            typeof(PhysicsVelocity),
            typeof(PhysicsCollider),
            typeof(Health));
        BlobAssetReference<Collider> collider = SphereCollider.Create(
            new SphereGeometry
            {
                Center = float3.zero,
                Radius = 1f
            },
            CollisionFilter.Default,
            new Material
            {
                CollisionResponse = CollisionResponsePolicy.None
            });
        m_Manager.SetComponentData(_target, new PhysicsCollider { Value = collider });
        m_Manager.SetComponentData(_target, new Translation { Value = new float3(0f) });
        m_Manager.SetComponentData(_projectile, new Translation { Value = new float3(5f, 0f, 0f) });
    }

    [Test]
    public void When_ProjectileIsNotNearTarget_NoNewCollisionEventIsCreated()
    {
        World.GetExistingSystem<ProjectileHitDetectionSystem>().Update();

        NativeEventStream.Reader eventReader = CreateEventReader();
        AreEqual(0, eventReader.Count() - 1);
    }

    [Test]
    public void When_ProjectileWillHitTarget_HitEventIsCreated()
    {
        var projectilePos = new float3(0.5f + ForcedDeltaTime, 0f, 0f);
        var projectileVelocity = new float3(-1f, 0f, 0f);
        m_Manager.SetComponentData(_projectile, new Translation { Value = projectilePos });
        m_Manager.SetComponentData(_projectile, new Velocity { Value = projectileVelocity });
        CreateAndSetUpPhysicsSystems();

        World.Update();

        NativeEventStream.Reader stream = CreateEventReader();
        int eventCount = stream.BeginForEachIndex(0);
        AreEqual(1, eventCount);
    }

    [Test]
    public void When_HitEventIsCreated_EntitiesAreNotEqual()
    {
        var projectilePos = new float3(0.5f + ForcedDeltaTime, 0f, 0f);
        var projectileVelocity = new float3(-1f, 0f, 0f);
        m_Manager.SetComponentData(_projectile, new Translation { Value = projectilePos });
        m_Manager.SetComponentData(_projectile, new Velocity { Value = projectileVelocity });
        CreateAndSetUpPhysicsSystems();

        World.Update();

        NativeEventStream.Reader stream = CreateEventReader();
        stream.BeginForEachIndex(0);
        var eventData = stream.Read<ProjectileHitEvent>();
        AreNotEqual(eventData.ProjectileEntity, eventData.HitEntity);
    }

    [Test]
    public void When_HitEventIsCreated_DamageHasProjectileDamage()
    {
        var projectilePos = new float3(0.5f + ForcedDeltaTime, 0f, 0f);
        var projectileVelocity = new float3(-1f, 0f, 0f);
        m_Manager.SetComponentData(_projectile, new Translation { Value = projectilePos });
        m_Manager.SetComponentData(_projectile, new Velocity { Value = projectileVelocity });
        CreateAndSetUpPhysicsSystems();

        World.Update();

        NativeEventStream.Reader stream = CreateEventReader();
        stream.BeginForEachIndex(0);
        var eventData = stream.Read<ProjectileHitEvent>();
        float projectileDamage = m_Manager.GetComponentData<Damage>(_projectile).Value;
        AreEqual(projectileDamage, eventData.Damage);
    }
}
}
