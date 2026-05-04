using UnityEngine;

public class Locomotion_MoveState : LocomotionState
{
    public Locomotion_MoveState(StateMachine sm, string anim, Player player) : base(sm, anim, player) { }

    public override void Update()
    {
        base.Update();

        if (player.GetMoveInput() == Vector2.zero)
        {
            stateMachine.ChangeState(((PlayerLocomotionFSM)player.locomotion).idle);
            return;
        }

        if (player.GetJumpPressedInput() && player.getGrounded() && Profile.canJump)
        {
            stateMachine.ChangeState(((PlayerLocomotionFSM)player.locomotion).jump);
            return;
        }

        if (!player.getGrounded() && player.rb.linearVelocity.y < 0)
        {
            stateMachine.ChangeState(((PlayerLocomotionFSM)player.locomotion).fall);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (player.lockMovement || player.isDashing || player.lungeTime > 0) return;
        player.SetVelocity(player.GetMoveInput().x * Profile.baseSpeed, player.rb.linearVelocity.y);
    }
}