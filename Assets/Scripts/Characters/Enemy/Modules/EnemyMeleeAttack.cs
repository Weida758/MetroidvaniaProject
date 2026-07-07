using System;
using UnityEngine;
using Sirenix.OdinInspector;

[Serializable]
public class EnemyMeleeAttackStep
{
    [BoxGroup("Identity")]
    [SerializeField] private string stepName = "Swing";

    [BoxGroup("Timing")]
    [MinValue(0f), SuffixLabel("s", true)]
    [SerializeField] private float telegraphTime = 0.4f;

    [BoxGroup("Timing")]
    [MinValue(0f), SuffixLabel("s", true)]
    [SerializeField] private float activeTime = 0.2f;

    [BoxGroup("Damage")]
    [MinValue(0)]
    [SerializeField] private int damage = 1;

    [BoxGroup("Hitboxes")]
    [ListDrawerSettings(Expanded = true, DraggableItems = true, ShowFoldout = true)]
    [SerializeField] private AttackHitbox[] hitboxes = { new AttackHitbox() };

    [BoxGroup("Telegraph Velocity")]
    [ToggleLeft]
    [LabelText("Apply Telegraph Velocity")]
    [SerializeField] private bool applyTelegraphVelocity;

    [BoxGroup("Telegraph Velocity")]
    [ShowIf(nameof(applyTelegraphVelocity))]
    [LabelText("Velocity")]
    [SerializeField] private Vector2 telegraphVelocity;

    [BoxGroup("Active Velocity")]
    [ToggleLeft]
    [LabelText("Apply Active Velocity")]
    [SerializeField] private bool applyActiveVelocity;

    [BoxGroup("Active Velocity")]
    [ShowIf(nameof(applyActiveVelocity))]
    [LabelText("Velocity")]
    [SerializeField] private Vector2 activeVelocity;

    public string StepName => stepName;
    public float TelegraphTime => telegraphTime;
    public float ActiveTime => activeTime;
    public int Damage => damage;
    public AttackHitbox[] Hitboxes => hitboxes;
    public bool ApplyTelegraphVelocity => applyTelegraphVelocity;
    public Vector2 TelegraphVelocity => telegraphVelocity;
    public bool ApplyActiveVelocity => applyActiveVelocity;
    public Vector2 ActiveVelocity => activeVelocity;
}

[RequireComponent(typeof(Enemy))]
public class EnemyMeleeAttack : MonoBehaviour, IEnemyAttack
{
    [BoxGroup("Sequence")]
    [ListDrawerSettings(Expanded = true, DraggableItems = true, ShowFoldout = true, ListElementLabelName = nameof(EnemyMeleeAttackStep.StepName))]
    [SerializeField] private EnemyMeleeAttackStep[] steps = { new EnemyMeleeAttackStep() };

    [BoxGroup("Sequence Recovery")]
    [MinValue(0f), SuffixLabel("s", true)]
    [SerializeField] private float recoveryTime = 0.5f;

    [BoxGroup("Sequence Recovery")]
    [MinValue(0f), SuffixLabel("s", true)]
    [SerializeField] private float cooldown = 2f;

    [BoxGroup("Targeting")]
    [InfoBox("Defaults to the Player layer when this is left empty.", InfoMessageType.None)]
    [SerializeField] private LayerMask targetLayers;

    [BoxGroup("Self Velocity")]
    [ToggleLeft]
    [LabelText("Scale X By Facing Direction")]
    [SerializeField] private bool scaleVelocityXByFacing = true;

    [BoxGroup("Self Velocity")]
    [ToggleLeft]
    [LabelText("Preserve Current Y When Velocity Y Is 0")]
    [SerializeField] private bool preserveYWhenVelocityYIsZero = true;

    [BoxGroup("Self Velocity")]
    [ToggleLeft]
    [LabelText("Stop When Phase Has No Velocity")]
    [SerializeField] private bool stopWhenPhaseHasNoVelocity = true;

    [BoxGroup("Recovery Velocity")]
    [ToggleLeft]
    [LabelText("Apply Recovery Velocity")]
    [SerializeField] private bool applyRecoveryVelocity = true;

    [BoxGroup("Recovery Velocity")]
    [ShowIf(nameof(applyRecoveryVelocity))]
    [LabelText("Velocity")]
    [SerializeField] private Vector2 recoveryVelocity;

    [BoxGroup("Decision")]
    [MinValue(0f)]
    [SerializeField] private float range = 1.5f;

    [BoxGroup("Parry")]
    [ToggleLeft]
    [SerializeField] private bool parryable = true;

    private Enemy enemy;
    private int stepIndex;
    private bool hasHit;

    public float Range => range;
    public float TelegraphTime => CurrentStep != null ? CurrentStep.TelegraphTime : 0f;
    public float ActiveTime => CurrentStep != null ? CurrentStep.ActiveTime : 0f;
    public float RecoveryTime => recoveryTime;
    public float Cooldown => cooldown;
    public bool IsParryable => parryable;

    private EnemyMeleeAttackStep CurrentStep
    {
        get
        {
            if (steps == null || steps.Length == 0)
            {
                return null;
            }

            return steps[Mathf.Clamp(stepIndex, 0, steps.Length - 1)];
        }
    }

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    public void OnAttackSequenceStart(Enemy self)
    {
        stepIndex = 0;
        hasHit = false;
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

        EnemyMeleeAttackStep step = CurrentStep;
        if (step == null || step.Hitboxes == null)
        {
            return;
        }

        foreach (AttackHitbox hitbox in step.Hitboxes)
        {
            if (hitbox == null)
            {
                continue;
            }

            Collider2D[] hits = hitbox.GetHits(transform, enemy.FacingDirection, GetTargetLayerMask());
            foreach (Collider2D hit in hits)
            {
                if (TryRegisterHit(hit, attackContext, step.Damage))
                {
                    return;
                }
            }
        }
    }

    public void OnPhaseFixedUpdate(Enemy self, EnemyAttackPhase phase)
    {
        if (!TryGetVelocity(phase, out Vector2 velocity))
        {
            if (!stopWhenPhaseHasNoVelocity)
            {
                return;
            }

            velocity = Vector2.zero;
        }

        if (scaleVelocityXByFacing)
        {
            velocity.x *= self.FacingDirection;
        }

        if (preserveYWhenVelocityYIsZero && Mathf.Approximately(velocity.y, 0f))
        {
            velocity.y = self.rb.linearVelocity.y;
        }

        self.SetVelocity(velocity.x, velocity.y);
    }

    public bool TryAdvanceStep(Enemy self)
    {
        if (steps == null || stepIndex + 1 >= steps.Length)
        {
            return false;
        }

        stepIndex++;
        hasHit = false;
        self.Stop();
        return true;
    }

    private bool TryRegisterHit(Collider2D hit, IParryable attackContext, int damage)
    {
        GuardSystem guard = hit.GetComponentInParent<GuardSystem>();
        if (guard != null && guard.TryGuard(attackContext))
        {
            hasHit = true;
            return true;
        }

        HealthSystem health = hit.GetComponentInParent<HealthSystem>();
        if (health == null)
        {
            return false;
        }

        health.TakeDamage(damage);
        hasHit = true;
        return true;
    }

    private bool TryGetVelocity(EnemyAttackPhase phase, out Vector2 velocity)
    {
        EnemyMeleeAttackStep step = CurrentStep;
        if (phase == EnemyAttackPhase.Telegraph)
        {
            velocity = step != null ? step.TelegraphVelocity : Vector2.zero;
            return step != null && step.ApplyTelegraphVelocity;
        }

        if (phase == EnemyAttackPhase.Active)
        {
            velocity = step != null ? step.ActiveVelocity : Vector2.zero;
            return step != null && step.ApplyActiveVelocity;
        }

        velocity = recoveryVelocity;
        return applyRecoveryVelocity;
    }

    private int GetTargetLayerMask()
    {
        if (targetLayers.value != 0)
        {
            return targetLayers.value;
        }

        int playerLayer = LayerMask.NameToLayer("Player");
        return playerLayer >= 0 ? 1 << playerLayer : Physics2D.AllLayers;
    }

    private void OnDrawGizmosSelected()
    {
        float facingDir = transform.localScale.x < 0f ? -1f : 1f;

        Gizmos.color = Color.red;
        if (steps == null)
        {
            return;
        }

        foreach (EnemyMeleeAttackStep step in steps)
        {
            if (step == null || step.Hitboxes == null)
            {
                continue;
            }

            foreach (AttackHitbox hitbox in step.Hitboxes)
            {
                if (hitbox != null)
                {
                    hitbox.DrawGizmos(transform, facingDir);
                }
            }
        }
    }
}
