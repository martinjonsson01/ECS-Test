using Game.Enemy;
using Game.Movement;

using NUnit.Framework;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using static NUnit.Framework.Assert;

namespace Tests.Movement
{
public class AccelerateToTargetSystemTests : SystemTestBase<AccelerateToTargetSystem>
{
    private Entity _entity;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _entity = m_Manager.CreateEntity(
            typeof(Acceleration),
            typeof(Target),
            typeof(Translation));
        m_Manager.SetComponentData(_entity, new Translation { Value = float3.zero });
    }

    [Test]
    public void When_WithinStopDistance_StopAccelerating()
    {
        m_Manager.SetComponentData(_entity, new Acceleration { Value = new float3(1f) });
        const float stopDistance = 2f;
        m_Manager.SetComponentData(_entity, new Target
        {
            Position = new float3(0f, 0f, stopDistance),
            StopDistanceSq = stopDistance * stopDistance
        });

        World.Update();

        AreEqual(float3.zero, m_Manager.GetComponentData<Acceleration>(_entity).Value);
    }

    [Test]
    public void When_OutsideStopDistance_MaxAcceleration()
    {
        const float maxAcceleration = 10f;
        m_Manager.SetComponentData(_entity, new Acceleration
        {
            Value = float3.zero,
            Max = maxAcceleration
        });
        const float stopDistance = 2f;
        m_Manager.SetComponentData(_entity, new Target
        {
            Position = new float3(0f, 0f, 100f),
            StopDistanceSq = stopDistance * stopDistance
        });

        World.Update();

        AreEqual(maxAcceleration, math.length(m_Manager.GetComponentData<Acceleration>(_entity).Value));
    }

    [Test]
    public void When_OutsideStopDistance_AccelerateDirectionTowardsTarget()
    {
        const float maxAcceleration = 10f;
        m_Manager.SetComponentData(_entity, new Acceleration
        {
            Value = float3.zero,
            Max = maxAcceleration
        });
        const float stopDistance = 2f;
        m_Manager.SetComponentData(_entity, new Target
        {
            Position = new float3(0f, 0f, 100f),
            StopDistanceSq = stopDistance * stopDistance
        });

        World.Update();

        AreEqual(new float3(0, 0, 1f), math.normalizesafe(m_Manager.GetComponentData<Acceleration>(_entity).Value));
    }
}
}
