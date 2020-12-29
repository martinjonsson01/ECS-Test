using Game.Life;

using NUnit.Framework;

using Unity.Entities;

using static NUnit.Framework.Assert;

namespace Tests.Life
{
public class DeathSystemTests : SystemTestBase<DeathSystem>
{
    private Entity _entity;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _entity = m_Manager.CreateEntity();
        m_Manager.AddComponentData(_entity, new Health { Value = 10f });
    }

    [Test]
    public void When_EntityHasHealth_EntityStillExists()
    {
        World.Update();

        IsTrue(m_Manager.Exists(_entity));
    }

    [Test]
    public void When_EntityHasZeroHealth_EntityIsRemoved()
    {
        m_Manager.SetComponentData(_entity, new Health { Value = 0f });

        World.Update();

        IsFalse(m_Manager.Exists(_entity));
    }

    [Test]
    public void When_EntityHasNegativeHealth_EntityIsRemoved()
    {
        m_Manager.SetComponentData(_entity, new Health { Value = -10f });

        World.Update();

        IsFalse(m_Manager.Exists(_entity));
    }
}
}
