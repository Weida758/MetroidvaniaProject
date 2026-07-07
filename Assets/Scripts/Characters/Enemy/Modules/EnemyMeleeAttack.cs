using System;
using UnityEngine;
using Sirenix.OdinInspector;

public enum EnemyHitboxPreviewMode
{
    Off,
    CurrentAttackMove,
    SelectedAttackMove,
    AllAttackMoves
}

[Serializable]
public class EnemyMeleeAttackStep
{
    [BoxGroup("Step")]
    [SerializeField] private string stepName = "Swing";

    [BoxGroup("Step")]
    [MinValue(0f), SuffixLabel("s", true)]
    [SerializeField] private float telegraphTime = 0.4f;

    [BoxGroup("Step")]
    [MinValue(0f), SuffixLabel("s", true)]
    [SerializeField] private float activeTime = 0.2f;

    [BoxGroup("Step")]
    [MinValue(0)]
    [SerializeField] private int damage = 1;

    [BoxGroup("Step")]
    [LabelText("Attack Anim Trigger")]
    [SerializeField] private string attackAnimTrigger;

    [BoxGroup("Hitbox")]
    [ListDrawerSettings(Expanded = true, DraggableItems = true, ShowFoldout = true)]
    [SerializeField] private AttackHitbox[] hitboxes = { new AttackHitbox() };

    [BoxGroup("Hitbox")]
    [ToggleLeft]
    [LabelText("Draw Preview")]
    [SerializeField] private bool drawDebug = true;

    [BoxGroup("Hitbox")]
    [ShowIf(nameof(drawDebug))]
    [LabelText("Preview Color")]
    [SerializeField] private Color debugColor = Color.red;

    [BoxGroup("Velocity Windows")]
    [InfoBox("Applied for a fixed time when this attack step starts its animation.")]
    [ToggleLeft]
    [LabelText("Apply At Animation Start")]
    [SerializeField] private bool applyTelegraphVelocity;

    [BoxGroup("Velocity Windows")]
    [ShowIf(nameof(applyTelegraphVelocity))]
    [LabelText("Start Velocity")]
    [SerializeField] private Vector2 telegraphVelocity;

    [BoxGroup("Velocity Windows")]
    [ShowIf(nameof(applyTelegraphVelocity))]
    [MinValue(0f), SuffixLabel("s", true)]
    [LabelText("Start Duration")]
    [SerializeField] private float animationStartVelocityDuration = 0.15f;

    [BoxGroup("Velocity Windows")]
    [ShowIf(nameof(applyTelegraphVelocity))]
    [ToggleLeft]
    [LabelText("Stop Start Velocity When Target Enters Hitbox")]
    [SerializeField] private bool stopStartVelocityWhenTargetEntersHitbox;

    [BoxGroup("Velocity Windows")]
    [InfoBox("Applied while the hitbox is active, between AnimEvent_StartHitbox and AnimEvent_EndHitbox.")]
    [ToggleLeft]
    [LabelText("Apply During Active")]
    [SerializeField] private bool applyActiveVelocity;

    [BoxGroup("Velocity Windows")]
    [ShowIf(nameof(applyActiveVelocity))]
    [LabelText("Active Velocity")]
    [SerializeField] private Vector2 activeVelocity;

    [BoxGroup("Velocity Windows")]
    [ToggleLeft]
    [LabelText("Apply At Animation End")]
    [SerializeField] private bool applyAnimationEndVelocity;

    [BoxGroup("Velocity Windows")]
    [ShowIf(nameof(applyAnimationEndVelocity))]
    [LabelText("End Velocity")]
    [SerializeField] private Vector2 animationEndVelocity;

    [BoxGroup("Velocity Windows")]
    [ShowIf(nameof(applyAnimationEndVelocity))]
    [MinValue(0f), SuffixLabel("s", true)]
    [LabelText("End Duration")]
    [SerializeField] private float animationEndVelocityDuration = 0.15f;

    public string StepName => stepName;
    public float TelegraphTime => telegraphTime;
    public float ActiveTime => activeTime;
    public int Damage => damage;
    public string AttackAnimTrigger => attackAnimTrigger;
    public AttackHitbox[] Hitboxes => hitboxes;
    public bool DrawDebug => drawDebug;
    public Color DebugColor => debugColor;
    public bool ApplyTelegraphVelocity => applyTelegraphVelocity;
    public Vector2 TelegraphVelocity => telegraphVelocity;
    public float AnimationStartVelocityDuration => animationStartVelocityDuration;
    public bool StopStartVelocityWhenTargetEntersHitbox => stopStartVelocityWhenTargetEntersHitbox;
    public bool ApplyActiveVelocity => applyActiveVelocity;
    public Vector2 ActiveVelocity => activeVelocity;
    public bool ApplyAnimationEndVelocity => applyAnimationEndVelocity;
    public Vector2 AnimationEndVelocity => animationEndVelocity;
    public float AnimationEndVelocityDuration => animationEndVelocityDuration;
}

[Serializable]
public class EnemyMeleeAttackSequence
{
    [BoxGroup("Attack Move")]
    [LabelText("Attack Move Name")]
    [SerializeField] private string sequenceName = "Attack";

    [BoxGroup("Attack Move")]
    [ListDrawerSettings(Expanded = true, DraggableItems = true, ShowFoldout = true, ListElementLabelName = nameof(EnemyMeleeAttackStep.StepName))]
    [SerializeField] private EnemyMeleeAttackStep[] steps = { new EnemyMeleeAttackStep() };

    public string SequenceName => sequenceName;
    public EnemyMeleeAttackStep[] Steps => steps;
}

[RequireComponent(typeof(Enemy))]
public class EnemyMeleeAttack : MonoBehaviour, IEnemyAttack
{
    [BoxGroup("Attack Moves")]
    [LabelText("Attack Move Name")]
    [SerializeField] private string defaultSequenceName = "DashAttack";

    [BoxGroup("Attack Moves")]
    [LabelText("Attack Steps")]
    [ListDrawerSettings(Expanded = true, DraggableItems = true, ShowFoldout = true, ListElementLabelName = nameof(EnemyMeleeAttackStep.StepName))]
    [SerializeField] private EnemyMeleeAttackStep[] steps = { new EnemyMeleeAttackStep() };

    [BoxGroup("Attack Moves")]
    [ListDrawerSettings(Expanded = true, DraggableItems = true, ShowFoldout = true, ListElementLabelName = nameof(EnemyMeleeAttackSequence.SequenceName))]
    [SerializeField] private EnemyMeleeAttackSequence[] extraSequences;

    [BoxGroup("Timing")]
    [MinValue(0f), SuffixLabel("s", true)]
    [SerializeField] private float recoveryTime = 0.5f;

    [BoxGroup("Timing")]
    [MinValue(0f), SuffixLabel("s", true)]
    [SerializeField] private float cooldown = 2f;

    [BoxGroup("Timing")]
    [ToggleLeft]
    [LabelText("Use Animation Events")]
    [SerializeField] private bool useAnimationEvents = true;

    [BoxGroup("Timing")]
    [ShowIf(nameof(useAnimationEvents))]
    [InfoBox("Add animation events that call AnimEvent_StartHitbox, AnimEvent_EndHitbox, and AnimEvent_CompleteStep. Turn this off for enemies that need timer-driven hitboxes.")]
    [SerializeField, HideLabel, ReadOnly] private string animationEventHelp;

    [BoxGroup("Timing")]
    [HideIf(nameof(useAnimationEvents))]
    [InfoBox("Timer-driven mode uses Telegraph Time and Active Time on each Attack Step instead of animation events.")]
    [SerializeField, HideLabel, ReadOnly] private string timerDrivenHelp;

    [BoxGroup("Combat")]
    [InfoBox("Defaults to the Player layer when this is left empty.", InfoMessageType.None)]
    [SerializeField] private LayerMask targetLayers;

    [BoxGroup("Movement")]
    [ToggleLeft]
    [LabelText("Scale X By Facing Direction")]
    [SerializeField] private bool scaleVelocityXByFacing = true;

    [BoxGroup("Movement")]
    [ToggleLeft]
    [LabelText("Preserve Current Y When Velocity Y Is 0")]
    [SerializeField] private bool preserveYWhenVelocityYIsZero = true;

    [BoxGroup("Movement")]
    [ToggleLeft]
    [LabelText("Stop When Phase Has No Velocity")]
    [SerializeField] private bool stopWhenPhaseHasNoVelocity = true;

    [BoxGroup("Movement")]
    [ToggleLeft]
    [LabelText("Apply Recovery Velocity")]
    [SerializeField] private bool applyRecoveryVelocity = true;

    [BoxGroup("Movement")]
    [ShowIf(nameof(applyRecoveryVelocity))]
    [LabelText("Recovery Velocity")]
    [SerializeField] private Vector2 recoveryVelocity;

    [BoxGroup("Combat")]
    [MinValue(0f)]
    [LabelText("Attack Range")]
    [SerializeField] private float range = 1.5f;

    [BoxGroup("Combat")]
    [ToggleLeft]
    [LabelText("Parryable")]
    [SerializeField] private bool parryable = true;

    [BoxGroup("Preview")]
    [SerializeField] private EnemyHitboxPreviewMode hitboxPreviewMode = EnemyHitboxPreviewMode.CurrentAttackMove;

    [BoxGroup("Preview")]
    [ShowIf(nameof(ShowSelectedAttackMovePicker))]
    [ValueDropdown(nameof(GetPreviewAttackMoveChoices))]
    [LabelText("Attack Move")]
    [SerializeField] private string previewAttackMoveName;

    [BoxGroup("Preview")]
    [ShowIf(nameof(ShowStepPreviewToggle))]
    [ToggleLeft]
    [LabelText("Preview All Steps")]
    [SerializeField] private bool previewAllSteps = true;

    [BoxGroup("Preview")]
    [ShowIf(nameof(ShowStepPreviewPicker))]
    [ValueDropdown(nameof(GetPreviewStepChoices))]
    [LabelText("Attack Step")]
    [SerializeField] private int previewStepIndex;

    private Enemy enemy;
    private EnemyMeleeAttackStep[] activeSteps;
    private string activeSequenceName;
    private int stepIndex;
    private bool hasHit;
    private bool activeStartedEvent;
    private bool activeEndedEvent;
    private bool stepCompletedEvent;
    private bool animationStartVelocityActive;
    private float animationStartVelocityTimer;
    private Vector2 animationStartVelocity;
    private bool animationEndVelocityActive;
    private float animationEndVelocityTimer;
    private Vector2 animationEndVelocity;

    public float Range => range;
    public float TelegraphTime => CurrentStep != null ? CurrentStep.TelegraphTime : 0f;
    public float ActiveTime => CurrentStep != null ? CurrentStep.ActiveTime : 0f;
    public float RecoveryTime => recoveryTime;
    public float Cooldown => cooldown;
    public bool IsParryable => parryable;
    public bool UsesAnimationEvents => useAnimationEvents;
    public bool HasPendingCompletionVelocity => animationEndVelocityActive;

    [ShowInInspector, ReadOnly, BoxGroup("Debug")]
    public string ActiveAttackMoveName => string.IsNullOrWhiteSpace(activeSequenceName) ? defaultSequenceName : activeSequenceName;

    private EnemyMeleeAttackStep CurrentStep
    {
        get
        {
            EnemyMeleeAttackStep[] sequenceSteps = CurrentSequenceSteps;
            if (sequenceSteps == null || sequenceSteps.Length == 0)
            {
                return null;
            }

            return sequenceSteps[Mathf.Clamp(stepIndex, 0, sequenceSteps.Length - 1)];
        }
    }

    private EnemyMeleeAttackStep[] CurrentSequenceSteps
    {
        get
        {
            return activeSteps != null && activeSteps.Length > 0 ? activeSteps : steps;
        }
    }

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    public bool TryUseSequence(string sequenceName)
    {
        if (string.IsNullOrWhiteSpace(sequenceName) || sequenceName == defaultSequenceName)
        {
            activeSteps = steps;
            activeSequenceName = defaultSequenceName;
            return true;
        }

        if (extraSequences != null)
        {
            foreach (EnemyMeleeAttackSequence sequence in extraSequences)
            {
                if (sequence == null || sequence.SequenceName != sequenceName)
                {
                    continue;
                }

                activeSteps = sequence.Steps;
                activeSequenceName = sequence.SequenceName;
                return true;
            }
        }

        return false;
    }

    public void OnAttackSequenceStart(Enemy self)
    {
        if (activeSteps == null)
        {
            TryUseSequence(defaultSequenceName);
        }

        stepIndex = 0;
        hasHit = false;
        ClearAnimationEventRequests();
        ClearTimedVelocities();
    }

    public void OnPhaseEnter(Enemy self, EnemyAttackPhase phase)
    {
        string trigger = null;
        EnemyMeleeAttackStep step = CurrentStep;

        if (phase == EnemyAttackPhase.Telegraph && step != null)
        {
            trigger = step.AttackAnimTrigger;
        }
        if (!string.IsNullOrWhiteSpace(trigger) && self.animator != null)
        {
            self.animator.SetTrigger(trigger);
        }

        if (phase == EnemyAttackPhase.Telegraph)
        {
            StartAnimationStartVelocity(step);
        }
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
        bool hasVelocity = TryGetTimedVelocity(out Vector2 velocity) || TryGetVelocity(phase, out velocity);
        if (!hasVelocity)
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
        TickTimedVelocities();
    }

    public bool ConsumeActiveStarted()
    {
        if (!activeStartedEvent)
        {
            return false;
        }

        activeStartedEvent = false;
        return true;
    }

    public bool ConsumeActiveEnded()
    {
        if (!activeEndedEvent)
        {
            return false;
        }

        activeEndedEvent = false;
        return true;
    }

    public bool ConsumeStepCompleted()
    {
        if (!stepCompletedEvent)
        {
            return false;
        }

        stepCompletedEvent = false;
        return true;
    }

    public void AnimEvent_StartHitbox()
    {
        if (useAnimationEvents)
        {
            activeStartedEvent = true;
        }
    }

    public void AnimEvent_EndHitbox()
    {
        if (useAnimationEvents)
        {
            activeEndedEvent = true;
        }
    }

    public void AnimEvent_CompleteStep()
    {
        if (useAnimationEvents)
        {
            StartAnimationEndVelocity(CurrentStep);
            stepCompletedEvent = true;
        }
    }

    public bool TryAdvanceStep(Enemy self)
    {
        EnemyMeleeAttackStep[] sequenceSteps = CurrentSequenceSteps;
        if (sequenceSteps == null || stepIndex + 1 >= sequenceSteps.Length)
        {
            return false;
        }

        stepIndex++;
        hasHit = false;
        ClearAnimationEventRequests();
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
            return false;
        }

        if (phase == EnemyAttackPhase.Active)
        {
            velocity = step != null ? step.ActiveVelocity : Vector2.zero;
            return step != null && step.ApplyActiveVelocity;
        }

        velocity = recoveryVelocity;
        return applyRecoveryVelocity;
    }

    private void StartAnimationStartVelocity(EnemyMeleeAttackStep step)
    {
        animationEndVelocityActive = false;
        animationEndVelocityTimer = 0f;

        if (step == null || !step.ApplyTelegraphVelocity || step.AnimationStartVelocityDuration <= 0f)
        {
            animationStartVelocityActive = false;
            animationStartVelocityTimer = 0f;
            return;
        }

        animationStartVelocity = step.TelegraphVelocity;
        animationStartVelocityTimer = step.AnimationStartVelocityDuration;
        animationStartVelocityActive = true;
    }

    private void StartAnimationEndVelocity(EnemyMeleeAttackStep step)
    {
        if (step == null || !step.ApplyAnimationEndVelocity || step.AnimationEndVelocityDuration <= 0f)
        {
            animationEndVelocityActive = false;
            animationEndVelocityTimer = 0f;
            return;
        }

        animationEndVelocity = step.AnimationEndVelocity;
        animationEndVelocityTimer = step.AnimationEndVelocityDuration;
        animationEndVelocityActive = true;
    }

    private bool TryGetTimedVelocity(out Vector2 velocity)
    {
        if (animationEndVelocityActive)
        {
            velocity = animationEndVelocity;
            return true;
        }

        if (animationStartVelocityActive)
        {
            EnemyMeleeAttackStep step = CurrentStep;
            if (step != null && step.StopStartVelocityWhenTargetEntersHitbox && TargetIsInsideStepHitbox(step))
            {
                animationStartVelocityActive = false;
                animationStartVelocityTimer = 0f;
                velocity = Vector2.zero;
                return false;
            }

            velocity = animationStartVelocity;
            return true;
        }

        velocity = Vector2.zero;
        return false;
    }

    private bool TargetIsInsideStepHitbox(EnemyMeleeAttackStep step)
    {
        if (step.Hitboxes == null)
        {
            return false;
        }

        foreach (AttackHitbox hitbox in step.Hitboxes)
        {
            if (hitbox == null)
            {
                continue;
            }

            Collider2D[] hits = hitbox.GetHits(transform, enemy.FacingDirection, GetTargetLayerMask());
            if (hits.Length > 0)
            {
                return true;
            }

            if (enemy.HasTarget && hitbox.IsPositionWithinForwardReach(transform, enemy.FacingDirection, enemy.Target.position))
            {
                return true;
            }
        }

        return false;
    }

    private void TickTimedVelocities()
    {
        if (animationStartVelocityActive)
        {
            animationStartVelocityTimer -= Time.fixedDeltaTime;
            if (animationStartVelocityTimer <= 0f)
            {
                animationStartVelocityActive = false;
                animationStartVelocityTimer = 0f;
            }
        }

        if (animationEndVelocityActive)
        {
            animationEndVelocityTimer -= Time.fixedDeltaTime;
            if (animationEndVelocityTimer <= 0f)
            {
                animationEndVelocityActive = false;
                animationEndVelocityTimer = 0f;
            }
        }
    }

    private void ClearAnimationEventRequests()
    {
        activeStartedEvent = false;
        activeEndedEvent = false;
        stepCompletedEvent = false;
    }

    private void ClearTimedVelocities()
    {
        animationStartVelocityActive = false;
        animationStartVelocityTimer = 0f;
        animationEndVelocityActive = false;
        animationEndVelocityTimer = 0f;
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
        if (hitboxPreviewMode == EnemyHitboxPreviewMode.Off)
        {
            return;
        }

        float facingDir = transform.localScale.x < 0f ? -1f : 1f;

        if (hitboxPreviewMode == EnemyHitboxPreviewMode.AllAttackMoves)
        {
            DrawAttackMoveGizmos(steps, facingDir);
            if (extraSequences == null)
            {
                return;
            }

            foreach (EnemyMeleeAttackSequence sequence in extraSequences)
            {
                if (sequence != null)
                {
                    DrawAttackMoveGizmos(sequence.Steps, facingDir);
                }
            }
            return;
        }

        DrawAttackMoveGizmos(GetPreviewSteps(), facingDir);
    }

    private void DrawAttackMoveGizmos(EnemyMeleeAttackStep[] attackSteps, float facingDir)
    {
        if (attackSteps == null)
        {
            return;
        }

        if (previewAllSteps || hitboxPreviewMode == EnemyHitboxPreviewMode.AllAttackMoves)
        {
            foreach (EnemyMeleeAttackStep step in attackSteps)
            {
                DrawStepGizmos(step, facingDir);
            }
            return;
        }

        int index = Mathf.Clamp(previewStepIndex, 0, attackSteps.Length - 1);
        DrawStepGizmos(attackSteps[index], facingDir);
    }

    private void DrawStepGizmos(EnemyMeleeAttackStep step, float facingDir)
    {
        if (step == null || !step.DrawDebug || step.Hitboxes == null)
        {
            return;
        }

        Gizmos.color = step.DebugColor;
        foreach (AttackHitbox hitbox in step.Hitboxes)
        {
            if (hitbox != null)
            {
                hitbox.DrawGizmos(transform, facingDir);
            }
        }
    }

    private EnemyMeleeAttackStep[] GetPreviewSteps()
    {
        if (hitboxPreviewMode == EnemyHitboxPreviewMode.CurrentAttackMove)
        {
            return CurrentSequenceSteps;
        }

        if (string.IsNullOrWhiteSpace(previewAttackMoveName) || previewAttackMoveName == defaultSequenceName)
        {
            return steps;
        }

        if (extraSequences == null)
        {
            return steps;
        }

        foreach (EnemyMeleeAttackSequence sequence in extraSequences)
        {
            if (sequence != null && sequence.SequenceName == previewAttackMoveName)
            {
                return sequence.Steps;
            }
        }

        return steps;
    }

    private bool ShowSelectedAttackMovePicker()
    {
        return hitboxPreviewMode == EnemyHitboxPreviewMode.SelectedAttackMove;
    }

    private bool ShowStepPreviewToggle()
    {
        return hitboxPreviewMode == EnemyHitboxPreviewMode.CurrentAttackMove
            || hitboxPreviewMode == EnemyHitboxPreviewMode.SelectedAttackMove;
    }

    private bool ShowStepPreviewPicker()
    {
        return ShowStepPreviewToggle() && !previewAllSteps;
    }

    private ValueDropdownList<string> GetPreviewAttackMoveChoices()
    {
        ValueDropdownList<string> choices = new ValueDropdownList<string>();
        choices.Add(string.IsNullOrWhiteSpace(defaultSequenceName) ? "Primary Attack Move" : defaultSequenceName, defaultSequenceName);

        if (extraSequences == null)
        {
            return choices;
        }

        foreach (EnemyMeleeAttackSequence sequence in extraSequences)
        {
            if (sequence == null || string.IsNullOrWhiteSpace(sequence.SequenceName))
            {
                continue;
            }

            choices.Add(sequence.SequenceName, sequence.SequenceName);
        }

        return choices;
    }

    private ValueDropdownList<int> GetPreviewStepChoices()
    {
        ValueDropdownList<int> choices = new ValueDropdownList<int>();
        EnemyMeleeAttackStep[] previewSteps = GetPreviewSteps();
        if (previewSteps == null || previewSteps.Length == 0)
        {
            choices.Add("No steps configured", 0);
            return choices;
        }

        for (int i = 0; i < previewSteps.Length; i++)
        {
            string label = previewSteps[i] == null || string.IsNullOrWhiteSpace(previewSteps[i].StepName)
                ? "Unnamed"
                : previewSteps[i].StepName;
            choices.Add($"{i + 1}: {label}", i);
        }

        return choices;
    }
}
