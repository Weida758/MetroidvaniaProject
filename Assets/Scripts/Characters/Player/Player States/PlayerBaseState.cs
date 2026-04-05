using UnityEngine;

public abstract class PlayerBaseState : CharacterBaseState
{

    protected Player player;
    public PlayerBaseState(StateMachine stateMachine, string animBoolName, Player player) : 
        base(stateMachine, animBoolName)
    {
        this.player = player;
    }

    public override void Enter()
    {
        base.Enter();

        player.animator.SetBool(animBoolName, true);
    
    }

    public override void Update()
    {
        base.Update();
        if(player.walljumptime>=0){
            player.walljumptime-= Time.deltaTime;
            Debug.Log(player.walljumptime);
        }
        if(player.dashCooldown>=0){
            player.dashCooldown-= Time.deltaTime;
        }
        if(player.dashTime>=0){
            player.dashTime-= Time.deltaTime;
            if(player.dashTime<=0){
                player.rb.linearVelocity = new Vector2(0, 0);
            }
            //disable hitbox
        }
        
        
        player.animator.SetFloat("xInput", player.inputs.moveInput.x);
    }

    public override void Exit()
    {
        base.Exit();
        
        player.animator.SetBool(animBoolName, false);
    }
    public bool CheckDash(){
        return player.dashCooldown<=0 && !player.isDashing;
    }
}
