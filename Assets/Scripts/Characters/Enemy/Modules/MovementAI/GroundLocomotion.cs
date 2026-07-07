using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Enemy))]
public class GroundLocomotion : MonoBehaviour, ILocomotion
{
    [BoxGroup("Speeds")]
    [MinValue(0f)]
    [SerializeField] protected float patrolSpeed = 3f;

    [BoxGroup("Speeds")]
    [MinValue(0f)]
    [SerializeField] protected float combatMoveSpeed = 3f;

    [BoxGroup("Speeds")]
    [MinValue(0f)]
    [SerializeField] protected float stopDistance = 1f;

    [BoxGroup("Patrol")]
    [MinValue(0f), SuffixLabel("s", true)]
    [SerializeField] protected float turnPauseTime = 0.6f;

    [BoxGroup("Combat Facing")]
    [MinValue(0f), SuffixLabel("s", true)]
    [SerializeField] protected float combatTurnDelay = 0.2f;

    [BoxGroup("Combat Facing")]
    [MinValue(0f)]
    [SerializeField] protected float combatTurnDeadZone = 0.2f;

    [BoxGroup("Sensors")]
    [MinValue(0f)]
    [SerializeField] protected float groundCheckDistance = 1.75f;

    [BoxGroup("Sensors")]
    [MinValue(0f)]
    [SerializeField] protected float wallCheckDistance = 1f;

    protected Enemy enemy;
    protected int groundMask;
    protected float patrolDirection = 1f;
    protected float pauseTimer;
    private float combatTurnTimer;
    private float pendingCombatDirection;

    protected void Awake()
    {
        enemy = GetComponent<Enemy>();
        groundMask = 1 << LayerMask.NameToLayer("Ground");
    }

    public virtual void Patrol()
    {
        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.fixedDeltaTime;

            if (pauseTimer <= 0f)
            {
                patrolDirection = -patrolDirection;
                enemy.FaceDirection(patrolDirection);
            }
            else
            {
                enemy.Stop();
            }
            return;
        }

        if (!GroundAhead(patrolDirection) || WallAhead(patrolDirection))
        {
            if (turnPauseTime > 0f)
            {
                pauseTimer = turnPauseTime;
                enemy.Stop();
                return;
            }

            patrolDirection = -patrolDirection;
            enemy.FaceDirection(patrolDirection);
            return;
        }

        enemy.SetVelocity(patrolDirection * patrolSpeed, enemy.rb.linearVelocity.y);
        enemy.FaceDirection(patrolDirection);
    }

    public virtual void InCombatMovement(Vector2 targetPosition)
    {
        float xDelta = targetPosition.x - transform.position.x;
        float direction = DirectionFromDelta(xDelta);

        if (direction == 0f)
        {
            Stop();
            return;
        }

        if (Mathf.Abs(xDelta) <= stopDistance || !GroundAhead(direction))
        {
            Stop();
            FaceCombatDirection(direction);
            return;
        }

        if (!FaceCombatDirection(direction))
        {
            Stop();
            return;
        }

        enemy.SetVelocity(direction * combatMoveSpeed, enemy.rb.linearVelocity.y);
    }

    public virtual void FaceCombatTarget(Vector2 targetPosition)
    {
        float direction = DirectionFromDelta(targetPosition.x - transform.position.x);
        if (direction != 0f)
        {
            FaceCombatDirection(direction);
        }
    }

    public void Stop()
    {
        enemy.Stop();
    }

    protected bool GroundAhead(float direction)
    {
        Vector2 origin = (Vector2)transform.position + new Vector2(direction, 0f);
        return Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundMask);
    }

    protected bool WallAhead(float direction)
    {
        return Physics2D.Raycast(transform.position, new Vector2(direction, 0f), wallCheckDistance, groundMask);
    }

    private float DirectionFromDelta(float xDelta)
    {
        if (Mathf.Abs(xDelta) <= combatTurnDeadZone)
        {
            combatTurnTimer = 0f;
            pendingCombatDirection = 0f;
            return 0f;
        }

        return Mathf.Sign(xDelta);
    }

    private bool FaceCombatDirection(float direction)
    {
        if (direction == enemy.FacingDirection)
        {
            combatTurnTimer = 0f;
            pendingCombatDirection = 0f;
            return true;
        }

        if (combatTurnDelay <= 0f)
        {
            enemy.FaceDirection(direction);
            combatTurnTimer = 0f;
            pendingCombatDirection = 0f;
            return true;
        }

        if (pendingCombatDirection != direction)
        {
            pendingCombatDirection = direction;
            combatTurnTimer = 0f;
        }

        combatTurnTimer += Time.fixedDeltaTime;
        if (combatTurnTimer < combatTurnDelay)
        {
            return false;
        }

        enemy.FaceDirection(direction);
        combatTurnTimer = 0f;
        pendingCombatDirection = 0f;
        return true;
    }
}
