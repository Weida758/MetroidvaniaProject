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
       if(player.GetOnePressedInput()&&player.stateMachine.currentState.GetType()!=typeof(Player_Sword_MoveState)){
            stateMachine.ChangeState(player.Sword_moveState);
            return;
        }
        else if(player.GetTwoPressedInput()&&player.stateMachine.currentState.GetType()!=typeof(Player_Dagger_MoveState)&&player.HasDagger){
            stateMachine.ChangeState(player.Dagger_moveState);
            return;
        }
        else if(player.GetThreePressedInput()&&player.stateMachine.currentState.GetType()!=typeof(Player_Spear_MoveState)&&player.HasSpear){
            stateMachine.ChangeState(player.Spear_moveState);
            return;
        }
        else if(player.GetFourPressedInput()&&player.stateMachine.currentState.GetType()!=typeof(Player_Hammer_MoveState)&&player.HasHammer){
            stateMachine.ChangeState(player.Hammer_moveState);
            return;
        }



    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if(player.dashTime <=0&&player.lungeTime<=0&&player.lungeHeldTime<=0.5f){
        player.SetVelocity(player.GetMoveInput().x * player.speed, player.rb.linearVelocity.y);
        }

        
    }
}
