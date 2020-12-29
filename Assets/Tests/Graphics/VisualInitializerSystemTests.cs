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
    private Entity _entity;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _entity = m_Manager.CreateEntity(
            typeof(NeedsVisualTag),
            typeof(EnemyFighterTag));

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
    public void When_EntityHasNeedsVisualTag_TagIsRemoved()
    {
        IsTrue(m_Manager.HasComponent<NeedsVisualTag>(_entity));

        World.Update();

        IsFalse(m_Manager.HasComponent<NeedsVisualTag>(_entity));
    }

    [Test]
    public void When_EntityIsSpawnedForFirstTime_NewArchetypesAreCreated()
    {
        var initialArchetypes = new NativeList<EntityArchetype>(Allocator.Temp);
        m_Manager.GetAllArchetypes(initialArchetypes);

        World.Update();

        var newArchetypes = new NativeList<EntityArchetype>(Allocator.Temp);
        m_Manager.GetAllArchetypes(newArchetypes);

        AreNotEqual(initialArchetypes.Length, newArchetypes.Length);
    }
}
}
