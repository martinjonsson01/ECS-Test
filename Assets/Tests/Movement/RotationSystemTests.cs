using Game.Movement;

using NUnit.Framework;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using static NUnit.Framework.Assert;

namespace Tests.Movement
{
public class RotationSystemTests : SystemTestBase<RotationSystem>
{
    private Entity _entity;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _entity = m_Manager.CreateEntity(
            typeof(Rotation),
            typeof(Velocity));
    }

    [Test]
    public void When_VelocityIsZeroAndRotationIsZero_RotationRemainsUnchanged()
    {
        quaternion initialRotation = m_Manager.GetComponentData<Rotation>(_entity).Value;
        m_Manager.SetComponentData(_entity, new Velocity { Value = new float3(0f) });

        World.Update();

        AreEqual(initialRotation, m_Manager.GetComponentData<Rotation>(_entity).Value);
    }

    [Test]
    public void When_VelocityIsZeroAndRotationIsOne_RotationRemainsUnchanged()
    {
        quaternion initialRotation = quaternion.EulerZXY(new float3(math.PI / 4f));
        m_Manager.SetComponentData(_entity, new Velocity { Value = new float3(0f) });
        m_Manager.SetComponentData(_entity,
            new Rotation { Value = initialRotation });

        World.Update();

        AreEqual(initialRotation, m_Manager.GetComponentData<Rotation>(_entity).Value);
    }

    [Test]
    public void When_VelocityIsOne_RotationIsInSameDirection()
    {
        m_Manager.SetComponentData(_entity, new Velocity { Value = new float3(1f) });

        World.Update();

        var expected = new quaternion(-0.2798481f, 0.3647052f, 0.1159169f, 0.8804762f);
        AreEqual(expected.ToString(), m_Manager.GetComponentData<Rotation>(_entity).Value.ToString());
    }

    [Test]
    public void When_VelocityIsX1Z1_RotationIsInSameDirection()
    {
        m_Manager.SetComponentData(_entity, new Velocity { Value = new float3(1f, 0f, 1f) });

        World.Update();

        var expected = new quaternion(0f, 0.3826835f, 0f, 0.9238796f);
        AreEqual(expected.ToString(), m_Manager.GetComponentData<Rotation>(_entity).Value.ToString());
    }
}
}
