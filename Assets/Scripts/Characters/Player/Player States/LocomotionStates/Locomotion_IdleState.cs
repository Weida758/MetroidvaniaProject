using UnityEngine;

public class Locomotion_IdleState : LocomotionState
{
    public Locomotion_IdleState(StateMachine sm, string anim, Player player) : base(sm, anim, player) { }

    public override void Enter()
    {
        base.Enter();
        if (!player.lockMovement)
            player.rb.linearVelocity = new Vector2(0, player.rb.linearVelocity.y);
    }

    public override void Update()
    {
        base.Update();

        if (Mathf.Abs(player.GetMoveInput().x) > 0)
        {
            stateMachine.ChangeState(((PlayerLocomotionFSM)player.locomotion).move);
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
}