using BovineLabs.Event.Containers;

namespace Tests.Life
{
using Game.Life;

using NUnit.Framework;

using Unity.Entities;

using static NUnit.Framework.Assert;

namespace Tests.Life
{
public class KillSystemTests : EventSystemTestBase<KillSystem, DeathEvent>
{
    private Entity _entity;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _entity = m_Manager.CreateEntity();
    }

    [Test]
    public void When_NoDeathEvent_EntityStillExists()
    {
        World.Update();

        IsTrue(m_Manager.Exists(_entity));
    }

    [Test]
    public void When_DeathEventContainsEntity_EntityIsRemoved()
    {
        var deathEvent = new DeathEvent
        {
            Entity = _entity
        };
        NativeEventStream.ThreadWriter writer = CreateEventWriter();
        writer.Write(deathEvent);

        World.GetExistingSystem<KillSystem>().Update();
        World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>().Update();

        IsFalse(m_Manager.Exists(_entity));
    }
}
}

}
