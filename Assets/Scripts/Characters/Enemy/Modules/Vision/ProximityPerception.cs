using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Enemy))]
public class ProximityPerception : MonoBehaviour, IPerception
{
    [BoxGroup("Detection")]
    [MinValue(0f)]
    [SerializeField] private float detectionRange = 8f;

    [BoxGroup("Detection")]
    [MinValue(0f)]
    [SerializeField] private float loseInterestRange = 12f;

    [BoxGroup("Detection")]
    [MinValue(0f)]
    [SerializeField] private float combatDetectionRange = 12f;

    private Enemy enemy;
    private bool inCombat;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    public void SetCombatMode(bool inCombat)
    {
        this.inCombat = inCombat;
    }

    public bool CanSeeTarget()
    {
        float range = inCombat ? combatDetectionRange : detectionRange;
        return enemy.HasTarget && enemy.DistanceToTarget <= range;
    }

    public bool HasLostTarget()
    {
        if (!enemy.HasTarget)
        {
            return true;
        }

        if (inCombat)
        {
            return enemy.DistanceToTarget > loseInterestRange;
        }

        return !CanSeeTarget();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, loseInterestRange);
    }
}
