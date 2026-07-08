using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(HealthSystem))]
public class Enemy : MonoBehaviour
{
    [Header("Identity")]
    public string weight;

    [BoxGroup("Combat")]
    [MinValue(0f)]
    [SerializeField] private float attackFacingDeadZone = 0.15f;

    [BoxGroup("Combat")]
    [ToggleLeft]
    [LabelText("Use Horizontal Attack Distance")]
    [SerializeField] private bool useHorizontalAttackDistance = true;

    [BoxGroup("Combat")]
    [MinValue(0f)]
    [SerializeField] private float attackRangeBuffer = 0.05f;

    [BoxGroup("Combat")]
    [MinValue(0f)]
    [SerializeField] private float attackRangeExitBuffer = 0.4f;

    [BoxGroup("Combat Preview")]
    [ToggleLeft]
    [SerializeField] private bool drawAttackRangeGizmos = true;

    [BoxGroup("Combat Preview")]
    [ShowIf(nameof(drawAttackRangeGizmos))]
    [SerializeField] private Color attackRangeColor = new Color(1f, 0f, 0f, 0.8f);

    [BoxGroup("Combat Preview")]
    [ShowIf(nameof(drawAttackRangeGizmos))]
    [SerializeField] private Color attackHoldRangeColor = new Color(1f, 0.6f, 0f, 0.8f);

    [BoxGroup("Execute Targeting")]
    [SerializeField] private Color executeTargetColor = Color.red;

    [DisplayOnly] public float lightningCooldown;
    [DisplayOnly] public bool isSpeared;
    [DisplayOnly] public bool isFreezed;
    public bool isMarked;
    [DisplayOnly] public bool isTarget;
    [DisplayOnly] public bool stunned = false;
    [DisplayOnly] [SerializeField] private string currentState;

    public Rigidbody2D rb { get; private set; }
    public Animator animator { get; private set; }
    public HealthSystem health { get; private set; }
    public EnemyBrain brain { get; private set; }
    public ILocomotion locomotion { get; private set; }
    public IPerception perception { get; private set; }
    public IEnemyAttack attack { get; private set; }
    public EnemyAttackTelegraph telegraph { get; private set; }

    [DisplayOnly] public float attackCooldown;
    [DisplayOnly] public float contactGrace;

    private Player player;
    private SpriteRenderer spriteRenderer;
    private Color defaultSpriteColor;
    private bool lastIsTarget;
    
    // ---------------------------- Helper Methods -------------------------------

    public Transform Target => player != null ? player.transform : null;
    public bool HasTarget => player != null;
    public float DistanceToTarget => player != null ? Vector2.Distance(player.transform.position, transform.position) : Mathf.Infinity;
    public float HorizontalDistanceToTarget => player != null ? Mathf.Abs(player.transform.position.x - transform.position.x) : Mathf.Infinity;
    public float AttackDistanceToTarget => useHorizontalAttackDistance ? HorizontalDistanceToTarget : DistanceToTarget;
    public float DirectionToTarget => player != null ? Mathf.Sign(player.transform.position.x - transform.position.x) : FacingDirection;

    public int FacingDirection { get; private set; } = 1;
    public bool CanAct => !isFreezed && !stunned && !isSpeared;
    public bool InAttackRange => attack != null && AttackDistanceToTarget <= attack.Range + attackRangeBuffer;
    public bool ShouldHoldAttackPosition => attack != null && AttackDistanceToTarget <= attack.Range + attackRangeExitBuffer;
    public bool IsFacingTarget => HasTarget && IsFacingPosition(player.transform.position);
    public bool CanAttack => InAttackRange && IsFacingTarget && attackCooldown <= 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        health = GetComponent<HealthSystem>();
        locomotion = GetComponent<ILocomotion>();
        perception = GetComponent<IPerception>();
        attack = GetComponent<IEnemyAttack>();
        telegraph = GetComponent<EnemyAttackTelegraph>();
        brain = GetComponent<EnemyBrain>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            defaultSpriteColor = spriteRenderer.color;
        }
    }

    private void OnEnable()
    {
        if (health != null)
        {
            health.OnDeath += HandleDeath;
        }
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.OnDeath -= HandleDeath;
        }
    }

    private void Start()
    {
        player = Player.instance;
        if (brain != null)
        {
            brain.Begin();
        }
        else
        {
            Debug.LogError($"{name} has no EnemyBrain component.", this);
        }
    }

    private void Update()
    {
        if (lightningCooldown > 0f)
        {
            lightningCooldown -= Time.deltaTime;
        }

        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }

        if (contactGrace > 0f)
        {
            contactGrace -= Time.deltaTime;
        }

        UpdateExecuteTargetVisual();

        if (brain != null)
        {
            brain.Tick();
            currentState = brain.DebugStateName;
        }
    }

    private void FixedUpdate()
    {
        if (brain != null)
        {
            brain.FixedTick();
        }
    }

    private void HandleDeath()
    {
        Destroy(gameObject, 2f);
    }

    public void SuppressContactDamage(float seconds)
    {
        if (seconds > contactGrace)
        {
            contactGrace = seconds;
        }
    }

    public void SetVelocity(float x, float y)
    {
        rb.linearVelocity = new Vector2(x, y);
    }

    public void Stop()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    public void FaceDirection(float dir)
    {
        if (dir > 0.01f && FacingDirection < 0)
        {
            Flip();
        }
        else if (dir < -0.01f && FacingDirection > 0)
        {
            Flip();
        }
    }

    private bool IsFacingPosition(Vector2 position)
    {
        float xDelta = position.x - transform.position.x;
        if (Mathf.Abs(xDelta) <= attackFacingDeadZone)
        {
            return true;
        }

        return Mathf.Sign(xDelta) == FacingDirection;
    }

    private void UpdateExecuteTargetVisual()
    {
        if (spriteRenderer == null || lastIsTarget == isTarget)
        {
            return;
        }

        spriteRenderer.color = isTarget ? executeTargetColor : defaultSpriteColor;
        lastIsTarget = isTarget;
    }

    public void Flip()
    {
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        FacingDirection *= -1;
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawAttackRangeGizmos)
        {
            return;
        }

        IEnemyAttack previewAttack = attack ?? GetComponent<IEnemyAttack>();
        if (previewAttack == null)
        {
            return;
        }

        float attackRange = previewAttack.Range + attackRangeBuffer;
        float holdRange = previewAttack.Range + attackRangeExitBuffer;

        if (useHorizontalAttackDistance)
        {
            DrawHorizontalRangeGizmo(attackRange, attackRangeColor);
            DrawHorizontalRangeGizmo(holdRange, attackHoldRangeColor);
            return;
        }

        Gizmos.color = attackRangeColor;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = attackHoldRangeColor;
        Gizmos.DrawWireSphere(transform.position, holdRange);
    }

    private void DrawHorizontalRangeGizmo(float range, Color color)
    {
        Vector3 center = transform.position;
        Vector3 left = center + Vector3.left * range;
        Vector3 right = center + Vector3.right * range;
        float capHeight = 0.75f;

        Gizmos.color = color;
        Gizmos.DrawLine(left, right);
        Gizmos.DrawLine(left + Vector3.down * capHeight * 0.5f, left + Vector3.up * capHeight * 0.5f);
        Gizmos.DrawLine(right + Vector3.down * capHeight * 0.5f, right + Vector3.up * capHeight * 0.5f);
    }

    public IEnumerator Stun(float time)
    {
        stunned = true;
        yield return new WaitForSeconds(time);
        stunned = false;
    }

    public IEnumerator Freeze(float time)
    {
        isFreezed = true;
        yield return new WaitForSeconds(time);
        isFreezed = false;
    }
}
