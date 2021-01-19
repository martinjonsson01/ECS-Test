using Game.Enemy;
using Game.Life;

using NUnit.Framework;

using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;

using static NUnit.Framework.Assert;

namespace Tests.Life
{
public class DeathSystemTests : SystemTestBase<DeathSystem>
{
    private Entity _entity;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _entity = m_Manager.CreateEntity(
            typeof(Health),
            typeof(Translation),
            typeof(RenderMesh));
        m_Manager.AddComponentData(_entity, new Health { Value = 10f });
    }

    [Test]
    public void When_EntityHasHealth_EntityStillExists()
    {
        World.Update();

        IsTrue(m_Manager.Exists(_entity));
    }

    [Test]
    public void When_EntityHasZeroHealth_EntityIsRemoved()
    {
        m_Manager.SetComponentData(_entity, new Health { Value = 0f });

        World.Update();

        IsFalse(m_Manager.Exists(_entity));
    }

    [Test]
    public void When_EntityHasNegativeHealth_EntityIsRemoved()
    {
        m_Manager.SetComponentData(_entity, new Health { Value = -10f });

        World.Update();

        IsFalse(m_Manager.Exists(_entity));
    }

    [Test]
    public void When_EntityIsRemoved_AllTargetComponentsReferencingItAreAlsoRemoved()
    {
        var target = new Target { Entity = _entity };
        Entity targetingEntity1 = m_Manager.CreateEntity(typeof(Target));
        m_Manager.SetComponentData(targetingEntity1, target);
        Entity targetingEntity2 = m_Manager.CreateEntity(typeof(Target));
        m_Manager.SetComponentData(targetingEntity2, target);
        Entity targetingEntity3 = m_Manager.CreateEntity(typeof(Target));
        m_Manager.SetComponentData(targetingEntity3, target);
        m_Manager.SetComponentData(_entity, new Health { Value = 0f });

        World.Update();

        IsFalse(m_Manager.Exists(_entity));
        IsFalse(m_Manager.HasComponent<Target>(targetingEntity1));
        IsFalse(m_Manager.HasComponent<Target>(targetingEntity2));
        IsFalse(m_Manager.HasComponent<Target>(targetingEntity3));
    }
}
}
