using Unity.VisualScripting;
using UnityEngine;

public class Player_FallState : Player_GroundedState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Player_FallState(StateMachine stateMachine, string animBoolName, Player player) : 
        base(stateMachine, animBoolName, player)
    {
    }

    public override void Update()
    {
        //Debug.Log(player.getGrounded());
        if(player.getGrounded()  == true && Mathf.Abs(player.inputs.moveInput.x) > 0){
            
            stateMachine.ChangeState(player.moveState);
            return;

        }
        else if(player.getGrounded() == true && player.GetMoveInput() == Vector2.zero){
            stateMachine.ChangeState(player.idleState);
            return;
        }

        if(player.DoubleJump == true && player.GetJumpPressedInput() == true &&player.HasDoubleJump == true){
            stateMachine.ChangeState(player.jumpState);
            player.DoubleJump =false;
            return;

        }
       
    }
    public override void FixedUpdate()
    {
       base.FixedUpdate();
        player.SetVelocity(player.GetMoveInput().x * player.speed, player.rb.linearVelocity.y);

        
    }
}
