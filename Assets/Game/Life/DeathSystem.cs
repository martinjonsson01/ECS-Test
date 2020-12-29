using Unity.Entities;
using Unity.Jobs;

namespace Game.Life
{
public class DeathSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimEcbSystem;

    protected override void OnCreate()
    {
        _endSimEcbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer.ParallelWriter ecb = _endSimEcbSystem.CreateCommandBuffer().AsParallelWriter();

        JobHandle targetHandle =
            Entities
                .WithName(nameof(DeathSystem))
                .ForEach((
                    Entity entity,
                    int entityInQueryIndex,
                    in Health health) =>
                {
                    if (health.Value > 0f) return;

                    ecb.DestroyEntity(entityInQueryIndex, entity);
                })
                .WithoutBurst()
                .Schedule(Dependency);

        _endSimEcbSystem.AddJobHandleForProducer(targetHandle);

        Dependency = targetHandle;
    }
}
}
