using Game.Movement;

using Unity.Entities;
using Unity.Transforms;

using static Unity.Mathematics.math;

using float3 = Unity.Mathematics.float3;

namespace Game.Enemy
{
[UpdateInGroup(typeof(SimulationSystemGroup))]
public class AccelerateToTargetSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithName(nameof(AccelerateToTargetSystem))
            .ForEach((
                ref Acceleration acceleration,
                in Target target,
                in Translation translation) =>
            {
                float distanceToTargetSq = distancesq(target.Position, translation.Value);
                float3 vectorToTarget = normalizesafe(target.Position - translation.Value);

                if (distanceToTargetSq <= target.StopDistanceSq)
                    acceleration.Value = float3.zero;
                else
                    acceleration.Value = acceleration.Max * vectorToTarget;
            })
            .WithBurst()
            .ScheduleParallel();
    }
}
}
