using Sirenix.OdinInspector;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAnimationDriver : MonoBehaviour
{
    [BoxGroup("Animation Parameters")]
    [ToggleLeft]
    [LabelText("Drive Speed")]
    [SerializeField] private bool driveSpeedParameter = true;

    [BoxGroup("Animation Parameters")]
    [ShowIf(nameof(driveSpeedParameter))]
    [LabelText("Speed Parameter")]
    [SerializeField] private string speedParameter = "Speed";

    [BoxGroup("Animation Parameters")]
    [ToggleLeft]
    [LabelText("Drive Y Velocity")]
    [SerializeField] private bool driveYVelocityParameter = true;

    [BoxGroup("Animation Parameters")]
    [ShowIf(nameof(driveYVelocityParameter))]
    [LabelText("Y Velocity Parameter")]
    [SerializeField] private string yVelocityParameter = "YVelocity";

    [BoxGroup("Animation Parameters")]
    [ToggleLeft]
    [LabelText("Drive Grounded")]
    [SerializeField] private bool driveGroundedParameter = true;

    [BoxGroup("Animation Parameters")]
    [ShowIf(nameof(driveGroundedParameter))]
    [LabelText("Grounded Parameter")]
    [SerializeField] private string groundedParameter = "Grounded";

    [BoxGroup("Animation Parameters")]
    [ShowIf(nameof(driveSpeedParameter))]
    [MinValue(0.01f)]
    [LabelText("Full Speed Velocity")]
    [SerializeField] private float fullRunVelocity = 3f;

    [BoxGroup("Animation Parameters")]
    [ShowIf(nameof(driveSpeedParameter))]
    [MinValue(0f)]
    [SuffixLabel("s", true)]
    [LabelText("Speed Damp Time")]
    [SerializeField] private float speedDampTime = 0.08f;

    [BoxGroup("Ground Detection")]
    [ShowIf(nameof(driveGroundedParameter))]
    [SerializeField] private LayerMask groundMask;

    [BoxGroup("Ground Detection")]
    [ShowIf(nameof(driveGroundedParameter))]
    [SerializeField] private Transform groundCheckPoint;

    [BoxGroup("Ground Detection")]
    [ShowIf(nameof(driveGroundedParameter))]
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0f, -0.85f);

    [BoxGroup("Ground Detection")]
    [ShowIf(nameof(driveGroundedParameter))]
    [MinValue(0.01f)]
    [SerializeField] private float groundCheckRadius = 0.16f;

    private Animator animator;
    private Rigidbody2D rb;
    private int speedHash;
    private int yVelocityHash;
    private int groundedHash;

    [ShowInInspector, ReadOnly, BoxGroup("Debug")]
    [ShowIf(nameof(driveSpeedParameter))]
    private float CurrentSpeed { get; set; }

    [ShowInInspector, ReadOnly, BoxGroup("Debug")]
    [ShowIf(nameof(driveYVelocityParameter))]
    private float CurrentYVelocity { get; set; }

    [ShowInInspector, ReadOnly, BoxGroup("Debug")]
    [ShowIf(nameof(driveGroundedParameter))]
    private bool IsGrounded { get; set; }

    private void Reset()
    {
        groundMask = LayerMask.GetMask("Ground");
    }

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (driveSpeedParameter)
        {
            speedHash = Animator.StringToHash(speedParameter);
        }

        if (driveYVelocityParameter)
        {
            yVelocityHash = Animator.StringToHash(yVelocityParameter);
        }

        if (driveGroundedParameter)
        {
            groundedHash = Animator.StringToHash(groundedParameter);
        }

        if (groundMask.value == 0)
        {
            groundMask = LayerMask.GetMask("Ground");
        }
    }

    private void Update()
    {
        if (animator == null || rb == null)
        {
            return;
        }

        Vector2 velocity = rb.linearVelocity;
        if (driveSpeedParameter && !string.IsNullOrWhiteSpace(speedParameter))
        {
            CurrentSpeed = Mathf.Clamp01(Mathf.Abs(velocity.x) / fullRunVelocity);
            animator.SetFloat(speedHash, CurrentSpeed, speedDampTime, Time.deltaTime);
        }

        if (driveYVelocityParameter && !string.IsNullOrWhiteSpace(yVelocityParameter))
        {
            CurrentYVelocity = velocity.y;
            animator.SetFloat(yVelocityHash, CurrentYVelocity);
        }

        if (driveGroundedParameter && !string.IsNullOrWhiteSpace(groundedParameter))
        {
            IsGrounded = CheckGrounded();
            animator.SetBool(groundedHash, IsGrounded);
        }
    }

    private bool CheckGrounded()
    {
        Vector2 origin = groundCheckPoint != null
            ? groundCheckPoint.position
            : (Vector2)transform.position + groundCheckOffset;

        return Physics2D.OverlapCircle(origin, groundCheckRadius, groundMask);
    }

    private void OnDrawGizmosSelected()
    {
        if (!driveGroundedParameter)
        {
            return;
        }

        Vector2 origin = groundCheckPoint != null
            ? groundCheckPoint.position
            : (Vector2)transform.position + groundCheckOffset;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, groundCheckRadius);
    }
}
