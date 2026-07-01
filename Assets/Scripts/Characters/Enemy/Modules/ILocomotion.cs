using UnityEngine;

public interface ILocomotion
{
    void Patrol();
    void Chase(Vector2 targetPosition);
    void Stop();
}
