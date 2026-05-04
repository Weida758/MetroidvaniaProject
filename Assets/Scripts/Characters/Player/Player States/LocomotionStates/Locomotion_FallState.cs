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
            if (Mathf.Abs(player.GetMoveInput().x) > 0) stateMachine.ChangeState(fsm.move);
            else                                         stateMachine.ChangeState(fsm.idle);
            return;
        }

        if (WallCheck() && player.walljumptime <= 0 && Profile.canWallSlide)
        {
            stateMachine.ChangeState(((PlayerLocomotionFSM)player.locomotion).wall);
            return;
        }

        if (player.GetJumpPressedInput() && player.DoubleJump && player.HasDoubleJump && Profile.canDoubleJump)
        {
            player.DoubleJump = false;
            stateMachine.ChangeState(((PlayerLocomotionFSM)player.locomotion).jump);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (player.lockMovement || player.walljumptime > 0 || player.isDashing || player.lungeTime > 0) return;
        if (player.GetMoveInput().x != 0)
            player.SetVelocity(player.GetMoveInput().x * Profile.baseSpeed, player.rb.linearVelocity.y);
    }
}