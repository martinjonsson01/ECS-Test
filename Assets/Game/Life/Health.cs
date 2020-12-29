using Unity.Entities;

namespace Game.Life
{
[GenerateAuthoringComponent]
public struct Health : IComponentData
{
    /**
         * Hitpoints representing how much damage can be taken.
         */
    public float Value;
}
}
