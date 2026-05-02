using UnityEngine;

public class Player_IdleState : Player_GroundedState
{
    public Player_IdleState(StateMachine stateMachine, string animBoolName, Player player) : 
        base(stateMachine, animBoolName, player)
    { }

    public override void Enter()
    {
        base.Enter();
        if(player.isDashing||player.lockMovement){
            player.rb.linearVelocity = new Vector2(player.rb.linearVelocityX, player.rb.linearVelocity.y);
        }
        else{
            player.rb.linearVelocity = new Vector2(0, player.rb.linearVelocity.y);
        }
    }
    public override void Update()
    {
        base.Update();
        // Can change to getter and setters
       
            if(player.GetOnePressedInput()&&player.stateMachine.currentState.GetType()!=typeof(Player_Sword_IdleState)){
                stateMachine.ChangeState(player.Sword_idleState);
                return;
            }
            else if(player.GetTwoPressedInput()&&player.stateMachine.currentState.GetType()!=typeof(Player_Dagger_IdleState)&&player.HasDagger){
                stateMachine.ChangeState(player.Dagger_idleState);
                return;
            }
            else if(player.GetThreePressedInput()&&player.stateMachine.currentState.GetType()!=typeof(Player_Spear_IdleState)&&player.HasSpear){
                stateMachine.ChangeState(player.Spear_idleState);
                return;
            }
            else if(player.GetFourPressedInput()&&player.stateMachine.currentState.GetType()!=typeof(Player_Hammer_IdleState)&&player.HasHammer){
                stateMachine.ChangeState(player.Hammer_idleState);
                return;
            }
        



       
    }
}
