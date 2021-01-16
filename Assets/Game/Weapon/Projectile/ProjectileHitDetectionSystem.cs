using BovineLabs.Event.Containers;
using BovineLabs.Event.Systems;

using Game.Utils;

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

using RaycastHit = Unity.Physics.RaycastHit;
using Velocity = Game.Movement.Velocity;

namespace Game.Weapon.Projectile
{
[UpdateInGroup(typeof(SimulationSystemGroup))]
public class ProjectileHitDetectionSystem : SystemBase
{
    private const float Delta = 0.00001f;

    private EventSystem _eventSystem;
    private BuildPhysicsWorld _physicsWorldSystem;
    private EntityQuery _query;

    protected override void OnCreate()
    {
        _eventSystem = World.GetOrCreateSystem<EventSystem>();
        _physicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
    }

    protected override void OnUpdate()
    {
        NativeEventStream.IndexWriter eventWriter =
            _eventSystem.CreateEventWriter<ProjectileHitEvent>(_query.CalculateEntityCount());

        CollisionWorld collisionWorld = _physicsWorldSystem.PhysicsWorld.CollisionWorld;

        float deltaTime = Time.DeltaTime;

        JobHandle handle =
            Entities
                .WithName(nameof(ProjectileHitDetectionSystem))
                .WithReadOnly(collisionWorld)
                .WithAll<ProjectileTag>()
                .ForEach((
                    Entity entity,
                    int entityInQueryIndex,
                    in Translation translation,
                    in Velocity velocity,
                    in Damage damage) =>
                {
                    float3 nextFramePosition = translation.Value + velocity.Value * deltaTime;

                    var raycastHits = new NativeList<RaycastHit>(Allocator.Temp);
                    if (!CastRayHits(translation, nextFramePosition, collisionWorld, ref raycastHits))
                        return;

                    RaycastHit closestHit = FindClosestHit(raycastHits);

                    CreateAndWriteHitEvent(entityInQueryIndex, entity, closestHit, damage, eventWriter);
                })
                .WithStoreEntityQueryInField(ref _query)
                .WithBurst()
                .ScheduleParallel(Dependency);

        _eventSystem.AddJobHandleForProducer<ProjectileHitEvent>(handle);
        Dependency = handle;
    }

    private static bool CastRayHits(
        Translation translation,
        float3 nextFramePosition,
        CollisionWorld collisionWorld,
        ref NativeList<RaycastHit> raycastHits)
    {
        var raycastInput = new RaycastInput
        {
            Start = translation.Value,
            End = nextFramePosition,
            Filter = RaycastUtil.LayerToFilter(
                RaycastUtil.ProjectileLayer,
                RaycastUtil.ProjectileCollidableLayer)
        };
        return collisionWorld.CastRay(raycastInput, ref raycastHits);
    }

    private static RaycastHit FindClosestHit(NativeList<RaycastHit> raycastHits)
    {
        RaycastHit closestHit = raycastHits[0];
        var closestHitFraction = 1000f;
        foreach (RaycastHit raycastHit in raycastHits)
        {
            if (raycastHit.Fraction - closestHitFraction > Delta) continue;

            closestHit = raycastHit;
            closestHitFraction = raycastHit.Fraction;
        }
        raycastHits.Dispose();
        return closestHit;
    }

    private static void CreateAndWriteHitEvent(
        int entityInQueryIndex,
        Entity projectileEntity,
        RaycastHit closestHit,
        Damage damage,
        NativeEventStream.IndexWriter eventWriter)
    {
        var hitEvent = new ProjectileHitEvent
        {
            ProjectileEntity = projectileEntity,
            HitEntity = closestHit.Entity,
            Damage = damage.Value
        };
        eventWriter.BeginForEachIndex(entityInQueryIndex);
        eventWriter.Write(hitEvent);
        eventWriter.EndForEachIndex();
    }
}
}
