using Game.Adapter;
using Game.Enemy;

using NSubstitute;

using NUnit.Framework;

using Unity.Collections;
using Unity.Entities;

using static NUnit.Framework.Assert;

namespace Tests.Spawner
{
[Category("Core Tests")]
public class EntityCreatorTests
{
    private EntityCreator _entityCreator;

    private IEntityArchetype _archetype;
    private IEntityManager _entityManager;

    [SetUp]
    public void SetUp()
    {
        _archetype = Substitute.For<IEntityArchetype>();
        _entityManager = Substitute.For<IEntityManager>();
        _entityCreator = new EntityCreator(
            _entityManager,
            _archetype,
            "Test entity"
        );
    }

    [Test]
    public void When_CreateOneEntity_OneEntityIsCreated()
    {
        NativeArray<Entity> spawned = _entityCreator.CreateEntities(1);

        AreEqual(1, spawned.Length);
    }

    [Test]
    public void When_CreateMultipleEntities_MultipleEntitiesAreCreated()
    {
        NativeArray<Entity> spawned = _entityCreator.CreateEntities(10);

        AreEqual(10, spawned.Length);
    }
}
}
