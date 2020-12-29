using System;
using System.Collections.Generic;

using Game.Adapter;
using Game.Enemy;

using Unity.Entities;

using Random = Unity.Mathematics.Random;

namespace Game.Spawner
{
public class SpawnerFactory
{
    public ISpawner CreateBaseSpawner(
        EntityManager manager,
        EntityArchetype archetype,
        Random random,
        int amount,
        ICollection<Func<IRandom, IComponentData>> componentDataFunctions,
        string entityName)
    {
        return CreateBaseSpawner(
            new EntityManagerAdapter(manager),
            new EntityArchetypeAdapter(archetype),
            new RandomAdapter(random),
            amount,
            componentDataFunctions,
            entityName
        );
    }

    public ISpawner CreateBaseSpawner(
        IEntityManager manager,
        IEntityArchetype archetype,
        IRandom random,
        int amount,
        ICollection<Func<IRandom, IComponentData>> componentDataFunctions,
        string entityName)
    {
        var creator = new EntityCreator(
            manager,
            archetype,
            entityName
        );
        var configurator = new EntityComponentConfigurator(
            manager,
            random,
            componentDataFunctions
        );
        var baseSpawner = new BaseSpawner(
            creator,
            configurator,
            amount
        );
        return baseSpawner;
    }
}
}
