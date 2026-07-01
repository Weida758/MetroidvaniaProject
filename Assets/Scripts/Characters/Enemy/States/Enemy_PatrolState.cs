public class Enemy_PatrolState : EnemyState
{
    public Enemy_PatrolState(Enemy enemy) : base(enemy)
    {
    }

    public override void FixedUpdate()
    {
        if (BlockedByStatus())
        {
            return;
        }

        enemy.locomotion.Patrol();
    }
}
