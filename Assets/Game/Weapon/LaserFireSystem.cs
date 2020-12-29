using Game.Graphics;
using Game.Life;
using Game.Weapon.Projectile;

using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

using Velocity = Game.Movement.Velocity;

namespace Game.Weapon
{
[UpdateInGroup(typeof(SimulationSystemGroup))]
public class LaserFireSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimEcbSystem;
    private EntityArchetype _laserBoltArchetype;

    protected override void OnCreate()
    {
        _endSimEcbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        _laserBoltArchetype = EntityManager.CreateArchetype(
            typeof(LaserBoltTag),
            typeof(ProjectileTag),
            typeof(NeedsVisualTag),
            /**/
            typeof(Translation),
            typeof(Rotation),
            /**/
            typeof(Velocity),
            typeof(Damage),
            typeof(Lifetime)
        );
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer.ParallelWriter ecb = _endSimEcbSystem.CreateCommandBuffer().AsParallelWriter();

        EntityArchetype laserBoltArchetype = _laserBoltArchetype;

        const float boltSpeed = 100f;
        const float boltLifetime = 10f;

        JobHandle targetHandle =
            Entities
                .WithName(nameof(LaserFireSystem))
                .ForEach((
                    Entity entity,
                    int entityInQueryIndex,
                    in Translation translation,
                    in FireLaser fireLaser) =>
                {
                    FireLaserBolt(
                        ecb,
                        entityInQueryIndex,
                        laserBoltArchetype,
                        fireLaser,
                        boltSpeed,
                        boltLifetime,
                        translation,
                        entity
                    );
                })
                .WithBurst()
                .ScheduleParallel(Dependency);

        _endSimEcbSystem.AddJobHandleForProducer(targetHandle);

        Dependency = targetHandle;
    }

    private static void FireLaserBolt(
        EntityCommandBuffer.ParallelWriter ecb,
        int entityInQueryIndex,
        EntityArchetype laserBoltArchetype,
        FireLaser fireLaser,
        float boltSpeed,
        float boltLifetime,
        Translation translation,
        Entity entity)
    {
        Entity laserBoltEntity = ecb.CreateEntity(entityInQueryIndex, laserBoltArchetype);
        AddComponents(ecb, entityInQueryIndex, fireLaser, translation, entity, laserBoltEntity, boltSpeed,
            boltLifetime);
    }

    private static void AddComponents(
        EntityCommandBuffer.ParallelWriter ecb,
        int entityInQueryIndex,
        FireLaser fireLaser,
        Translation translation,
        Entity entity,
        Entity laserBoltEntity,
        float boltSpeed,
        float boltLifetime)
    {
        AddVelocity(ecb, entityInQueryIndex, fireLaser, laserBoltEntity, boltSpeed);
        ecb.SetComponent(entityInQueryIndex, laserBoltEntity, new Damage { Value = fireLaser.Damage });
        ecb.SetComponent(entityInQueryIndex, laserBoltEntity, new Lifetime { Seconds = boltLifetime });
        AddRotation(ecb, entityInQueryIndex, fireLaser, laserBoltEntity);
        ecb.SetComponent(entityInQueryIndex, laserBoltEntity, translation);
        ecb.RemoveComponent<FireLaser>(entityInQueryIndex, entity);
    }

    private static void AddVelocity(
        EntityCommandBuffer.ParallelWriter ecb,
        int entityInQueryIndex,
        FireLaser fireLaser,
        Entity laserBoltEntity,
        float boltSpeed)
    {
        var boltVelocity = new Velocity
        {
            Value = math.normalizesafe(fireLaser.Direction) * boltSpeed
        };
        ecb.SetComponent(entityInQueryIndex, laserBoltEntity, boltVelocity);
    }

    private static void AddRotation(
        EntityCommandBuffer.ParallelWriter ecb,
        int entityInQueryIndex,
        FireLaser fireLaser,
        Entity laserBoltEntity)
    {
        var rotation = new Rotation
        {
            Value = quaternion.LookRotation(fireLaser.Direction, math.up())
        };
        ecb.SetComponent(entityInQueryIndex, laserBoltEntity, rotation);
    }
}
}
