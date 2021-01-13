using System;

using UnityEngine;

namespace Game.Player
{
public class SmoothRotator : MonoBehaviour
{
    public Quaternion desiredRotation = Quaternion.identity;

    [SerializeField]
    private float turningRate = 30f;

    private Quaternion _previousRotation = Quaternion.identity;
    private float _secondsToTurn;
    private float _secondsPassed;
    private void Update()
    {
        if (!desiredRotation.Equals(_previousRotation))
        {
            float degreesToTurn = Quaternion.Angle(transform.rotation, desiredRotation);

            _secondsToTurn = degreesToTurn / turningRate;
            _secondsPassed = 0f;
        }

        _secondsPassed += Time.deltaTime;
        float fraction = Mathf.SmoothStep(0f, 1f, _secondsPassed / _secondsToTurn);
        Quaternion turnTo = Quaternion.Lerp(transform.rotation, desiredRotation, fraction);
        transform.rotation = turnTo;

        _previousRotation = desiredRotation;
    }
}
}
