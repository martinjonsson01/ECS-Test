using System;

using Unity.Collections;
using Unity.Entities;

namespace Game.Spawner
{
public class LimitedSpawner : ISpawner
{
    public int SpawnLimit
    {
        get => _spawnLimit;
        set
        {
            if (value >= 0)
            {
                _spawnLimit = value;
            }
            else
            {
                throw new ArgumentException("Spawn limit can not be negative.");
            }
        }
    }

    private readonly ISpawner _spawner;
    private int _spawnCount;
    private int _spawnLimit;

    public LimitedSpawner(ISpawner spawner, int spawnLimit)
    {
        _spawner = spawner;
        SpawnLimit = spawnLimit;
    }

    public NativeArray<Entity> Spawn(float currentTime = 0, Allocator allocator = Allocator.Temp)
    {
        if (_spawnCount >= SpawnLimit) return new NativeArray<Entity>(0, allocator);

        _spawnCount++;
        return _spawner.Spawn(currentTime, allocator);
    }
}
}
