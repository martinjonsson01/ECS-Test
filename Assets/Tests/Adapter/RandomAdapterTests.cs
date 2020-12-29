using Game.Adapter;

using NUnit.Framework;

using Unity.Mathematics;

using UnityEngine;

using static NUnit.Framework.Assert;

using Random = Unity.Mathematics.Random;

namespace Tests.Adapter
{
[Category("Core Tests")]
public class RandomAdapterTests
{
    private const float Delta = 0.000001f;
    private IRandom _adaptedRandom;

    private Random _random;

    [SetUp]
    public void SetUp()
    {
        const uint seed = 10;
        _random = new Random(seed);
        _adaptedRandom = new RandomAdapter(new Random(seed));
    }

    [Test]
    public void NextFloat_Matches_Random()
    {
        float float1 = _adaptedRandom.NextFloat();
        float float2 = _random.NextFloat();

        AreEqual(float1, float2, Delta);
    }

    [Test]
    public void NextFloatRange_Matches_Random()
    {
        const float min = 1f;
        const float max = 10f;
        float float1 = _adaptedRandom.NextFloat(min, max);
        float float2 = _random.NextFloat(min, max);

        AreEqual(float1, float2, Delta);
    }

    [Test]
    public void NextFloat3Range_Matches_Random()
    {
        float3 min = 1f;
        float3 max = 10f;
        float3 float1 = _adaptedRandom.NextFloat3(min, max);
        float3 float2 = _random.NextFloat3(min, max);

        AreEqual(float1, float2);
    }

    [Test]
    public void NextQuaternion_Matches_Random()
    {
        Quaternion quaternion1 = _adaptedRandom.NextQuaternionRotation();
        Quaternion quaternion2 = _random.NextQuaternionRotation();

        AreEqual(quaternion1, quaternion2);
    }
}
}
