using Game.Adapter;

using NUnit.Framework;

using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

using static NUnit.Framework.Assert;

namespace Tests.Adapter
{
[Category("Core Tests")]
public class EntityManagerAdapterTests : WorldTestBase
{
    private IEntityManager _adaptedEntityManager;
    private EntityArchetype _archetype;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _adaptedEntityManager = new EntityManagerAdapter(EntityManager);
        _archetype = EntityManager.CreateArchetype(typeof(Translation));
    }

    [Test]
    public void CreateEntityLength_Matches_EntityManagerLength()
    {
        using var entities1 = new NativeArray<Entity>(2, Allocator.Temp);
        using var entities2 = new NativeArray<Entity>(2, Allocator.Temp);

        _adaptedEntityManager.CreateEntity(new EntityArchetypeAdapter(_archetype), entities1);
        EntityManager.CreateEntity(_archetype, entities2);

        AreEqual(entities1.Length, entities2.Length);
    }

    [Test]
    public void CreateEntity_DoesNot_CreateNewArchetypes()
    {
        using var entities1 = new NativeArray<Entity>(2, Allocator.Temp);
        using var archetypesBefore = new NativeList<EntityArchetype>(Allocator.Temp);
        using var archetypesAfter = new NativeList<EntityArchetype>(Allocator.Temp);

        EntityManager.GetAllArchetypes(archetypesBefore);

        _adaptedEntityManager.CreateEntity(new EntityArchetypeAdapter(_archetype), entities1);

        EntityManager.GetAllArchetypes(archetypesAfter);

        AreEqual(archetypesBefore.ToArray(), archetypesAfter.ToArray());
    }

    [Test]
    public void CreateEntitySingle_Returns_AnEntity()
    {
        Entity entity = _adaptedEntityManager.CreateEntity(new EntityArchetypeAdapter(_archetype));

        NotNull(entity);
    }

    [Test]
    public void GetComponentData_Value_Matches_EntityManager_Value()
    {
        Entity entity1 = _adaptedEntityManager.CreateEntity(new EntityArchetypeAdapter(_archetype));
        Entity entity2 = EntityManager.CreateEntity(_archetype);
        var translation = new Translation { Value = 3f };


        EntityManager.SetComponentData(entity1, translation);
        EntityManager.SetComponentData(entity2, translation);
        var entity1ComponentData = _adaptedEntityManager.GetComponentData<Translation>(entity1);
        var entity2ComponentData = EntityManager.GetComponentData<Translation>(entity2);

        AreEqual(entity1ComponentData, entity2ComponentData);
    }

    [Test]
    public void AddComponentData_Types_Matches_EntityManager_Types()
    {
        Entity entity1 = _adaptedEntityManager.CreateEntity(new EntityArchetypeAdapter(_archetype));
        Entity entity2 = EntityManager.CreateEntity(_archetype);
        var translation = new Translation { Value = 3f };


        _adaptedEntityManager.AddComponentData(entity1, translation);
        EntityManager.SetComponentData(entity2, translation);
        using NativeArray<ComponentType> entity1ComponentTypes = EntityManager.GetComponentTypes(entity1);
        using NativeArray<ComponentType> entity2ComponentTypes = EntityManager.GetComponentTypes(entity2);

        AreEqual(entity1ComponentTypes.ToArray(), entity2ComponentTypes.ToArray());
    }

    [Test]
    public void AddComponentData_Values_Matches_EntityManager_Values()
    {
        Entity entity1 = _adaptedEntityManager.CreateEntity(new EntityArchetypeAdapter(_archetype));
        Entity entity2 = EntityManager.CreateEntity(_archetype);
        var translation = new Translation { Value = 3f };


        _adaptedEntityManager.AddComponentData(entity1, translation);
        EntityManager.SetComponentData(entity2, translation);
        var entity1ComponentData = EntityManager.GetComponentData<Translation>(entity1);
        var entity2ComponentData = EntityManager.GetComponentData<Translation>(entity2);

        AreEqual(entity1ComponentData, entity2ComponentData);
    }

    [Test]
    public void CreateArchetype_Value_Matches_EntityManager_Value()
    {
        EntityArchetype entityArchetype1 = _adaptedEntityManager.CreateArchetype(typeof(Translation)).GetArchetype();
        EntityArchetype entityArchetype2 = EntityManager.CreateArchetype(typeof(Translation));

        AreEqual(entityArchetype1, entityArchetype2);
    }

    [Test]
    public void SetName_Value_Matches_EntityManager_Value()
    {
        Entity entity = EntityManager.CreateEntity();
        const string entityName = "Test name";

        _adaptedEntityManager.SetName(entity, entityName);

        AreEqual(entityName, EntityManager.GetName(entity));
    }
}
}
