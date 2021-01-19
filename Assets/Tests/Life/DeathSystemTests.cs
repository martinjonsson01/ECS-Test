using BovineLabs.Event.Containers;

using Game.Life;

using NUnit.Framework;

using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;

using static NUnit.Framework.Assert;

namespace Tests.Life
{
public class DeathSystemTests : EventSystemTestBase<DeathSystem, DeathEvent>
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
    public void When_EntityHasHealth_DeathEventIsNotCreated()
    {
        World.Update();

        NativeEventStream.Reader eventReader = CreateEventReader();
        That(eventReader.Count(), Is.EqualTo(0));
    }

    [Test]
    public void When_EntityHasZeroHealth_DeathEventIsCreated()
    {
        m_Manager.SetComponentData(_entity, new Health { Value = 0f });

        World.Update();

        NativeEventStream.Reader eventReader = CreateEventReader();
        That(eventReader.Count(), Is.EqualTo(1));
    }

    [Test]
    public void When_EntityHasNegativeHealth_DeathEventIsCreated()
    {
        m_Manager.SetComponentData(_entity, new Health { Value = -10f });

        World.Update();

        NativeEventStream.Reader eventReader = CreateEventReader();
        That(eventReader.Count(), Is.EqualTo(1));
    }
}
}
