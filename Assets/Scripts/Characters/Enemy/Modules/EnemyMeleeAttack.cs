using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMeleeAttack : MonoBehaviour, IEnemyAttack
{
    [Header("Timing")]
    [SerializeField] private float telegraphTime = 0.4f;
    [SerializeField] private float activeTime = 0.2f;
    [SerializeField] private float recoveryTime = 0.5f;
    [SerializeField] private float cooldown = 2f;

    [Header("Damage")]
    [SerializeField] private int damage = 1;

    [Header("Hitbox")]
    [SerializeField] private Vector2 hitboxOffset = new Vector2(1f, 0f);
    [SerializeField] private Vector2 hitboxSize = new Vector2(1.5f, 1f);

    [Header("Reach")]
    [SerializeField] private float range = 1.5f;

    [Header("Parry")]
    [SerializeField] private bool parryable = true;

    private Enemy enemy;
    private int playerMask;
    private bool hasHit;

    public float Range => range;
    public float TelegraphTime => telegraphTime;
    public float ActiveTime => activeTime;
    public float RecoveryTime => recoveryTime;
    public float Cooldown => cooldown;
    public bool IsParryable => parryable;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        playerMask = 1 << LayerMask.NameToLayer("Player");
    }

    public void OnAttackStart(Enemy self)
    {
        hasHit = false;
    }

    public void OnActiveFrame(Enemy self, IParryable attackContext)
    {
        if (hasHit)
        {
            return;
        }

        Collider2D[] hits = Physics2D.OverlapBoxAll(HitboxCenter(), hitboxSize, 0f, playerMask);
        foreach (Collider2D hit in hits)
        {
            GuardSystem guard = hit.GetComponentInParent<GuardSystem>();
            if (guard != null && guard.TryGuard(attackContext))
            {
                hasHit = true;
                return;
            }

            HealthSystem health = hit.GetComponentInParent<HealthSystem>();
            if (health == null)
            {
                continue;
            }

            health.TakeDamage(damage);
            hasHit = true;
            return;
        }
    }

    private Vector2 HitboxCenter()
    {
        Vector2 offset = new Vector2(hitboxOffset.x * enemy.FacingDirection, hitboxOffset.y);
        return (Vector2)transform.position + offset;
    }

    private void OnDrawGizmosSelected()
    {
        float facingDir = transform.localScale.x < 0f ? -1f : 1f;
        Vector2 offset = new Vector2(hitboxOffset.x * facingDir, hitboxOffset.y);
        Vector2 center = (Vector2)transform.position + offset;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, hitboxSize);
    }
}
