using UnityEngine;

public class Player_Sword_FallState : Player_FallState
{
    public Player_Sword_FallState(StateMachine stateMachine, string animBoolName, Player player) : 
        base(stateMachine, animBoolName, player)
    { }

    public override void Enter()
    {
        base.Enter();
      
    }
    public override void Update()
    {
        base.Update();
        if(player.getGrounded()  == true && Mathf.Abs(player.inputs.moveInput.x) > 0){
            
            stateMachine.ChangeState(player.Sword_moveState);
            return;

        }
        else if(player.getGrounded() == true && player.GetMoveInput() == Vector2.zero){
            stateMachine.ChangeState(player.Sword_idleState);
            return;
        }

        if(player.DoubleJump == true && player.GetJumpPressedInput() == true &&player.HasDoubleJump == true){
            stateMachine.ChangeState(player.Sword_jumpState);
            player.DoubleJump =false;
            return;

        }
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();   
    }
}
