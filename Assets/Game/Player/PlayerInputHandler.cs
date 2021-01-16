using Game.Movement;
using Game.Utils;

using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

using UnityEngine;
using UnityEngine.InputSystem;

using IPlayerActions = Game.Player.Controls.IPlayerActions;
using Ray = UnityEngine.Ray;
using RaycastHit = Unity.Physics.RaycastHit;

namespace Game.Player
{
public class PlayerInputHandler : MonoBehaviour, IPlayerActions
{
    public InputActionAsset Asset { get; private set; }
    public Camera playerCamera;

    [SerializeField] private float turningRate = 1f;

    private const float RayDistance = 1000f;

    private Controls _controls;

    private EntityManager _entityManager;
    private BuildPhysicsWorld _buildPhysicsWorld;
    private EntityQuery _playerQuery;

    public void OnEnable()
    {
        SetUpControls();

        _buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _playerQuery = _entityManager.CreateEntityQuery(
            typeof(PlayerTag),
            typeof(Translation));
    }

    public void OnDisable()
    {
        _controls.Player.Disable();
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Vector2 mousePos = Pointer.current.position.ReadValue();

        Entity firstHitEntity = GetEntityAtPointer(mousePos);
        if (firstHitEntity == default) return;

        Entity player = GetPlayer();
        Quaternion directionToTarget = CalculateDirectionToTarget(firstHitEntity, player);
        var desiredRotation = new DesiredRotation
        {
            Value = directionToTarget,
            TurningRate = turningRate
        };
        _entityManager.AddComponentData(player, desiredRotation);
    }

    private Entity GetPlayer()
    {
        using NativeArray<Entity> players = _playerQuery.ToEntityArray(Allocator.Temp);
        return players.Length > 0 ? players[0] : default;
    }

    private Quaternion CalculateDirectionToTarget(Entity target, Entity player)
    {
        var playerTransform = _entityManager.GetComponentData<LocalToWorld>(player);
        Vector3 targetPosition = _entityManager.GetComponentData<Translation>(target).Value;
        Vector3 toTarget = targetPosition - (Vector3) playerTransform.Position;
        Quaternion lookingAtTarget = Quaternion.LookRotation(toTarget, playerTransform.Up);
        return lookingAtTarget;
    }

    private Entity GetEntityAtPointer(Vector2 vector2)
    {
        var raycastHits = new NativeList<RaycastHit>(Allocator.Temp);
        Ray cameraRay = playerCamera.ScreenPointToRay(vector2);
        RaycastInput raycastInput = CreateRaycast(cameraRay);
        CollisionWorld collisionWorld = _buildPhysicsWorld.PhysicsWorld.CollisionWorld;
        if (!collisionWorld.CastRay(raycastInput, ref raycastHits)) return default;
        Entity firstHitEntity = raycastHits[0].Entity;
        raycastHits.Dispose();
        return firstHitEntity;
    }

    private static RaycastInput CreateRaycast(Ray fromRay)
    {
        var raycastInput = new RaycastInput
        {
            Start = fromRay.origin,
            End = fromRay.origin + fromRay.direction * RayDistance,
            Filter = RaycastUtil.LayerToFilter(RaycastUtil.PlayerMoveInputRayLayer)
        };
        return raycastInput;
    }

    private void SetUpControls()
    {
        if (_controls == null)
        {
            _controls = new Controls();
            _controls.Player.SetCallbacks(this);
        }
        Asset = _controls.asset;
        _controls.Player.Enable();
    }
}
}
