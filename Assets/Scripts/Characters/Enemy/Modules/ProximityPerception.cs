using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class ProximityPerception : MonoBehaviour, IPerception
{
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float loseInterestRange = 12f;

    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    public bool CanSeeTarget()
    {
        return enemy.HasTarget && enemy.DistanceToTarget <= detectionRange;
    }

    public bool HasLostTarget()
    {
        return !enemy.HasTarget || enemy.DistanceToTarget > loseInterestRange;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, loseInterestRange);
    }
}
