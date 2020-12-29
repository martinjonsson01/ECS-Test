using Game.Enemy;
using Game.Extensions;
using Game.Weapon;

using Unity.Entities;

namespace Game.Cooldown
{
[UpdateInGroup(typeof(SimulationSystemGroup))]
public class CooldownSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Dependency =
            Entities
                .WithName(nameof(CooldownSystem))
                .ForEach((
                    Entity entity,
                    int entityInQueryIndex,
                    ref DynamicBuffer<CooldownElement> cooldowns) =>
                {
                    if (cooldowns.IsEmpty) return;

                    RemoveCooldownsWithTypeNone(cooldowns);

                    cooldowns = DecreaseCooldowns(cooldowns, deltaTime);

                    RemoveCooldownsThatHaveRunOut(cooldowns);
                })
                .WithoutBurst()
                .ScheduleParallel(Dependency);
    }

    private static DynamicBuffer<CooldownElement> DecreaseCooldowns(
        DynamicBuffer<CooldownElement> cooldowns,
        float deltaTime)
    {
        for (var i = 0; i < cooldowns.Length; i++)
        {
            cooldowns = DecreaseCooldown(cooldowns, deltaTime, i);
        }
        return cooldowns;
    }

    private static DynamicBuffer<CooldownElement> DecreaseCooldown(
        DynamicBuffer<CooldownElement> cooldowns,
        float deltaTime,
        int index)
    {
        CooldownElement newCooldown = cooldowns[index];
        newCooldown.Seconds -= deltaTime;
        cooldowns[index] = newCooldown;
        return cooldowns;
    }

    private static void RemoveCooldownsThatHaveRunOut(DynamicBuffer<CooldownElement> cooldowns)
    {
        static bool ExcludeCooldownsOverZero(CooldownElement cooldown) => cooldown.Seconds > 0f;
        cooldowns.RemoveElementsButExcludeSome(ExcludeCooldownsOverZero);
    }

    private static void RemoveCooldownsWithTypeNone(DynamicBuffer<CooldownElement> cooldowns)
    {
        static bool ExcludeCooldownsOfOtherTypes(CooldownElement cooldown) => cooldown.Type != CooldownType.None;
        cooldowns.RemoveElementsButExcludeSome(ExcludeCooldownsOfOtherTypes);
    }
}
}
