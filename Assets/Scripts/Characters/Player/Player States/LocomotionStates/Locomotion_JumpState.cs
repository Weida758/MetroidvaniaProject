using UnityEngine;

public class Locomotion_JumpState : LocomotionState
{
    public Locomotion_JumpState(StateMachine sm, string anim, Player player) : base(sm, anim, player) { }

    public override void Enter()
    {
        base.Enter();
        player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x, Profile.jumpVelocity);
    }

    public override void Update()
    {
        base.Update();

        if (player.GetJumpReleasedInput())
        {
            player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x, 0);
            player.rb.AddForceY(player.initialFallForce, ForceMode2D.Impulse);
        }

        if (WallCheck() && !player.getGrounded() && player.walljumptime <= 0 && Profile.canWallSlide)
        {
            stateMachine.ChangeState(((PlayerLocomotionFSM)player.locomotion).wall);
            return;
        }

        if (player.rb.linearVelocity.y < 0)
        {
            stateMachine.ChangeState(((PlayerLocomotionFSM)player.locomotion).fall);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (player.lockMovement || player.walljumptime > 0 || player.isDashing || player.lungeTime > 0) return;
        if (player.GetMoveInput().x != 0)
            player.SetVelocity(player.GetMoveInput().x * player.speed, player.rb.linearVelocity.y);
    }
}