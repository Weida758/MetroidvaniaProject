using Characters.Player.PlayerWeapons.Data;
using UnityEngine;

/// <summary>Generic attack combo runner. Advances through AttackStep[] on attack input within comboWindow.</summary>
public class AttackAction : ActionState
{
    private readonly AttackStep[] steps;
    private readonly float comboWindow;

    private int comboIndex;
    private float comboTime;

    private RaycastHit2D[] hits;

    public AttackAction(StateMachine sm, Player player, AttackStep[] steps, float comboWindow = 0.5f)
        : base(sm, "Attack", player)
    {
        this.steps = steps;
        this.comboWindow = comboWindow;
    }

    public override void Enter()
    {
        base.Enter();
        comboIndex = 0;
        comboTime = comboWindow;
        FireStep(comboIndex);
    }

    public override void Update()
    {
        base.Update();
        comboTime -= Time.deltaTime;

        if (player.GetAttackPressedInput() && comboTime >= 0f)
        {
            comboIndex = (comboIndex + 1) % steps.Length;
            FireStep(comboIndex);
        }

        if (comboTime <= 0f)
        {
            player.actions.ExitToNone();
        }
    }

    private void FireStep(int i)
    {   //haven't applied dmg
        if (steps == null || i < 0 || i >= steps.Length) return;
        AttackStep s = steps[i];
        hits = Physics2D.CircleCastAll(
            player.rb.position,
            s.radius,
            player.getFacingDirection() * player.transform.right,
            s.range,
            LayerMask.NameToLayer("Enemy"));
        comboTime = comboWindow;
        if (!string.IsNullOrEmpty(s.animTrigger))
            player.animator.SetTrigger(s.animTrigger);
    }
}
