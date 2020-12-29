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
public class ChanceSpawnerTests
{
    private const int Amount = 1;
    private ChanceSpawner _chanceSpawner;

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
        var baseSpawner = new BaseSpawner(
            creator,
            configurator,
            Amount
        );
        var random = Substitute.For<IRandom>();
        random.NextFloat().Returns(0.5f);
        _chanceSpawner = new ChanceSpawner(baseSpawner, random, 1f);
    }

    [Test]
    public void When_ChanceIsZero_NothingIsSpawned()
    {
        _chanceSpawner.Chance = 0f;

        using NativeArray<Entity> spawned = _chanceSpawner.Spawn();

        AreEqual(0, spawned.Length);
    }

    [Test]
    public void When_ChanceIsOne_EntitiesAreAlwaysSpawned()
    {
        using NativeArray<Entity> spawned = _chanceSpawner.Spawn();

        AreEqual(Amount, spawned.Length);
    }
}
}
