using Game.Cooldown;
using Game.Weapon;

using NUnit.Framework;

using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

using static NUnit.Framework.Assert;

using Entity = Unity.Entities.Entity;

namespace Tests.Cooldown
{
public class CooldownSystemTests : SystemTestBase<CooldownSystem>
{
    private Entity _entity;
    private DynamicBuffer<CooldownElement> _cooldownBuffer;
    private ConstantDeltaTimeSystem _constantDeltaTimeSystem;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _constantDeltaTimeSystem = World.GetExistingSystem<ConstantDeltaTimeSystem>();
        _entity = m_Manager.CreateEntity(
            typeof(LaserCannon),
            typeof(Translation));
        _cooldownBuffer = m_Manager.AddBuffer<CooldownElement>(_entity);
    }

    [Test]
    public void When_EntityHasCooldownWithoutType_CooldownIsRemoved()
    {
        var cooldown = new CooldownElement { Seconds = 1f };
        _cooldownBuffer.Add(cooldown);

        World.Update();

        DynamicBuffer<CooldownElement> bufferAfter = m_Manager.GetBuffer<CooldownElement>(_entity);
        AreEqual(0, bufferAfter.Length);
    }

    [Test]
    public void When_EntityHasCooldownAndNoTimeHasPassed_CooldownRemainsTheSame()
    {
        const float initialCooldown = 10f;
        const CooldownType cooldownType = CooldownType.Test1;
        var cooldown = new CooldownElement { Type = cooldownType, Seconds = initialCooldown };
        _cooldownBuffer.Add(cooldown);

        _constantDeltaTimeSystem.ForceDeltaTime(0f);

        World.Update();

        DynamicBuffer<CooldownElement> bufferAfter = m_Manager.GetBuffer<CooldownElement>(_entity);
        CooldownElement actualCooldown = bufferAfter[0];
        AreEqual(initialCooldown, actualCooldown.Seconds);
    }

    [Test]
    public void When_EntityHasCooldown_CooldownIsDecreasedByDeltaTime()
    {
        const float initialCooldown = 10f;
        var cooldown = new CooldownElement { Type = CooldownType.Test1, Seconds = initialCooldown };
        _cooldownBuffer.Add(cooldown);

        World.Update();

        DynamicBuffer<CooldownElement> bufferAfter = m_Manager.GetBuffer<CooldownElement>(_entity);
        const float expectedCooldown = initialCooldown - ForcedDeltaTime;
        float actualCooldown = bufferAfter[0].Seconds;
        AreEqual(expectedCooldown, actualCooldown);
    }

    [Test]
    public void When_EntityHasCooldownThatIsZero_CooldownIsRemoved()
    {
        var cooldown = new CooldownElement { Type = CooldownType.Test1, Seconds = 0f };
        _cooldownBuffer.Add(cooldown);

        World.Update();

        DynamicBuffer<CooldownElement> bufferAfter = m_Manager.GetBuffer<CooldownElement>(_entity);
        AreEqual(0, bufferAfter.Length);
    }

    [Test]
    public void When_EntityHasCooldownThatRunsOut_CooldownIsRemoved()
    {
        var cooldown = new CooldownElement { Type = CooldownType.Test1, Seconds = ForcedDeltaTime };
        _cooldownBuffer.Add(cooldown);

        World.Update();

        DynamicBuffer<CooldownElement> bufferAfter = m_Manager.GetBuffer<CooldownElement>(_entity);
        AreEqual(0, bufferAfter.Length);
    }

    [Test]
    public void When_EntityHasCooldownThatIsNegative_CooldownIsRemoved()
    {
        var cooldown = new CooldownElement { Type = CooldownType.Test1, Seconds = -1f };
        _cooldownBuffer.Add(cooldown);

        World.Update();

        DynamicBuffer<CooldownElement> bufferAfter = m_Manager.GetBuffer<CooldownElement>(_entity);
        AreEqual(0, bufferAfter.Length);
    }

    [Test]
    public void When_EntityHasMultipleCooldowns_TheyAreAllDecreasedIndividually()
    {
        const float initialCooldownSecond1 = 1f;
        const float initialCooldownSecond2 = 5f;
        const float initialCooldownSecond3 = 10f;
        const CooldownType type1 = CooldownType.Test1;
        const CooldownType type2 = CooldownType.Test2;
        const CooldownType type3 = CooldownType.Test3;
        var initialCooldown1 = new CooldownElement { Type = type1, Seconds = initialCooldownSecond1 };
        var initialCooldown2 = new CooldownElement { Type = type2, Seconds = initialCooldownSecond2 };
        var initialCooldown3 = new CooldownElement { Type = type3, Seconds = initialCooldownSecond3 };
        _cooldownBuffer.Add(initialCooldown1);
        _cooldownBuffer.Add(initialCooldown2);
        _cooldownBuffer.Add(initialCooldown3);

        World.Update();

        DynamicBuffer<CooldownElement> bufferAfter = m_Manager.GetBuffer<CooldownElement>(_entity);

        const float expectedCooldown1 = initialCooldownSecond1 - ForcedDeltaTime;
        float actualCooldown1 = bufferAfter[0].Seconds;
        const float expectedCooldown2 = initialCooldownSecond2 - ForcedDeltaTime;
        float actualCooldown2 = bufferAfter[1].Seconds;
        const float expectedCooldown3 = initialCooldownSecond3 - ForcedDeltaTime;
        float actualCooldown3 = bufferAfter[2].Seconds;

        AreEqual(expectedCooldown1, actualCooldown1);
        AreEqual(expectedCooldown2, actualCooldown2);
        AreEqual(expectedCooldown3, actualCooldown3);
    }

    [Test]
    public void When_EntityHasMultipleCooldowns_TheyAreAllRemovedIndividually()
    {
        const float initialCooldownSecond1 = 1f;
        const float initialCooldownSecond2 = 5f;
        const float initialCooldownSecond3 = 10f;
        const CooldownType type1 = CooldownType.Test1;
        const CooldownType type2 = CooldownType.Test2;
        const CooldownType type3 = CooldownType.Test3;
        var initialCooldown1 = new CooldownElement { Type = type1, Seconds = initialCooldownSecond1 };
        var initialCooldown2 = new CooldownElement { Type = type2, Seconds = initialCooldownSecond2 };
        var initialCooldown3 = new CooldownElement { Type = type3, Seconds = initialCooldownSecond3 };
        _cooldownBuffer.Add(initialCooldown1);
        _cooldownBuffer.Add(initialCooldown2);
        _cooldownBuffer.Add(initialCooldown3);

        _constantDeltaTimeSystem.ForceDeltaTime(initialCooldownSecond1);
        World.Update();
        NativeArray<CooldownElement> bufferAfterUpdate1 =
            m_Manager.GetBuffer<CooldownElement>(_entity).ToNativeArray(Allocator.Temp);

        _constantDeltaTimeSystem.ForceDeltaTime(initialCooldownSecond2);
        World.Update();
        NativeArray<CooldownElement> bufferAfterUpdate2 =
            m_Manager.GetBuffer<CooldownElement>(_entity).ToNativeArray(Allocator.Temp);

        _constantDeltaTimeSystem.ForceDeltaTime(initialCooldownSecond3);
        World.Update();
        NativeArray<CooldownElement> bufferAfterUpdate3 =
            m_Manager.GetBuffer<CooldownElement>(_entity).ToNativeArray(Allocator.Temp);

        That(bufferAfterUpdate1, Has.No.Member(initialCooldown1));
        That(bufferAfterUpdate2, Has.No.Member(initialCooldown2));
        That(bufferAfterUpdate3, Has.No.Member(initialCooldown3));
    }

    [TearDown]
    public override void TearDown()
    {
        base.TearDown();
        _constantDeltaTimeSystem.ForceDeltaTime(ForcedDeltaTime);
    }
}
}
