using UnityEngine;

public class Player_Hammer_FallState : Player_FallState
{
    public Player_Hammer_FallState(StateMachine stateMachine, string animBoolName, Player player) : 
        base(stateMachine, animBoolName, player)
    { }

    public override void Enter()
    {
        player.rb.linearVelocity = new Vector2(0, 0);
        player.rb.AddForceY(player.initialFallForce, ForceMode2D.Impulse);
        base.Enter();
      
    }
    public override void Update()
    {
        base.Update();
        if(player.getGrounded()  == true && Mathf.Abs(player.inputs.moveInput.x) > 0){
            
            stateMachine.ChangeState(player.Hammer_moveState);
            return;

        }
        else if(player.getGrounded() == true && player.GetMoveInput() == Vector2.zero){
            stateMachine.ChangeState(player.Hammer_idleState);
            return;
        }

        
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();   
    }
}
