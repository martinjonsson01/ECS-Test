using BovineLabs.Event.Systems;

using Unity.Mathematics;

using UnityEngine;

namespace Game.Life.Explosion
{
public class ExplosionSystem : ConsumeSingleEventSystemBase<ExplosionEvent>
{
    private const float MinimumDuration = 1.5f;
    private const float MinimumSize = 1f;
    private const float ScaleFactor = 1 / 5f;
    private ParticleSystem _particleSystem;

    private void FindParticleSystem()
    {
        GameObject explosionParticleSystemObject = GameObject.Find("Explosion Particle System");
        _particleSystem = explosionParticleSystemObject != null
            ? explosionParticleSystemObject.GetComponent<ParticleSystem>()
            : null;
    }

    protected override void BeforeEvent()
    {
        if (_particleSystem is null) FindParticleSystem();
    }

    protected override void OnEvent(ExplosionEvent explosion)
    {
        float scaledExplosionSize = explosion.Size * ScaleFactor;
        var emitParams = new ParticleSystem.EmitParams
        {
            position = explosion.Position,
            startSize = math.max(MinimumSize, scaledExplosionSize),
            startLifetime = math.max(MinimumDuration, scaledExplosionSize),
            applyShapeToPosition = true,
        };
        _particleSystem.Emit(emitParams, 100);
    }
}
}
