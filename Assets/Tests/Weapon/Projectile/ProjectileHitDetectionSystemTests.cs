using System.Collections.Generic;

using BovineLabs.Event.Containers;
using BovineLabs.Event.Systems;

using Game.Life;
using Game.Weapon;
using Game.Weapon.Projectile;

using NUnit.Framework;

using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

using static NUnit.Framework.Assert;

using Velocity = Game.Movement.Velocity;

namespace Tests.Weapon.Projectile
{
public class ProjectileHitDetectionSystemTests : SystemTestBase<ProjectileHitDetectionSystem>
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

    private EventSystem SetUpEventSystem()
    {
        var eventSystem = World.GetExistingSystem<EventSystem>();
        // Need to create a single event so that there is something to read from when asserting.
        NativeEventStream.ThreadWriter writer = eventSystem.CreateEventWriter<ProjectileHitEvent>();
        eventSystem.AddJobHandleForProducer<ProjectileHitEvent>(default);
        writer.Write(new ProjectileHitEvent{Damage = 1f});
        return eventSystem;
    }

    private static NativeEventStream.Reader GetEventReader(EventSystemBase eventSystem)
    {
        JobHandle getEventReadersHandle = eventSystem.GetEventReaders<ProjectileHitEvent>(
            default, out IReadOnlyList<NativeEventStream.Reader> eventReaders);
        getEventReadersHandle.Complete();
        eventSystem.AddJobHandleForConsumer<ProjectileHitEvent>(getEventReadersHandle);
        NativeEventStream.Reader eventReader = eventReaders[0];
        return eventReader;
    }

    [Test]
    public void When_ProjectileIsNotNearTarget_NoNewCollisionEventIsCreated()
    {
        EventSystem eventSystem = SetUpEventSystem();

        World.GetExistingSystem<ProjectileHitDetectionSystem>().Update();

        NativeEventStream.Reader eventReader = GetEventReader(eventSystem);
        AreEqual(0, eventReader.Count() - 1);
    }

    [Test]
    public void When_ProjectileWillHitTarget_HitEventIsCreated()
    {
        EventSystem eventSystem = SetUpEventSystem();
        var projectilePos = new float3(0.5f + ForcedDeltaTime, 0f, 0f);
        var projectileVelocity = new float3(-1f, 0f, 0f);
        m_Manager.SetComponentData(_projectile, new Translation { Value = projectilePos });
        m_Manager.SetComponentData(_projectile, new Velocity { Value = projectileVelocity });
        CreateAndSetUpPhysicsSystems();

        World.Update();

        NativeEventStream.Reader stream = GetEventReader(eventSystem);
        int eventCount = stream.BeginForEachIndex(0);
        AreEqual(1, eventCount);
    }

    [Test]
    public void When_HitEventIsCreated_EntitiesAreNotEqual()
    {
        EventSystem eventSystem = SetUpEventSystem();
        var projectilePos = new float3(0.5f + ForcedDeltaTime, 0f, 0f);
        var projectileVelocity = new float3(-1f, 0f, 0f);
        m_Manager.SetComponentData(_projectile, new Translation { Value = projectilePos });
        m_Manager.SetComponentData(_projectile, new Velocity { Value = projectileVelocity });
        CreateAndSetUpPhysicsSystems();

        World.Update();

        NativeEventStream.Reader stream = GetEventReader(eventSystem);
        stream.BeginForEachIndex(0);
        var eventData = stream.Read<ProjectileHitEvent>();
        AreNotEqual(eventData.ProjectileEntity, eventData.HitEntity);
    }

    [Test]
    public void When_HitEventIsCreated_DamageHasProjectileDamage()
    {
        EventSystem eventSystem = SetUpEventSystem();
        var projectilePos = new float3(0.5f + ForcedDeltaTime, 0f, 0f);
        var projectileVelocity = new float3(-1f, 0f, 0f);
        m_Manager.SetComponentData(_projectile, new Translation { Value = projectilePos });
        m_Manager.SetComponentData(_projectile, new Velocity { Value = projectileVelocity });
        CreateAndSetUpPhysicsSystems();

        World.Update();

        NativeEventStream.Reader stream = GetEventReader(eventSystem);
        stream.BeginForEachIndex(0);
        var eventData = stream.Read<ProjectileHitEvent>();
        float projectileDamage = m_Manager.GetComponentData<Damage>(_projectile).Value;
        AreEqual(projectileDamage, eventData.Damage);
    }
}
}
