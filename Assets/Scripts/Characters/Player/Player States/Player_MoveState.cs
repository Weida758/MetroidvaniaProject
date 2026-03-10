using Unity.VisualScripting;
using UnityEngine;

public class Player_MoveState : Player_GroundedState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Player_MoveState(StateMachine stateMachine, string animBoolName, Player player) : 
        base(stateMachine, animBoolName, player)
    {
    }

    public override void Update()
    {
        base.Update();
        if (player.GetMoveInput() == Vector2.zero)
        {
            stateMachine.ChangeState(player.idleState);
        }
        else if(player.GetJumpPressedInput() == true && player.getGrounded() == true ){
            stateMachine.ChangeState(player.jumpState);
        }
        else if(player.rb.linearVelocity.y <0 && player.getGrounded() == false){
            stateMachine.ChangeState(player.fallState);
        }
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        player.SetVelocity(player.GetMoveInput().x * player.speed, player.rb.linearVelocity.y);

        
    }
}
