using Game.Movement;

using NUnit.Framework;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using static NUnit.Framework.Assert;

namespace Tests.Movement
{
[TestFixture]
public class MovementSystemTests : SystemTestBase<MovementSystem>
{
    private Entity _entity;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _entity = m_Manager.CreateEntity(
            typeof(Velocity),
            typeof(Translation));
        m_Manager.SetComponentData(_entity, new Translation { Value = new float3(0f) });
    }

    [Test]
    public void When_VelocityIsZero_Then_EntityDoesntMove()
    {
        m_Manager.SetComponentData(_entity, new Velocity { Value = new float3(0f) });

        World.Update();

        float3 actual = m_Manager.GetComponentData<Translation>(_entity).Value;
        AreEqual(new float3(0f), actual);
    }

    [Test]
    public void When_VelocityIsOne_Then_EntityMoves()
    {
        m_Manager.SetComponentData(_entity, new Velocity { Value = new float3(1f) });

        World.Update();

        float3 actual = m_Manager.GetComponentData<Translation>(_entity).Value;
        AreEqual(new float3(ForcedDeltaTime), actual);
    }

    [Test]
    public void When_VelocityIsNegative_Then_EntityMovesBackwards()
    {
        m_Manager.SetComponentData(_entity, new Velocity { Value = new float3(-1f) });

        World.Update();

        float3 actual = m_Manager.GetComponentData<Translation>(_entity).Value;
        AreEqual(new float3(-ForcedDeltaTime), actual);
    }

    [Test]
    public void When_VelocityIsVeryBig_Then_EntityMovesVeryFar()
    {
        const float factor = 100_000_000f;
        m_Manager.SetComponentData(_entity, new Velocity { Value = new float3(factor) });

        World.Update();

        float3 actual = m_Manager.GetComponentData<Translation>(_entity).Value;
        AreEqual(new float3(factor * ForcedDeltaTime), actual);
    }

    [Test]
    public void When_EntityHasConstantVelocity_Then_EntityWillMoveLinearlyForMultipleUpdates()
    {
        const float factor = 10f;
        m_Manager.SetComponentData(_entity, new Velocity { Value = new float3(factor) });

        World.Update();
        AreEqual(new float3(1 * factor * ForcedDeltaTime), m_Manager.GetComponentData<Translation>(_entity).Value);
        World.Update();
        AreEqual(new float3(2 * factor * ForcedDeltaTime), m_Manager.GetComponentData<Translation>(_entity).Value);
        World.Update();
        AreEqual(new float3(3 * factor * ForcedDeltaTime), m_Manager.GetComponentData<Translation>(_entity).Value);
        World.Update();
        AreEqual(new float3(4 * factor * ForcedDeltaTime), m_Manager.GetComponentData<Translation>(_entity).Value);
        World.Update();
        AreEqual(new float3(5 * factor * ForcedDeltaTime), m_Manager.GetComponentData<Translation>(_entity).Value);
    }
}
}
