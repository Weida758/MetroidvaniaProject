using UnityEngine;

public class Player_Spear_IdleState : Player_IdleState
{
    public Player_Spear_IdleState(StateMachine stateMachine, string animBoolName, Player player) : 
        base(stateMachine, animBoolName, player)
    { }

    public override void Enter()
    {
        base.Enter();
      
    }
    public override void Update()
    {
        base.Update();
        if (Mathf.Abs(player.inputs.moveInput.x) > 0)
        {
            stateMachine.ChangeState(player.Spear_moveState);
            return;
        }
        else if(player.GetJumpPressedInput() == true && player.getGrounded() == true ){
            stateMachine.ChangeState(player.Spear_jumpState);
        }
        else if(player.rb.linearVelocity.y <0 && player.getGrounded() == false){
            stateMachine.ChangeState(player.Spear_fallState);
        }
        
    }
}
