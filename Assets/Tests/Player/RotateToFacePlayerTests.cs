using Game.Player;

using NUnit.Framework;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;
// ReSharper disable Unity.InefficientPropertyAccess

namespace Tests.Player
{
public class RotateToFacePlayerTests
{
    private EntityManager _manager;
    private Entity _player;
    private GameObject _gameObject;
    private RotateToFacePlayer _rotator;

    [SetUp]
    public void SetUp()
    {
        World world = World.DefaultGameObjectInjectionWorld = new World("Test World");
        _manager = world.EntityManager;
        _player = _manager.CreateEntity(typeof(PlayerTag),
            typeof(Rotation));
        _gameObject = new GameObject("Test GameObject");
        _rotator = _gameObject.AddComponent<RotateToFacePlayer>();
        _rotator.Awake();
    }

    [Test]
    public void When_ThereIsNoPlayer_GameObjectIsNotRotated()
    {
        _manager.DestroyEntity(_player);
        Quaternion rotationBefore = _gameObject.transform.rotation;

        _rotator.Update();

        Quaternion rotationAfter = _gameObject.transform.rotation;
        Assert.That(rotationBefore, Is.EqualTo(rotationAfter));
    }

    [Test]
    public void When_GameObjectIsNotFacingPlayer_GameObjectIsRotatedToFacePlayer()
    {
        const float degrees = 100f;
        quaternion rotation = quaternion.Euler(0f, degrees, 0f);
        _manager.SetComponentData(_player, new Rotation{Value = rotation});

        _rotator.Update();

        var inverseRotation = new Quaternion(0, 0.964966059f, 0, 0.262374848f);
        Quaternion gameObjectRotation = _gameObject.transform.rotation;
        Assert.That(gameObjectRotation, Is.EqualTo(inverseRotation));
    }
}
}
