using System;
using System.Collections.Generic;

using BovineLabs.Event.Systems;

using NUnit.Framework;

using Unity.Entities;
using Unity.Physics.Systems;

namespace Tests
{
[Category("Unity ECS Tests")]
public abstract class SystemTestBase<T> : ECSTestsFixture where T : ComponentSystemBase
{
    protected const float ForcedDeltaTime = 1 / 90f;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        SetUpWorld();
    }

    private void SetUpWorld()
    {
        DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(World,
            new List<Type>
            {
                typeof(EventSystem),
                typeof(EndSimulationEntityCommandBufferSystem),
                typeof(T),
                typeof(ConstantDeltaTimeSystem)
            });
        World.GetExistingSystem<ConstantDeltaTimeSystem>().ForceDeltaTime(ForcedDeltaTime);
    }

    protected void CreateAndSetUpPhysicsSystems()
    {
        World.GetOrCreateSystem<BuildPhysicsWorld>().Update();
        World.GetOrCreateSystem<ExportPhysicsWorld>();
        World.GetOrCreateSystem<StepPhysicsWorld>();
    }
}
}
