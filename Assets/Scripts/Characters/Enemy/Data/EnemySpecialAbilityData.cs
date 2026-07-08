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
    [field: Header("Decision")]
    [field: SerializeField] public string AbilityName { get; private set; } = "Ability";
    [field: SerializeField] public int Priority { get; private set; }
    [field: SerializeField] public float RandomWeight { get; private set; } = 1f;
    [field: Range(0f, 1f)]
    [field: SerializeField] public float TriggerChance { get; private set; } = 1f;
    [field: SerializeField] public EnemyDecisionConditions Conditions { get; private set; } = new EnemyDecisionConditions();

    [field: Header("Execution")]
    [field: SerializeField] public EnemySpecialAbilityType AbilityType { get; private set; } = EnemySpecialAbilityType.TeleportBehindTarget;
    [field: SerializeField] public float WindupTime { get; private set; } = 0.15f;
    [field: SerializeField] public float RecoveryTime { get; private set; } = 0.25f;
    [field: SerializeField] public float Cooldown { get; private set; } = 3f;

    [field: Header("Teleport")]
    [field: SerializeField] public Vector2 TeleportOffset { get; private set; } = new Vector2(2f, 0f);
    [field: SerializeField] public float RandomOffsetRadius { get; private set; }
    [field: SerializeField] public bool FaceTargetAfterTeleport { get; private set; } = true;
}
