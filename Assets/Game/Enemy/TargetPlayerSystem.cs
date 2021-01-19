using Game.Player;

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;

using UnityEngine;

namespace Game.Enemy
{
[UpdateInGroup(typeof(SimulationSystemGroup))]
public class TargetPlayerSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimEcbSystem;
    private Entity _player;
    private EntityQuery _playerQuery;
    private EntityQuery _targetsPlayerQuery;

    protected override void OnCreate()
    {
        _endSimEcbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        _playerQuery = GetEntityQuery(
            typeof(PlayerTag),
            typeof(Translation),
            typeof(Rotation),
            typeof(RenderMesh));
        _playerQuery.AddChangedVersionFilter(typeof(Translation));
        _playerQuery.AddChangedVersionFilter(typeof(Rotation));
    }

    protected override void OnUpdate()
    {
        if (_playerQuery.IsEmpty) return;

        if (_player == default)
        {
            using NativeArray<Entity> players = _playerQuery.ToEntityArray(Allocator.Temp);
            if (players.Length <= 0) return;
            _player = players[0];
        }

        NativeArray<float3> playerMeshPoints = CalculatePlayerMeshPoints(_player, _targetsPlayerQuery.CalculateEntityCount());

        EntityCommandBuffer.ParallelWriter ecb = _endSimEcbSystem.CreateCommandBuffer().AsParallelWriter();

        Entity player = _player;

        JobHandle targetHandle =
            Entities
                .WithName(nameof(TargetPlayerSystem))
                .WithAll<TargetsPlayerTag>()
                .ForEach((
                    Entity entity,
                    int entityInQueryIndex) =>
                {
                    float3 playerPos = playerMeshPoints[entityInQueryIndex % playerMeshPoints.Length];
                    var followTarget = new Target
                    {
                        Entity = player,
                        Position = playerPos,
                        StopDistance = 10f
                    };
                    ecb.AddComponent(entityInQueryIndex, entity, followTarget);
                })
                .WithStoreEntityQueryInField(ref _targetsPlayerQuery)
                .WithDisposeOnCompletion(playerMeshPoints)
                .WithBurst()
                .ScheduleParallel(Dependency);

        _endSimEcbSystem.AddJobHandleForProducer(targetHandle);

        Dependency = targetHandle;
    }

    private NativeArray<float3> CalculatePlayerMeshPoints(Entity player, int entityCount)
    {
        var renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(player);

        Vector3[] vertices = renderMesh.mesh.vertices;
        var playerMeshPoints =
            new NativeArray<float3>(vertices.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

        for (var i = 0; i < math.min(entityCount, vertices.Length); i++)
        {
            playerMeshPoints[i] = vertices[i];
        }

        return playerMeshPoints;
    }
}
}
