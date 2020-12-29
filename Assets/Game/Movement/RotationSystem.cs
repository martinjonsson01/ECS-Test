using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Game.Movement
{
[UpdateAfter(typeof(VelocityAccelerationSystem))]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public class RotationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithName(nameof(RotationSystem))
            .ForEach((
                ref Rotation rotation,
                in Velocity velocity) =>
            {
                if (velocity.Value.Equals(float3.zero)) return;

                // TODO: Apply rotation in acceleration direction instead.

                float3 direction = math.normalizesafe(velocity.Value);
                quaternion newRotation = quaternion.LookRotation(direction, math.up());
                rotation.Value = newRotation;
            })
            .WithBurst()
            .ScheduleParallel();
    }
}
}
