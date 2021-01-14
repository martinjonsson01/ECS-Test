using Game.Player;

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
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

    protected override void OnCreate()
    {
        _endSimEcbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        _playerQuery = EntityManager.CreateEntityQuery(
            typeof(PlayerTag), typeof(Translation));
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

        float3 playerPos = EntityManager.GetComponentData<Translation>(_player).Value;

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
                    var followTarget = new Target
                    {
                        Entity = player,
                        Position = playerPos,
                        StopDistanceSq = 10f * 10f
                    };
                    ecb.AddComponent(entityInQueryIndex, entity, followTarget);
                })
                .WithBurst()
                .ScheduleParallel(Dependency);

        _endSimEcbSystem.AddJobHandleForProducer(targetHandle);

        Dependency = targetHandle;
    }
}
}
