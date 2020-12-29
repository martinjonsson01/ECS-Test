using BovineLabs.Event.Jobs;
using BovineLabs.Event.Systems;

using Game.Life;

using Unity.Burst;
using Unity.Entities;

namespace Game.Weapon.Projectile
{
[UpdateInGroup(typeof(SimulationSystemGroup))]
public class ProjectileHitDamageSystem : SystemBase
{
    private EntityQuery _projectileTagGroup;
    private EndSimulationEntityCommandBufferSystem _bufferSystem;
    private EventSystem _eventSystem;

    protected override void OnCreate()
    {
        _eventSystem = World.GetOrCreateSystem<EventSystem>();
        _projectileTagGroup = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                typeof(ProjectileTag)
            }
        });
        _bufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    [BurstCompile]
    private struct ProjectileDamageCollisionJob : IJobEvent<ProjectileHitEvent>
    {
        public ComponentDataFromEntity<Health> HealthGroup;
        public EntityCommandBuffer Ecb;

        public void Execute(ProjectileHitEvent hitEvent)
        {
            if (!HealthGroup.HasComponent(hitEvent.HitEntity))
                return;

            Health healthComponent = HealthGroup[hitEvent.HitEntity];

            healthComponent.Value -= hitEvent.Damage;
            HealthGroup[hitEvent.HitEntity] = healthComponent;

            Ecb.DestroyEntity(hitEvent.ProjectileEntity);
        }
    }

    protected override void OnUpdate()
    {
        if (_projectileTagGroup.CalculateEntityCount() == 0) return;

        EntityCommandBuffer ecb = _bufferSystem.CreateCommandBuffer();

        var job = new ProjectileDamageCollisionJob
        {
            HealthGroup = GetComponentDataFromEntity<Health>(),
            Ecb = ecb
        };

        Dependency =
            job.Schedule<ProjectileDamageCollisionJob, ProjectileHitEvent>(_eventSystem, Dependency);

        _bufferSystem.AddJobHandleForProducer(Dependency);
    }
}
}
