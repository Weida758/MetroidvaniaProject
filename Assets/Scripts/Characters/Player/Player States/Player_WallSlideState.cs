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
        //player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x, Mathf.Clamp(player.rb.linearVelocity.y,-2f,float.MaxValue));
    }

    public override void Update()
    {
        player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x, Mathf.Clamp(player.rb.linearVelocity.y,-2f,float.MaxValue));
        //Debug.Log(player.getGrounded());
        if(player.GetJumpPressedInput() == true){
            player.Flip();
            //player.rb.AddForce(new Vector2(2,2), ForceMode2D.Impulse);
            player.rb.linearVelocity = new Vector2(player.getFacingDirection()*2,2);
            
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


    }
    public override void FixedUpdate()
    {
       base.FixedUpdate();

        
    }
}
