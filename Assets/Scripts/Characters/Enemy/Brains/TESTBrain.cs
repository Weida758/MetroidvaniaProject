public class TESTBrain : EnemyBrain
{
    protected override void Build()
    {
        Enemy_PatrolState patrol = new Enemy_PatrolState(enemy);
        Enemy_AttackState attack = new Enemy_AttackState(enemy);

        AddTransition(patrol, attack, () => enemy.perception.CanSeeTarget() && enemy.CanAttack);
        AddTransition(attack, patrol, () => attack.IsFinished);

        SetInitial(patrol);
    }
}
