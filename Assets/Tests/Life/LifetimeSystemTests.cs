using Game.Life;

using NUnit.Framework;

using Unity.Entities;

using static NUnit.Framework.Assert;

namespace Tests.Life
{
public class LifetimeSystemTests : SystemTestBase<LifetimeSystem>
{
    private Entity _entity;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _entity = m_Manager.CreateEntity(
            typeof(Lifetime));
        m_Manager.SetComponentData(_entity, new Lifetime { Seconds = 0f });
    }

    [Test]
    public void When_EntityHasLifetimeLeft_EntityIsNotDestroyed()
    {
        m_Manager.SetComponentData(_entity, new Lifetime { Seconds = 10f });

        World.Update();

        IsTrue(m_Manager.Exists(_entity));
    }

    [Test]
    public void When_EntityHasNoLifetimeLeft_EntityIsDestroyed()
    {
        m_Manager.SetComponentData(_entity, new Lifetime { Seconds = 0f });

        World.Update();

        IsFalse(m_Manager.Exists(_entity));
    }

    [Test]
    public void When_EntityHasNegativeLifetimeLeft_EntityIsDestroyed()
    {
        m_Manager.SetComponentData(_entity, new Lifetime { Seconds = -10f });

        World.Update();

        IsFalse(m_Manager.Exists(_entity));
    }

    [Test]
    public void When_EntityHasLifetime_LifetimeIsDecreased()
    {
        const float initialSeconds = 10f;
        m_Manager.SetComponentData(_entity, new Lifetime { Seconds = initialSeconds });

        World.Update();

        const float expectedSeconds = initialSeconds - ForcedDeltaTime;
        AreEqual(expectedSeconds, m_Manager.GetComponentData<Lifetime>(_entity).Seconds);
    }
}
}
