using Unity.VisualScripting;
using UnityEngine;

public class Player_FallState : Player_AirState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Player_FallState(StateMachine stateMachine, string animBoolName, Player player) : 
        base(stateMachine, animBoolName, player)
    {
    }

    public override void Update()
    {
        //Debug.Log(player.getGrounded());
        base.Update();

        if(player.GetOnePressedInput()&&player.stateMachine.currentState.GetType()!=typeof(Player_Sword_FallState)){
            stateMachine.ChangeState(player.Sword_fallState);
            return;
        }
        else if(player.GetTwoPressedInput()&&player.stateMachine.currentState.GetType()!=typeof(Player_Dagger_FallState)&&player.HasDagger){
            stateMachine.ChangeState(player.Dagger_fallState);
            return;
        }
        else if(player.GetThreePressedInput()&&player.stateMachine.currentState.GetType()!=typeof(Player_Spear_FallState)&&player.HasSpear){
            stateMachine.ChangeState(player.Spear_fallState);
            return;
        }
        else if(player.GetFourPressedInput()&&player.stateMachine.currentState.GetType()!=typeof(Player_Hammer_FallState)&&player.HasHammer){
            stateMachine.ChangeState(player.Hammer_fallState);
            return;
        }
        // Debug.Log("wall");
        // Debug.Log(WallCheck());
        if(WallCheck()&&!player.getGrounded()&&player.walljumptime<=0){
            stateMachine.ChangeState(player.Wall_slideState);
            return;
        }

    }
    public override void FixedUpdate()
    {
       base.FixedUpdate();
        
    }
}
