using UnityEngine;

public class Player_GroundedState : PlayerBaseState
{
    public Player_GroundedState(StateMachine stateMachine, string animBoolName, Player player) :
        base(stateMachine, animBoolName, player)
    { }
    public override void Enter(){
        base.Enter();
        if(player.speed==12&&player.stateMachine.currentState.GetType()!=typeof(Player_Sword_MoveState)&&player.stateMachine.currentState.GetType()!=typeof(Player_Sword_IdleState)){
            player.speed=7;
        }
    }
    
    
}
