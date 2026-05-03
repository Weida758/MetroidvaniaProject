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
        if(player.throwCooldown>0){
            player.throwCooldown -= Time.deltaTime;
        }
        //fix later when weapon change
        System.Type stateType = player.stateMachine.currentState.GetType();
        if(stateType !=typeof(Player_Spear_IdleState) && stateType !=typeof(Player_Spear_JumpState)&& stateType !=typeof(Player_Spear_FallState)&& stateType !=typeof(Player_Spear_MoveState)){
            player.isAiming = false;
            player.aim.SetActive(false);
            Time.timeScale = 1f;
        }
        if(player.isAiming){
            
            Vector2 center = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Vector2 mousePosition = player.GetMousePosition();
            Vector2 direction = mousePosition - center;
            if(mousePosition.y > center.y + 40 || mousePosition.x > center.x + 40||mousePosition.y < center.y - 40 || mousePosition.x < center.x - 40 ){
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
        
        if(player.SpearDistance.magnitude>0f){
            player.SetVelocity(player.SpearDistance.normalized.x * 25f,player.SpearDistance.normalized.y * 25f);
            player.SpearDistance = (Vector2)player.SpearHit - (Vector2)player.transform.position;
            
        }
        //to lazy to make a is grappling bool rn
        if(player.collidedObject == player.SpearEnemy && player.SpearHit != Vector2.zero){
                player.rb.linearVelocity = Vector2.zero;
                player.SpearHit = Vector2.zero;
                player.SpearDistance = Vector2.zero;
                //player.SpearEnemy=null;
                UnityEngine.Object.Destroy(player.Spear);
                player.lockMovement = false;
                player.lockStateChange = false;
                Debug.Log("Spear Done");
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
        Time.timeScale = 0.25f;
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
        Time.timeScale = 1f;
        Vector2 center = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 mousePosition = player.GetMousePosition();
        Vector2 direction = mousePosition - center;
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, direction, 10f, 1 << LayerMask.NameToLayer("Enemy"));
        float angle = (Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg) -90;
        Vector3 rotation = new Vector3(player.aim.transform.eulerAngles.x, player.aim.transform.eulerAngles.y, angle);
        //add cooldown
        player.throwCooldown = 2f;

        if(hit == true){

            player.Spear = Object.Instantiate(player.spear, hit.point, Quaternion.Euler(rotation));
            Debug.Log(hit.collider.gameObject);
            player.lockMovement = true;
            player.SpearHit = hit.point;
            player.SpearEnemy = hit.collider.gameObject;
            player.SpearDistance = (Vector2)player.SpearHit - (Vector2)player.transform.position;
            //player.lockStateChange = true;
        }
           Debug.Log(hit);
           Debug.DrawRay(player.transform.position,direction,Color.red);
           Debug.DrawLine(player.transform.position, hit.point,Color.green);
    }

    public void Lightning(){
        Vector2 Distance = Vector2.zero;
        RaycastHit2D hit = Physics2D.Raycast(player.SpearEnemy.transform.position, Vector2.down, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground"));
        if(hit == true){
            Distance = (Vector2)hit.point - (Vector2)player.SpearEnemy.transform.position;
        }
        int Damage = (int)(((Mathf.Abs(Distance.y)/10 )+ 1f) * 2f);
        // create Enemy lighting cooldown to prevent infinte loops 
        Debug.DrawLine(player.SpearEnemy.transform.position, hit.point,Color.green);
        Debug.Log(Damage);
        Collider2D[] chain = Physics2D.OverlapCircleAll(player.SpearEnemy.transform.position, 2.5f, 1 << LayerMask.NameToLayer("Enemy"));
        GameObject previous = player.SpearEnemy;
        foreach(Collider2D i in chain){
            if(i.gameObject != previous){
                player.SpearEnemy = i.gameObject;
                Debug.Log("hit");
                //DO NOT UNCOMMENT INFINITE RECURSION ONLY UNCOMMENT AFTER ADDING COOLDOWN
                //Lightning();
            }

        }
        //UNCOMMENT AFTER INFINITE RECURSION FIX
        //player.SpearEnemy = null;
        

    }
}
