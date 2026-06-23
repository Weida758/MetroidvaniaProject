using System;
using System.Collections.Generic;
using UnityEngine;

public class FreezeAttackAction : ActionState
{
    private readonly AttackStep step;
    
    private float freezeTime;
    private float startupTimer;
    private float activeHitboxTimer;
    private float debugDrawTimer;

    private readonly HashSet<GameObject> hitTargets;

    public FreezeAttackAction(StateMachine stateMachine, Player player, AttackStep step, float freezeTime)
        : base(stateMachine, "Freeze", player)
    {
        this.step = step;
        this.freezeTime = freezeTime;
    }

    public override void Enter()
    {
        base.Enter();
        startupTimer = step.StartupTime;
        activeHitboxTimer = step.ActiveTime;
        
        debugDrawTimer = step.StartupTime + Mathf.Max(step.DebugDrawDuration, step.ActiveTime);

        hitTargets.Clear();

        if (!string.IsNullOrEmpty(step.AnimTrigger))
            player.animator.SetTrigger(step.AnimTrigger);
    }

    public override void Update()
    {
        base.Update();

        if (startupTimer > 0f)
            startupTimer -= Time.deltaTime;
        
        if (startupTimer <= 0f && activeHitboxTimer > 0f)
        {
            CheckAndApplyActiveHitboxes();
            activeHitboxTimer -= Time.deltaTime;
        }

        if (debugDrawTimer > 0f) debugDrawTimer -= Time.deltaTime;
        
        if (startupTimer <= 0f && activeHitboxTimer <= 0f)
            player.actions.ExitToNone();
    }

    private void CheckAndApplyActiveHitboxes()
    {
        int layerMask = step.GetTargetLayerMask();

        foreach (AttackHitbox hitbox in step.Hitboxes)
        {
            foreach (Collider2D hit in hitbox.GetHits(player, layerMask))
            {
                RegisterHit(hit, step);
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
        
        if (!hitTargets.Add(health.gameObject)) return;

        health.TakeDamage((int)player.TransformDamage(step.Damage));
        
        //TODO: Freezing the enemy :(
    }

    public void DrawGizmos()
    {
        if (step == null || !step.DrawDebug || debugDrawTimer <= 0f || startupTimer > 0f) return;

        Gizmos.color = step.DebugColor;
        foreach (AttackHitbox hitbox in step.Hitboxes)
        {
            hitbox.DrawGizmos(player);
        }
    }
}
