using BovineLabs.Event.Containers;
using BovineLabs.Event.Systems;

using Game.Enemy;

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Game.Life
{
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(KillSystem))]
public class TargetDereferenceSystem : ConsumeEventSystemBase<DeathEvent>
{
    private EndSimulationEntityCommandBufferSystem _endSimEcbSystem;

    protected override void Create()
    {
        _endSimEcbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnEventStream(ref NativeEventStream.Reader reader, int eventCount)
    {
        if (eventCount < 1) return;

        var killedEntities = new NativeList<Entity>(eventCount, Allocator.TempJob);

        for (var i = 0; i < reader.Count(); i++)
        {
            var death = reader.Read<DeathEvent>();
            killedEntities.Add(death.Entity);
        }

        EntityCommandBuffer.ParallelWriter ecb = _endSimEcbSystem.CreateCommandBuffer().AsParallelWriter();

        Dependency = ScheduleTargetDereferenceJob(ecb, killedEntities, Dependency);
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
}
}
