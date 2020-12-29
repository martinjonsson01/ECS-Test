using Unity.Entities;

namespace Game.Adapter
{
public class EntityArchetypeAdapter : IEntityArchetype
{
    private readonly EntityArchetype _archetype;

    public EntityArchetypeAdapter(EntityArchetype archetype)
    {
        _archetype = archetype;
    }

    public EntityArchetype GetArchetype()
    {
        return _archetype;
    }
}
}
