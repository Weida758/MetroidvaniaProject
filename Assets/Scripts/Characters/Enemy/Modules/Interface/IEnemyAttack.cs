public enum EnemyAttackPhase
{
    Telegraph,
    Active,
    Recovery
}

public interface IEnemyAttack
{
    float Range { get; }
    float TelegraphTime { get; }
    float ActiveTime { get; }
    float RecoveryTime { get; }
    float Cooldown { get; }
    bool IsParryable { get; }
    bool UsesAnimationEvents { get; }
    bool HasPendingCompletionVelocity { get; }

    bool TryUseSequence(string sequenceName);
    void OnAttackSequenceStart(Enemy self);
    void OnPhaseEnter(Enemy self, EnemyAttackPhase phase);
    void OnAttackStart(Enemy self);
    void OnActiveFrame(Enemy self, IParryable attackContext);
    void OnPhaseFixedUpdate(Enemy self, EnemyAttackPhase phase);
    bool TryAdvanceStep(Enemy self);
    bool ConsumeActiveStarted();
    bool ConsumeActiveEnded();
    bool ConsumeStepCompleted();
}
