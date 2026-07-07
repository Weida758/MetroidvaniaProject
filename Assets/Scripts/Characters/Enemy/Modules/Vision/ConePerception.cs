using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Enemy))]
public class ConePerception : MonoBehaviour, IPerception
{
    [BoxGroup("Patrol Sight")]
    [MinValue(0f)]
    [SerializeField] private float viewRange = 8f;

    [BoxGroup("Patrol Sight")]
    [Range(0f, 360f)]
    [SerializeField] private float viewAngle = 90f;

    [BoxGroup("Combat Sight")]
    [MinValue(0f)]
    [SerializeField] private float combatViewRange = 12f;

    [BoxGroup("Combat Sight")]
    [Range(0f, 360f)]
    [SerializeField] private float combatViewAngle = 140f;

    [BoxGroup("Combat Interest")]
    [ToggleLeft]
    [LabelText("Use Interest Radius After Combat Starts")]
    [SerializeField] private bool useInterestRadiusAfterCombat = true;

    [BoxGroup("Combat Interest")]
    [ShowIf(nameof(useInterestRadiusAfterCombat))]
    [MinValue(0f)]
    [SerializeField] private float loseInterestRange = 12f;

    private Enemy enemy;
    private int visionBlockerMask;
    private bool inCombat;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        visionBlockerMask = LayerMask.GetMask("Ground");
    }

    public void SetCombatMode(bool inCombat)
    {
        this.inCombat = inCombat;
    }

    public bool CanSeeTarget()
    {
        if (!enemy.HasTarget)
        {
            return false;
        }

        Vector2 toTarget = (Vector2)enemy.Target.position - (Vector2)transform.position;

        float range = inCombat ? combatViewRange : viewRange;
        float angle = inCombat ? combatViewAngle : viewAngle;

        if (toTarget.magnitude > range)
        {
            return false;
        }

        Vector2 facing = new Vector2(enemy.FacingDirection, 0f);
        if (Vector2.Angle(facing, toTarget) > angle * 0.5f)
        {
            return false;
        }

        return HasLineOfSight();
    }

    public bool HasLostTarget()
    {
        if (!enemy.HasTarget)
        {
            return true;
        }

        if (inCombat && useInterestRadiusAfterCombat)
        {
            return enemy.DistanceToTarget > loseInterestRange;
        }

        return !CanSeeTarget();
    }

    private bool HasLineOfSight()
    {
        Vector2 origin = transform.position;
        Vector2 toTarget = (Vector2)enemy.Target.position - origin;
        return !Physics2D.Raycast(origin, toTarget.normalized, toTarget.magnitude, visionBlockerMask);
    }

    private void OnDrawGizmosSelected()
    {
        float facingDir = transform.localScale.x < 0f ? -1f : 1f;
        Vector3 forward = new Vector3(facingDir, 0f, 0f);
        Vector3 patrolEdgeA = Quaternion.Euler(0f, 0f, viewAngle * 0.5f) * forward;
        Vector3 patrolEdgeB = Quaternion.Euler(0f, 0f, -viewAngle * 0.5f) * forward;
        Vector3 combatEdgeA = Quaternion.Euler(0f, 0f, combatViewAngle * 0.5f) * forward;
        Vector3 combatEdgeB = Quaternion.Euler(0f, 0f, -combatViewAngle * 0.5f) * forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + patrolEdgeA * viewRange);
        Gizmos.DrawLine(transform.position, transform.position + patrolEdgeB * viewRange);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + combatEdgeA * combatViewRange);
        Gizmos.DrawLine(transform.position, transform.position + combatEdgeB * combatViewRange);

        if (useInterestRadiusAfterCombat)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f);
            Gizmos.DrawWireSphere(transform.position, loseInterestRange);
        }
    }
}
