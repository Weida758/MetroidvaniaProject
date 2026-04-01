using UnityEngine;

public class Player_Sword_JumpState : Player_JumpState
{
    public Player_Sword_JumpState(StateMachine stateMachine, string animBoolName, Player player) : 
        base(stateMachine, animBoolName, player)
    { }

    public override void Enter()
    {
        base.Enter();
      
    }
    public override void Update()
    {
        base.Update();
        if(player.rb.linearVelocity.y <0 ){
            stateMachine.ChangeState(player.Sword_fallState);
        }
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();   
    }
}
