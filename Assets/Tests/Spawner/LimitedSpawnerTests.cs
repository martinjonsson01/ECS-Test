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
public class LimitedSpawnerTests
{

    private LimitedSpawner _limitedSpawner;
    private const int SpawnAmount = 1;

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
        var spawner = new BaseSpawner(
            creator,
            configurator,
            SpawnAmount
        );
        _limitedSpawner = new LimitedSpawner(spawner, 0);
    }

    [Test]
    public void When_SpawnLimitIsZero_NothingIsSpawned()
    {
        _limitedSpawner.SpawnLimit = 0;

        using NativeArray<Entity> spawned = _limitedSpawner.Spawn();

        AreEqual(0, spawned.Length);
    }

    [Test]
    public void When_SpawnLimitIsSetToNegative_Throws_ArgumentException()
    {
        Throws<ArgumentException>(() =>
        {
            _limitedSpawner.SpawnLimit = -1;
        });
    }

    [Test]
    public void When_SpawnLimitIsSet_MoreThanLimitAreNotSpawned()
    {
        _limitedSpawner.SpawnLimit = 2;

        using NativeArray<Entity> spawned = _limitedSpawner.Spawn();
        using NativeArray<Entity> spawned2 = _limitedSpawner.Spawn();
        using NativeArray<Entity> spawned3 = _limitedSpawner.Spawn();

        AreEqual(SpawnAmount, spawned.Length);
        AreEqual(SpawnAmount, spawned2.Length);
        AreEqual(0, spawned3.Length);
    }
}
}
