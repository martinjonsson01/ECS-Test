#if !UNITY_DOTSRUNTIME
#endif

using NUnit.Framework;

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;

using UnityEngine.LowLevel;

#if !NET_DOTS

#endif

namespace Tests
{
#if NET_DOTS
    public class EmptySystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
        }

        public new EntityQuery GetEntityQuery(params EntityQueryDesc[] queriesDesc)
        {
            return base.GetEntityQuery(queriesDesc);
        }

        public new EntityQuery GetEntityQuery(params ComponentType[] componentTypes)
        {
            return base.GetEntityQuery(componentTypes);
        }

        public new EntityQuery GetEntityQuery(NativeArray<ComponentType> componentTypes)
        {
            return base.GetEntityQuery(componentTypes);
        }

        public new BufferFromEntity<T> GetBufferFromEntity<T>(bool isReadOnly =
 false) where T : struct, IBufferElementData
        {
            AddReaderWriter(isReadOnly ? ComponentType.ReadOnly<T>() : ComponentType.ReadWrite<T>());
            return EntityManager.GetBufferFromEntity<T>(isReadOnly);
        }
    }
#else
public class EmptySystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle dep)
    {
        return dep;
    }


    public new EntityQuery GetEntityQuery(params EntityQueryDesc[] queriesDesc)
    {
        return base.GetEntityQuery(queriesDesc);
    }

    public new EntityQuery GetEntityQuery(params ComponentType[] componentTypes)
    {
        return base.GetEntityQuery(componentTypes);
    }

    public new EntityQuery GetEntityQuery(NativeArray<ComponentType> componentTypes)
    {
        return base.GetEntityQuery(componentTypes);
    }
}

#endif

public class ECSTestsCommonBase
{
    [SetUp]
    public virtual void Setup()
    {
#if UNITY_DOTSRUNTIME
            Unity.Core.TempMemoryScope.EnterScope();
#endif
    }

    [TearDown]
    public virtual void TearDown()
    {
#if UNITY_DOTSRUNTIME
            Unity.Core.TempMemoryScope.ExitScope();
#endif
    }
}

public abstract class ECSTestsFixture : ECSTestsCommonBase
{
    private bool JobsDebuggerWasEnabled;
    protected EntityManager m_Manager;
    protected EntityManager.EntityManagerDebug m_ManagerDebug;
#if !UNITY_DOTSRUNTIME
    protected PlayerLoopSystem m_PreviousPlayerLoop;
#endif
    protected World m_PreviousWorld;

    protected int StressTestEntityCount = 1000;
    protected World World;

    protected EntityQueryBuilder Entities => new EntityQueryBuilder();

    public EmptySystem EmptySystem => World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EmptySystem>();

    [SetUp]
    public override void Setup()
    {
        base.Setup();

#if !UNITY_DOTSRUNTIME
        // unit tests preserve the current player loop to restore later, and start from a blank slate.
        m_PreviousPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
        PlayerLoop.SetPlayerLoop(PlayerLoop.GetDefaultPlayerLoop());
#endif

        m_PreviousWorld = World.DefaultGameObjectInjectionWorld;
        World = World.DefaultGameObjectInjectionWorld = new World("Test World");
        m_Manager = World.EntityManager;
        m_ManagerDebug = new EntityManager.EntityManagerDebug(m_Manager);

        // Many ECS tests will only pass if the Jobs Debugger enabled;
        // force it enabled for all tests, and restore the original value at teardown.
        JobsDebuggerWasEnabled = JobsUtility.JobDebuggerEnabled;
        JobsUtility.JobDebuggerEnabled = true;
    }

    [TearDown]
    public override void TearDown()
    {
        if (World != null && World.IsCreated)
        {
            // Clean up systems before calling CheckInternalConsistency because we might have filters etc
            // holding on SharedComponentData making checks fail
            while (World.Systems.Count > 0) World.DestroySystem(World.Systems[0]);

            m_ManagerDebug.CheckInternalConsistency();

            World.Dispose();
            World = null;

            World.DefaultGameObjectInjectionWorld = m_PreviousWorld;
            m_PreviousWorld = null;
            m_Manager = default;
        }

        JobsUtility.JobDebuggerEnabled = JobsDebuggerWasEnabled;

#if !UNITY_DOTSRUNTIME
        PlayerLoop.SetPlayerLoop(m_PreviousPlayerLoop);
#endif

        base.TearDown();
    }

    public void AssertDoesNotExist(Entity entity)
    {
        Assert.IsFalse(m_Manager.HasComponent<EcsTestData>(entity));
        Assert.IsFalse(m_Manager.HasComponent<EcsTestData2>(entity));
        Assert.IsFalse(m_Manager.HasComponent<EcsTestData3>(entity));
        Assert.IsFalse(m_Manager.Exists(entity));
    }

    public void AssertComponentData(Entity entity, int index)
    {
        Assert.IsTrue(m_Manager.HasComponent<EcsTestData>(entity));
        Assert.IsTrue(m_Manager.HasComponent<EcsTestData2>(entity));
        Assert.IsFalse(m_Manager.HasComponent<EcsTestData3>(entity));
        Assert.IsTrue(m_Manager.Exists(entity));

        Assert.AreEqual(-index, m_Manager.GetComponentData<EcsTestData2>(entity).value0);
        Assert.AreEqual(-index, m_Manager.GetComponentData<EcsTestData2>(entity).value1);
        Assert.AreEqual(index, m_Manager.GetComponentData<EcsTestData>(entity).value);
    }

    public Entity CreateEntityWithDefaultData(int index)
    {
        Entity entity = m_Manager.CreateEntity(typeof(EcsTestData), typeof(EcsTestData2));

        // HasComponent & Exists setup correctly
        Assert.IsTrue(m_Manager.HasComponent<EcsTestData>(entity));
        Assert.IsTrue(m_Manager.HasComponent<EcsTestData2>(entity));
        Assert.IsFalse(m_Manager.HasComponent<EcsTestData3>(entity));
        Assert.IsTrue(m_Manager.Exists(entity));

        // Create must initialize values to zero
        Assert.AreEqual(0, m_Manager.GetComponentData<EcsTestData2>(entity).value0);
        Assert.AreEqual(0, m_Manager.GetComponentData<EcsTestData2>(entity).value1);
        Assert.AreEqual(0, m_Manager.GetComponentData<EcsTestData>(entity).value);

        // Setup some non zero default values
        m_Manager.SetComponentData(entity, new EcsTestData2(-index));
        m_Manager.SetComponentData(entity, new EcsTestData(index));

        AssertComponentData(entity, index);

        return entity;
    }

    public void AssertSameChunk(Entity e0, Entity e1)
    {
        Assert.AreEqual(m_Manager.GetChunk(e0), m_Manager.GetChunk(e1));
    }

    public void AssetHasChangeVersion<T>(Entity e, uint version) where T :
#if UNITY_DISABLE_MANAGED_COMPONENTS
        struct,
#endif
        IComponentData
    {
        ComponentTypeHandle<T> type = m_Manager.GetComponentTypeHandle<T>(true);
        ArchetypeChunk chunk = m_Manager.GetChunk(e);
        Assert.AreEqual(version, chunk.GetChangeVersion(type));
        Assert.IsFalse(chunk.DidChange(type, version));
        Assert.IsTrue(chunk.DidChange(type, version - 1));
    }

    public void AssetHasChunkOrderVersion(Entity e, uint version)
    {
        ArchetypeChunk chunk = m_Manager.GetChunk(e);
        Assert.AreEqual(version, chunk.GetOrderVersion());
    }

    public void AssetHasBufferChangeVersion<T>(Entity e, uint version) where T : struct, IBufferElementData
    {
        BufferTypeHandle<T> type = m_Manager.GetBufferTypeHandle<T>(true);
        ArchetypeChunk chunk = m_Manager.GetChunk(e);
        Assert.AreEqual(version, chunk.GetChangeVersion(type));
        Assert.IsFalse(chunk.DidChange(type, version));
        Assert.IsTrue(chunk.DidChange(type, version - 1));
    }

    public void AssetHasSharedChangeVersion<T>(Entity e, uint version) where T : struct, ISharedComponentData
    {
        SharedComponentTypeHandle<T> type = m_Manager.GetSharedComponentTypeHandle<T>();
        ArchetypeChunk chunk = m_Manager.GetChunk(e);
        Assert.AreEqual(version, chunk.GetChangeVersion(type));
        Assert.IsFalse(chunk.DidChange(type, version));
        Assert.IsTrue(chunk.DidChange(type, version - 1));
    }

    private class EntityForEachSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
        }
    }
}
}
