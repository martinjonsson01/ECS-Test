using System;
using System.Collections.Generic;

using Game.Adapter;
using Game.Cooldown;
using Game.Enemy;
using Game.Graphics;
using Game.Life;
using Game.Life.Explosion;
using Game.Movement;
using Game.Weapon;

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

using UnityEngine;
using UnityEngine.Serialization;

using Random = UnityEngine.Random;

namespace Game.Spawner
{
public class SpawnerBehaviour : MonoBehaviour
{
    [Header("Modifying these values at runtime will not affect the spawning.")]
    [SerializeField]
    [Tooltip("How many entities to spawn.")]
    private int amount = 1;

    [SerializeField] [Tooltip("How many times to spawn entities.")]
    private int times = 1;

    [SerializeField] [Tooltip("How often to spawn, in seconds.")]
    private float interval = 1.0f;

    [SerializeField] [Tooltip("The probability that enemies will be spawned every interval.")]
    private float chance = 1.0f;

    [SerializeField] [Tooltip("The range in which entities will be spawned around the spawner.")]
    private float range;

    [SerializeField] [Tooltip("How fast a spawned entity can accelerate.")]
    private float maxAcceleration = 10f;

    [FormerlySerializedAs("minVelocity")] [SerializeField] [Tooltip("Minimum random start velocity.")]
    private float3 minInitialVelocity = -10f;

    [FormerlySerializedAs("maxVelocity")] [SerializeField] [Tooltip("Maximum random start velocity.")]
    private float3 maxInitialVelocity = 10f;

    [SerializeField] [Tooltip("The start health of all spawned entities.")]
    private float maxHealth = 10f;

    [SerializeField] [Tooltip("How much damage is dealt on hit.")]
    private float damage = 2f;

    [SerializeField] [Tooltip("The cooldown between each attack.")]
    private float cooldown = 0.1f;

    [SerializeField] [Tooltip("The minimum angle at which the fighter must be pointed towards the target to fire.")]
    private float firingArc = math.PI / 4f;

    [SerializeField] [Tooltip("The maximum distance from the target at which the fighter can fire.")]
    private float firingRange = 100f;

    [SerializeField] [Tooltip("The name of the entities spawned.")]
    private string entityName = "Entity";

    private readonly SpawnerFactory _spawnerFactory = new SpawnerFactory();
    private ISpawner _spawner;

    private void Awake()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var random = new Unity.Mathematics.Random((uint) Random.Range(1, 100000));
        EntityArchetype enemyFighterArchetype = entityManager.CreateArchetype(
            typeof(EnemyFighterTag),
            typeof(TargetsPlayerTag),
            typeof(LaserCannon),
            /**/
            typeof(RenderMesh),
            typeof(RenderBounds),
            typeof(Translation),
            /**/
            typeof(Acceleration),
            typeof(Velocity),
            /**/
            typeof(Health),
            typeof(CooldownElement),
            typeof(NeedsVisualTag),
            typeof(ExplodesOnDeath)
        );
        var componentDataFunctions = new List<Func<IRandom, IComponentData>>
        {
            rand => GetRandomTranslationInRange(rand, transform.position),
            rand => new LaserCannon {
                Damage = damage,
                Cooldown = cooldown ,
                FiringArc = firingArc,
                Range = firingRange
            },
            rand => new Health { Value = maxHealth },
            rand => new Velocity { Value = rand.NextFloat3(minInitialVelocity, maxInitialVelocity) },
            rand => new Acceleration { Value = 0f, Max = maxAcceleration }
        };
        ISpawner baseSpawner = _spawnerFactory.CreateBaseSpawner(
            entityManager,
            enemyFighterArchetype,
            random,
            amount,
            componentDataFunctions,
            entityName
        );
        ISpawner limitedSpawner = new LimitedSpawner(baseSpawner, times);
        ISpawner chanceSpawner = new ChanceSpawner(limitedSpawner, new RandomAdapter(random), chance);
        _spawner = new IntervalSpawner(chanceSpawner, interval);
    }

    private void Update()
    {
        using NativeArray<Entity> spawned = _spawner.Spawn(Time.time);
    }

    private Translation GetRandomTranslationInRange(IRandom random, float3 spawnPos)
    {
        var offset = new float3(-range, -range, -range);
        float3 minPos = new float3(spawnPos) - offset;
        float3 maxPos = new float3(spawnPos) + offset;
        float3 randPos = random.NextFloat3(minPos, maxPos);
        return new Translation { Value = randPos };
    }
}
}
