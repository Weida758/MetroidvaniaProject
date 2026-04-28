using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

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
        if(player.isAiming){
            
            Vector2 center = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Vector2 mousePosition = player.GetMousePosition();
            Vector2 direction = mousePosition - center;
            if(mousePosition.y > center.y + 30 || mousePosition.x > center.x + 30||mousePosition.y < center.y - 30 || mousePosition.x < center.x - 30 ){
                Vector2 warp = new Vector2(center.x + direction.x/2,center.y + direction.y/2);
                Mouse.current.WarpCursorPosition(warp);
            }
            float angle = (Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg) -90;
            if(angle < 0){
                angle += 360;
            }
            player.aim.transform.eulerAngles = new Vector3(player.aim.transform.eulerAngles.x, player.aim.transform.eulerAngles.y, angle);
            player.aim.SetActive(true);
            //Quaternion targetRotation = Quaternion.Euler(0, 90, 0);
            //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);

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
    public void SpearAim(){
        if(player.GetDownCurrentlyPressed()){
            Vector2 warp = new Vector2(Screen.width / 2f , Screen.height / 2f - 15f );
            Mouse.current.WarpCursorPosition(warp);

        }
        else if(player.GetUpCurrentlyPressed()){
            Vector2 warp = new Vector2(Screen.width / 2f , Screen.height / 2f + 15f );
            Mouse.current.WarpCursorPosition(warp);
        }
        else{
            Vector2 warp = new Vector2(Screen.width / 2f +15f * player.getFacingDirection(), Screen.height / 2f );
            Mouse.current.WarpCursorPosition(warp);
        }
        player.isAiming = true;
        

    }
    public void SpearThrow(){
        player.isAiming = false;
        player.aim.SetActive(false);
        Vector2 center = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 mousePosition = player.GetMousePosition();
        Vector2 direction = mousePosition - center;
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, direction, 10f);
        if(hit == true){
        Debug.Log(hit.collider.gameObject);
        }
            Debug.Log(hit);
           Debug.DrawRay(player.transform.position,direction,Color.red);
            Debug.DrawLine(player.transform.position, hit.point,Color.green);
    }
}
