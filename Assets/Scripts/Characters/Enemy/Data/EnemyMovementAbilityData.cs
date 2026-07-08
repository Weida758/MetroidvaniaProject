using UnityEngine;

public enum EnemyMovementVelocitySpace
{
    World,
    FacingDirection,
    TowardTarget,
    AwayFromTarget
}

[CreateAssetMenu(menuName = "Enemies/Movement Ability")]
public class EnemyMovementAbilityData : ScriptableObject
{
    [field: Header("Decision")]
    [field: SerializeField] public string MovementName { get; private set; } = "Movement";
    [field: SerializeField] public int Priority { get; private set; }
    [field: SerializeField] public float RandomWeight { get; private set; } = 1f;
    [field: Range(0f, 1f)]
    [field: SerializeField] public float TriggerChance { get; private set; } = 1f;
    [field: SerializeField] public EnemyDecisionConditions Conditions { get; private set; } = new EnemyDecisionConditions();

    [field: Header("Motion")]
    [field: SerializeField] public EnemyMovementVelocitySpace VelocitySpace { get; private set; } = EnemyMovementVelocitySpace.TowardTarget;
    [field: SerializeField] public Vector2 Velocity { get; private set; } = new Vector2(4f, 0f);
    [field: SerializeField] public float Duration { get; private set; } = 0.3f;
    [field: SerializeField] public float Cooldown { get; private set; } = 1f;
    [field: SerializeField] public bool FaceTargetOnStart { get; private set; } = true;
    [field: SerializeField] public bool StopWhenFinished { get; private set; } = true;
}
