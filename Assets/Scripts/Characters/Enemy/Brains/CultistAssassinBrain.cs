public class CultistAssassinBrain : EnemyBrain
{
    protected override void Build()
    {
        Enemy_PatrolState patrol = new Enemy_PatrolState(enemy);
        Enemy_InCombatState combat = new Enemy_InCombatState(enemy);
        Enemy_AttackState attack = new Enemy_AttackState(enemy);

        AddTransition(patrol, combat, () => enemy.perception.CanSeeTarget(), "Saw target");
        AddTransition(combat, patrol, () => enemy.perception.HasLostTarget(), "Lost target");
        AddTransition(combat, attack, () => enemy.CanAttack, "Attack ready");
        AddTransition(attack, combat, () => attack.IsFinished, "Attack finished");

        SetInitial(patrol);
    }
}
