public interface IEnemyAttack
{
    float Range { get; }
    float TelegraphTime { get; }
    float ActiveTime { get; }
    float RecoveryTime { get; }
    float Cooldown { get; }
    bool IsParryable { get; }

    void OnAttackStart(Enemy self);
    void OnActiveFrame(Enemy self, IParryable attackContext);
}
