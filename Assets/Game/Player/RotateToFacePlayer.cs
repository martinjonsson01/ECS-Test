using System;

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;

namespace Game.Player
{
public class RotateToFacePlayer : MonoBehaviour
{
    private EntityQuery _playerQuery;
    private EntityManager _entityManager;

    public void Awake()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _playerQuery = _entityManager.CreateEntityQuery(
            typeof(PlayerTag), typeof(Rotation));
    }

    public void Update()
    {
        using NativeArray<Entity> players = _playerQuery.ToEntityArray(Allocator.Temp);
        if (players.Length < 1) return;
        Entity player = players[0];

        Quaternion playerRotation = _entityManager.GetComponentData<Rotation>(player).Value;
        Quaternion inverseRotation = playerRotation * Quaternion.Euler(0, 180f, 0);

        transform.rotation = inverseRotation;
    }
}
}
