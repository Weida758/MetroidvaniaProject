public class CultistAssassinBrain : EnemyBrain
{
    protected override void Build()
    {
        Enemy_PatrolState patrol = new Enemy_PatrolState(enemy);
        Enemy_ChaseState chase = new Enemy_ChaseState(enemy);
        Enemy_AttackState attack = new Enemy_AttackState(enemy);

        AddTransition(patrol, chase, () => enemy.perception.CanSeeTarget());
        AddTransition(chase, patrol, () => enemy.perception.HasLostTarget());
        AddTransition(chase, attack, () => enemy.CanAttack);
        AddTransition(attack, chase, () => attack.IsFinished);

        SetInitial(patrol);
    }
}
