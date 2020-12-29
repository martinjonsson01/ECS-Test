using Unity.Entities;

namespace Game.Movement
{
[UpdateBefore(typeof(MovementSystem))]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public class VelocityAccelerationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities
            .WithName(nameof(VelocityAccelerationSystem))
            .ForEach((
                ref Velocity velocity,
                in Acceleration acceleration) =>
            {
                // v = v0 + a * t
                velocity.Value += acceleration.Value * deltaTime;
            })
            .WithBurst()
            .ScheduleParallel();
    }
}
}
