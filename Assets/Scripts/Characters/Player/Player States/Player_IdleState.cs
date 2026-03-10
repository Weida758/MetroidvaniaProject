using UnityEngine;

public class Player_IdleState : Player_GroundedState
{
    public Player_IdleState(StateMachine stateMachine, string animBoolName, Player player) : 
        base(stateMachine, animBoolName, player)
    { }

    public override void Enter()
    {
        base.Enter();
        player.rb.linearVelocity = new Vector2(0, player.rb.linearVelocity.y);
    }
    public override void Update()
    {
        base.Update();
        if (Mathf.Abs(player.inputs.moveInput.x) > 0)
        {
            stateMachine.ChangeState(player.moveState);
            return;
        }
        else if(player.GetJumpPressedInput() == true && player.getGrounded() == true ){
            stateMachine.ChangeState(player.jumpState);
        }
    }
}
