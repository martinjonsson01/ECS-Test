using Game.Player;

using NUnit.Framework;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;
// ReSharper disable Unity.InefficientPropertyAccess

namespace Tests.Player
{
public class MoveToInFrontOfPlayerTests
{
    private EntityManager _manager;
    private Entity _player;
    private GameObject _gameObject;
    private MoveToInFrontOfPlayer _mover;

    [SetUp]
    public void SetUp()
    {
        World world = World.DefaultGameObjectInjectionWorld = new World("Test World");
        _manager = world.EntityManager;
        _player = _manager.CreateEntity(typeof(PlayerTag),
            typeof(Rotation),
            typeof(Translation));
        _gameObject = new GameObject("Test GameObject");
        _mover = _gameObject.AddComponent<MoveToInFrontOfPlayer>();
        _mover.Awake();
    }

    [Test]
    public void When_ThereIsNoPlayer_GameObjectIsNotMoved()
    {
        _manager.DestroyEntity(_player);
        Vector3 positionBefore = _gameObject.transform.position;

        _mover.Update();

        Vector3 positionAfter = _gameObject.transform.position;
        Assert.That(positionBefore, Is.EqualTo(positionAfter));
    }

    [Test]
    public void When_GameObjectIsNotInFrontOfPlayer_GameObjectIsMovedToInFrontOfPlayer()
    {
        quaternion rotation = quaternion.LookRotation(new float3(1f, 0, 0), math.up());
        _manager.SetComponentData(_player, new Rotation{Value=rotation});

        _mover.Update();

        Vector3 positionAfter = _gameObject.transform.position;
        var position10InFrontOfPlayer = new Vector3(10f, 0, 0);
        Assert.IsTrue(position10InFrontOfPlayer == positionAfter);
    }
}
}
