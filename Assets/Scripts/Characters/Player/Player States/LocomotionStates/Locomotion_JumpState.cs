using UnityEngine;

public class Locomotion_JumpState : LocomotionState
{
    private bool hasQueuedWallJump;
    private Vector2 queuedWallJumpVelocity;

    public Locomotion_JumpState(StateMachine sm, string anim, Player player) : base(sm, anim, player) { }

    public void StartWallJump(Vector2 launchVelocity)
    {
        queuedWallJumpVelocity = launchVelocity;
        hasQueuedWallJump = true;
        stateMachine.ChangeState(this);
    }

    public override void Enter()
    {
        base.Enter();

        if (hasQueuedWallJump)
        {
            player.SetVelocity(queuedWallJumpVelocity.x, queuedWallJumpVelocity.y);
            hasQueuedWallJump = false;
            return;
        }

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

        if (WallCheck() && !player.getGrounded() && player.wallJumpReattachTime <= 0f && Profile.canWallSlide)
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
        if (player.lockMovement || player.wallJumpControlLockTime > 0f || player.isDashing || player.lungeTime > 0) return;
        if (player.GetMoveInput().x != 0)
            player.SetVelocity(player.GetMoveInput().x * player.speed, player.rb.linearVelocity.y);
    }
}
