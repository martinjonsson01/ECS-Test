using BovineLabs.Event.Containers;
using BovineLabs.Event.Systems;

using Game.Life;
using Game.Movement;
using Game.Weapon;
using Game.Weapon.Projectile;

using NUnit.Framework;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using static NUnit.Framework.Assert;

namespace Tests.Weapon.Projectile
{
public class ProjectileHitDamageSystemTests : SystemTestBase<ProjectileHitDamageSystem>
{
    private Entity _target;
    private Entity _projectile;
    private Entity _targetWithoutHealth;

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
            typeof(Health));
        _targetWithoutHealth = m_Manager.CreateEntity(
            typeof(Translation));
        m_Manager.AddComponentData(_target, new Translation { Value = float3.zero });
        m_Manager.AddComponentData(_targetWithoutHealth, new Translation { Value = float3.zero });
        m_Manager.AddComponentData(_target, new Health { Value = 10f });
        m_Manager.AddComponentData(_projectile, new Translation { Value = new float3(5f, 0f, 0f) });
    }

    [Test]
    public void When_NoProjectileHitEvent_TargetTakesNoDamage()
    {
        var previousHealth = m_Manager.GetComponentData<Health>(_target);

        World.Update();

        AreEqual(previousHealth, m_Manager.GetComponentData<Health>(_target));
    }

    [Test]
    public void When_ProjectileHitEventExists_TargetTakesSpecifiedDamage()
    {
        float previousHealth = m_Manager.GetComponentData<Health>(_target).Value;
        var eventSystem = World.GetExistingSystem<EventSystem>();
        NativeEventStream.ThreadWriter writer = eventSystem.CreateEventWriter<ProjectileHitEvent>();
        const float damage = 4.5f;
        var hitEvent = new ProjectileHitEvent
        {
            ProjectileEntity = _projectile,
            HitEntity = _target,
            Damage = damage
        };
        writer.Write(hitEvent);

        World.GetExistingSystem<ProjectileHitDamageSystem>().Update();

        float expectedHealth = previousHealth - damage;
        AreEqual(expectedHealth, m_Manager.GetComponentData<Health>(_target).Value);
    }

    [Test]
    public void When_ProjectileHitEventHasProjectile_ProjectileIsDestroyed()
    {
        var eventSystem = World.GetExistingSystem<EventSystem>();
        NativeEventStream.ThreadWriter writer = eventSystem.CreateEventWriter<ProjectileHitEvent>();
        var hitEvent = new ProjectileHitEvent
        {
            ProjectileEntity = _projectile,
            HitEntity = _target,
            Damage = 1f
        };
        writer.Write(hitEvent);

        World.GetExistingSystem<ProjectileHitDamageSystem>().Update();
        World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>().Update();

        IsFalse(m_Manager.Exists(_projectile));
    }

    [Test]
    public void When_ProjectileHitEventHasHitEntityWithoutHealth_ProjectileIsNotDestroyed()
    {
        var eventSystem = World.GetExistingSystem<EventSystem>();
        NativeEventStream.ThreadWriter writer = eventSystem.CreateEventWriter<ProjectileHitEvent>();
        var hitEvent = new ProjectileHitEvent
        {
            ProjectileEntity = _projectile,
            HitEntity = _targetWithoutHealth,
            Damage = 1f
        };
        writer.Write(hitEvent);

        World.GetExistingSystem<ProjectileHitDamageSystem>().Update();
        World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>().Update();

        IsTrue(m_Manager.Exists(_projectile));
    }
}
}
