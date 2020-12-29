using Unity.Collections;
using Unity.Entities;

namespace Game.Spawner
{
public class IntervalSpawner : ISpawner
{
    private readonly ISpawner _spawner;

    private float _lastUpdate;

    public IntervalSpawner(ISpawner spawner, float interval)
    {
        _spawner = spawner;
        Interval = interval;
    }

    public float Interval { get; set; }

    public NativeArray<Entity> Spawn(
        float currentTime,
        Allocator allocator = Allocator.Temp)
    {
        if (IntervalNotElapsed(currentTime)) return new NativeArray<Entity>(0, allocator);
        _lastUpdate = currentTime;
        return _spawner.Spawn(currentTime, allocator);
    }

    public bool IntervalNotElapsed(float currentTime)
    {
        return currentTime - _lastUpdate < Interval;
    }
}
}
