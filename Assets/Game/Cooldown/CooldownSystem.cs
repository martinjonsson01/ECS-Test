using Unity.Collections;
using Unity.Entities;

// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable ForCanBeConvertedToForeach

// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator

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

                    /***
                     * I want to apply functional decomposition all this below,
                     * but Burst won't allow me. If this is fixed in the future,
                     * please do.
                     */
                    var cooldownsWithTypeNone = new NativeList<CooldownElement>(Allocator.Temp);
                    for (var i = 0; i < cooldowns.Length; i++)
                    {
                        CooldownElement cooldown = cooldowns[i];
                        if (cooldown.Type != CooldownType.None) continue;
                        cooldownsWithTypeNone.Add(cooldown);
                    }
                    for (var index = 0; index < cooldownsWithTypeNone.Length; index++)
                    {
                        CooldownElement toRemove = cooldownsWithTypeNone[index];
                        int removeIndex = -1;
                        for (var i = 0; i < cooldowns.Length; i++)
                        {
                            if (!cooldowns.ElementAt(i).Equals(toRemove)) continue;
                            removeIndex = i;
                            break;
                        }
                        if (removeIndex == -1) break;
                        cooldowns.RemoveAtSwapBack(removeIndex);
                    }

                    for (var i = 0; i < cooldowns.Length; i++)
                    {
                        CooldownElement newCooldown = cooldowns[i];
                        newCooldown.Seconds -= deltaTime;
                        cooldowns[i] = newCooldown;
                    }

                    var runOutCooldowns = new NativeList<CooldownElement>(Allocator.Temp);
                    for (var i = 0; i < cooldowns.Length; i++)
                    {
                        CooldownElement cooldown = cooldowns[i];
                        if (cooldown.Seconds > 0f) continue;
                        runOutCooldowns.Add(cooldown);
                    }
                    for (var i = 0; i < runOutCooldowns.Length; i++)
                    {
                        CooldownElement toRemove1 = runOutCooldowns[i];
                        int removeIndex = -1;
                        for (var j = 0; j < cooldowns.Length; j++)
                        {
                            if (!cooldowns.ElementAt(j).Equals(toRemove1)) continue;
                            removeIndex = j;
                            break;
                        }
                        if (removeIndex == -1) break;
                        cooldowns.RemoveAtSwapBack(removeIndex);
                    }
                })
                .WithBurst()
                .ScheduleParallel(Dependency);
    }
}
}
