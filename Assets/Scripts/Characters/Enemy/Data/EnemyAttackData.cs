using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/Attack")]
public class EnemyAttackData : ScriptableObject
{
    [field: Header("Decision")]
    [field: SerializeField] public string AttackName { get; private set; } = "Attack";
    [field: SerializeField] public int Priority { get; private set; }
    [field: SerializeField] public float RandomWeight { get; private set; } = 1f;
    [field: Range(0f, 1f)]
    [field: SerializeField] public float TriggerChance { get; private set; } = 1f;
    [field: SerializeField] public EnemyDecisionConditions Conditions { get; private set; } = new EnemyDecisionConditions();

    [field: Header("Timing")]
    [field: SerializeField] public float TelegraphTime { get; private set; } = 0.35f;
    [field: SerializeField] public float ActiveTime { get; private set; } = 0.2f;
    [field: SerializeField] public float RecoveryTime { get; private set; } = 0.45f;
    [field: SerializeField] public float Cooldown { get; private set; } = 1.5f;

    [field: Header("Damage")]
    [field: SerializeField] public int Damage { get; private set; } = 1;
    [field: SerializeField] public bool Parryable { get; private set; } = true;
    [field: SerializeField] public bool MultiHit { get; private set; }
    [SerializeField] private float hitInterval = 0.12f;

    [field: Header("Hitbox")]
    [field: SerializeField] public Vector2 HitboxOffset { get; private set; } = new Vector2(1f, 0f);
    [field: SerializeField] public Vector2 HitboxSize { get; private set; } = new Vector2(1.5f, 1f);

    [field: Header("Self Velocity")]
    [field: SerializeField] public bool ScaleVelocityXByFacing { get; private set; } = true;
    [field: SerializeField] public Vector2 TelegraphVelocity { get; private set; }
    [field: SerializeField] public Vector2 ActiveVelocity { get; private set; }
    [field: SerializeField] public Vector2 RecoveryVelocity { get; private set; }

    public float HitInterval => Mathf.Max(0.01f, hitInterval);
}
