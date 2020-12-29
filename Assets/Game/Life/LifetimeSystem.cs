using Unity.Entities;
using Unity.Jobs;

namespace Game.Life
{
[UpdateInGroup(typeof(SimulationSystemGroup))]
public class LifetimeSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimEcbSystem;

    protected override void OnCreate()
    {
        _endSimEcbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer.ParallelWriter ecb = _endSimEcbSystem.CreateCommandBuffer().AsParallelWriter();

        float deltaTime = Time.DeltaTime;

        JobHandle handle =
            Entities
                .WithName(nameof(LifetimeSystem))
                .ForEach((
                    Entity entity,
                    int entityInQueryIndex,
                    ref Lifetime lifetime) =>
                {
                    DestroyIfLifetimeIsUp(lifetime, ecb, entityInQueryIndex, entity);
                    lifetime.Seconds -= deltaTime;
                })
                .WithBurst()
                .ScheduleParallel(Dependency);

        _endSimEcbSystem.AddJobHandleForProducer(handle);

        Dependency = handle;
    }

    private static void DestroyIfLifetimeIsUp(Lifetime lifetime, EntityCommandBuffer.ParallelWriter ecb, int entityInQueryIndex, Entity entity)
    {
        if (lifetime.Seconds <= 0f)
        {
            ecb.DestroyEntity(entityInQueryIndex, entity);
        }
    }
}
}
