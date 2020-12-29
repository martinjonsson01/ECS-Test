using Unity.Collections;
using Unity.Entities;

namespace Game.Adapter
{
public interface IEntityManager
{
    Entity CreateEntity(IEntityArchetype archetype);
    void CreateEntity(IEntityArchetype archetype, NativeArray<Entity> entities);
    T GetComponentData<T>(Entity entity) where T : struct, IComponentData;
    void AddComponentData<T>(Entity entity, T componentData) where T : struct, IComponentData;
    IEntityArchetype CreateArchetype(params ComponentType[] types);
    void SetName(Entity entity, string name);
}
}
