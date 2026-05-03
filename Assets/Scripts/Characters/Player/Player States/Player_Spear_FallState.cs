using UnityEngine;

public class Player_Spear_FallState : Player_FallState
{
    public Player_Spear_FallState(StateMachine stateMachine, string animBoolName, Player player) : 
        base(stateMachine, animBoolName, player)
    { }

    public override void Enter()
    {
        base.Enter();
      
    }
    public override void Update()
    {
        base.Update();
        if(!player.lockStateChange){
            if(player.getGrounded()  == true && Mathf.Abs(player.inputs.moveInput.x) > 0){
                
                stateMachine.ChangeState(player.Spear_moveState);
                return;

            }
            else if(player.getGrounded() == true && player.GetMoveInput() == Vector2.zero){
                stateMachine.ChangeState(player.Spear_idleState);
                return;
            }

            if(player.DoubleJump == true && player.GetJumpPressedInput() == true &&player.HasDoubleJump == true){
                stateMachine.ChangeState(player.Spear_jumpState);
                player.DoubleJump =false;
                return;

            }
        }
        if(player.GetSpecialAttackPressedInput() && player.throwCooldown<=0 ){
            base.SpearAim();

        }
        if(player.GetSpecialAttackReleasedInput()&& player.isAiming){
            base.SpearThrow();
        }
        if(player.inputs.magicAttackPressed && player.SpearEnemy!=null){
            base.Lightning();
        }

    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();   
    }
}
