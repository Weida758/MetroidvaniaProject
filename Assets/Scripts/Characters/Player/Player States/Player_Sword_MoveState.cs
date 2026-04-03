using UnityEngine;

public class Player_Sword_MoveState : Player_MoveState
{
    public Player_Sword_MoveState(StateMachine stateMachine, string animBoolName, Player player) : 
        base(stateMachine, animBoolName, player)
    { }

    public override void Enter()
    {
        base.Enter();
        if(player.AutoSprint){
            player.speed=12;
        }
      
    }
    public override void Update()
    {
        base.Update();
         if (player.GetMoveInput() == Vector2.zero)
        {
            stateMachine.ChangeState(player.Sword_idleState);
        }
        else if(player.GetJumpPressedInput() == true && player.getGrounded() == true ){
            stateMachine.ChangeState(player.Sword_jumpState);
        }
        else if(player.rb.linearVelocity.y <0 && player.getGrounded() == false){
            stateMachine.ChangeState(player.Sword_fallState);
        }
        if(!player.TapSprint){
            if(player.GetShiftPressedInput()){
                player.speed=12;
            }
            else if(player.GetShiftReleasedInput()){
                player.speed=7;

            }
        }
        if(player.TapSprint){
            if(player.GetShiftPressedInput()&&player.speed == 7){
                player.speed=12;
            }
            else if(player.GetShiftPressedInput()&&player.speed == 12){
                player.speed=7;
            }
        }
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();   
    }
}
