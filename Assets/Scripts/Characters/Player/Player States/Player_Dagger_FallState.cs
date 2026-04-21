using UnityEngine;
using System.Collections;
public class Player_Dagger_FallState : Player_FallState
{
    public Player_Dagger_FallState(StateMachine stateMachine, string animBoolName, Player player) : 
        base(stateMachine, animBoolName, player)
    { }

    public override void Enter()
    {
        base.Enter();
      
    }
    public override void Update()
    {
        base.Update();

        if(player.getGrounded()  == true && Mathf.Abs(player.inputs.moveInput.x) > 0){
            
            stateMachine.ChangeState(player.Dagger_moveState);
            return;

        }
        else if(player.getGrounded() == true && player.GetMoveInput() == Vector2.zero){
            stateMachine.ChangeState(player.Dagger_idleState);
            return;
        }

        if(player.DoubleJump == true && player.GetJumpPressedInput() == true &&player.HasDoubleJump == true){
            stateMachine.ChangeState(player.Dagger_jumpState);
            player.DoubleJump =false;
            return;

        }
        if(CheckDash() && player.GetShiftPressedInput()){
           player.StartCoroutine(base.Dash());
        }
  
       
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();   
       
    }
}
