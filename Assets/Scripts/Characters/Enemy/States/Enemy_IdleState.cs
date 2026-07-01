public class Enemy_IdleState : EnemyState
{
    public Enemy_IdleState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        enemy.Stop();
    }
}
