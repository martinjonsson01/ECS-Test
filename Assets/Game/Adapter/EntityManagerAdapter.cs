using Unity.Collections;
using Unity.Entities;

namespace Game.Adapter
{
public class EntityManagerAdapter : IEntityManager
{
    private EntityManager _entityManager;

    public EntityManagerAdapter(EntityManager entityManager)
    {
        _entityManager = entityManager;
    }

    public Entity CreateEntity(IEntityArchetype archetype)
    {
        return _entityManager.CreateEntity(archetype.GetArchetype());
    }

    public void CreateEntity(IEntityArchetype archetype, NativeArray<Entity> entities)
    {
        _entityManager.CreateEntity(archetype.GetArchetype(), entities);
    }

    public T GetComponentData<T>(Entity entity) where T : struct, IComponentData
    {
        return _entityManager.GetComponentData<T>(entity);
    }

    public void AddComponentData<T>(Entity entity, T componentData) where T : struct, IComponentData
    {
        _entityManager.AddComponentData(entity, componentData);
    }

    public IEntityArchetype CreateArchetype(params ComponentType[] types)
    {
        return new EntityArchetypeAdapter(_entityManager.CreateArchetype(types));
    }

    public void SetName(Entity entity, string name)
    {
        _entityManager.SetName(entity, name);
    }
}
}
