using UnityEngine;

public class Player_AirState : PlayerBaseState
{
    public Player_AirState(StateMachine stateMachine, string animBoolName, Player player) :
        base(stateMachine, animBoolName, player)
    { }

   
    public override void Enter(){
        base.Enter();
        if(player.speed==12){
            player.speed=7;
        }
    }
    public override void FixedUpdate()
    {
        base.Update();
        if(player.walljumptime<=0&&player.GetMoveInput().x != 0&&!player.isDashing &&player.lungeTime<=0){
            player.SetVelocity(player.GetMoveInput().x * player.speed, player.rb.linearVelocity.y);
        }
        
    }
    
    
}
