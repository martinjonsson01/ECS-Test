using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;

namespace Game.Movement
{
[UpdateInGroup(typeof(SimulationSystemGroup))]
public class SmoothRotationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities
            .WithStructuralChanges()
            .ForEach((
                Entity entity,
                ref Rotation rotation,
                ref DesiredRotation desiredRotation) =>
            {
                if (!desiredRotation.Value.Equals(desiredRotation.PreviousValue))
                {
                    float degreesToTurn = Quaternion.Angle(rotation.Value, desiredRotation.Value);

                    desiredRotation.SecondsToTurn = degreesToTurn / desiredRotation.TurningRate;
                    desiredRotation.SecondsPassed = 0f;
                }

                desiredRotation.SecondsPassed += deltaTime;
                float fraction = Mathf.SmoothStep(0f, 1f, desiredRotation.SecondsPassed / desiredRotation.SecondsToTurn);
                float4 turnTo = math.lerp(rotation.Value.value, desiredRotation.Value.value, fraction);

                rotation.Value = turnTo;

                desiredRotation.PreviousValue = desiredRotation.Value;

                if (rotation.Value.Equals(desiredRotation.Value))
                {
                    EntityManager.RemoveComponent<DesiredRotation>(entity);
                }
            })
            .WithBurst()
            .Run();
    }
}
}
