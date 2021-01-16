using Game.Enemy;
using Game.Graphics;
using Game.Weapon;

using NUnit.Framework;

using Unity.Collections;
using Unity.Entities;

using UnityEngine;

using static NUnit.Framework.Assert;

namespace Tests.Graphics
{
public class VisualInitializerSystemTests : SystemTestBase<VisualInitializerSystem>
{
    private Entity _enemyFighter;
    private Entity _laserBolt;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _enemyFighter = m_Manager.CreateEntity(
            typeof(NeedsVisualTag),
            typeof(EnemyFighterTag));
        _laserBolt = m_Manager.CreateEntity(
            typeof(NeedsVisualTag),
            typeof(LaserBoltTag));

        InstantiateVisualSingletons();
    }

    private void InstantiateVisualSingletons()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        CreateMockVisualSingleton<EnemyFighterVisual>(shader);
        CreateMockVisualSingleton<LaserBoltVisual>(shader);
    }

    private void CreateMockVisualSingleton<TVisualData>(Shader shader)
        where TVisualData : class, IComponentData, IVisualData, new()
    {
        Entity visual = m_Manager.CreateEntity(typeof(TVisualData));
        var visualData = new TVisualData
        {
            Material = new Material(shader),
            Mesh = new Mesh()
        };
        m_Manager.AddComponentData(visual, visualData);
    }

    [Test]
    public void When_EnemyFighterHasNeedsVisualTag_TagIsRemoved()
    {
        IsTrue(m_Manager.HasComponent<NeedsVisualTag>(_enemyFighter));

        World.Update();

        IsFalse(m_Manager.HasComponent<NeedsVisualTag>(_enemyFighter));
    }

    [Test]
    public void When_EntitiesAreSpawnedForFirstTime_NewArchetypesAreCreated()
    {
        var initialArchetypes = new NativeList<EntityArchetype>(Allocator.Temp);
        m_Manager.GetAllArchetypes(initialArchetypes);

        World.Update();

        var newArchetypes = new NativeList<EntityArchetype>(Allocator.Temp);
        m_Manager.GetAllArchetypes(newArchetypes);

        AreNotEqual(initialArchetypes.Length, newArchetypes.Length);
    }

    [Test]
    public void When_LaserBoltHasNeedsVisualTag_TagIsRemoved()
    {
        IsTrue(m_Manager.HasComponent<NeedsVisualTag>(_laserBolt));

        World.Update();

        IsFalse(m_Manager.HasComponent<NeedsVisualTag>(_laserBolt));
    }
}
}
