using Unity.Entities;
using Unity.Mathematics;

namespace Game.Movement
{
[GenerateAuthoringComponent]
public struct DesiredRotation : IComponentData
{
    public quaternion Value;

    public float TurningRate;

    public quaternion PreviousValue;

    public float SecondsToTurn;

    public float SecondsPassed;
}
}
