using System.Collections;

using Game.Enemy;
using Game.Weapon;

using NUnit.Framework;

using Unity.Entities;

using UnityEngine;
using UnityEngine.TestTools;

using static NUnit.Framework.Assert;

namespace TestsPlayMode
{
[Category("PlayMode Tests")]
public class ConversionTests
{
    private World _world;
    private EntityManager _manager;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        _world = World.DefaultGameObjectInjectionWorld = new World("Test World");
        _manager = _world.EntityManager;
        yield return null;
    }

    [UnityTest]
    public IEnumerator When_HasEnemyFighterVisualSingletonAuthoring_GetsEnemyFighterVisual()
    {
        var gameObject = new GameObject();
        gameObject.AddComponent<EnemyFighterVisualSingletonAuthoring>();
        gameObject.AddComponent<ConvertToEntity>();


        GameObjectConversionSettings settings =
            GameObjectConversionSettings.FromWorld(_world, new BlobAssetStore());
        Entity entity =
            GameObjectConversionUtility.ConvertGameObjectHierarchy(gameObject, settings);

        yield return new WaitForFixedUpdate();

        IsTrue(_manager.HasComponent<EnemyFighterVisual>(entity));
    }

    [UnityTest]
    public IEnumerator When_HasLaserBoltVisualSingletonAuthoring_GetsLaserBoltVisual()
    {
        var gameObject = new GameObject();
        gameObject.AddComponent<LaserBoltVisualSingletonAuthoring>();
        gameObject.AddComponent<ConvertToEntity>();


        GameObjectConversionSettings settings =
            GameObjectConversionSettings.FromWorld(_world, new BlobAssetStore());
        Entity entity =
            GameObjectConversionUtility.ConvertGameObjectHierarchy(gameObject, settings);

        yield return new WaitForFixedUpdate();

        IsTrue(_manager.HasComponent<LaserBoltVisual>(entity));
    }
}
}
