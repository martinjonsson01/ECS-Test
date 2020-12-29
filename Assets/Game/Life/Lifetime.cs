using Unity.Entities;

namespace Game.Life
{
[GenerateAuthoringComponent]
public struct Lifetime : IComponentData
{
    public float Seconds;
}
}
