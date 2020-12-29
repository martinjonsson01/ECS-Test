using Unity.Mathematics;

namespace Game.Adapter
{
public interface IRandom
{
    float NextFloat();
    float NextFloat(float f, float f1);
    float3 NextFloat3(float3 min, float3 max);
    quaternion NextQuaternionRotation();
}
}
