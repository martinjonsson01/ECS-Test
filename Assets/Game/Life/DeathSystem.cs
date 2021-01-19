using BovineLabs.Event.Containers;
using BovineLabs.Event.Systems;

using Game.Enemy;
using Game.Weapon.Projectile;

using Unity.Entities;
using Unity.Jobs;

namespace Game.Life
{
[UpdateInGroup(typeof(SimulationSystemGroup))]
public class DeathSystem : SystemBase
{
    private EventSystem _eventSystem;

    protected override void OnCreate()
    {
        _eventSystem = World.GetExistingSystem<EventSystem>();
    }

    private EntityQuery _killQuery;

    protected override void OnUpdate()
    {
        NativeEventStream.IndexWriter deathWriter =
            _eventSystem.CreateEventWriter<DeathEvent>(_killQuery.CalculateEntityCount());

        Dependency = KillEntities(deathWriter);
    }

    private JobHandle KillEntities(NativeEventStream.IndexWriter deathWriter)
    {
        return ScheduleKillEntitiesJob(deathWriter);
    }

    private JobHandle ScheduleKillEntitiesJob(NativeEventStream.IndexWriter deathWriter)
    {
        JobHandle killHandle =
            Entities
                .WithName("Kill_Job")
                .ForEach((
                    Entity entity,
                    int entityInQueryIndex,
                    in Health health) =>
                {
                    if (EntityShouldLive(health)) return;

                    KillEntity(deathWriter, entityInQueryIndex, entity);
                })
                .WithStoreEntityQueryInField(ref _killQuery)
                .WithBurst()
                .Schedule(Dependency);

        _eventSystem.AddJobHandleForProducer<DeathEvent>(killHandle);
        return killHandle;
    }

    private static void KillEntity(
        NativeEventStream.IndexWriter deathWriter,
        int entityInQueryIndex,
        Entity entity
    )
    {
        /* DeathSystem */
        var deathEvent = new DeathEvent
        {
            Entity = entity
        };
        deathWriter.BeginForEachIndex(entityInQueryIndex);
        deathWriter.Write(deathEvent);
        deathWriter.EndForEachIndex();
    }

    private static bool EntityShouldLive(Health health)
    {
        return health.Value > 0f;
    }
}
}
