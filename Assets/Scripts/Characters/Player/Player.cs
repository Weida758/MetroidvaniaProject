using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public class Player : MonoBehaviour, IDataPersistence
{
    public static Player instance { get; private set; }

    public PlayerInputs inputs { get; private set; }

    // -------- Player Components ------------
    public Rigidbody2D rb { get; private set; }
    public Animator animator { get; private set; }
    public GameObject aim { get; private set; }

    // ------- Movement Tuning -------------
    [field: TitleGroup("Movement Tuning", "Editable values that control how player movement feels.")]
    [field: BoxGroup("Movement Tuning/General")]
    [field: SerializeField, LabelText("Jump Release Fall Force")]
    [field: Tooltip("Impulse applied when jump is released early.")]
    public float initialFallForce { get; private set; }

    private int facingDirection = 1;
    private bool isFacingRight = true;

    [BoxGroup("Movement Tuning/Wall Movement")]
    [InfoBox("Wall jump velocity uses positive magnitudes. The controller automatically launches away from the wall.")]
    [SerializeField, Min(0f), LabelText("Slide Speed")]
    [SuffixLabel("units/s", true)]
    private float wallSlideSpeed = 2f;

    [BoxGroup("Movement Tuning/Wall Movement")]
    [SerializeField, LabelText("Jump Velocity (X / Y)")]
    [Tooltip("Horizontal and vertical launch speed. X is automatically aimed away from the wall.")]
    private Vector2 wallJumpVelocity = new Vector2(6f, 9f);

    [BoxGroup("Movement Tuning/Wall Movement")]
    [SerializeField, Min(0f), LabelText("Control Lock")]
    [SuffixLabel("seconds", true)]
    [Tooltip("How briefly horizontal input is ignored after leaving the wall.")]
    private float wallJumpControlLockDuration = 0.1f;

    [BoxGroup("Movement Tuning/Wall Movement")]
    [SerializeField, Min(0f), LabelText("Reattach Delay")]
    [SuffixLabel("seconds", true)]
    [Tooltip("How long before the player is allowed to enter wall slide again after a wall jump.")]
    private float wallJumpReattachDelay = 0.2f;

    public float WallSlideSpeed => wallSlideSpeed;
    public Vector2 WallJumpVelocity => wallJumpVelocity;
    public float WallJumpControlLockDuration => wallJumpControlLockDuration;
    public float WallJumpReattachDelay => wallJumpReattachDelay;

    // ------- Capabilities -------------
    [TitleGroup("Capabilities", "Features available to this player.")]
    [BoxGroup("Capabilities/Movement")]
    [LabelText("Double Jump")]
    [Tooltip("Allows one additional jump while airborne.")]
    public bool hasDoubleJump = true;

    public WeaponInventory inventory { get; private set; }
    public PlayerLocomotionFSM locomotion { get; private set; }
    public PlayerActionFSM actions { get; private set; }

    // ------- Combat Tuning -------------
    [TitleGroup("Combat Tuning", "Player-wide modifiers applied to weapon damage.")]
    [BoxGroup("Combat Tuning/Damage")]
    [SerializeField, Min(0f), LabelText("Attack Multiplier")]
    private float weaponAttackModifierMul = 1;

    [BoxGroup("Combat Tuning/Damage")]
    [SerializeField, LabelText("Flat Attack Bonus")]
    private float weaponAttackModifierAdd = 0;

    // ------- Working State -------------
    [FoldoutGroup("Runtime Debug")]
    [BoxGroup("Runtime Debug/Locomotion")]
    [ShowInInspector, ReadOnly, LabelText("Locomotion State")]
    private string LocomotionStateDebug => locomotion?.Current?.GetType().Name ?? "Not initialized";

    [FoldoutGroup("Runtime Debug")]
    [BoxGroup("Runtime Debug/Actions")]
    [ShowInInspector, ReadOnly, LabelText("Action State")]
    private string ActionStateDebug => actions?.currentState?.GetType().Name ?? "Not initialized";

    [FoldoutGroup("Runtime Debug")]
    [BoxGroup("Runtime Debug/Locomotion")]
    [ShowInInspector, ReadOnly, LabelText("Velocity")]
    private Vector2 VelocityDebug => rb != null ? rb.linearVelocity : Vector2.zero;

    [FoldoutGroup("Runtime Debug")]
    [BoxGroup("Runtime Debug/Locomotion")]
    [ShowInInspector, ReadOnly, LabelText("Current Move Speed")]
    [SuffixLabel("units/s", true)]
    public float speed { get; set; }

    [FoldoutGroup("Runtime Debug")]
    [BoxGroup("Runtime Debug/Locomotion")]
    [ShowInInspector, ReadOnly, LabelText("Grounded")]
    private bool isGrounded = true;

    [FoldoutGroup("Runtime Debug")]
    [BoxGroup("Runtime Debug/Locomotion")]
    [ShowInInspector, ReadOnly, LabelText("Facing Direction")]
    private int FacingDirectionDebug => facingDirection;

    [FoldoutGroup("Runtime Debug")]
    [BoxGroup("Runtime Debug/Locomotion")]
    [ShowInInspector, ReadOnly, LabelText("Movement Locked")]
    public bool lockMovement { get; set; }

    [FoldoutGroup("Runtime Debug")]
    [BoxGroup("Runtime Debug/Locomotion")]
    [ShowInInspector, ReadOnly, LabelText("Dashing")]
    public bool isDashing { get; set; }

    [FoldoutGroup("Runtime Debug")]
    [BoxGroup("Runtime Debug/Capabilities")]
    [ShowInInspector, ReadOnly, LabelText("Double Jump Available")]
    public bool doubleJump { get; set; }

    [FoldoutGroup("Runtime Debug")]
    [BoxGroup("Runtime Debug/Timers")]
    [ShowInInspector, ReadOnly, LabelText("Wall Jump Control Lock")]
    [SuffixLabel("seconds", true)]
    public float wallJumpControlLockTime { get; set; }

    [FoldoutGroup("Runtime Debug")]
    [BoxGroup("Runtime Debug/Timers")]
    [ShowInInspector, ReadOnly, LabelText("Wall Reattach Lock")]
    [SuffixLabel("seconds", true)]
    public float wallJumpReattachTime { get; set; }

    [FoldoutGroup("Runtime Debug")]
    [BoxGroup("Runtime Debug/Timers")]
    [ShowInInspector, ReadOnly, LabelText("Coyote Time")]
    [SuffixLabel("seconds", true)]
    private float coyoteTime;

    [FoldoutGroup("Runtime Debug")]
    [BoxGroup("Runtime Debug/Timers")]
    [ShowInInspector, ReadOnly, LabelText("Lunge Time")]
    [SuffixLabel("seconds", true)]
    public float lungeTime { get; set; }

    [FoldoutGroup("Runtime Debug")]
    [BoxGroup("Runtime Debug/Timers")]
    [ShowInInspector, ReadOnly, LabelText("Lunge Held Time")]
    [SuffixLabel("seconds", true)]
    public float lungeHeldTime { get; set; }

    [FoldoutGroup("Runtime Debug")]
    [BoxGroup("Runtime Debug/Actions")]
    [ShowInInspector, ReadOnly, LabelText("Spear Target")]
    public GameObject SpearEnemy { get; set; }

    private void Awake()
    {
        instance = this;
        inputs = GetComponent<PlayerInputs>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        inventory = GetComponent<WeaponInventory>();
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

    public float TransformDamage(float damage)
    {
        return (damage * weaponAttackModifierMul) + weaponAttackModifierAdd;
    }
}
