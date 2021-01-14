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

    public override string ToString()
    {
        return $"{nameof(DesiredRotation)}(Value={Value.ToString()}, TurningRate={TurningRate}, PreviousValue={PreviousValue.ToString()}, SecondsToTurn={SecondsToTurn}, SecondsPassed={SecondsPassed})";
    }
}
}
