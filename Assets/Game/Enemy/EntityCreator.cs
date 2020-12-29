using Game.Adapter;

using Unity.Collections;
using Unity.Entities;

namespace Game.Enemy
{
public class EntityCreator
{
    private readonly IEntityArchetype _archetype;
    private readonly string _entityName;
    private readonly IEntityManager _entityManager;

    public EntityCreator(IEntityManager entityManager, IEntityArchetype archetype, string entityName)
    {
        _entityManager = entityManager;
        _archetype = archetype;
        _entityName = entityName;
    }

    public NativeArray<Entity> CreateEntities(int amount = 1, Allocator allocator = Allocator.Temp)
    {
        var spawned = new NativeArray<Entity>(amount, allocator);
        _entityManager.CreateEntity(_archetype, spawned);
        foreach (Entity entity in spawned)
        {
            _entityManager.SetName(entity, $"{_entityName} {entity.Index}");
        }
        return spawned;
    }
}
}
