using UnityEngine;

public class Enemy_AttackState : EnemyState, IParryable
{
    private enum Phase
    {
        Telegraph,
        Active,
        Recovery,
        Done
    }

    private IEnemyAttack attack;
    private Phase phase;
    private float timer;

    public Enemy_AttackState(Enemy enemy) : base(enemy)
    {
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
        enemy.Stop();
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
                if (timer <= 0f)
                {
                    EnterPhase(Phase.Active);
                }
                break;
            case Phase.Active:
                attack.OnActiveFrame(enemy, this);
                if (timer <= 0f)
                {
                    EnterPhase(Phase.Recovery);
                }
                break;
            case Phase.Recovery:
                if (timer <= 0f)
                {
                    Finish();
                }
                break;
        }
    }

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
        else if (next == Phase.Recovery)
        {
            timer = attack.RecoveryTime;
        }

        UpdateTelegraphVisual();
    }

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
