using Game.Movement;
using Game.Player;
using Game.Utils;

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
    private World _world;
    private EntityManager _manager;

    private Entity _player;
    private Camera _playerCamera;

    private PlayerInputHandler _handler;
    private Mouse _mouse;
    private Vector2 _moveTargetScreenPos;

    [SetUp]
    public override void Setup()
    {
        base.Setup();

        _world = World.DefaultGameObjectInjectionWorld = new World("Test World");
        _manager = _world.EntityManager;
        _player = _manager.CreateEntity(
            typeof(PlayerTag),
            typeof(LocalToWorld),
            typeof(Translation),
            typeof(Rotation));
        _playerCamera = SetUpPlayerCamera(_player, _manager);
        CreateMoveTargetEntity(new float3(10f));

        _handler = _playerCamera.gameObject.AddComponent<PlayerInputHandler>();
        _handler.playerCamera = _playerCamera;
        _handler.OnEnable();
        _mouse = InputSystem.AddDevice<Mouse>();
    }

    [TearDown]
    public override void TearDown()
    {
        base.TearDown();

        _handler.OnDisable();
    }

    private void CreateAndSetUpPhysicsSystems()
    {
        _world.GetOrCreateSystem<BuildPhysicsWorld>().Update();
        _world.GetOrCreateSystem<ExportPhysicsWorld>();
        _world.GetOrCreateSystem<StepPhysicsWorld>();
    }

    private void CreateMoveTargetEntity(float3 worldPos)
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
            RaycastUtil.LayerToFilter(RaycastUtil.PlayerMoveInputRayLayer),
            new Material
            {
                CollisionResponse = CollisionResponsePolicy.None
            });
        _manager.SetComponentData(moveTargetEntity, new PhysicsCollider { Value = collider });
        _moveTargetScreenPos = _playerCamera.WorldToScreenPoint(worldPos);
        _manager.SetComponentData(moveTargetEntity, new Translation { Value = worldPos });
    }

    private static Camera SetUpPlayerCamera(Entity player, EntityManager manager)
    {
        var cameraGameObject = new GameObject("Test Player Camera");
        cameraGameObject.transform.position = new Vector3(0, 0, -10f);
        cameraGameObject.transform.LookAt(manager.GetComponentData<Translation>(player).Value);
        return cameraGameObject.AddComponent<Camera>();
    }

    [Test]
    public void When_MoveInputIsGivenToNowhere_PlayerDoesNotWantToRotate()
    {
        InputActionAsset input = _handler.Asset;
        Set(_mouse, "position", new Vector2(0, 0));

        Trigger(input["Player/Move"]);

        Assert.IsFalse(_manager.HasComponent<DesiredRotation>(_player));
    }

    [Test, Ignore("Fails randomly, requiring tests to be re-run for it to pass.")]
    public void When_MoveInputIsGivenToTarget_PlayerWantsToRotateToTarget()
    {
        InputActionAsset input = _handler.Asset;
        Set(_mouse, "position", _moveTargetScreenPos);
        CreateAndSetUpPhysicsSystems();

        Trigger(input["Player/Move"]);

        var rotationToTarget = new float4(-0.3250576f, 0.3250576f, 0f, 0.8880739f);
        float4 desiredPlayerRotation = _manager.GetComponentData<DesiredRotation>(_player).Value.value;
        Assert.That(rotationToTarget.x, Is.EqualTo(desiredPlayerRotation.x).Within(0.0000001f));
        Assert.That(rotationToTarget.y, Is.EqualTo(desiredPlayerRotation.y).Within(0.0000001f));
        Assert.That(rotationToTarget.z, Is.EqualTo(desiredPlayerRotation.z).Within(0.0000001f));
        Assert.That(rotationToTarget.w, Is.EqualTo(desiredPlayerRotation.w).Within(0.0000001f));
    }
}
}
