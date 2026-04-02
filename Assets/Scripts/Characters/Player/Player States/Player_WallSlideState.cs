using Unity.VisualScripting;
using UnityEngine;

public class Player_WallSlideState : Player_AirState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Player_WallSlideState(StateMachine stateMachine, string animBoolName, Player player) : 
        base(stateMachine, animBoolName, player)
    {
    }
    public override void Enter(){

    }

    public override void Update()
    {
        player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x, Mathf.Clamp(player.rb.linearVelocity.y,-2f,float.MaxValue));

        if(player.GetJumpPressedInput() == true){
            player.rb.AddForce(new Vector2(6*player.getFacingDirection()*-1,9),ForceMode2D.Impulse);
            player.Flip();
            player.walljumptime = 0.75f;
            stateChange();
  
        }

        if(player.getGrounded()){
            if(player.stateMachine.previousState.GetType() == typeof(Player_Sword_FallState) || player.stateMachine.previousState.GetType() == typeof(Player_Sword_JumpState)){
                stateMachine.ChangeState(player.Sword_idleState);
                return;
            }
            else if(player.stateMachine.previousState.GetType() == typeof(Player_Dagger_FallState) || player.stateMachine.previousState.GetType() == typeof(Player_Dagger_JumpState)){
                stateMachine.ChangeState(player.Dagger_idleState);
                return;
            }
            else if(player.stateMachine.previousState.GetType() == typeof(Player_Spear_FallState) || player.stateMachine.previousState.GetType() == typeof(Player_Spear_JumpState)){
                stateMachine.ChangeState(player.Spear_idleState);
                return;
            }
            else if(player.stateMachine.previousState.GetType() == typeof(Player_Hammer_FallState) ){
                stateMachine.ChangeState(player.Hammer_idleState);
                return;
            }
        }

        if(!WallCheck()){
           stateChange();
        }


    }
    private void stateChange(){
        if(player.stateMachine.previousState.GetType() == typeof(Player_Sword_FallState) || player.stateMachine.previousState.GetType() == typeof(Player_Sword_JumpState)){
            stateMachine.ChangeState(player.Sword_fallState);
            return;
        }
        else if(player.stateMachine.previousState.GetType() == typeof(Player_Dagger_FallState) || player.stateMachine.previousState.GetType() == typeof(Player_Dagger_JumpState)){
            stateMachine.ChangeState(player.Dagger_fallState);
            return;
        }
        else if(player.stateMachine.previousState.GetType() == typeof(Player_Spear_FallState) || player.stateMachine.previousState.GetType() == typeof(Player_Spear_JumpState)){
            stateMachine.ChangeState(player.Spear_fallState);
            return;
        }
        else if(player.stateMachine.previousState.GetType() == typeof(Player_Hammer_FallState) ){
            stateMachine.ChangeState(player.Hammer_fallState);
            return;
        }
    }

    public override void FixedUpdate()
    {

       base.FixedUpdate();
       
    }
}
