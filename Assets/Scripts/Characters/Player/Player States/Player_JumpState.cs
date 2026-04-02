using Unity.VisualScripting;
using UnityEngine;

public class Player_JumpState : Player_AirState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Player_JumpState(StateMachine stateMachine, string animBoolName, Player player) : 
        base(stateMachine, animBoolName, player)
    {
    }
    public override void Enter()
    {

        //player.animator.SetBool(animBoolName, true);
        player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x, player.jumpVelocity);
        
    }

    public override void Update()
    {
        base.Update();
          if(player.GetOnePressedInput()&&player.stateMachine.currentState.GetType()!=typeof(Player_Sword_JumpState)){
            stateMachine.ChangeState(player.Sword_jumpState);
            return;
        }
        else if(player.GetTwoPressedInput()&&player.stateMachine.currentState.GetType()!=typeof(Player_Dagger_JumpState)&&player.HasDagger){
            stateMachine.ChangeState(player.Dagger_jumpState);
            return;
        }
        else if(player.GetThreePressedInput()&&player.stateMachine.currentState.GetType()!=typeof(Player_Spear_JumpState)&&player.HasSpear){
            stateMachine.ChangeState(player.Spear_jumpState);
            return;
        }
        else if(player.GetFourPressedInput()&&player.HasHammer){
            stateMachine.ChangeState(player.Hammer_fallState);
            return;
        }

        if(player.GetJumpReleasedInput())
        {
           player.rb.linearVelocity = new Vector2(player.rb.linearVelocityX, 0);
           player.rb.AddForceY(player.initialFallForce, ForceMode2D.Impulse);
        }
        
        if(WallCheck()&&!player.getGrounded()&& player.walljumptime<=0){
            stateMachine.ChangeState(player.Wall_slideState);
            return;
        }
       
    }
    public override void FixedUpdate()
    {
       base.FixedUpdate();
        
        
    }
}
