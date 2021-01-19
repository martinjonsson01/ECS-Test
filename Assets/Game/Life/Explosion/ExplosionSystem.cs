using BovineLabs.Event.Systems;

using Tests.Graphics;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;

using static Unity.Mathematics.math;

namespace Game.Life.Explosion
{
[UpdateInGroup(typeof(SimulationSystemGroup))]
public class ExplosionSystem : ConsumeSingleEventSystemBase<DeathEvent>
{
    public const string ParticleSystemName = "Explosion Particle System";
    private const float DefaultEntitySize = 3f;
    private const float MinimumDuration = 1.5f;
    private const float MinimumSize = 1f;
    private const float ScaleFactor = 1 / 5f;

    private ParticleSystem _particleSystem;
    private ComponentDataFromEntity<ExplodesOnDeath> _entityExplodesOnDeath;
    private ComponentDataFromEntity<Translation> _entityTranslations;
    private ComponentDataFromEntity<Size> _entitySizes;

    protected override void BeforeEvent()
    {
        if (_particleSystem is null) FindParticleSystem();
        GetComponentDataFromEntity();
    }

    private void GetComponentDataFromEntity()
    {
        _entityExplodesOnDeath = GetComponentDataFromEntity<ExplodesOnDeath>(true);
        _entityTranslations = GetComponentDataFromEntity<Translation>(true);
        _entitySizes = GetComponentDataFromEntity<Size>(true);
    }

    private void FindParticleSystem()
    {
        GameObject explosionParticleSystemObject = GameObject.Find(ParticleSystemName);
        _particleSystem = explosionParticleSystemObject != null
            ? explosionParticleSystemObject.GetComponent<ParticleSystem>()
            : null;
    }

    protected override void OnEvent(DeathEvent death)
    {
        if (death.Entity == Entity.Null) return;
        if (!_entityTranslations.HasComponent(death.Entity)) return;

        float3 position = _entityTranslations[death.Entity].Value;
        float size = _entitySizes.HasComponent(death.Entity) ? _entitySizes[death.Entity].Value : DefaultEntitySize;

        ExplodeEntity(position, size);
    }

    private void ExplodeEntity(float3 position, float size)
    {
        float scaledExplosionSize = size * ScaleFactor;
        var emitParams = new ParticleSystem.EmitParams
        {
            position = position,
            startSize = max(MinimumSize, scaledExplosionSize),
            startLifetime = max(MinimumDuration, scaledExplosionSize),
            applyShapeToPosition = true,
        };
        _particleSystem.Emit(emitParams, 100);
    }
}
}
