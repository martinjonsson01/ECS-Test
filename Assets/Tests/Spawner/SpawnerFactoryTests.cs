using System;
using System.Collections.Generic;

using Game.Adapter;
using Game.Spawner;

using NSubstitute;

using NUnit.Framework;

using Unity.Entities;

using static NUnit.Framework.Assert;

namespace Tests.Spawner
{
[Category("Core Tests")]
public class SpawnerFactoryTests
{
    private SpawnerFactory _factory;

    [SetUp]
    public void SetUp()
    {
        _factory = new SpawnerFactory();
    }

    [Test]
    public void When_CreateBaseSpawnerIsCalled_BaseSpawnerIsCreated()
    {
        ISpawner spawner = _factory.CreateBaseSpawner(
            Substitute.For<IEntityManager>(),
            Substitute.For<IEntityArchetype>(),
            Substitute.For<IRandom>(),
            1,
            new List<Func<IRandom, IComponentData>>(),
            "Test entity"
        );

        IsNotNull(spawner);
        IsInstanceOf<BaseSpawner>(spawner);
    }
}
}
