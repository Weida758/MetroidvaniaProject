using UnityEngine;

public class Player_Spear_JumpState : Player_JumpState
{
    public Player_Spear_JumpState(StateMachine stateMachine, string animBoolName, Player player) : 
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
        if(player.rb.linearVelocity.y <0 ){
                stateMachine.ChangeState(player.Spear_fallState);
        }
        }
        if(player.GetSpecialAttackPressedInput()&& player.throwCooldown<=0 ){
            base.SpearAim();

        }
        if(player.GetSpecialAttackReleasedInput()&& player.isAiming){
            base.SpearThrow();
        }
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();   
    }
}
