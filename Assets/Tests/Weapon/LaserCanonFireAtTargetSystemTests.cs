using Game.Cooldown;
using Game.Enemy;
using Game.Weapon;

using NUnit.Framework;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using static NUnit.Framework.Assert;

namespace Tests.Weapon
{
public class LaserCanonFireAtTargetSystemTests : SystemTestBase<LaserCanonFireAtTargetSystem>
{
    private Entity _entity;
    private readonly float3 _targetPosition = new float3(5f, 0f, 0f);
    private readonly float3 _entityPosition = float3.zero;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _entity = m_Manager.CreateEntity(
            typeof(LaserCannon),
            typeof(Translation),
            typeof(Rotation),
            typeof(Target),
            typeof(CooldownElement));
        m_Manager.SetComponentData(_entity, new LaserCannon
        {
            Damage = 1f,
            Cooldown = 2f,
            FiringArc = math.PI / 4,
            Range = 900f
        });
        m_Manager.SetComponentData(_entity, new Translation { Value = _entityPosition });
        m_Manager.AddComponentData(_entity, new Target { Position = _targetPosition });
        m_Manager.AddComponentData(_entity, new Rotation { Value = quaternion.EulerXYZ(0, math.PI / 2, 0) });
    }

    [Test]
    public void When_EntityWithCannonHasTarget_FireLaserIsCreated()
    {
        World.Update();

        IsTrue(m_Manager.HasComponent<FireLaser>(_entity));
    }

    [Test]
    public void When_FireLaserIsCreated_EntityIsGivenCorrectCooldown()
    {
        World.Update();

        float expectedCooldown = m_Manager.GetComponentData<LaserCannon>(_entity).Cooldown;
        AreEqual(expectedCooldown, m_Manager.GetBuffer<CooldownElement>(_entity)[0].Seconds);
    }

    [Test]
    public void When_FireLaserIsCreated_FireLaserHasDirectionToTarget()
    {
        float3 vectorToTarget = math.normalizesafe(_targetPosition - _entityPosition);

        World.Update();

        AreEqual(vectorToTarget, m_Manager.GetComponentData<FireLaser>(_entity).Direction);
    }

    [Test]
    public void When_FireLaserIsCreated_FireLaserHasCannonDamage()
    {
        World.Update();

        float expectedDamage = m_Manager.GetComponentData<LaserCannon>(_entity).Damage;
        float actualDamage = m_Manager.GetComponentData<FireLaser>(_entity).Damage;
        AreEqual(expectedDamage, actualDamage);
    }

    [Test]
    public void When_EntityHasTargetAndWeaponCooldown_FireLaserIsNotCreated()
    {
        DynamicBuffer<CooldownElement> cooldownBuffer = m_Manager.GetBuffer<CooldownElement>(_entity);
        var cooldown = new CooldownElement { Type = CooldownType.Weapon, Seconds = 1f };
        cooldownBuffer.Add(cooldown);

        World.Update();

        IsFalse(m_Manager.HasComponent<FireLaser>(_entity));
    }

    [Test]
    public void When_WeaponCooldownRunsOut_FireLaserIsCreated()
    {
        DynamicBuffer<CooldownElement> cooldownBuffer = m_Manager.GetBuffer<CooldownElement>(_entity);
        var cooldown = new CooldownElement { Type = CooldownType.Weapon, Seconds = 0f };
        cooldownBuffer.Add(cooldown);
        World.GetOrCreateSystem<CooldownSystem>().Update();

        World.Update();

        IsTrue(m_Manager.HasComponent<FireLaser>(_entity));
    }

    [Test]
    public void When_NotPointedAtTarget_FireLaserIsNotCreated()
    {
        quaternion rotationAway = quaternion.EulerXYZ(0, -math.PI / 2, 0);
        m_Manager.SetComponentData(_entity, new Rotation { Value = rotationAway });

        World.Update();

        IsFalse(m_Manager.HasComponent<FireLaser>(_entity));
    }

    [Test]
    public void When_FacingSameDirectionAsTarget_ButNotPointedAtTarget_FireLaserIsNotCreated()
    {
        quaternion sameRotation = quaternion.EulerXYZ(0, 0, 0);
        m_Manager.SetComponentData(_entity, new Rotation { Value = sameRotation });

        World.Update();

        IsFalse(m_Manager.HasComponent<FireLaser>(_entity));
    }

    [Test]
    public void When_OutOfRange_FireLaserIsNotCreated()
    {
        m_Manager.SetComponentData(_entity, new Translation { Value = new float3(-1000f, 0, 0) });

        World.Update();

        IsFalse(m_Manager.HasComponent<FireLaser>(_entity));
    }
}
}
