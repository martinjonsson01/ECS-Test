using System.Collections.Generic;

using BovineLabs.Event.Containers;
using BovineLabs.Event.Systems;

using Game.Enemy;
using Game.Life;
using Game.Life.Explosion;

using NUnit.Framework;

using Unity.Entities;
using Unity.Jobs;
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

    private EventSystem SetUpEventSystem()
    {
        var eventSystem = World.GetExistingSystem<EventSystem>();
        // Need to create a single event so that there is something to read from when asserting.
        NativeEventStream.ThreadWriter writer = eventSystem.CreateEventWriter<ExplosionEvent>();
        eventSystem.AddJobHandleForProducer<ExplosionEvent>(default);
        writer.Write(new ExplosionEvent { Size = 1f, Position = 3f });
        return eventSystem;
    }

    private static NativeEventStream.Reader GetEventReader(EventSystemBase eventSystem)
    {
        JobHandle getEventReadersHandle = eventSystem.GetEventReaders<ExplosionEvent>(
            default, out IReadOnlyList<NativeEventStream.Reader> eventReaders);
        getEventReadersHandle.Complete();
        eventSystem.AddJobHandleForConsumer<ExplosionEvent>(getEventReadersHandle);
        NativeEventStream.Reader eventReader = eventReaders[0];
        return eventReader;
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

    [Test]
    public void When_EntityDies_ExplosionEventIsCreated()
    {
        EventSystem eventSystem = SetUpEventSystem();
        m_Manager.AddComponent<ExplodesOnDeath>(_entity);
        m_Manager.SetComponentData(_entity, new Health { Value = 0f });

        World.Update();

        NativeEventStream.Reader stream = GetEventReader(eventSystem);
        int eventCount = stream.BeginForEachIndex(0);
        AreEqual(1, eventCount);
    }
}
}
