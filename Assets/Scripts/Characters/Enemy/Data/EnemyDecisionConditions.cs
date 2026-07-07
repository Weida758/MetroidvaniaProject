using System;
using UnityEngine;

public enum EnemySightRequirement
{
    Any,
    MustSeeTarget,
    MustNotSeeTarget
}

[Serializable]
public class EnemyDecisionConditions
{
    [SerializeField] private bool requireTarget = true;
    [SerializeField] private bool requireCanAct = true;
    [SerializeField] private EnemySightRequirement sightRequirement = EnemySightRequirement.Any;
    [SerializeField] private bool requireCombat;
    [SerializeField] private bool requireOutOfCombat;

    [Header("Distance")]
    [SerializeField] private bool useMinDistance;
    [SerializeField] private float minDistance;
    [SerializeField] private bool useMaxDistance;
    [SerializeField] private float maxDistance = 5f;

    public bool Passes(Enemy enemy, bool inCombat, bool canSeeTarget)
    {
        if (enemy == null)
        {
            return false;
        }

        if (requireTarget && !enemy.HasTarget)
        {
            return false;
        }

        if (requireCanAct && !enemy.CanAct)
        {
            return false;
        }

        if (requireCombat && !inCombat)
        {
            return false;
        }

        if (requireOutOfCombat && inCombat)
        {
            return false;
        }

        if (sightRequirement == EnemySightRequirement.MustSeeTarget && !canSeeTarget)
        {
            return false;
        }

        if (sightRequirement == EnemySightRequirement.MustNotSeeTarget && canSeeTarget)
        {
            return false;
        }

        float distance = enemy.DistanceToTarget;
        if (useMinDistance && distance < minDistance)
        {
            return false;
        }

        if (useMaxDistance && distance > maxDistance)
        {
            return false;
        }

        return true;
    }
}
