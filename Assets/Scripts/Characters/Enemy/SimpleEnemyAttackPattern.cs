using UnityEngine;

public class SimpleEnemyAttackPattern : MonoBehaviour
{
    private enum AttackState
    {
        Waiting,
        Telegraph,
        Active,
        Recovery
    }
    [Header("Timing")]
    [SerializeField] private float timeBetweenAttacks = 2f;
    [SerializeField] private float telegraphTime = 0.4f;
    [SerializeField] private float activeTime = 0.2f;
    [SerializeField] private float recoveryTime = 0.5f;

    [Header("Hitbox")]
    [SerializeField] private Transform attackOrigin;
    [SerializeField] private Vector2 hitboxOffset = new Vector2(1f, 0f);
    [SerializeField] private Vector2 hitboxSize = new Vector2(1.5f, 1f);
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private int damage = 1;

    [Header("Prototype")]
    [SerializeField] private bool pauseSimpleMovement = true;
    [SerializeField] private bool showDebugHitbox = true;
    [SerializeField] private Color telegraphColor = Color.yellow;
    [SerializeField] private Color activeColor = Color.red;
    [SerializeField] private Color recoveryColor = Color.gray;

    public bool IsParryable => state == AttackState.Telegraph || state == AttackState.Active;
    public bool IsAttackActive => state == AttackState.Active;

    private AttackState state = AttackState.Waiting;
    private float timer;
    private bool hasResolvedHit;
    private Enemy enemy;
    private SimpleMoveScript simpleMove;
    private SpriteRenderer spriteRenderer;
    private Color defaultColor;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        simpleMove = GetComponent<SimpleMoveScript>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null) defaultColor = spriteRenderer.color;
        if (attackOrigin == null) attackOrigin = transform;
    }

    private void OnEnable()
    {
        EnterWaiting();
    }

    private void Update()
    {
        if (enemy != null && enemy.isSpeared)
        {
            EnterWaiting();
            return;
        }

        timer -= Time.deltaTime;

        switch (state)
        {
            case AttackState.Waiting:
                if (timer <= 0f) EnterTelegraph();
                break;
            case AttackState.Telegraph:
                if (timer <= 0f) EnterActive();
                break;
            case AttackState.Active:
                CheckHitbox();
                if (timer <= 0f) EnterRecovery();
                break;
            case AttackState.Recovery:
                if (timer <= 0f) EnterWaiting();
                break;
        }
    }

    private void EnterWaiting()
    {
        state = AttackState.Waiting;
        timer = timeBetweenAttacks;
        hasResolvedHit = false;
        SetSimpleMovementEnabled(true);
        SetSpriteColor(defaultColor);
    }

    private void EnterTelegraph()
    {
        state = AttackState.Telegraph;
        timer = telegraphTime;
        SetSimpleMovementEnabled(false);
        SetSpriteColor(telegraphColor);
    }

    private void EnterActive()
    {
        state = AttackState.Active;
        timer = activeTime;
        hasResolvedHit = false;
        SetSpriteColor(activeColor);
    }

    private void EnterRecovery()
    {
        state = AttackState.Recovery;
        timer = recoveryTime;
        hasResolvedHit = true;
        SetSpriteColor(recoveryColor);
    }

    private void CheckHitbox()
    {
        if (hasResolvedHit) return;

        Collider2D[] hits = Physics2D.OverlapBoxAll(GetHitboxCenter(), hitboxSize, 0f, playerLayer);
        foreach (Collider2D hit in hits)
        {
            GuardSystem guard = hit.GetComponentInParent<GuardSystem>();
            if (guard != null && guard.TryGuard(this))
            {
                hasResolvedHit = true;
                return;
            }

            HealthSystem health = hit.GetComponentInParent<HealthSystem>();
            if (health == null) continue;

            Debug.Log($"{name} hit {health.name}");
            health.TakeDamage(damage);
            hasResolvedHit = true;
            return;
        }
    }

    public void OnParried()
    {
        Debug.Log($"{name} was parried");
        enemy.isMarked = true;
        EnterRecovery();
    }

    private Vector2 GetHitboxCenter()
    {
        float facing = transform.localScale.x < 0f ? -1f : 1f;
        Vector2 offset = new Vector2(hitboxOffset.x * facing, hitboxOffset.y);
        return (Vector2)attackOrigin.position + offset;
    }

    private void SetSimpleMovementEnabled(bool enabled)
    {
        if (pauseSimpleMovement && simpleMove != null)
            simpleMove.enabled = enabled;
    }

    private void SetSpriteColor(Color color)
    {
        if (spriteRenderer != null)
            spriteRenderer.color = color;
    }

    private void OnDrawGizmosSelected()
    {
        if (!showDebugHitbox) return;

        Transform origin = attackOrigin != null ? attackOrigin : transform;
        float facing = transform.localScale.x < 0f ? -1f : 1f;
        Vector2 offset = new Vector2(hitboxOffset.x * facing, hitboxOffset.y);
        Vector2 center = (Vector2)origin.position + offset;

        Gizmos.color = Application.isPlaying && IsAttackActive ? Color.red : Color.yellow;
        Gizmos.DrawWireCube(center, hitboxSize);
    }
}
