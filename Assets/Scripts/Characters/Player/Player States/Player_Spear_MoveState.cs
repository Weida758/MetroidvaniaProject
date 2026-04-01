using UnityEngine;

public class Player_Spear_MoveState : Player_MoveState
{
    public Player_Spear_MoveState(StateMachine stateMachine, string animBoolName, Player player) : 
        base(stateMachine, animBoolName, player)
    { }

    public override void Enter()
    {
        base.Enter();
      
    }
    public override void Update()
    {
        base.Update();
        if (player.GetMoveInput() == Vector2.zero)
        {
            stateMachine.ChangeState(player.Spear_idleState);
        }
        else if(player.GetJumpPressedInput() == true && player.getGrounded() == true ){
            stateMachine.ChangeState(player.Spear_jumpState);
        }
        else if(player.rb.linearVelocity.y <0 && player.getGrounded() == false){
            stateMachine.ChangeState(player.Spear_fallState);
        }
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();   
    }
}
