using Game.Movement;

using NUnit.Framework;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Tests.Movement
{
public class SmoothRotationSystemTests : SystemTestBase<SmoothRotationSystem>
{
    private Entity _entity;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _entity = m_Manager.CreateEntity(
            typeof(Rotation),
            typeof(DesiredRotation));
        m_Manager.SetComponentData(_entity, new DesiredRotation
        {
            Value = new quaternion(0.5f, 0.5f, 0f, 0.9f),
            TurningRate = 3f
        });
    }

    [Test]
    public void When_EntityHasNoDesiredRotation_RotationIsUnchanged()
    {
        m_Manager.RemoveComponent<DesiredRotation>(_entity);
        quaternion rotationBefore = m_Manager.GetComponentData<Rotation>(_entity).Value;

        World.Update();

        quaternion rotationAfter = m_Manager.GetComponentData<Rotation>(_entity).Value;
        Assert.That(rotationAfter, Is.EqualTo(rotationBefore));
    }

    [Test]
    public void When_EntityHasDesiredRotation_RotationIsChanged()
    {
        quaternion rotationBefore = m_Manager.GetComponentData<Rotation>(_entity).Value;

        World.Update();

        quaternion rotationAfter = m_Manager.GetComponentData<Rotation>(_entity).Value;
        Assert.That(rotationAfter, Is.Not.EqualTo(rotationBefore));
    }
}
}
