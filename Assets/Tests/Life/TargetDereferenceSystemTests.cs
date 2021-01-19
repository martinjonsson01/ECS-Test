using BovineLabs.Event.Containers;

using Game.Enemy;
using Game.Life;

using NUnit.Framework;

using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

namespace Tests.Life
{
public class TargetDereferenceSystemTests : EventSystemTestBase<TargetDereferenceSystem, DeathEvent>
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
        var deathEvent = new DeathEvent
        {
            Entity = _entity
        };
        NativeEventStream.ThreadWriter writer = CreateEventWriter();
        writer.Write(deathEvent);
    }
    [Test]
    public void When_EntityDies_AllTargetComponentsReferencingItAreRemoved()
    {
        var target = new Target { Entity = _entity };
        Entity targetingEntity1 = m_Manager.CreateEntity(typeof(Target));
        m_Manager.SetComponentData(targetingEntity1, target);
        Entity targetingEntity2 = m_Manager.CreateEntity(typeof(Target));
        m_Manager.SetComponentData(targetingEntity2, target);
        Entity targetingEntity3 = m_Manager.CreateEntity(typeof(Target));
        m_Manager.SetComponentData(targetingEntity3, target);

        World.GetExistingSystem<TargetDereferenceSystem>().Update();
        World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>().Update();

        Assert.IsFalse(m_Manager.HasComponent<Target>(targetingEntity1));
        Assert.IsFalse(m_Manager.HasComponent<Target>(targetingEntity2));
        Assert.IsFalse(m_Manager.HasComponent<Target>(targetingEntity3));
    }
}
}
