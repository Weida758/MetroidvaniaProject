using UnityEngine;

public enum EnemySpecialAbilityType
{
    TeleportToTargetOffset,
    TeleportBehindTarget,
    TeleportToHome
}

[CreateAssetMenu(menuName = "Enemies/Special Ability")]
public class EnemySpecialAbilityData : ScriptableObject
{
    [Header("Decision")]
    [SerializeField] private string abilityName = "Ability";
    [SerializeField] private int priority;
    [SerializeField] private float randomWeight = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float triggerChance = 1f;
    [SerializeField] private EnemyDecisionConditions conditions = new EnemyDecisionConditions();

    [Header("Execution")]
    [SerializeField] private EnemySpecialAbilityType abilityType = EnemySpecialAbilityType.TeleportBehindTarget;
    [SerializeField] private float windupTime = 0.15f;
    [SerializeField] private float recoveryTime = 0.25f;
    [SerializeField] private float cooldown = 3f;

    [Header("Teleport")]
    [SerializeField] private Vector2 teleportOffset = new Vector2(2f, 0f);
    [SerializeField] private float randomOffsetRadius;
    [SerializeField] private bool faceTargetAfterTeleport = true;

    public string AbilityName => abilityName;
    public int Priority => priority;
    public float RandomWeight => randomWeight;
    public float TriggerChance => triggerChance;
    public EnemyDecisionConditions Conditions => conditions;
    public EnemySpecialAbilityType AbilityType => abilityType;
    public float WindupTime => windupTime;
    public float RecoveryTime => recoveryTime;
    public float Cooldown => cooldown;
    public Vector2 TeleportOffset => teleportOffset;
    public float RandomOffsetRadius => randomOffsetRadius;
    public bool FaceTargetAfterTeleport => faceTargetAfterTeleport;
}
