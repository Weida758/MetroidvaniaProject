using UnityEngine;

public class Player_AirState : PlayerBaseState
{
    public Player_AirState(StateMachine stateMachine, string animBoolName, Player player) :
        base(stateMachine, animBoolName, player)
    { }

    public bool WallCheck(){

        return Physics2D.Raycast(player.rb.transform.position, new Vector2(player.getFacingDirection(),0), 0.75f ,1 << LayerMask.NameToLayer("Wall"));

    }
    public override void Enter(){
        base.Enter();
        if(player.speed==12){
            player.speed=7;
        }
    }
    public override void FixedUpdate()
    {
        base.Update();
        if(player.walljumptime<=0&&player.GetMoveInput().x != 0&&player.dashTime <=0){
            player.SetVelocity(player.GetMoveInput().x * player.speed, player.rb.linearVelocity.y);
        }
        
    }
    
    
}
