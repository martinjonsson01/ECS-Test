using Unity.Entities;

namespace Game.Life.Explosion
{
[GenerateAuthoringComponent]
public struct ExplodesOnDeath : IComponentData
{
    // Right now only a tag, but could contain data about how to explode in the future.
}
}
