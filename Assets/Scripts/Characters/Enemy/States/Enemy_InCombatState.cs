/// <summary>
/// Combat movement state. The enemy approaches until it reaches attack range,
/// then holds position until it leaves the wider hold range.
/// </summary>
public class Enemy_InCombatState : EnemyState
{
    private bool holdingAttackPosition;

    public Enemy_InCombatState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        enemy.perception?.SetCombatMode(true);
        holdingAttackPosition = false;
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
            holdingAttackPosition = true;
        }
        else if (holdingAttackPosition && !enemy.ShouldHoldAttackPosition)
        {
            holdingAttackPosition = false;
        }

        if (holdingAttackPosition)
        {
            enemy.locomotion.Stop();
            enemy.locomotion.FaceCombatTarget(enemy.Target.position);
            return;
        }

        enemy.locomotion.InCombatMovement(enemy.Target.position);
    }
}
