using System;

public class EnemyTransition
{
    public readonly string Name;
    public readonly EnemyState Target;
    public readonly Func<bool> Condition;

    public EnemyTransition(string name, EnemyState target, Func<bool> condition)
    {
        Name = name;
        Target = target;
        Condition = condition;
    }
}
