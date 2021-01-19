using Unity.Entities;

namespace Tests.Graphics
{
[GenerateAuthoringComponent]
public struct Size : IComponentData
{
    public float Value;
}
}
