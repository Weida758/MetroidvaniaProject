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
    [Header("Decision")]
    [SerializeField] private string movementName = "Movement";
    [SerializeField] private int priority;
    [SerializeField] private float randomWeight = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float triggerChance = 1f;
    [SerializeField] private EnemyDecisionConditions conditions = new EnemyDecisionConditions();

    [Header("Motion")]
    [SerializeField] private EnemyMovementVelocitySpace velocitySpace = EnemyMovementVelocitySpace.TowardTarget;
    [SerializeField] private Vector2 velocity = new Vector2(4f, 0f);
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private bool faceTargetOnStart = true;
    [SerializeField] private bool stopWhenFinished = true;

    public string MovementName => movementName;
    public int Priority => priority;
    public float RandomWeight => randomWeight;
    public float TriggerChance => triggerChance;
    public EnemyDecisionConditions Conditions => conditions;
    public EnemyMovementVelocitySpace VelocitySpace => velocitySpace;
    public Vector2 Velocity => velocity;
    public float Duration => duration;
    public float Cooldown => cooldown;
    public bool FaceTargetOnStart => faceTargetOnStart;
    public bool StopWhenFinished => stopWhenFinished;
}
