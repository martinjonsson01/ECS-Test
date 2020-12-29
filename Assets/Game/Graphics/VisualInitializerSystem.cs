using Game.Enemy;
using Game.Weapon;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace Game.Graphics
{
[UpdateInGroup(typeof(InitializationSystemGroup))]
public class VisualInitializerSystem : SystemBase
{
    private EntityQuery _enemyFighterVisualSingletonQuery;
    private EntityQuery _laserBoltVisualSingletonQuery;

    protected override void OnCreate()
    {
        _enemyFighterVisualSingletonQuery = GetEntityQuery(typeof(EnemyFighterVisual));
        _laserBoltVisualSingletonQuery = GetEntityQuery(typeof(LaserBoltVisual));
    }

    protected override void OnUpdate()
    {
        var enemyFighterVisual = EntityManager.GetComponentData<EnemyFighterVisual>(
            _enemyFighterVisualSingletonQuery.GetSingletonEntity());
        Entities
            .WithStructuralChanges()
            .WithAll<NeedsVisualTag, EnemyFighterTag>()
            .ForEach((Entity entity) =>
            {
                InitializeVisuals(entity, enemyFighterVisual);
            })
            .WithBurst()
            .Run();

        var laserBoltVisual = EntityManager.GetComponentData<LaserBoltVisual>(
            _laserBoltVisualSingletonQuery.GetSingletonEntity());
        Entities
            .WithStructuralChanges()
            .WithAll<NeedsVisualTag, LaserBoltTag>()
            .ForEach((Entity entity) =>
            {
                InitializeVisuals(entity, laserBoltVisual);
            })
            .WithBurst()
            .Run();
    }

    private void InitializeVisuals(Entity entity, IVisualData visuals)
    {
        EntityManager.AddSharedComponentData(entity,
            new RenderMesh { material = visuals.Material, mesh = visuals.Mesh });
        ApplyHybridRendererComponents(EntityManager, entity);
        EntityManager.RemoveComponent<NeedsVisualTag>(entity);
    }

    private static void ApplyHybridRendererComponents(
        EntityManager manager,
        Entity entity)
    {
        manager.AddComponentData(entity, new RenderBounds {  });
        manager.AddComponentData(entity, new PerInstanceCullingTag());
        manager.AddComponent(entity, ComponentType.ReadOnly<WorldToLocal_Tag>());
        manager.AddComponentData(entity, new BuiltinMaterialPropertyUnity_RenderingLayer
        {
            Value = new uint4(1, 0, 0, 0)
        });
        manager.AddComponentData(entity, new BuiltinMaterialPropertyUnity_WorldTransformParams
        {
            Value = new float4(0, 0, 0, 1)
        });
        manager.AddComponentData(entity, new BuiltinMaterialPropertyUnity_LightData
        {
            Value = new float4(0, 0, 1, 0)
        });
        manager.AddComponent<LocalToWorld>(entity);
        if (!manager.HasComponent<Translation>(entity)) manager.AddComponent<Translation>(entity);
        if (!manager.HasComponent<Rotation>(entity)) manager.AddComponent<Rotation>(entity);
    }
}
}
