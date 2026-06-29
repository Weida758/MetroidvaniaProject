using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public class Player : MonoBehaviour, IDataPersistence
{
    public PlayerInputs inputs { get; private set; }

    //---------- Debug --------------------
    [DisplayOnly] [SerializeField] private string currentState;

    // -------- Player Components ------------
    public Rigidbody2D rb { get; private set; }
    public Animator animator { get; private set; }
    private SpriteRenderer spriteRenderer;
    public GameObject aim;

    // ------- Player Data -------------
    [field: SerializeField] public float speed;
    [field: SerializeField] public float initialFallForce { get; private set; }

    private int facingDirection = 1;
    private bool isFacingRight = true;

    public bool lockMovement = false;
    public bool lockStateChange = false;

    public GameObject collidedObject;
    
    // Spear
    public Vector2 SpearDistance;
    public Vector2 SpearHit;
    public GameObject SpearEnemy;

    private bool isGrounded = true;
    
    // Special movement values
    [HideInInspector] public float wallJumpTime = 0f;
    [HideInInspector] public float coyoteTime = 0f;
    [HideInInspector] public bool hasDoubleJump = true;
    [DisplayOnly] public bool doubleJump = false;

    [HideInInspector] public bool isDashing = false;
    [HideInInspector] public bool isAiming = false;
    [HideInInspector] public bool isChoosing = false;

    [field: SerializeField] public float lungeTime;
    [DisplayOnly] public float lungeHeldTime;

    public WeaponInventory inventory { get; private set; }
    public PlayerLocomotionFSM locomotion { get; private set; }
    public PlayerActionFSM actions { get; private set; }
    
    // Player Upgrades
    [DisplayOnly] [SerializeField] private float weaponAttackModifierMul = 1;
    [DisplayOnly] [SerializeField] private float weaponAttackModifierAdd = 0;

    private void Awake()
    {
        inputs = GetComponent<PlayerInputs>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        inventory = GetComponent<WeaponInventory>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        aim = transform.Find("Aim").gameObject;

        locomotion = new PlayerLocomotionFSM();
        locomotion.Initialize(this);
        actions = new PlayerActionFSM();
        actions.Initialize(this);
    }

    //For Debugging
    public void OnDrawGizmos()
    {
        if (actions?.currentState is AttackAction attackAction)
        {
            attackAction.DrawGizmos();
        }

        if (actions?.currentState is FreezeAttackAction freezeAction)
        {
            freezeAction.DrawGizmos();
        }
        
        // Draw hitboxes while not in playmode
        if (!Application.isPlaying)
        {
            GetComponent<WeaponInventory>()?.GetPreviewWeapon()?.DrawHitboxPreview(this);
        }
        
    }

    private void Update()
    {
        locomotion.Tick();
        actions.Tick();
        currentState = locomotion.Current.ToString();

        UpdateGrounded();
        if (isGrounded) doubleJump = true;
    }

    private void FixedUpdate()
    {
        locomotion.FixedTick();
        actions.FixedTick();
    }

    private void UpdateGrounded()
    {
        RaycastHit2D ray = Physics2D.Raycast(rb.transform.position, Vector2.down, 1.75f, 1 << LayerMask.NameToLayer("Ground"));

        bool inJump = locomotion.Current is Locomotion_JumpState;
        bool inMoveOrIdle = locomotion.Current is Locomotion_MoveState
                         || locomotion.Current is Locomotion_IdleState;

        if (inJump) { isGrounded = false; }
        else if (coyoteTime > 0f)
        {
            coyoteTime -= Time.deltaTime;
            if (coyoteTime <= 0f) { isGrounded = false; coyoteTime = 0; }
        }
        else if (ray == false && inMoveOrIdle && coyoteTime == 0f) { coyoteTime = 0.5f; }
        else { isGrounded = ray; }
    }

    public Vector2 GetMoveInput() => inputs.moveInput;
    public Vector2 GetMousePosition() => inputs.mousePosition;
    public bool GetJumpPressedInput() => inputs.jumpPressed;
    public bool GetJumpReleasedInput() => inputs.jumpReleased;
    public bool GetDownPressedInput() => inputs.downPressed;
    public bool GetDownCurrentlyPressed() => inputs.downCurrentlyPressed;
    public bool GetShiftPressedInput() => inputs.shiftPressed;
    public bool GetShiftReleasedInput() => inputs.shiftReleased;
    public bool GetShiftCurrentlyPressed() => inputs.shiftCurrentlyPressed;
    public bool GetOnePressedInput() => inputs.onePressed;
    public bool GetTwoPressedInput() => inputs.twoPressed;
    public bool GetThreePressedInput() => inputs.threePressed;
    public bool GetFourPressedInput() => inputs.fourPressed;
    public bool GetAttackPressedInput() => inputs.attackPressed;
    public bool GetSpecialAttackPressedInput() => inputs.specialAttackPressed;
    public bool GetSpecialAttackReleasedInput() => inputs.specialAttackReleased;
    public bool GetUpCurrentlyPressed() => inputs.upCurrentlyPressed;
    public bool GetAbilityPressed() => inputs.magicAttackPressed;

    public bool getGrounded() => isGrounded;
    public int getFacingDirection() => facingDirection;

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        if (xVelocity > 0 && !isFacingRight) Flip();
        else if (xVelocity < 0 && isFacingRight) Flip();
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
        gameData.sceneName = SceneManager.GetActiveScene().name;
    }

    public void LoadData(GameData gameData)
    {
        if (SceneManager.GetActiveScene().name != gameData.sceneName)
        {
            AsyncOperation sceneProgress = SceneManager.LoadSceneAsync(gameData.sceneName);
        }
        transform.position = gameData.playerPositionData;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        collidedObject = collision.gameObject;
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        collidedObject = collision.gameObject;
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        collidedObject = null;
    }
    public float TransformDamage(float damage)
    {
        return (damage * weaponAttackModifierMul) + weaponAttackModifierAdd;
    }
}
