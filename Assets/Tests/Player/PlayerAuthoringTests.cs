using Game.Player;

using NSubstitute;

using NUnit.Framework;

using Unity.Entities;

using UnityEngine;

using static NUnit.Framework.Assert;

namespace Tests.Player
{
[Category("GameObject Authoring")]
public class PlayerAuthoringTests : ECSTestsFixture
{
    private GameObject _gameObject;
    private IConvertGameObjectToEntity _authoring;
    private GameObjectConversionSystem _conversionSystem;

    [SetUp]
    public void SetUp()
    {
        _gameObject = new GameObject("Test Player");
        _authoring = _gameObject.AddComponent<PlayerAuthoring>();
        _conversionSystem = Substitute.For<GameObjectConversionSystem>();
    }

    [Test]
    public void When_GameObjectIsConverted_EntityIsGivenCorrectComponents()
    {
        Entity entity = m_Manager.CreateEntity();

        _authoring.Convert(entity, m_Manager, _conversionSystem);

        IsTrue(m_Manager.HasComponent<PlayerTag>(entity), $"Has {nameof(PlayerTag)}");
    }
}
}
