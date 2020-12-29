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
public class IntervalSpawnerTests
{
    private const int SpawnAmount = 10;
    private IntervalSpawner _intervalSpawner;

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
        _intervalSpawner = new IntervalSpawner(spawner, 1f);
    }

    [Test]
    public void When_IntervalNotElapsed_NothingIsSpawned()
    {
        _intervalSpawner.Interval = 1f;

        const float currentTime = float.MinValue;
        using NativeArray<Entity> spawned = _intervalSpawner.Spawn(currentTime);

        AreEqual(0, spawned.Length);
    }

    [Test]
    public void When_IntervalHasExactlyElapsed_AmountIsSpawned()
    {
        const float interval = 1f;
        _intervalSpawner.Interval = interval;

        using NativeArray<Entity> spawned = _intervalSpawner.Spawn(interval);

        AreEqual(SpawnAmount, spawned.Length);
    }

    [Test]
    public void When_IntervalHasElapsedLongTimeAgo_AmountIsSpawned()
    {
        const float interval = 1f;
        _intervalSpawner.Interval = interval;

        const float currentTime = interval * 10f;
        using NativeArray<Entity> spawned = _intervalSpawner.Spawn(currentTime);

        AreEqual(SpawnAmount, spawned.Length);
    }

    [Test]
    public void When_CurrentTimeIsZeroAndIntervalIsOne_IntervalNotElapsed()
    {
        _intervalSpawner.Interval = 1f;

        IsTrue(_intervalSpawner.IntervalNotElapsed(0));
    }

    [Test]
    public void When_CurrentTimeIsInterval_IntervalElapsed()
    {
        const float interval = 1f;
        _intervalSpawner.Interval = interval;

        IsFalse(_intervalSpawner.IntervalNotElapsed(interval));
    }

    [Test]
    public void When_CurrentTimeIsOverInterval_IntervalElapsed()
    {
        const float interval = 1f;
        _intervalSpawner.Interval = interval;

        IsFalse(_intervalSpawner.IntervalNotElapsed(interval * 10f));
    }
}
}
