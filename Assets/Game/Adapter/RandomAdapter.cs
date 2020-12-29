using Unity.Mathematics;

using Random = Unity.Mathematics.Random;

namespace Game.Adapter
{
public class RandomAdapter : IRandom
{
    private Random _random;

    public RandomAdapter(Random random)
    {
        _random = random;
    }

    public float NextFloat()
    {
        return _random.NextFloat();
    }

    public float NextFloat(float f, float f1)
    {
        return _random.NextFloat(f, f1);
    }

    public float3 NextFloat3(float3 min, float3 max)
    {
        return _random.NextFloat3(min, max);
    }

    public quaternion NextQuaternionRotation()
    {
        return _random.NextQuaternionRotation();
    }
}
}
