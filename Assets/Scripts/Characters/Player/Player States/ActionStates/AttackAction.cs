using System.Collections.Generic;
using UnityEngine;

/// <summary>Runs a weapon's basic-attack combo and resolves its hitboxes each active frame.</summary>
public class AttackAction : ActionState
{
    private readonly AttackStep[] steps;

    private int comboIndex;
    private float comboTime;

    private float startupTimer;
    private float activeHitboxTimer;
    private float debugDrawTimer;
    private AttackStep activeStep;

    private readonly HashSet<GameObject> hitTargets = new HashSet<GameObject>();


    public AttackAction(StateMachine sm, Player player, AttackStep[] steps)
        : base(sm, "Attack", player)
    {
        this.steps = steps;
    }

    public override void Enter()
    {
        base.Enter();
        comboIndex = 0;
        NextAttack(comboIndex);
    }

    public override void Update()
    {
        base.Update();
        comboTime -= Time.deltaTime;
        
        if (startupTimer > 0f)
            startupTimer -= Time.deltaTime;

        // After the windup, check for hits while the swing is out.
        if (startupTimer <= 0f && activeHitboxTimer > 0f)
        {
            CheckAndApplyActiveHitboxes();
            activeHitboxTimer -= Time.deltaTime;
        }

        if (debugDrawTimer > 0f) debugDrawTimer -= Time.deltaTime;

        if (player.GetAttackPressedInput() && comboTime >= 0f)
        {
            comboIndex = (comboIndex + 1) % steps.Length;
            NextAttack(comboIndex);
        }

        if (comboTime <= 0f)
        {
            player.actions.ExitToNone();
        }
    }

    private void NextAttack(int i)
    {
        activeStep = steps[i];
        startupTimer = activeStep.StartupTime;
        activeHitboxTimer = activeStep.ActiveTime;
        comboTime = activeStep.ComboInputWindow;
        // drawing windup and active time
        debugDrawTimer = activeStep.StartupTime + Mathf.Max(activeStep.DebugDrawDuration, activeStep.ActiveTime);

        // Clear the previously added hit targets
        hitTargets.Clear();

        if (!string.IsNullOrEmpty(activeStep.AnimTrigger))
        {
            player.animator.SetTrigger(activeStep.AnimTrigger);
        }
    }

    private void CheckAndApplyActiveHitboxes()
    {
        int layerMask = activeStep.GetTargetLayerMask();

        foreach (AttackHitbox hitbox in activeStep.Hitboxes)
        {
            foreach (Collider2D hit in hitbox.GetHits(player, layerMask))
            {
                RegisterHit(hit, activeStep);
            }
        }
    }

    private void RegisterHit(Collider2D hit, AttackStep step)
    {
        HealthSystem health = hit.GetComponentInParent<HealthSystem>();
        if (health == null)
        {
            Debug.Log("No HealthSystem in Target");
            return;
        }

        // Check the hashmap
        if (!hitTargets.Add(health.gameObject)) return;

        health.TakeDamage((int)player.TransformDamage(step.Damage));
    }

    public void DrawGizmos()
    {
        if (activeStep == null || !activeStep.DrawDebug || debugDrawTimer <= 0f || startupTimer > 0f) return;
        
        Gizmos.color = activeStep.DebugColor;
        foreach (AttackHitbox hitbox in activeStep.Hitboxes)
        {
            hitbox.DrawGizmos(player);
        }
    }


}
