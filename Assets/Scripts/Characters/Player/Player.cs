using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public PlayerInputs inputs { get; private set; }
    public StateMachine stateMachine { get; private set; }
    
    //---------- Debug --------------------
    [DisplayOnly] [SerializeField] private string currentState;
    
    //--------- Player States -------------
    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Player_JumpState jumpState { get; private set; }
    public Player_FallState fallState { get; private set; }


    // -------- Player Components ------------
    public Rigidbody2D rb { get; private set; }
    public Animator animator { get; private set; }
    private SpriteRenderer spriteRenderer;
    
    // ------- Player Data -------------
    [field: SerializeField] public float speed { get; private set; }
    private int facingDirection = 1;
    private bool isFacingRight = true;
    private bool isGrounded = true;
    private float coyotetime =0f;



    private void Awake()
    {
        inputs = GetComponent<PlayerInputs>();
        rb =  GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        stateMachine = new StateMachine();

        idleState = new Player_IdleState(stateMachine, "idle", this);
        moveState = new Player_MoveState(stateMachine, "move", this);
        jumpState = new Player_JumpState(stateMachine, "jump", this);
        fallState = new Player_FallState(stateMachine, "fall", this);
        
        stateMachine.Initialize(idleState);

    }

    private void Update()
    {
        stateMachine.UpdateActiveState();
        currentState = stateMachine.currentState.ToString();
        updateGrounded();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdateActiveState();
    }

    private void updateGrounded(){
        RaycastHit2D ray = Physics2D.Raycast(rb.transform.position, Vector2.down, 1.5f ,1 << LayerMask.NameToLayer("Ground"));
        // if(ray == true){
        // Debug.Log(ray.collider.gameObject);
        // }
         //    Debug.Log(ray);
        //    Debug.DrawRay(rb.transform.position, transform.TransformDirection(Vector3.down) *1.5f,Color.red);
        //     Debug.DrawLine(rb.transform.position, ray.point,Color.green);
        Debug.Log(coyotetime);
        Debug.Log(stateMachine.currentState.ToString());
        if(stateMachine.currentState.ToString() == "Player_JumpState"){
            isGrounded = false;
        }
        else if (coyotetime > 0f){
            coyotetime -= Time.deltaTime;
            if(coyotetime <= 0f){
                isGrounded = false;
                coyotetime =0;
            }
        }
        else if(ray==false && (stateMachine.currentState.ToString() == "Player_MoveState"  || stateMachine.currentState.ToString() =="Player_IdleState") && coyotetime==0f){
            coyotetime = 1f;
        }
        else{
            isGrounded = ray;
        }
  
    }
    public Vector2 GetMoveInput() => inputs.moveInput;

    public bool GetJumpPressedInput() => inputs.jumpPressed;

    public bool GetJumpReleasedInput() => inputs.jumpReleased;

    public bool getGrounded() => isGrounded;


    public void SetVelocity(float xVelocity, float yVelocity)
    {
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        if (xVelocity > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (xVelocity < 0 && isFacingRight)
        {
            Flip();
        }
    }

    public void Flip()
    {
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        facingDirection *= -1;
        isFacingRight = !isFacingRight;
    }



}
