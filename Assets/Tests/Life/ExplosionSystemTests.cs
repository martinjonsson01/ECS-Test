using BovineLabs.Event.Containers;
using BovineLabs.Event.Systems;

using Game.Life.Explosion;

using NUnit.Framework;

using Unity.Mathematics;

using UnityEngine;

namespace Tests.Life
{
public class ExplosionSystemTests : SystemTestBase<ExplosionSystem>
{
    private GameObject _particleSystemGameObject;

    public override void Setup()
    {
        base.Setup();
        _particleSystemGameObject = new GameObject("Test Explosion Particle System")
        {
            tag = "ExplosionParticleSystem"
        };
        _particleSystemGameObject.AddComponent<ParticleSystem>();

        var eventSystem = World.GetExistingSystem<EventSystem>();
        NativeEventStream.ThreadWriter writer = eventSystem.CreateEventWriter<ExplosionEvent>();
        var explosion = new ExplosionEvent
        {
            Position = float3.zero,
            Size = 1f
        };
        writer.Write(explosion);
    }

    [Test]
    public void When_ThereIsNoParticleSystem_DoesNotThrow()
    {
        Object.DestroyImmediate(_particleSystemGameObject);

        Assert.DoesNotThrow(World.Update);
    }

    [Test]
    public void When_ExplosionSystemRuns_DoesNotThrow()
    {
        Assert.DoesNotThrow(World.Update);
    }
}
}
