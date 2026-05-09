using UnityEngine;

public class DashAction : ActionState
{
    private readonly float dashSpeed;
    private readonly float dashTime;

    private float elapsed;
    private float savedGravity;

    public DashAction(StateMachine sm, Player player, float dashSpeed, float dashTime)
        : base(sm, "Dash", player)
    {
        this.dashSpeed = dashSpeed;
        this.dashTime = dashTime;
    }

    public override void Enter()
    {
        base.Enter();
        player.isDashing = true;
        player.rb.linearVelocity = new Vector2(0, 0);
        player.rb.linearVelocityX = dashSpeed * player.getFacingDirection();
        savedGravity = player.rb.gravityScale;
        player.rb.gravityScale = 0f;
        elapsed = 0f;
    }

    public override void Update()
    {
        base.Update();
        elapsed += Time.deltaTime;
        if (elapsed >= dashTime)
        {
            player.actions.ExitToNone();
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.isDashing = false;
        player.rb.linearVelocity = new Vector2(0, 0);
        player.rb.gravityScale = savedGravity;
    }
}