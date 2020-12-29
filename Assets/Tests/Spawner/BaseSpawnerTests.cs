using System;
using System.Collections.Generic;

using Game.Adapter;
using Game.Enemy;
using Game.Spawner;

using NSubstitute;

using NUnit.Framework;

using Unity.Collections;
using Unity.Entities;

using static NUnit.Framework.Assert;

namespace Tests.Spawner
{
[Category("Core Tests")]
public class BaseSpawnerTests
{
    private BaseSpawner _baseSpawner;

    [SetUp]
    public void SetUp()
    {
        var creator = new EntityCreator(
            Substitute.For<IEntityManager>(),
            Substitute.For<IEntityArchetype>(),
            "Test entity"
        );
        var configurator = new EntityComponentConfigurator(
            Substitute.For<IEntityManager>(),
            Substitute.For<IRandom>(),
            new List<Func<IRandom, IComponentData>>()
        );
        var random = Substitute.For<IRandom>();
        random.NextFloat().Returns(0.5f);
        _baseSpawner = new BaseSpawner(
            creator,
            configurator,
            1
        );
    }

    [Test]
    public void When_SpawnAmountIsZero_NothingIsSpawned()
    {
        _baseSpawner.Amount = 0;

        using NativeArray<Entity> spawned = _baseSpawner.Spawn();

        AreEqual(0, spawned.Length);
    }

    [Test]
    public void When_SpawnAmountIsOne_OneEntityIsSpawned()
    {
        const int amount = 1;
        _baseSpawner.Amount = amount;

        using NativeArray<Entity> spawned = _baseSpawner.Spawn();

        AreEqual(amount, spawned.Length);
    }

    [Test]
    public void When_SpawnAmountIsHundred_HundredEntityAreSpawned()
    {
        const int amount = 100;
        _baseSpawner.Amount = amount;

        using NativeArray<Entity> spawned = _baseSpawner.Spawn();

        AreEqual(amount, spawned.Length);
    }
}
}
