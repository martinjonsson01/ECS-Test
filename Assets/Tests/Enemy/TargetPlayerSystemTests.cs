using Game.Enemy;

using NUnit.Framework;

using Unity.Entities;
using Unity.Mathematics;

using UnityEngine;

using static NUnit.Framework.Assert;

namespace Tests.Enemy
{
public class TargetPlayerSystemTests : SystemTestBase<TargetPlayerSystem>
{
    private Entity _entity;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _entity = m_Manager.CreateEntity(
            typeof(TargetsPlayerTag));
    }

    [Test]
    public void When_ThereIsNoPlayer_DontTargetAnything()
    {
        GameObject mainCamera = GameObject.FindWithTag("MainCamera");
        mainCamera.SetActive(false);

        World.Update();

        IsFalse(m_Manager.HasComponent<Target>(_entity));
        mainCamera.SetActive(true);
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
        IsNotNull(Camera.main);
        float3 playerPos = Camera.main.transform.position;

        World.Update();

        float3 targetPos = m_Manager.GetComponentData<Target>(_entity).Position;
        AreEqual(playerPos, targetPos);
    }

    [Test]
    public void When_ThereIsAPlayer_AndPlayerMoves_FollowTarget_IsUpdated()
    {
        IsNotNull(Camera.main);
        Transform playerTransform = Camera.main.transform;
        float3 playerPos = playerTransform.position;

        World.Update();

        float3 targetPos = m_Manager.GetComponentData<Target>(_entity).Position;
        AreEqual(playerPos, targetPos);

        playerTransform.Translate(10f, 0f, 0f);

        playerPos = playerTransform.position;

        World.Update();

        targetPos = m_Manager.GetComponentData<Target>(_entity).Position;
        AreEqual(playerPos, targetPos);
    }
}
}
