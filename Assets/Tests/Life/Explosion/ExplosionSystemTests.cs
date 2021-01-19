using System.Collections;

using BovineLabs.Event.Containers;

using Game.Life;
using Game.Life.Explosion;

using NUnit.Framework;

using Unity.Entities;
using Unity.Transforms;

using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Life.Explosion
{
public class ExplosionSystemTests : EventSystemTestBase<ExplosionSystem, DeathEvent>
{
    private Entity _entity;
    private ParticleSystem _particleSystem;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _entity = m_Manager.CreateEntity(typeof(Translation));
        var particleSystemGameObject = new GameObject(ExplosionSystem.ParticleSystemName);
        _particleSystem = particleSystemGameObject.AddComponent<ParticleSystem>();

    }

    [UnityTest, Ignore("ParticleSystem.particleCount is not returning correct value", Until = "2021-01-19")]
    public IEnumerator When_EntityDies_WithExplodesOnDeath_ExplosionParticlesAreCreated()
    {
        m_Manager.AddComponent<ExplodesOnDeath>(_entity);
        var deathEvent = new DeathEvent
        {
            Entity = _entity
        };
        NativeEventStream.ThreadWriter writer = CreateEventWriter();
        writer.Write(deathEvent);

        World.GetExistingSystem<ExplosionSystem>().Update();
        yield return null;

        Assert.That(_particleSystem.particleCount, Is.GreaterThan(0));
    }

    [Test]
    public void When_EntityDies_WithoutExplodesOnDeath_ExplosionParticlesAreNotCreated()
    {
        var deathEvent = new DeathEvent
        {
            Entity = _entity
        };
        NativeEventStream.ThreadWriter writer = CreateEventWriter();
        writer.Write(deathEvent);

        World.Update();

        Assert.That(_particleSystem.particleCount, Is.EqualTo(0));
    }
}
}
