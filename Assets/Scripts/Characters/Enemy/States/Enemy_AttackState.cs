using UnityEngine;

/// <summary>
/// Generic enemy attack state. It runs one configured attack move through telegraph,
/// active, post-active, and recovery phases, letting the attack module provide timing and hitboxes.
/// </summary>
public class Enemy_AttackState : EnemyState, IParryable
{
    private enum Phase
    {
        Telegraph,
        Active,
        PostActive,
        Recovery,
        Done
    }

    private IEnemyAttack attack;
    private readonly string attackMoveName;
    private Phase phase;
    private float timer;

    public Enemy_AttackState(Enemy enemy, string attackMoveName = null) : base(enemy)
    {
        this.attackMoveName = attackMoveName;
    }

    public bool IsFinished
    {
        get { return phase == Phase.Done; }
    }

    public bool IsParryable
    {
        get
        {
            if (attack == null)
            {
                return false;
            }

            return (phase == Phase.Telegraph || phase == Phase.Active) && attack.IsParryable;
        }
    }

    public bool IsAttackActive
    {
        get { return phase == Phase.Active; }
    }

    public override void Enter()
    {
        attack = enemy.attack;
        enemy.perception?.SetCombatMode(true);
        enemy.Stop();

        if (!string.IsNullOrWhiteSpace(attackMoveName) && !attack.TryUseSequence(attackMoveName))
        {
            Debug.LogWarning($"{enemy.name} could not find attack move '{attackMoveName}'", enemy);
        }

        attack.OnAttackSequenceStart(enemy);
        EnterPhase(Phase.Telegraph);
    }

    public override void Exit()
    {
        if (enemy.telegraph != null)
        {
            enemy.telegraph.ResetVisual();
        }
    }

    public override void Update()
    {
        if (phase == Phase.Done)
        {
            return;
        }

        if (!enemy.CanAct)
        {
            Finish();
            return;
        }

        timer -= Time.deltaTime;

        switch (phase)
        {
            case Phase.Telegraph:
                if (attack.UsesAnimationEvents)
                {
                    if (attack.ConsumeActiveStarted())
                    {
                        EnterPhase(Phase.Active);
                    }
                }
                else if (timer <= 0f)
                {
                    EnterPhase(Phase.Active);
                }
                break;
            case Phase.Active:
                attack.OnActiveFrame(enemy, this);
                if (attack.UsesAnimationEvents)
                {
                    if (attack.ConsumeActiveEnded())
                    {
                        EnterPhase(Phase.PostActive);
                    }
                    else if (attack.ConsumeStepCompleted())
                    {
                        AdvanceStepOrRecover();
                    }
                }
                else if (timer <= 0f)
                {
                    AdvanceStepOrRecover();
                }
                break;
            case Phase.PostActive:
                if (attack.UsesAnimationEvents && attack.ConsumeStepCompleted())
                {
                    AdvanceStepOrRecover();
                }
                break;
            case Phase.Recovery:
                if (timer <= 0f && !attack.HasPendingCompletionVelocity)
                {
                    Finish();
                }
                break;
        }
    }

    public override void FixedUpdate()
    {
        if (phase == Phase.Done || attack == null)
        {
            return;
        }

        attack.OnPhaseFixedUpdate(enemy, ToEnemyAttackPhase(phase));
    }

    /// <summary>
    /// Applies phase-specific timers, animation triggers, velocity windows, and telegraph visuals.
    /// </summary>
    private void EnterPhase(Phase next)
    {
        phase = next;

        if (next == Phase.Telegraph)
        {
            timer = attack.TelegraphTime;
        }
        else if (next == Phase.Active)
        {
            timer = attack.ActiveTime;
            attack.OnAttackStart(enemy);
        }
        else if (next == Phase.PostActive)
        {
            timer = 0f;
        }
        else if (next == Phase.Recovery)
        {
            timer = attack.RecoveryTime;
        }

        attack.OnPhaseEnter(enemy, ToEnemyAttackPhase(phase));
        UpdateTelegraphVisual();
    }

    private static EnemyAttackPhase ToEnemyAttackPhase(Phase phase)
    {
        if (phase == Phase.Active)
        {
            return EnemyAttackPhase.Active;
        }

        if (phase == Phase.PostActive)
        {
            return EnemyAttackPhase.Recovery;
        }

        if (phase == Phase.Recovery)
        {
            return EnemyAttackPhase.Recovery;
        }

        return EnemyAttackPhase.Telegraph;
    }

    /// <summary>
    /// Moves to the next step in a multi-step attack, or enters shared recovery after the final step.
    /// </summary>
    private void AdvanceStepOrRecover()
    {
        if (attack.TryAdvanceStep(enemy))
        {
            EnterPhase(Phase.Telegraph);
        }
        else
        {
            EnterPhase(Phase.Recovery);
        }
    }

    /// <summary>
    /// Ends the attack state and starts the attack cooldown used by Enemy.CanAttack.
    /// </summary>
    private void Finish()
    {
        phase = Phase.Done;
        enemy.attackCooldown = attack.Cooldown;
    }

    public void OnParried()
    {
        enemy.isMarked = true;
        EnterPhase(Phase.Recovery);
    }

    private void UpdateTelegraphVisual()
    {
        if (enemy.telegraph == null)
        {
            return;
        }

        if (phase == Phase.Telegraph)
        {
            enemy.telegraph.ShowTelegraph();
        }
        else if (phase == Phase.Active)
        {
            enemy.telegraph.ShowActive();
        }
        else if (phase == Phase.Recovery)
        {
            enemy.telegraph.ShowRecovery();
        }
    }
}
