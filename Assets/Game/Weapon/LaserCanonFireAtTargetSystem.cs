using Game.Cooldown;
using Game.Enemy;

using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Game.Weapon
{
[UpdateAfter(typeof(CooldownSystem))]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public class LaserCanonFireAtTargetSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimEcbSystem;

    protected override void OnCreate()
    {
        _endSimEcbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer.ParallelWriter ecb = _endSimEcbSystem.CreateCommandBuffer().AsParallelWriter();

        JobHandle handle =
            Entities
                .WithName(nameof(LaserCanonFireAtTargetSystem))
                .ForEach((
                    Entity entity,
                    int entityInQueryIndex,
                    in LaserCannon cannon,
                    in Translation translation,
                    in Target target,
                    in DynamicBuffer<CooldownElement> cooldowns) =>
                {
                    if (HasAnyCooldownWithType(cooldowns, CooldownType.Weapon)) return;

                    AddFireLaserComponentAndAddCooldown(target, translation, cannon, ecb, entityInQueryIndex, entity);
                })
                .WithBurst()
                .ScheduleParallel(Dependency);

        _endSimEcbSystem.AddJobHandleForProducer(handle);

        Dependency = handle;
    }

    private static void AddFireLaserComponentAndAddCooldown(
        Target target,
        Translation translation,
        LaserCannon cannon,
        EntityCommandBuffer.ParallelWriter ecb,
        int entityInQueryIndex,
        Entity entity)
    {
        float3 vectorToTargetPosition = target.Position - translation.Value;
        var fireLaser = new FireLaser
        {
            Direction = math.normalizesafe(vectorToTargetPosition),
            Damage = cannon.Damage
        };
        ecb.AddComponent(entityInQueryIndex, entity, fireLaser);
        ecb.AppendToBuffer(entityInQueryIndex, entity, new CooldownElement
        {
            Type = CooldownType.Weapon,
            Seconds = cannon.Cooldown
        });
    }


    private static bool HasAnyCooldownWithType(DynamicBuffer<CooldownElement> collection, CooldownType type)
    {
        foreach (CooldownElement item in collection)
        {
            if (item.Type == type) return true;
        }
        return false;
    }

}
}
