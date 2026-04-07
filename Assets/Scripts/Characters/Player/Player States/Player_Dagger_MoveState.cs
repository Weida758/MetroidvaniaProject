using UnityEngine;

public class Player_Dagger_MoveState : Player_MoveState
{
    public Player_Dagger_MoveState(StateMachine stateMachine, string animBoolName, Player player) : 
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
            stateMachine.ChangeState(player.Dagger_idleState);
        }
        else if(player.GetJumpPressedInput() == true && player.getGrounded() == true ){
            stateMachine.ChangeState(player.Dagger_jumpState);
        }
        else if(player.rb.linearVelocity.y <0 && player.getGrounded() == false){
            stateMachine.ChangeState(player.Dagger_fallState);
        }
        if(CheckDash()&& player.GetShiftPressedInput()){
            player.rb.AddForce(new Vector2(player.dashSpeed*player.getFacingDirection(),0),ForceMode2D.Impulse);
            player.dashCooldown=1f;
            player.dashTime = 0.15f;
        }
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();   
    }
}
