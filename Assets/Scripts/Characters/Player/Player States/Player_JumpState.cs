using Unity.VisualScripting;
using UnityEngine;

public class Player_JumpState : Player_GroundedState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Player_JumpState(StateMachine stateMachine, string animBoolName, Player player) : 
        base(stateMachine, animBoolName, player)
    {
    }
    public override void Enter()
    {

        //player.animator.SetBool(animBoolName, true);
        player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x, 8);
        
    }

    public override void Update()
    {
        if(player.rb.linearVelocity.y <0 ){
            stateMachine.ChangeState(player.fallState);
        }
        else if(player.GetJumpReleasedInput() == true){
            player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x, -2);
        }
       
    }
    public override void FixedUpdate()
    {
       base.FixedUpdate();
        player.SetVelocity(player.GetMoveInput().x * player.speed, player.rb.linearVelocity.y);

        
    }
}
