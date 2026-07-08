/// <summary>
/// Base class for enemy behavior states. States own temporary behavior,
/// while EnemyBrain owns transition decisions between states.
/// </summary>
public abstract class EnemyState
{
    protected readonly Enemy enemy;

    protected EnemyState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public virtual void Enter()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void FixedUpdate()
    {
    }

    public virtual void Exit()
    {
    }

    /// <summary>
    /// Shared status gate for states that should pause or finish while the enemy cannot act.
    /// Freeze and stun also stop horizontal movement.
    /// </summary>
    protected bool BlockedByStatus()
    {
        if (enemy.CanAct)
        {
            return false;
        }

        if (enemy.isFreezed || enemy.stunned)
        {
            enemy.Stop();
        }

        return true;
    }
}
