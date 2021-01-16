using Game.Utils;

using NUnit.Framework;

using Unity.Entities;
using Unity.Mathematics;

namespace Tests.Utils
{
public class ComponentUtilTests
{
    private struct TestComponent : IComponentData
    {
        public float3 Value1;
        public quaternion Value2;
        public float Value3;
        public int Value4;
        public string Value5;
    }

    [Test]
    public void ComponentUtilsToString_MatchesData()
    {
        var component = new TestComponent
        {
            Value1 = new float3(1f, 2f, 3f),
            Value2 = new quaternion(1f, 2f, 3f, 4f),
            Value3 = 11f,
            Value4 = 1463,
            Value5 = "testing_1456"
        };

        var toString = ComponentUtil.ToString(component);


        var expected =
            $"TestComponent(Value1={component.Value1}, Value2={component.Value2}, Value3={component.Value3}, Value4={component.Value4}, Value5={component.Value5})";
        Assert.That(toString, Is.EqualTo(expected));
    }
}
}
