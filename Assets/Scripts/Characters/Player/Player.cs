using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IDataPersistence
{
    public PlayerInputs inputs { get; private set; }
    public StateMachine stateMachine { get; private set; }
    
    //---------- Debug --------------------
    [DisplayOnly] [SerializeField] private string currentState;
    
    //--------- Player States -------------
    public Player_Sword_IdleState Sword_idleState { get; private set; }
    public Player_Hammer_IdleState Hammer_idleState { get; private set; }
    public Player_Dagger_IdleState Dagger_idleState { get; private set; }
    public Player_Spear_IdleState Spear_idleState { get; private set; }
    
    public Player_Sword_MoveState Sword_moveState { get; private set; }
    public Player_Hammer_MoveState Hammer_moveState { get; private set; }
    public Player_Dagger_MoveState Dagger_moveState { get; private set; }
    public Player_Spear_MoveState Spear_moveState { get; private set; }

    public Player_Sword_JumpState Sword_jumpState { get; private set; }
    public Player_Dagger_JumpState Dagger_jumpState { get; private set; }
    public Player_Spear_JumpState Spear_jumpState { get; private set; }

    public Player_Sword_FallState Sword_fallState { get; private set; }
    public Player_Hammer_FallState Hammer_fallState { get; private set; }
    public Player_Dagger_FallState Dagger_fallState { get; private set; }
    public Player_Spear_FallState Spear_fallState { get; private set; }
    public Player_WallSlideState Wall_slideState { get; private set; }


    // -------- Player Components ------------
    public Rigidbody2D rb { get; private set; }
    public Animator animator { get; private set; }
    private SpriteRenderer spriteRenderer;
    
    // ------- Player Data -------------
    [field: SerializeField] public float speed;
    [field: SerializeField] public float jumpVelocity { get; private set; }
    [field: SerializeField] public float initialFallForce { get; private set; }
    private int facingDirection = 1;
    private bool isFacingRight = true;
    private bool isGrounded = true;
    public float walljumptime = 0f;
    public float coyotetime =0f;
    public bool HasDoubleJump=true;
    [DisplayOnly] public bool DoubleJump=false;
    [field: SerializeField] public bool HasDagger;
    [field: SerializeField] public bool HasSpear;
    [field: SerializeField] public bool HasHammer;
    [field: SerializeField] public bool TapSprint;
    [field: SerializeField] public bool AutoSprint;



    private void Awake()
    {
        inputs = GetComponent<PlayerInputs>();
        rb =  GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        stateMachine = new StateMachine();

        Sword_idleState = new Player_Sword_IdleState(stateMachine, "Sword_idle", this);
        Sword_moveState = new Player_Sword_MoveState(stateMachine, "Sword_move", this);
        Sword_jumpState = new Player_Sword_JumpState(stateMachine, "Sword_jump", this);
        Sword_fallState = new Player_Sword_FallState(stateMachine, "Sword_fall", this);

        Dagger_idleState = new Player_Dagger_IdleState(stateMachine, "Dagger_idle", this);
        Dagger_moveState = new Player_Dagger_MoveState(stateMachine, "Dagger_move", this);
        Dagger_jumpState = new Player_Dagger_JumpState(stateMachine, "Dagger_jump", this);
        Dagger_fallState = new Player_Dagger_FallState(stateMachine, "Dagger_fall", this);

        Spear_idleState = new Player_Spear_IdleState(stateMachine, "Spear_idle", this);
        Spear_moveState = new Player_Spear_MoveState(stateMachine, "Spear_move", this);
        Spear_jumpState = new Player_Spear_JumpState(stateMachine, "Spear_jump", this);
        Spear_fallState = new Player_Spear_FallState(stateMachine, "Spear_fall", this);

        Hammer_idleState = new Player_Hammer_IdleState(stateMachine, "Hammer_idle", this);
        Hammer_moveState = new Player_Hammer_MoveState(stateMachine, "Hammer_move", this);
        Hammer_fallState = new Player_Hammer_FallState(stateMachine, "Hammer_fall", this);

        Wall_slideState = new Player_WallSlideState(stateMachine, "Wall_Slide", this);
        
        stateMachine.Initialize(Sword_idleState);

    }

    private void Update()
    {
        stateMachine.UpdateActiveState();
        currentState = stateMachine.currentState.ToString();
        UpdateGrounded();
        Debug.Log(isGrounded);
        if(isGrounded){
            DoubleJump=true;
        }
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdateActiveState();
    }

    private void UpdateGrounded(){
        RaycastHit2D ray = Physics2D.Raycast(rb.transform.position, Vector2.down, 1.75f ,1 << LayerMask.NameToLayer("Ground"));
        // if(ray == true){
        // Debug.Log(ray.collider.gameObject);
        // }
         //    Debug.Log(ray);
        //    Debug.DrawRay(rb.transform.position, transform.TransformDirection(Vector3.down) *1.5f,Color.red);
        //     Debug.DrawLine(rb.transform.position, ray.point,Color.green);
        //Debug.Log(coyotetime);
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
            coyotetime = 0.5f;
        }
        else{
            isGrounded = ray;
        }
  
    }
    public Vector2 GetMoveInput() => inputs.moveInput;

    public bool GetJumpPressedInput() => inputs.jumpPressed;

    public bool GetJumpReleasedInput() => inputs.jumpReleased;

    public bool GetDownPressedInput() => inputs.downPressed;

    public bool GetShiftPressedInput() => inputs.shiftPressed;
    
    public bool GetShiftReleasedInput() => inputs.shiftReleased;

    public bool GetOnePressedInput() => inputs.onePressed;

    public bool GetTwoPressedInput() => inputs.twoPressed;

    public bool GetThreePressedInput() => inputs.threePressed;

    public bool GetFourPressedInput() => inputs.fourPressed;

    public bool getGrounded() => isGrounded;

    public int getFacingDirection() => facingDirection;





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

    public void SaveData(ref GameData gameData)
    {
        gameData.playerPositionData = transform.position;
    }

    public void LoadData(GameData gameData)
    {
        transform.position = gameData.playerPositionData;
    }



}
