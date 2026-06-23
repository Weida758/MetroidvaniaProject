using UnityEngine;

public class Locomotion_WallSlideState : LocomotionState
{
    public Locomotion_WallSlideState(StateMachine sm, string anim, Player player) : base(sm, anim, player) { }

    public override void Update()
    {
        base.Update();

        player.rb.linearVelocity = new Vector2(
            player.rb.linearVelocity.x,
            Mathf.Clamp(player.rb.linearVelocity.y, -2f, float.MaxValue));

        var fsm = (PlayerLocomotionFSM)player.locomotion;

        if (player.GetJumpPressedInput())
        {
            player.rb.AddForce(new Vector2(6 * player.getFacingDirection() * -1, 9), ForceMode2D.Impulse);
            player.Flip();
            player.wallJumpTime = 0.75f;
            stateMachine.ChangeState(fsm.fall);
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