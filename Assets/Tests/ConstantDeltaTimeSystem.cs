using Unity.Core;
using Unity.Entities;

namespace Tests
{
/// <summary>
///     This is for test only. It disables the regular one and can hack any delta time instead.
/// </summary>
//[UpdateAfter(typeof(UpdateWorldTimeSystem))]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[DisableAutoCreation]
public class ConstantDeltaTimeSystem : ComponentSystem
{
    private float constantDeltaTime;
    private readonly float defaultDeltaTime = 1 / 90f;
    private float simulatedElapsedTime;

    protected override void OnCreate()
    {
        //Only works if Unity's time system is there already, this would
        //try to replace it. You should not add this system before Unity's.
        var timeSystem = World.GetExistingSystem<UpdateWorldTimeSystem>();
        if (timeSystem != null) timeSystem.Enabled = false;
        constantDeltaTime = defaultDeltaTime;
    }

    public void ForceDeltaTime(float dt)
    {
        constantDeltaTime = dt;
    }

    public void RestoreDeltaTime()
    {
        constantDeltaTime = defaultDeltaTime;
    }

    protected override void OnUpdate()
    {
        simulatedElapsedTime += constantDeltaTime;
        World.SetTime(new TimeData(
            simulatedElapsedTime,
            constantDeltaTime
        ));
    }
}
}
