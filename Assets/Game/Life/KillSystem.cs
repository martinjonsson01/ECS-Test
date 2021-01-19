using BovineLabs.Event.Jobs;
using BovineLabs.Event.Systems;

using Game.Enemy;

using Unity.Burst;
using Unity.Entities;

namespace Game.Life
{
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(TargetPlayerSystem))]
public class KillSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _bufferSystem;
    private EventSystem _eventSystem;

    protected override void OnCreate()
    {
        _eventSystem = World.GetExistingSystem<EventSystem>();
        _bufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    [BurstCompile]
    private struct KillJob : IJobEvent<DeathEvent>
    {
        public EntityCommandBuffer Ecb;

        public void Execute(DeathEvent death)
        {
            if (death.Entity == Entity.Null) return;

            Ecb.DestroyEntity(death.Entity);
        }
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = _bufferSystem.CreateCommandBuffer();

        var job = new KillJob
        {
            Ecb = ecb
        };

        Dependency =
            job.Schedule<KillJob, DeathEvent>(_eventSystem, Dependency);

        _bufferSystem.AddJobHandleForProducer(Dependency);
    }
}
}
