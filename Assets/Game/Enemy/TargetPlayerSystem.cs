using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

using UnityEngine;

namespace Game.Enemy
{
[UpdateInGroup(typeof(SimulationSystemGroup))]
public class TargetPlayerSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimEcbSystem;

    protected override void OnCreate()
    {
        _endSimEcbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float3? playerPos = null;
        if (Camera.main != null)
        {
            playerPos = Camera.main.transform.position;
        }

        EntityCommandBuffer.ParallelWriter ecb = _endSimEcbSystem.CreateCommandBuffer().AsParallelWriter();

        JobHandle targetHandle =
            Entities
                .WithName(nameof(TargetPlayerSystem))
                .WithAll<TargetsPlayerTag>()
                .ForEach((
                    Entity entity,
                    int entityInQueryIndex) =>
                {
                    if (!playerPos.HasValue) return;

                    var followTarget = new Target { Position = playerPos.Value, StopDistanceSq = 10f * 10f};
                    ecb.AddComponent(entityInQueryIndex, entity, followTarget);
                })
                .WithBurst()
                .ScheduleParallel(Dependency);

        _endSimEcbSystem.AddJobHandleForProducer(targetHandle);

        Dependency = targetHandle;
    }
}
}
