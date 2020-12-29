using Unity.Collections;
using Unity.Entities;

namespace Game.Spawner
{
/// <summary>
/// Warning: The subtypes of this interface do not follow LSP. The subtypes
/// do not fulfill the same contract that this subtype presents.
/// </summary>
public interface ISpawner
{
    NativeArray<Entity> Spawn(float currentTime = 0f, Allocator allocator = Allocator.Temp);
}
}
