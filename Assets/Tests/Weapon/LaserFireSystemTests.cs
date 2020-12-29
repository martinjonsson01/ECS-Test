using System.Linq;

using Game.Enemy;
using Game.Life;
using Game.Movement;
using Game.Weapon;

using NUnit.Framework;

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using static NUnit.Framework.Assert;

namespace Tests.Weapon
{
public class LaserFireSystemTests : SystemTestBase<LaserFireSystem>
{
    private Entity _entity;
    private FireLaser _fireLaser;
    private const float Delta = 0.000001f;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _entity = m_Manager.CreateEntity(
            typeof(Acceleration),
            typeof(Target),
            typeof(Translation));
        m_Manager.SetComponentData(_entity, new Translation { Value = new float3(10f) });
        m_Manager.SetComponentData(_entity, new Acceleration { Value = new float3(1f, 0f, 0f), Max = 10f });
        _fireLaser = new FireLaser
        {
            Direction = new float3(1f, 0f, 0f),
            Damage = 10f
        };
        m_Manager.AddComponentData(_entity, _fireLaser);
    }

    [Test]
    public void When_EntityDoesntHaveFireLaser_NothingIsCreated()
    {
        m_Manager.RemoveComponent<FireLaser>(_entity);

        World.Update();

        NativeArray<Entity> entities = m_Manager.GetAllEntities();
        IsTrue(entities.All(entity => !m_Manager.HasComponent<LaserBoltTag>(entity)));
    }

    [Test]
    public void When_EntityHasFireLaser_LaserBoltIsCreated()
    {
        World.Update();

        NativeArray<Entity> entities = m_Manager.GetAllEntities();
        IsTrue(entities.Any(entity => m_Manager.HasComponent<LaserBoltTag>(entity)));
    }

    [Test]
    public void When_EntityHasFireLaser_FireLaserIsRemoved()
    {
        IsTrue(m_Manager.HasComponent<FireLaser>(_entity));

        World.Update();

        IsFalse(m_Manager.HasComponent<FireLaser>(_entity));
    }

    [Test]
    public void When_LaserBoltIsCreated_LaserBoltHasVelocity()
    {
        World.Update();

        NativeArray<Entity> entities = m_Manager.GetAllEntities();
        Entity laserBolt = entities.First(entity => m_Manager.HasComponent<LaserBoltTag>(entity));
        IsTrue(m_Manager.HasComponent<Velocity>(laserBolt));
    }


    [Test]
    public void When_LaserBoltIsCreated_LaserBoltHasLifetime()
    {
        World.Update();

        NativeArray<Entity> entities = m_Manager.GetAllEntities();
        Entity laserBolt = entities.First(entity => m_Manager.HasComponent<LaserBoltTag>(entity));
        IsTrue(m_Manager.HasComponent<Lifetime>(laserBolt));
    }

    [Test]
    public void When_LaserBoltIsCreated_LaserBoltHasTranslationOfEntity()
    {
        World.Update();

        NativeArray<Entity> entities = m_Manager.GetAllEntities();
        Entity laserBolt = entities.First(entity => m_Manager.HasComponent<LaserBoltTag>(entity));
        float3 entityPosition = m_Manager.GetComponentData<Translation>(_entity).Value;
        AreEqual(entityPosition, m_Manager.GetComponentData<Translation>(laserBolt).Value);
    }

    [Test]
    public void When_LaserBoltIsCreated_LaserBoltHasSpecifiedDirection()
    {
        World.Update();

        NativeArray<Entity> entities = m_Manager.GetAllEntities();
        Entity laserBolt = entities.First(entity => m_Manager.HasComponent<LaserBoltTag>(entity));
        float3 actualDirection = math.forward(m_Manager.GetComponentData<Rotation>(laserBolt).Value);
        AreEqual(_fireLaser.Direction.x, actualDirection.x, Delta);
        AreEqual(_fireLaser.Direction.y, actualDirection.y, Delta);
        AreEqual(_fireLaser.Direction.z, actualDirection.z, Delta);
    }

    [Test]
    public void When_LaserBoltIsCreated_LaserBoltHasSpecifiedDamage()
    {
        World.Update();

        NativeArray<Entity> entities = m_Manager.GetAllEntities();
        Entity laserBolt = entities.First(entity => m_Manager.HasComponent<LaserBoltTag>(entity));
        AreEqual(_fireLaser.Damage, m_Manager.GetComponentData<Damage>(laserBolt).Value);
    }

    [Ignore("Unity does not currently allow .SetName(Entity, string) on EntityCommandBuffers, so implementing this is not currently possible.")]
    [Test]
    public void When_LaserBoltIsCreated_LaserBoltEntityHasName()
    {
        const string name = "Laser Bolt";

        World.Update();

        NativeArray<Entity> entities = m_Manager.GetAllEntities();
        Entity laserBolt = entities.First(entity => m_Manager.HasComponent<LaserBoltTag>(entity));
        var expectedName = $"{name} {laserBolt.Index}";
        AreEqual(expectedName, m_Manager.GetName(laserBolt));
    }
}
}
