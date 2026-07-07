using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/Attack")]
public class EnemyAttackData : ScriptableObject
{
    [Header("Decision")]
    [SerializeField] private string attackName = "Attack";
    [SerializeField] private int priority;
    [SerializeField] private float randomWeight = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float triggerChance = 1f;
    [SerializeField] private EnemyDecisionConditions conditions = new EnemyDecisionConditions();

    [Header("Timing")]
    [SerializeField] private float telegraphTime = 0.35f;
    [SerializeField] private float activeTime = 0.2f;
    [SerializeField] private float recoveryTime = 0.45f;
    [SerializeField] private float cooldown = 1.5f;

    [Header("Damage")]
    [SerializeField] private int damage = 1;
    [SerializeField] private bool parryable = true;
    [SerializeField] private bool multiHit;
    [SerializeField] private float hitInterval = 0.12f;

    [Header("Hitbox")]
    [SerializeField] private Vector2 hitboxOffset = new Vector2(1f, 0f);
    [SerializeField] private Vector2 hitboxSize = new Vector2(1.5f, 1f);

    [Header("Self Velocity")]
    [SerializeField] private bool scaleVelocityXByFacing = true;
    [SerializeField] private Vector2 telegraphVelocity;
    [SerializeField] private Vector2 activeVelocity;
    [SerializeField] private Vector2 recoveryVelocity;

    public string AttackName => attackName;
    public int Priority => priority;
    public float RandomWeight => randomWeight;
    public float TriggerChance => triggerChance;
    public EnemyDecisionConditions Conditions => conditions;
    public float TelegraphTime => telegraphTime;
    public float ActiveTime => activeTime;
    public float RecoveryTime => recoveryTime;
    public float Cooldown => cooldown;
    public int Damage => damage;
    public bool Parryable => parryable;
    public bool MultiHit => multiHit;
    public float HitInterval => Mathf.Max(0.01f, hitInterval);
    public Vector2 HitboxOffset => hitboxOffset;
    public Vector2 HitboxSize => hitboxSize;
    public bool ScaleVelocityXByFacing => scaleVelocityXByFacing;
    public Vector2 TelegraphVelocity => telegraphVelocity;
    public Vector2 ActiveVelocity => activeVelocity;
    public Vector2 RecoveryVelocity => recoveryVelocity;
}
