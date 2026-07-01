public class Enemy_ChaseState : EnemyState
{
    public Enemy_ChaseState(Enemy enemy) : base(enemy)
    {
    }

    public override void FixedUpdate()
    {
        if (BlockedByStatus())
        {
            return;
        }

        if (!enemy.HasTarget)
        {
            enemy.locomotion.Stop();
            return;
        }

        if (enemy.InAttackRange)
        {
            enemy.locomotion.Stop();
            enemy.FaceDirection(enemy.DirectionToTarget);
            return;
        }

        enemy.locomotion.Chase(enemy.Target.position);
    }
}
