using UnityEngine;

/// <summary>Charge phase for the spear lunge. Tracks lungeHeldTime and forwards Shift release to the weapon.</summary>
public class SpearLungeAction : ActionState
{
    public SpearLungeAction(StateMachine sm, Player player)
        : base(sm, "SpearLunge", player) { }

    public override void Enter()
    {
        base.Enter();
        player.lungeHeldTime = 0.5f;
        player.rb.linearVelocity = new Vector2(0, player.rb.linearVelocity.y);
    }

    public override void Update()
    {
        base.Update();
        player.lungeHeldTime += Time.deltaTime;

        if (player.GetShiftReleasedInput())
        {
            bool consumed = player.inventory.Current?.OnMovementAbilityReleased(player) ?? false;
            if (!consumed)
            {
                player.lungeHeldTime = 0f;
                player.actions.ExitToNone();
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}