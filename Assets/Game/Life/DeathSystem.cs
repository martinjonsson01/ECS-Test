using BovineLabs.Event.Containers;
using BovineLabs.Event.Systems;

using Game.Enemy;
using Game.Life.Explosion;
using Game.Weapon.Projectile;

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

using UnityEngine;

using static Unity.Mathematics.math;

namespace Game.Life
{
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ProjectileHitDamageSystem))]
[UpdateAfter(typeof(TargetPlayerSystem))]
public class DeathSystem : SystemBase
{
    private const float DefaultEntitySize = 3f;
    private EventSystem _eventSystem;
    private EndSimulationEntityCommandBufferSystem _endSimEcbSystem;

    protected override void OnCreate()
    {
        _eventSystem = World.GetExistingSystem<EventSystem>();
        _endSimEcbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    private EntityQuery _killQuery;
    private EntityQuery _meshQuery;

    protected override void OnUpdate()
    {
        NativeEventStream.IndexWriter eventWriter =
            _eventSystem.CreateEventWriter<ExplosionEvent>(_killQuery.CalculateEntityCount());

        EntityCommandBuffer.ParallelWriter ecb = _endSimEcbSystem.CreateCommandBuffer().AsParallelWriter();

        ComponentDataFromEntity<ExplodesOnDeath> explodesOnDeath = GetComponentDataFromEntity<ExplodesOnDeath>();

        Dependency = KillEntities(ecb, eventWriter, FetchEntityBounds(), explodesOnDeath);
    }

    private JobHandle KillEntities(
        EntityCommandBuffer.ParallelWriter ecb,
        NativeEventStream.IndexWriter eventWriter,
        NativeHashMap<Entity, float> entityBounds,
        ComponentDataFromEntity<ExplodesOnDeath> explodesOnDeath)
    {
        var killedEntities = new NativeList<Entity>(Allocator.TempJob);

        JobHandle killHandle = ScheduleKillEntitiesJob(ecb, eventWriter, entityBounds, killedEntities,
            explodesOnDeath);

        JobHandle targetDereferenceHandle = ScheduleTargetDereferenceJob(ecb, killedEntities, killHandle);

        return targetDereferenceHandle;
    }

    private JobHandle ScheduleKillEntitiesJob(
        EntityCommandBuffer.ParallelWriter ecb,
        NativeEventStream.IndexWriter eventWriter,
        NativeHashMap<Entity, float> entityBounds,
        NativeList<Entity> killedEntities,
        ComponentDataFromEntity<ExplodesOnDeath> explodesOnDeath)
    {
        JobHandle killHandle =
            Entities
                .WithName("Kill_Job")
                .ForEach((
                    Entity entity,
                    int entityInQueryIndex,
                    in Health health,
                    in Translation translation) =>
                {
                    if (EntityShouldLive(health)) return;

                    KillEntity(ecb, eventWriter, entityBounds, killedEntities, entityInQueryIndex, entity, translation, explodesOnDeath);
                })
                .WithStoreEntityQueryInField(ref _killQuery)
                .WithDisposeOnCompletion(entityBounds)
                .WithBurst()
                .Schedule(Dependency);

        _endSimEcbSystem.AddJobHandleForProducer(killHandle);
        _eventSystem.AddJobHandleForProducer<ExplosionEvent>(killHandle);
        return killHandle;
    }

    private static void KillEntity(
        EntityCommandBuffer.ParallelWriter ecb,
        NativeEventStream.IndexWriter eventWriter,
        NativeHashMap<Entity, float> entityBounds,
        NativeList<Entity> killedEntities,
        int entityInQueryIndex,
        Entity entity,
        Translation translation,
        ComponentDataFromEntity<ExplodesOnDeath> explodesOnDeath
    )
    {
        ecb.DestroyEntity(entityInQueryIndex, entity);
        killedEntities.Add(entity);
        if (explodesOnDeath.HasComponent(entity))
        {
            CreateExplosionEvent(eventWriter, entityInQueryIndex, entity, translation.Value, entityBounds);
        }
    }

    private JobHandle ScheduleTargetDereferenceJob(
        EntityCommandBuffer.ParallelWriter ecb,
        NativeList<Entity> killedEntities,
        JobHandle killHandle)
    {
        JobHandle targetDereferenceHandle =
            Entities
                .WithName("Target_Dereference_Job")
                .ForEach((
                    Entity entity,
                    int entityInQueryIndex,
                    in Target target) =>
                {
                    foreach (Entity killedEntity in killedEntities)
                    {
                        if (target.Entity.Equals(killedEntity))
                        {
                            ecb.RemoveComponent<Target>(entityInQueryIndex, entity);
                        }
                    }
                })
                .WithDisposeOnCompletion(killedEntities)
                .WithBurst()
                .Schedule(killHandle);

        _endSimEcbSystem.AddJobHandleForProducer(targetDereferenceHandle);
        return targetDereferenceHandle;
    }

    private NativeHashMap<Entity, float> FetchEntityBounds()
    {
        int calculateEntityCount = _meshQuery.CalculateEntityCount();
        var entityBounds = new NativeHashMap<Entity, float>(calculateEntityCount, Allocator.TempJob);
        Entities
            .WithName("Fetch_Entity_LargestBound_Job")
            .ForEach((
                Entity entity,
                int entityInQueryIndex,
                in RenderMesh renderMesh) =>
            {
                if (renderMesh.mesh == null) return;
                Bounds bounds = renderMesh.mesh.bounds;
                float largestBound = max(max(bounds.size.x, bounds.size.y), bounds.size.z);
                entityBounds.Add(entity, largestBound);
            })
            .WithStoreEntityQueryInField(ref _meshQuery)
            .WithoutBurst()
            .Run();
        return entityBounds;
    }

    private static void CreateExplosionEvent(
        NativeEventStream.IndexWriter eventWriter,
        int entityInQueryIndex,
        Entity entity,
        float3 position,
        NativeHashMap<Entity, float> entityBounds)
    {
        float largestBound = GetLargestBound(entity, entityBounds);
        var explosion = new ExplosionEvent
        {
            Position = position,
            Size = largestBound
        };
        eventWriter.BeginForEachIndex(entityInQueryIndex);
        eventWriter.Write(explosion);
        eventWriter.EndForEachIndex();
    }

    private static float GetLargestBound(Entity entity, NativeHashMap<Entity, float> entityBounds)
    {
        if (!entityBounds.TryGetValue(entity, out float largestBound))
        {
            largestBound = DefaultEntitySize;
        }
        return largestBound;
    }

    private static bool EntityShouldLive(Health health)
    {
        return health.Value > 0f;
    }
}
}
