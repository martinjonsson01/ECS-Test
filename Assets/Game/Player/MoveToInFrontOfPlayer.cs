using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;

namespace Game.Player
{
public class MoveToInFrontOfPlayer : MonoBehaviour
{
    [SerializeField] private float distance = 10f;

    private EntityQuery _playerQuery;
    private EntityManager _entityManager;

    public void Awake()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _playerQuery = _entityManager.CreateEntityQuery(
            typeof(PlayerTag),
            typeof(Rotation),
            typeof(Translation));
    }

    public void Update()
    {
        using NativeArray<Entity> players = _playerQuery.ToEntityArray(Allocator.Temp);
        if (players.Length < 1) return;
        Entity player = players[0];

        quaternion playerRotation = _entityManager.GetComponentData<Rotation>(player).Value;
        float3 playerPosition = _entityManager.GetComponentData<Translation>(player).Value;
        float3 playerDirection = math.normalizesafe(math.mul(playerRotation, math.forward()));
        float3 inFrontOfPlayer = distance * playerDirection;

        transform.position = inFrontOfPlayer;
    }
}
}
