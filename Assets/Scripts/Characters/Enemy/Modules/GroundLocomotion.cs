using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class GroundLocomotion : MonoBehaviour, ILocomotion
{
    [Header("Speeds")]
    [SerializeField] private float patrolSpeed = 3f;
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float stopDistance = 1f;

    [Header("Patrol")]
    [SerializeField] private float turnPauseTime = 0.6f;

    [Header("Sensors")]
    [SerializeField] private float groundCheckDistance = 1.75f;
    [SerializeField] private float wallCheckDistance = 1f;

    private Enemy enemy;
    private int groundMask;
    private int wallMask;
    private float patrolDirection = 1f;
    private float pauseTimer;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        groundMask = 1 << LayerMask.NameToLayer("Ground");
        wallMask = 1 << LayerMask.NameToLayer("Wall");
    }

    public void Patrol()
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

    public void Chase(Vector2 targetPosition)
    {
        float direction = Mathf.Sign(targetPosition.x - transform.position.x);

        if (Mathf.Abs(targetPosition.x - transform.position.x) <= stopDistance || !GroundAhead(direction))
        {
            Stop();
            enemy.FaceDirection(direction);
            return;
        }

        enemy.SetVelocity(direction * chaseSpeed, enemy.rb.linearVelocity.y);
        enemy.FaceDirection(direction);
    }

    public void Stop()
    {
        enemy.Stop();
    }

    private bool GroundAhead(float direction)
    {
        Vector2 origin = (Vector2)transform.position + new Vector2(direction, 0f);
        return Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundMask);
    }

    private bool WallAhead(float direction)
    {
        return Physics2D.Raycast(transform.position, new Vector2(direction, 0f), wallCheckDistance, wallMask);
    }
}
