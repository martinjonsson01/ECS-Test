using System.Collections.Generic;

using BovineLabs.Event.Containers;
using BovineLabs.Event.Systems;

using Unity.Entities;
using Unity.Jobs;

namespace Tests
{
public class EventSystemTestBase<TSystem, TEvent> : SystemTestBase<TSystem>
    where TSystem : ComponentSystemBase
    where TEvent : struct
{
    protected EventSystem EventSystem;

    public override void Setup()
    {
        base.Setup();
        EventSystem = SetUpEventSystem();
    }

    private EventSystem SetUpEventSystem()
    {
        // EventSystem has to be created in SystemTestBase so that it is created before all other systems.
        var eventSystem = World.GetExistingSystem<EventSystem>();
        // Need to create a single event so that there is something to read from when asserting.
        NativeEventStream.ThreadWriter writer = eventSystem.CreateEventWriter<TEvent>();
        eventSystem.AddJobHandleForProducer<TEvent>(default);
        writer.Write(new TEvent());
        return eventSystem;
    }

    protected NativeEventStream.Reader GetEventReader()
    {
        JobHandle getEventReadersHandle = EventSystem.GetEventReaders<TEvent>(
            default, out IReadOnlyList<NativeEventStream.Reader> eventReaders);
        getEventReadersHandle.Complete();
        EventSystem.AddJobHandleForConsumer<TEvent>(getEventReadersHandle);
        NativeEventStream.Reader eventReader = eventReaders[0];
        return eventReader;
    }

}
}
