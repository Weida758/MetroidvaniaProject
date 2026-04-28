using UnityEngine;

public class Player_Spear_IdleState : Player_IdleState
{
    public Player_Spear_IdleState(StateMachine stateMachine, string animBoolName, Player player) : 
        base(stateMachine, animBoolName, player)
    { }

    public override void Enter()
    {
        base.Enter();
      
    }
    public override void Update()
    {
        base.Update();
        if (Mathf.Abs(player.inputs.moveInput.x) > 0 && player.lungeHeldTime <= 0.5f)
        {
            stateMachine.ChangeState(player.Spear_moveState);
            return;
        }
        else if(player.GetJumpPressedInput() == true && player.getGrounded() == true ){
            stateMachine.ChangeState(player.Spear_jumpState);
        }
        else if(player.rb.linearVelocity.y <0 && player.getGrounded() == false){
            stateMachine.ChangeState(player.Spear_fallState);
        }
        if(player.getGrounded() && player.GetShiftPressedInput()){
            player.lungeHeldTime=0.5f;
        }
        if(player.getGrounded() && player.GetShiftReleasedInput()){
            if(player.lungeHeldTime>=player.lungeHeldTimeMax){
                player.lungeHeldTime = player.lungeHeldTimeMax;
            }
            int TotalLungeSpeed = (int) player.lungeSpeed + ( (int) (player.lungeHeldTime/0.5f) * (int) player.lungeHeldTimeMultiplier);
            player.rb.AddForce(new Vector2(TotalLungeSpeed * player.getFacingDirection() , TotalLungeSpeed) ,ForceMode2D.Impulse);
            player.lungeTime= 0.75f + (int)(0.2 * (player.lungeHeldTime/0.5f));
            player.initialLungeTime = player.lungeTime;
            player.lungeHeldTime=0;
            player.stateMachine.ChangeState(player.Spear_jumpState);
        }
        if(player.GetAttackPressedInput()){
            stateMachine.ChangeState(player.Spear_attackState);
        }
        if(player.GetSpecialAttackPressedInput()){
            base.SpearAim();

        }
        if(player.GetSpecialAttackReleasedInput()){
            base.SpearThrow();

        }
        

        
    }
}
