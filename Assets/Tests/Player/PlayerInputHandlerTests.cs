using Game.Player;

using NUnit.Framework;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

using UnityEngine;
using UnityEngine.InputSystem;

using Collider = Unity.Physics.Collider;
using Material = Unity.Physics.Material;
using SphereCollider = Unity.Physics.SphereCollider;

// ReSharper disable Unity.InefficientPropertyAccess

// ReSharper disable UnusedMember.Global

namespace Tests.Player
{
public class PlayerInputHandlerTests : InputTestFixture
{
    private const int MoveTargetCollisionMask = 4;

    private World _world;
    private EntityManager _manager;
    private Entity _moveTargetEntity;

    private GameObject _playerShip;
    private SmoothRotator _playerRotator;
    private Camera _playerCamera;

    private PlayerInputHandler _handler;
    private Keyboard _keyboard;
    private Mouse _mouse;
    private Vector2 _moveTargetScreenPos;

    [SetUp]
    public override void Setup()
    {
        base.Setup();

        _playerShip = new GameObject("Test Player Ship") { tag = "Player" };
        _playerRotator = _playerShip.AddComponent<SmoothRotator>();
        _playerCamera = SetUpPlayerCamera(_playerShip);

        _world = World.DefaultGameObjectInjectionWorld = new World("Test World");
        _manager = _world.EntityManager;
        _moveTargetEntity = CreateMoveTargetEntity(new float3(10f));

        _handler = _playerShip.AddComponent<PlayerInputHandler>();
        _handler.PlayerCamera = _playerCamera;
        _handler.OnEnable();
        _keyboard = InputSystem.AddDevice<Keyboard>();
        _mouse = InputSystem.AddDevice<Mouse>();
    }

    private void CreateAndSetUpPhysicsSystems()
    {
        _world.GetOrCreateSystem<BuildPhysicsWorld>().Update();
        _world.GetOrCreateSystem<ExportPhysicsWorld>();
        _world.GetOrCreateSystem<StepPhysicsWorld>();
    }

    private Entity CreateMoveTargetEntity(float3 worldPos)
    {
        Entity moveTargetEntity = _manager.CreateEntity(typeof(Translation),
            typeof(Rotation),
            typeof(PhysicsVelocity),
            typeof(PhysicsCollider));

        BlobAssetReference<Collider> collider = SphereCollider.Create(
            new SphereGeometry
            {
                Center = float3.zero,
                Radius = 1f
            },
            new CollisionFilter
            {
                BelongsTo = MoveTargetCollisionMask,
                CollidesWith = MoveTargetCollisionMask
            },
            new Material
            {
                CollisionResponse = CollisionResponsePolicy.None
            });
        _manager.SetComponentData(moveTargetEntity, new PhysicsCollider { Value = collider });
        _moveTargetScreenPos = _playerCamera.WorldToScreenPoint(worldPos);
        _manager.SetComponentData(moveTargetEntity, new Translation { Value = worldPos });
        return moveTargetEntity;
    }

    private static Camera SetUpPlayerCamera(GameObject player)
    {
        var cameraGameObject = new GameObject("Test Player Camera");
        cameraGameObject.transform.position = new Vector3(0, 0, -10f);
        cameraGameObject.transform.LookAt(player.transform);
        return cameraGameObject.AddComponent<Camera>();
    }

    [Test]
    public void When_MoveInputIsGivenToNowhere_PlayerDoesNotWantToRotate()
    {
        InputActionAsset input = _handler.Asset;
        Quaternion playerRotation = _playerShip.transform.rotation;
        Set(_mouse, "position", new Vector2(0, 0));

        Trigger(input["Player/Move"]);

        Quaternion desiredPlayerRotation = _playerRotator.desiredRotation;
        Assert.AreEqual(playerRotation, desiredPlayerRotation);
    }

    [Test]
    public void When_MoveInputIsGivenToTarget_PlayerWantsToRotateToTarget()
    {
        InputActionAsset input = _handler.Asset;
        Set(_mouse, "position", _moveTargetScreenPos);
        CreateAndSetUpPhysicsSystems();

        Trigger(input["Player/Move"]);

        var rotationToTarget = new Quaternion(-0.279848158f, 0.364705205f, 0.1159169f, 0.880476296f);
        Quaternion desiredPlayerRotation = _playerRotator.desiredRotation;
        Assert.AreEqual(rotationToTarget, desiredPlayerRotation);
    }
}
}
