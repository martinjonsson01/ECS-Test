using Game.Enemy;

using Unity.Collections;
using Unity.Entities;

namespace Game.Spawner
{
public class BaseSpawner : ISpawner
{
    private readonly EntityComponentConfigurator _configurator;

    private readonly EntityCreator _creator;
    public int Amount { get; set; }

#region Public Interface

    public BaseSpawner(
        EntityCreator creator,
        EntityComponentConfigurator configurator,
        int amount)
    {
        Amount = amount;
        _configurator = configurator;
        _creator = creator;
    }

    public NativeArray<Entity> Spawn(float currentTime = 0f, Allocator allocator = Allocator.Temp)
    {
        NativeArray<Entity> spawned = _creator.CreateEntities(Amount, allocator);
        _configurator.ApplyComponentData(spawned.ToArray());
        return spawned;
    }

#endregion

}
}
