using Game.Enemy;
using Game.Weapon.Projectile;

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Game.Life
{
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ProjectileHitDamageSystem))]
[UpdateAfter(typeof(TargetPlayerSystem))]
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

        var killedEntities = new NativeList<Entity>(Allocator.TempJob);

        JobHandle killHandle =
            Entities
                .WithName("Kill_Job")
                .ForEach((
                    Entity entity,
                    int entityInQueryIndex,
                    in Health health) =>
                {
                    if (EntityShouldLive(health)) return;

                    ecb.DestroyEntity(entityInQueryIndex, entity);
                    killedEntities.Add(entity);
                })
                .WithBurst()
                .Schedule(Dependency);

        JobHandle targetDereferenceHandle =
            Entities
                .WithName("Target_Dereference_Job")
                .ForEach((
                    Entity entity,
                    int entityInQueryIndex,
                    in Target target) =>
                {
                    // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
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

        _endSimEcbSystem.AddJobHandleForProducer(killHandle);
        _endSimEcbSystem.AddJobHandleForProducer(targetDereferenceHandle);

        Dependency = targetDereferenceHandle;
    }

    private static bool EntityShouldLive(Health health)
    {
        return health.Value > 0f;
    }
}
}
