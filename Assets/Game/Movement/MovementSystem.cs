using Unity.Entities;
using Unity.Transforms;

namespace Game.Movement
{
[UpdateInGroup(typeof(SimulationSystemGroup))]
public class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.ForEach((
                    Entity entity,
                    ref Translation translation,
                    in Velocity velocity) =>
                {
                    // s = v * t
                    translation.Value += velocity.Value * deltaTime;
                })
                .WithBurst()
                .ScheduleParallel();
    }
}
}
