using System;
using System.Collections.Generic;

using Game.Adapter;
using Game.Enemy;

using NSubstitute;

using NUnit.Framework;

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using static NUnit.Framework.Assert;

namespace Tests.Spawner
{
[Category("Core Tests")]
public class EntityComponentConfiguratorTests : WorldTestBase
{
    private ICollection<Func<IRandom, IComponentData>> _componentDatas;
    private EntityComponentConfigurator _configurator;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        var manager = new EntityManagerAdapter(EntityManager);
        _componentDatas = new List<Func<IRandom, IComponentData>>();
        var random = Substitute.For<IRandom>();
        _configurator = new EntityComponentConfigurator(manager, random, _componentDatas);
    }

    [Test]
    public void When_ConfiguratorAppliesComponent_ComponentExists()
    {
        _configurator.AddComponentDataFunction(rand => new Translation { Value = 3f });
        Entity entity = EntityManager.CreateEntity();
        EntityManager.AddComponent<Translation>(entity);

        _configurator.ApplyComponentData(entity);

        IsTrue(EntityManager.HasComponent<Translation>(entity));
    }

    [Test]
    public void When_ConfiguratorAppliesComponentToMany_ComponentExistsOnAll()
    {
        _configurator.AddComponentDataFunction(rand => new Translation { Value = 3f });
        var entities = new NativeArray<Entity>(10, Allocator.Temp);
        EntityArchetype archetype = EntityManager.CreateArchetype(typeof(Translation));
        EntityManager.CreateEntity(archetype, entities);

        _configurator.ApplyComponentData(entities.ToArray());

        foreach (Entity entity in entities)
            IsTrue(EntityManager.HasComponent<Translation>(entity));
    }

    [Test]
    public void When_ConfiguratorAppliesComponent_ComponentDataValueMatches()
    {
        float3 expectedValue = 3f;
        _configurator.AddComponentDataFunction(rand => new Translation { Value = expectedValue });
        Entity entity = EntityManager.CreateEntity();
        EntityManager.AddComponent<Translation>(entity);

        _configurator.ApplyComponentData(entity);

        float3 actualValue = EntityManager.GetComponentData<Translation>(entity).Value;
        AreEqual(expectedValue, actualValue);
    }

    [Test]
    public void When_ConfiguratorAppliesComponentToMany_ComponentDataValueMatchesOnAll()
    {
        float3 expectedValue = 3f;
        _configurator.AddComponentDataFunction(rand => new Translation { Value = expectedValue });
        var entities = new NativeArray<Entity>(10, Allocator.Temp);
        EntityArchetype archetype = EntityManager.CreateArchetype(typeof(Translation));
        EntityManager.CreateEntity(archetype, entities);

        _configurator.ApplyComponentData(entities.ToArray());

        foreach (Entity entity in entities)
        {
            float3 actualValue = EntityManager.GetComponentData<Translation>(entity).Value;
            AreEqual(expectedValue, actualValue);
        }
    }
}
}
