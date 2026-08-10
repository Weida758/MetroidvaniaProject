using UnityEngine;

public class Locomotion_WallSlideState : LocomotionState
{
    public Locomotion_WallSlideState(StateMachine sm, string anim, Player player) : base(sm, anim, player) { }

    public override void Update()
    {
        base.Update();

        player.rb.linearVelocity = new Vector2(
            player.rb.linearVelocity.x,
            Mathf.Clamp(player.rb.linearVelocity.y, -player.WallSlideSpeed, float.MaxValue));

        var fsm = (PlayerLocomotionFSM)player.locomotion;

        if (player.GetJumpPressedInput())
        {
            Vector2 launchVelocity = player.WallJumpVelocity;
            launchVelocity.x = Mathf.Abs(launchVelocity.x) * -player.getFacingDirection();
            launchVelocity.y = Mathf.Abs(launchVelocity.y);

            player.wallJumpControlLockTime = player.WallJumpControlLockDuration;
            player.wallJumpReattachTime = player.WallJumpReattachDelay;
            fsm.jump.StartWallJump(launchVelocity);
            return;
        }

        if (player.getGrounded())
        {
            stateMachine.ChangeState(fsm.idle);
            return;
        }

        if (!WallCheck())
        {
            stateMachine.ChangeState(fsm.fall);
        }
    }
}
