using System;

public class EnemyTransition
{
    public readonly EnemyState Target;
    public readonly Func<bool> Condition;

    public EnemyTransition(EnemyState target, Func<bool> condition)
    {
        Target = target;
        Condition = condition;
    }
}
