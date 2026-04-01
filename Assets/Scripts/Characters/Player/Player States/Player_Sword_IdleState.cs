using UnityEngine;

public class Player_Sword_IdleState : Player_IdleState
{
    public Player_Sword_IdleState(StateMachine stateMachine, string animBoolName, Player player) : 
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
            stateMachine.ChangeState(player.Sword_moveState);
            return;
        }
        else if(player.GetJumpPressedInput() == true && player.getGrounded() == true ){
            stateMachine.ChangeState(player.Sword_jumpState);
        }
        else if(player.rb.linearVelocity.y <0 && player.getGrounded() == false){
            stateMachine.ChangeState(player.Sword_fallState);
        }
    }
}
