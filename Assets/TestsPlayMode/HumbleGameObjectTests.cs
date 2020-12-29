using System.Collections;

using Game.Spawner;

using NUnit.Framework;
using Unity.Entities;
using UnityEngine;
using UnityEngine.TestTools;
using static NUnit.Framework.Assert;

namespace TestsPlayMode
{
    [Category("PlayMode Tests")]
    public class HumbleGameObjectTests
    {
        private World _world;

        private GameObject _gameObject;

        [UnitySetUp]
        // ReSharper disable once UnusedMember.Global
        public IEnumerator SetUp()
        {
            _world = World.DefaultGameObjectInjectionWorld = new World("Test World");
            _gameObject = new GameObject("Test Object");
            yield return null;
        }

        [UnityTest]
        public IEnumerator When_Instantiated_SpawnerBehaviourExists()
        {
            _gameObject.AddComponent<SpawnerBehaviour>();

            yield return new WaitForFixedUpdate();

            IsTrue(_gameObject.TryGetComponent(out SpawnerBehaviour _));
        }
    }
}
