using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Enemy))]
public class BoxPerception : MonoBehaviour, IPerception
{
    [BoxGroup("Patrol Sight")]
    [SerializeField] private Vector2 viewOffset = new Vector2(3f, 0f);

    [BoxGroup("Patrol Sight")]
    [MinValue(0f)]
    [SerializeField] private Vector2 viewSize = new Vector2(6f, 3f);

    [BoxGroup("Combat Sight")]
    [SerializeField] private Vector2 combatViewOffset = new Vector2(4f, 0f);

    [BoxGroup("Combat Sight")]
    [MinValue(0f)]
    [SerializeField] private Vector2 combatViewSize = new Vector2(8f, 3f);

    [BoxGroup("Line Of Sight")]
    [ToggleLeft]
    [SerializeField] private bool requireLineOfSight = true;

    [BoxGroup("Combat Interest")]
    [ToggleLeft]
    [LabelText("Use Interest Radius After Combat Starts")]
    [SerializeField] private bool useInterestRadiusAfterCombat = true;

    [BoxGroup("Combat Interest")]
    [ShowIf(nameof(useInterestRadiusAfterCombat))]
    [MinValue(0f)]
    [SerializeField] private float loseInterestRange = 12f;

    [BoxGroup("Debug")]
    [SerializeField] private Color patrolColor = Color.yellow;

    [BoxGroup("Debug")]
    [SerializeField] private Color combatColor = Color.red;

    [BoxGroup("Debug")]
    [ShowIf(nameof(useInterestRadiusAfterCombat))]
    [SerializeField] private Color interestColor = new Color(1f, 0.5f, 0f);

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

        Vector2 offset = inCombat ? combatViewOffset : viewOffset;
        Vector2 size = inCombat ? combatViewSize : viewSize;

        if (!IsTargetInsideBox(offset, size))
        {
            return false;
        }

        return !requireLineOfSight || HasLineOfSight();
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

    private bool IsTargetInsideBox(Vector2 offset, Vector2 size)
    {
        Vector2 center = GetWorldCenter(offset);
        Vector2 targetPosition = enemy.Target.position;
        Vector2 halfSize = size * 0.5f;
        Vector2 delta = targetPosition - center;

        return Mathf.Abs(delta.x) <= halfSize.x && Mathf.Abs(delta.y) <= halfSize.y;
    }

    private bool HasLineOfSight()
    {
        Vector2 origin = transform.position;
        Vector2 toTarget = (Vector2)enemy.Target.position - origin;
        return !Physics2D.Raycast(origin, toTarget.normalized, toTarget.magnitude, visionBlockerMask);
    }

    private Vector2 GetWorldCenter(Vector2 offset)
    {
        float facing = Application.isPlaying ? enemy.FacingDirection : (transform.localScale.x < 0f ? -1f : 1f);
        return (Vector2)transform.position + new Vector2(offset.x * facing, offset.y);
    }

    private void OnDrawGizmosSelected()
    {
        DrawBox(viewOffset, viewSize, patrolColor);
        DrawBox(combatViewOffset, combatViewSize, combatColor);

        if (useInterestRadiusAfterCombat)
        {
            Gizmos.color = interestColor;
            Gizmos.DrawWireSphere(transform.position, loseInterestRange);
        }
    }

    private void DrawBox(Vector2 offset, Vector2 size, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawWireCube(GetWorldCenter(offset), size);
    }
}
