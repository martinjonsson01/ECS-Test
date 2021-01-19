using Game.Enemy;
using Game.Player;

using NUnit.Framework;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;

using static NUnit.Framework.Assert;

namespace Tests.Enemy
{
public class TargetPlayerSystemTests : SystemTestBase<TargetPlayerSystem>
{
    private Entity _entity;
    private Entity _player;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _player = m_Manager.CreateEntity(
            typeof(PlayerTag), typeof(Translation));
        _entity = m_Manager.CreateEntity(
            typeof(TargetsPlayerTag));
    }

    [Test]
    public void When_ThereIsNoPlayer_DontTargetAnything()
    {
        m_Manager.DestroyEntity(_player);

        World.Update();

        IsFalse(m_Manager.HasComponent<Target>(_entity));
    }

    [Test]
    public void When_ThereIsAPlayer_GetsFollowTargetComponent()
    {
        IsNotNull(Camera.main);

        World.Update();

        IsTrue(m_Manager.HasComponent<Target>(_entity));
    }

    [Test]
    public void When_ThereIsAPlayer_FollowTargetPositionMatchesPlayer()
    {
        float3 playerPos = m_Manager.GetComponentData<Translation>(_player).Value;

        World.Update();

        float3 targetPos = m_Manager.GetComponentData<Target>(_entity).Position;
        AreEqual(playerPos, targetPos);
    }

    [Test]
    public void When_ThereIsAPlayer_AndPlayerMoves_FollowTarget_IsUpdated()
    {
        float3 playerPos = m_Manager.GetComponentData<Translation>(_player).Value;

        World.Update();

        float3 targetPos = m_Manager.GetComponentData<Target>(_entity).Position;
        AreEqual(playerPos, targetPos);

        m_Manager.SetComponentData(_player, new Translation { Value = new float3(10f, 0f, 0f) });

        playerPos = m_Manager.GetComponentData<Translation>(_player).Value;

        World.Update();

        targetPos = m_Manager.GetComponentData<Target>(_entity).Position;
        AreEqual(playerPos, targetPos);
    }
}
}
