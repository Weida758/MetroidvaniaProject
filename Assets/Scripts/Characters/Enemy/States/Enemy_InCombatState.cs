public class Enemy_InCombatState : EnemyState
{
    public Enemy_InCombatState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        enemy.perception?.SetCombatMode(true);
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
            enemy.locomotion.FaceCombatTarget(enemy.Target.position);
            return;
        }

        enemy.locomotion.InCombatMovement(enemy.Target.position);
    }
}
