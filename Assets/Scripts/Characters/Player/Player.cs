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

    // -------- Player Components ------------
    public Rigidbody2D rb { get; private set; }
    public Animator animator { get; private set; }
    private SpriteRenderer spriteRenderer;
    
    // ------- Player Data -------------
    [field: SerializeField] public float speed { get; private set; }
    private int facingDirection = 1;
    private bool isFacingRight = true;





    private void Awake()
    {
        inputs = GetComponent<PlayerInputs>();
        rb =  GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        stateMachine = new StateMachine();

        idleState = new Player_IdleState(stateMachine, "idle", this);
        moveState = new Player_MoveState(stateMachine, "move", this);
        
        stateMachine.Initialize(idleState);

    }

    private void Update()
    {
        stateMachine.UpdateActiveState();
        currentState = stateMachine.currentState.ToString();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdateActiveState();
    }

    public Vector2 GetMoveInput() => inputs.moveInput;


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
