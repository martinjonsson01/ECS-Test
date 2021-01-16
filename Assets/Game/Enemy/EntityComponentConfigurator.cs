using System;
using System.Collections.Generic;

using Game.Adapter;

using JetBrains.Annotations;

using Unity.Entities;

namespace Game.Enemy
{
public class EntityComponentConfigurator
{
    [ItemNotNull] private readonly ICollection<Func<IRandom, IComponentData>> _componentDataFunctions;
    private readonly IEntityManager _entityManager;
    private readonly IRandom _random;

    public EntityComponentConfigurator(
        [NotNull] IEntityManager entityManager,
        [NotNull] IRandom random,
        [NotNull] [ItemNotNull] ICollection<Func<IRandom, IComponentData>> componentDataFunctions)
    {
        _entityManager = entityManager;
        _random = random;
        _componentDataFunctions = new List<Func<IRandom, IComponentData>>(componentDataFunctions);
    }

#region Public Interface

    public void ApplyComponentData(params Entity[] entities)
    {
        foreach (Entity entity in entities) ApplyComponentData(entity);
    }

    public void AddComponentDataFunction([NotNull] Func<IRandom, IComponentData> function)
    {
        _componentDataFunctions.Add(function);
    }

#endregion

    private void ApplyComponentData(Entity entity)
    {
        foreach (Func<IRandom, IComponentData> componentDataFunction in _componentDataFunctions)
        {
            dynamic componentData = componentDataFunction(_random);
            _entityManager.AddComponentData(entity, componentData);
        }
    }
}
}
