using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class ConePerception : MonoBehaviour, IPerception
{
    [SerializeField] private float viewRange = 8f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private float loseInterestRange = 12f;
    [SerializeField] private float loseInterestDelay = 3f;

    private Enemy enemy;
    private int visionBlockerMask;
    private float lostTrackTimer;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        visionBlockerMask = LayerMask.GetMask("Ground", "Wall");
    }

    private void Update()
    {
        if (CanStillTrack())
        {
            lostTrackTimer = 0f;
        }
        else
        {
            lostTrackTimer += Time.deltaTime;
        }
    }

    public bool CanSeeTarget()
    {
        if (!enemy.HasTarget)
        {
            return false;
        }

        Vector2 toTarget = (Vector2)enemy.Target.position - (Vector2)transform.position;

        if (toTarget.magnitude > viewRange)
        {
            return false;
        }

        Vector2 facing = new Vector2(enemy.FacingDirection, 0f);
        if (Vector2.Angle(facing, toTarget) > viewAngle * 0.5f)
        {
            return false;
        }

        return HasLineOfSight();
    }

    public bool HasLostTarget()
    {
        return lostTrackTimer >= loseInterestDelay;
    }

    private bool CanStillTrack()
    {
        if (!enemy.HasTarget)
        {
            return false;
        }

        if (enemy.DistanceToTarget > loseInterestRange)
        {
            return false;
        }

        return HasLineOfSight();
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
        Vector3 edgeA = Quaternion.Euler(0f, 0f, viewAngle * 0.5f) * forward;
        Vector3 edgeB = Quaternion.Euler(0f, 0f, -viewAngle * 0.5f) * forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + edgeA * viewRange);
        Gizmos.DrawLine(transform.position, transform.position + edgeB * viewRange);

        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, loseInterestRange);
    }
}
