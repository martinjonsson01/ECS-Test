using BovineLabs.Event.Containers;

using Game.Life;
using Game.Life.Explosion;

using NUnit.Framework;

using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

namespace Tests.Life.Explosion
{
public class ExplosionSystemTests : EventSystemTestBase<ExplosionSystem, ExplosionEvent>
{
    private Entity _dyingEntity;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _dyingEntity = m_Manager.CreateEntity(
            typeof(ExplodesOnDeath),
            typeof(Health),
            typeof(Translation),
            typeof(RenderMesh));
        m_Manager.AddComponentData(_dyingEntity, new Health { Value = 0f });
    }

    [Test]
    public void When_EntityDies_WithExplodesOnDeath_ExplosionEventIsCreated()
    {
        var deathSystem = World.CreateSystem<DeathSystem>();

        deathSystem.Update();

        NativeEventStream.Reader stream = GetEventReader();
        int eventCount = stream.BeginForEachIndex(0);
        Assert.That(eventCount, Is.EqualTo(1));
    }

    [Test]
    public void When_EntityDies_WithoutExplodesOnDeath_ExplosionEventIsNotCreated()
    {
        m_Manager.RemoveComponent<ExplodesOnDeath>(_dyingEntity);
        var deathSystem = World.CreateSystem<DeathSystem>();

        deathSystem.Update();

        NativeEventStream.Reader eventReader = GetEventReader();
        Assert.That(0, Is.EqualTo(eventReader.Count() - 1));
    }
}
}
