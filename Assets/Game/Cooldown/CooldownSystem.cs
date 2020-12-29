using Unity.Collections;
using Unity.Entities;

// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
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

                    RemoveCooldownsWithTypeNone(cooldowns);

                    cooldowns = DecreaseCooldowns(cooldowns, deltaTime);

                    RemoveCooldownsThatHaveRunOut(cooldowns);

                    static DynamicBuffer<CooldownElement> DecreaseCooldowns(
                        DynamicBuffer<CooldownElement> cooldowns,
                        float deltaTime)
                    {
                        for (var i = 0; i < cooldowns.Length; i++)
                        {
                            cooldowns = DecreaseCooldown(cooldowns, deltaTime, i);
                        }
                        return cooldowns;
                    }

                    static DynamicBuffer<CooldownElement> DecreaseCooldown(
                        DynamicBuffer<CooldownElement> cooldowns,
                        float deltaTime,
                        int index)
                    {
                        CooldownElement newCooldown = cooldowns[index];
                        newCooldown.Seconds -= deltaTime;
                        cooldowns[index] = newCooldown;
                        return cooldowns;
                    }

                    static void RemoveCooldownsThatHaveRunOut(DynamicBuffer<CooldownElement> cooldowns)
                    {
                        NativeList<CooldownElement> cooldownsExcludingSome = FindCooldownsThatHaveRunOut(cooldowns);
                        RemoveCooldowns(cooldowns, cooldownsExcludingSome);
                    }

                    static void RemoveCooldownsWithTypeNone(DynamicBuffer<CooldownElement> cooldowns)
                    {
                        NativeList<CooldownElement> cooldownsExcludingSome = FindCooldownsWithTypeNone(cooldowns);
                        RemoveCooldowns(cooldowns, cooldownsExcludingSome);
                    }

                    static void RemoveCooldowns(
                        DynamicBuffer<CooldownElement> elements,
                        NativeList<CooldownElement> toRemoves)
                    {
                        foreach (CooldownElement toRemove in toRemoves)
                        {
                            TryRemoveCooldown(elements, toRemove);
                        }
                    }

                    static void TryRemoveCooldown(DynamicBuffer<CooldownElement> elements, CooldownElement toRemove)
                    {
                        int removeIndex = FindIndexOf(elements, toRemove);
                        if (removeIndex == -1) return;
                        elements.RemoveAtSwapBack(removeIndex);
                    }

                    static NativeList<CooldownElement> FindCooldownsWithTypeNone(
                        DynamicBuffer<CooldownElement> elements)
                    {
                        var cooldownsWithTypeNone = new NativeList<CooldownElement>(Allocator.Temp);
                        foreach (CooldownElement cooldown in elements)
                        {
                            if (cooldown.Type != CooldownType.None) continue;
                            cooldownsWithTypeNone.Add(cooldown);
                        }

                        return cooldownsWithTypeNone;
                    }

                    static NativeList<CooldownElement> FindCooldownsThatHaveRunOut(
                        DynamicBuffer<CooldownElement> elements)
                    {
                        var cooldownsThatHaveRunOut = new NativeList<CooldownElement>(Allocator.Temp);
                        foreach (CooldownElement cooldown in elements)
                        {
                            if (cooldown.Seconds > 0f) continue;
                            cooldownsThatHaveRunOut.Add(cooldown);
                        }

                        return cooldownsThatHaveRunOut;
                    }

                    static int FindIndexOf(DynamicBuffer<CooldownElement> elements, CooldownElement toFind)
                    {
                        int removeIndex = -1;
                        for (var i = 0; i < elements.Length; i++)
                        {
                            if (!elements.ElementAt(i).Equals(toFind)) continue;
                            removeIndex = i;
                            break;
                        }

                        return removeIndex;
                    }
                })
                .WithBurst()
                .ScheduleParallel(Dependency);
    }
}
}
