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
