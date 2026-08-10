using UnityEngine;

public class Locomotion_FallState : LocomotionState
{
    public Locomotion_FallState(StateMachine sm, string anim, Player player) : base(sm, anim, player) { }

    public override void Update()
    {
        base.Update();

        if (player.getGrounded())
        {
            var fsm = (PlayerLocomotionFSM)player.locomotion;
            if (Mathf.Abs(player.GetMoveInput().x) > 0)
            {
                stateMachine.ChangeState(fsm.move);
            }
            else
            {
                stateMachine.ChangeState(fsm.idle);
            }
            return;
        }

        if (WallCheck() && player.wallJumpReattachTime <= 0f && Profile.canWallSlide)
        {
            stateMachine.ChangeState(((PlayerLocomotionFSM)player.locomotion).wall);
            return;
        }

        if (player.GetJumpPressedInput() && player.doubleJump && player.hasDoubleJump)
        {
            player.doubleJump = false;
            stateMachine.ChangeState(((PlayerLocomotionFSM)player.locomotion).jump);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (player.lockMovement || player.wallJumpControlLockTime > 0f || player.isDashing || player.lungeTime > 0) return;
        if (player.GetMoveInput().x != 0)
            player.SetVelocity(player.GetMoveInput().x * player.speed, player.rb.linearVelocity.y);
    }
}
