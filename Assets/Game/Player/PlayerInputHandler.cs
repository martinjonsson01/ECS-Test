using System;

using BovineLabs.Event.Containers;
using BovineLabs.Event.Systems;

using Game.Adapter;

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

using IPlayerActions = Game.Player.Controls.IPlayerActions;
using Ray = UnityEngine.Ray;
using RaycastHit = Unity.Physics.RaycastHit;

namespace Game.Player
{
[RequireComponent(typeof(SmoothRotator))]
public class PlayerInputHandler : MonoBehaviour, IPlayerActions
{
    public InputActionAsset Asset { get; private set; }
    public Camera PlayerCamera;

    private const float RayDistance = 1000f;
    private const int MoveRayLayer = 2;

    private Controls _controls;
    private SmoothRotator _rotator;

    private EntityManager _entityManager;
    private BuildPhysicsWorld _buildPhysicsWorld;

    public void OnEnable()
    {
        SetUpControls();
        _rotator = GetComponent<SmoothRotator>();

        _buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
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

        _rotator.desiredRotation = CalculateDirectionToTarget(firstHitEntity);
    }

    private Quaternion CalculateDirectionToTarget(Entity target)
    {
        Vector3 targetPosition = _entityManager.GetComponentData<Translation>(target).Value;
        Vector3 toTarget = targetPosition - transform.position;
        Quaternion lookingAtTarget = Quaternion.LookRotation(toTarget, transform.up);
        return lookingAtTarget;
    }

    private Entity GetEntityAtPointer(Vector2 vector2)
    {
        var raycastHits = new NativeList<RaycastHit>(Allocator.Temp);
        Ray cameraRay = PlayerCamera.ScreenPointToRay(vector2);
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
            Filter = LayerToFilter(MoveRayLayer)
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

    private static CollisionFilter LayerToFilter(int layer)
    {
        if (layer == -1) return CollisionFilter.Zero;

        var mask = new BitField32();
        mask.SetBits(layer, true);

        var filter = new CollisionFilter()
        {
            BelongsTo = mask.Value,
            CollidesWith = mask.Value
        };
        return filter;
    }
}
}
