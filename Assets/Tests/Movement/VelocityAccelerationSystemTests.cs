using Game.Movement;

using NUnit.Framework;

using Unity.Entities;
using Unity.Mathematics;

using static NUnit.Framework.Assert;

namespace Tests.Movement
{
public class VelocityAccelerationSystemTests : SystemTestBase<VelocityAccelerationSystem>
{
    private Entity _entity;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _entity = m_Manager.CreateEntity(
            typeof(Velocity),
            typeof(Acceleration));
        m_Manager.SetComponentData(_entity, new Acceleration { Value = float3.zero });
    }

    [Test]
    public void When_AccelerationAndVelocityIsZero_VelocityIsUnchanged()
    {
        float3 initialVelocity = m_Manager.GetComponentData<Velocity>(_entity).Value;

        World.Update();

        AreEqual(initialVelocity, m_Manager.GetComponentData<Velocity>(_entity).Value);
    }

    [Test]
    public void When_AccelerationIsZero_VelocityIsUnchanged()
    {
        m_Manager.SetComponentData(_entity, new Velocity { Value = new float3(123f, 456f, 789f) });
        float3 initialVelocity = m_Manager.GetComponentData<Velocity>(_entity).Value;

        World.Update();

        AreEqual(initialVelocity, m_Manager.GetComponentData<Velocity>(_entity).Value);
    }

    [Test]
    public void When_AccelerationIsOneAndVelocityIsZero_VelocityIsIncreased()
    {
        m_Manager.SetComponentData(_entity, new Acceleration { Value = new float3(1f) });

        World.Update();

        AreEqual(new float3(ForcedDeltaTime), m_Manager.GetComponentData<Velocity>(_entity).Value);
    }
}
}
