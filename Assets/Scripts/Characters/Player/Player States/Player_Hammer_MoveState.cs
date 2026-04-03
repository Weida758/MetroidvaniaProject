using UnityEngine;

public class Player_Hammer_MoveState : Player_MoveState
{
    public Player_Hammer_MoveState(StateMachine stateMachine, string animBoolName, Player player) : 
        base(stateMachine, animBoolName, player)
    { }

    public override void Enter()
    {
        base.Enter();
        //could change speed here
      
    }
    public override void Update()
    {
        base.Update();
         if (player.GetMoveInput() == Vector2.zero)
        {
            stateMachine.ChangeState(player.Hammer_idleState);
        }
        else if(player.rb.linearVelocity.y <0 && player.getGrounded() == false){
            stateMachine.ChangeState(player.Hammer_fallState);
        }
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();   
    }
}
