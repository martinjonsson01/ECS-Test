using Game.Adapter;

using Unity.Collections;
using Unity.Entities;

namespace Game.Spawner
{
public class ChanceSpawner : ISpawner
{
    public float Chance { get; set; }

    private ISpawner _spawner;
    private IRandom _random;

    public ChanceSpawner(ISpawner spawner, IRandom random, float chance)
    {
        Chance = chance;
        _random = random;
        _spawner = spawner;
    }

    public NativeArray<Entity> Spawn(float currentTime = 0, Allocator allocator = Allocator.Temp)
    {
        if (_random.NextFloat() > Chance) return new NativeArray<Entity>(0, allocator);

        return _spawner.Spawn(currentTime, allocator);
    }
}
}
