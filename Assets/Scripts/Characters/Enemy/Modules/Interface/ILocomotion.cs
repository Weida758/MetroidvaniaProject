using UnityEngine;

public interface ILocomotion
{
    void Patrol();
    void InCombatMovement(Vector2 targetPosition);
    void FaceCombatTarget(Vector2 targetPosition);
    void Stop();
}
