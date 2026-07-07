using System;

public enum EnemyDecisionKind
{
    Attack,
    Ability,
    Movement
}

public class EnemyDecisionCandidate
{
    public readonly EnemyDecisionKind Kind;
    public readonly string Name;
    public readonly int Priority;
    public readonly float Weight;
    public readonly Func<bool> Start;

    public EnemyDecisionCandidate(EnemyDecisionKind kind, string name, int priority, float weight, Func<bool> start)
    {
        Kind = kind;
        Name = name;
        Priority = priority;
        Weight = weight;
        Start = start;
    }
}
