using UnityEngine;
using System.Collections;

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
        // if(player.dashTime>=0){
        //     player.dashTime-= Time.deltaTime;
        //     if(player.dashTime<=0){
        //         player.rb.linearVelocity = new Vector2(0, 0);
        //         player.rb.gravityScale = 2f;
        //     }
        //     //disable hitbox
        // }
        if(player.lungeHeldTime>=0.5&& player.stateMachine.currentState.GetType()==typeof(Player_Spear_IdleState)){
            player.lungeHeldTime+= Time.deltaTime;
        }else{
            player.lungeHeldTime=0;
            
        }
        if(player.lungeTime>=0){
            player.lungeTime-= Time.deltaTime;
            if(player.getGrounded() && (player.initialLungeTime - player.lungeTime) >= 0.5f || WallCheck()){
                player.lungeTime =0;
                player.rb.linearVelocity = new Vector2(0, -2);
            }
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
    public bool WallCheck(){

        return Physics2D.Raycast(player.rb.transform.position, new Vector2(player.getFacingDirection(),0), 0.75f ,1 << LayerMask.NameToLayer("Wall"));

    }
    public IEnumerator Dash(){
        player.isDashing = true;
        player.rb.linearVelocity = new Vector2(0, 0);
        player.rb.linearVelocityX= player.dashSpeed * player.getFacingDirection();
        //player.rb.AddForce(new Vector2(player.dashSpeed * player.getFacingDirection(),0),ForceMode2D.Impulse);
        float previousGravity= player.rb.gravityScale;
        player.dashCooldown = 1f;
        float dashTime = player.dashTime; 
        player.rb.gravityScale = 0f;

        yield return new WaitForSeconds(dashTime);

        player.isDashing = false;
        player.rb.linearVelocity = new Vector2(0, 0);
        player.rb.gravityScale = previousGravity;

    }
}
