using System;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public enum EnemyHitboxPreviewMode
{
    Off,
    CurrentAttackMove,
    SelectedAttackMove,
    AllAttackMoves
}

/// <summary>
/// One step inside an enemy attack move. A step can have its own trigger, hitboxes,
/// damage, and velocity windows.
/// </summary>
[Serializable]
public class EnemyMeleeAttackStep
{
    [field: FormerlySerializedAs("stepName")]
    [field: BoxGroup("Step")]
    [field: SerializeField] public string StepName { get; private set; } = "Swing";

    [field: FormerlySerializedAs("telegraphTime")]
    [field: BoxGroup("Step")]
    [field: MinValue(0f), SuffixLabel("s", true)]
    [field: SerializeField] public float TelegraphTime { get; private set; } = 0.4f;

    [field: FormerlySerializedAs("activeTime")]
    [field: BoxGroup("Step")]
    [field: MinValue(0f), SuffixLabel("s", true)]
    [field: SerializeField] public float ActiveTime { get; private set; } = 0.2f;

    [field: FormerlySerializedAs("damage")]
    [field: BoxGroup("Step")]
    [field: MinValue(0)]
    [field: SerializeField] public int Damage { get; private set; } = 1;

    [field: FormerlySerializedAs("attackAnimTrigger")]
    [field: BoxGroup("Step")]
    [field: LabelText("Attack Anim Trigger")]
    [field: SerializeField] public string AttackAnimTrigger { get; private set; }

    [field: FormerlySerializedAs("hitboxes")]
    [field: BoxGroup("Hitbox")]
    [field: ListDrawerSettings(DraggableItems = true, ShowFoldout = true)]
    [field: SerializeField] public AttackHitbox[] Hitboxes { get; private set; } = { new AttackHitbox() };

    [field: FormerlySerializedAs("drawDebug")]
    [field: BoxGroup("Hitbox")]
    [field: ToggleLeft]
    [field: LabelText("Draw Preview")]
    [field: SerializeField] public bool DrawDebug { get; private set; } = true;

    [field: FormerlySerializedAs("debugColor")]
    [field: BoxGroup("Hitbox")]
    [field: ShowIf(nameof(DrawDebug))]
    [field: LabelText("Preview Color")]
    [field: SerializeField] public Color DebugColor { get; private set; } = Color.red;

    [field: FormerlySerializedAs("applyTelegraphVelocity")]
    [field: BoxGroup("Velocity Windows")]
    [field: InfoBox("Applied for a fixed time when this attack step starts its animation.")]
    [field: ToggleLeft]
    [field: LabelText("Apply At Animation Start")]
    [field: SerializeField] public bool ApplyTelegraphVelocity { get; private set; }

    [field: FormerlySerializedAs("telegraphVelocity")]
    [field: BoxGroup("Velocity Windows")]
    [field: ShowIf(nameof(ApplyTelegraphVelocity))]
    [field: LabelText("Start Velocity")]
    [field: SerializeField] public Vector2 TelegraphVelocity { get; private set; }

    [field: FormerlySerializedAs("animationStartVelocityDuration")]
    [field: BoxGroup("Velocity Windows")]
    [field: ShowIf(nameof(ApplyTelegraphVelocity))]
    [field: MinValue(0f), SuffixLabel("s", true)]
    [field: LabelText("Start Duration")]
    [field: SerializeField] public float AnimationStartVelocityDuration { get; private set; } = 0.15f;

    [field: FormerlySerializedAs("stopStartVelocityWhenTargetEntersHitbox")]
    [field: BoxGroup("Velocity Windows")]
    [field: ShowIf(nameof(ApplyTelegraphVelocity))]
    [field: ToggleLeft]
    [field: LabelText("Stop Start Velocity When Target Enters Hitbox")]
    [field: SerializeField] public bool StopStartVelocityWhenTargetEntersHitbox { get; private set; }

    [field: FormerlySerializedAs("applyActiveVelocity")]
    [field: BoxGroup("Velocity Windows")]
    [field: InfoBox("Applied while the hitbox is active, between AnimEvent_StartHitbox and AnimEvent_EndHitbox.")]
    [field: ToggleLeft]
    [field: LabelText("Apply During Active")]
    [field: SerializeField] public bool ApplyActiveVelocity { get; private set; }

    [field: FormerlySerializedAs("activeVelocity")]
    [field: BoxGroup("Velocity Windows")]
    [field: ShowIf(nameof(ApplyActiveVelocity))]
    [field: LabelText("Active Velocity")]
    [field: SerializeField] public Vector2 ActiveVelocity { get; private set; }

    [field: FormerlySerializedAs("applyAnimationEndVelocity")]
    [field: BoxGroup("Velocity Windows")]
    [field: ToggleLeft]
    [field: LabelText("Apply At Animation End")]
    [field: SerializeField] public bool ApplyAnimationEndVelocity { get; private set; }

    [field: FormerlySerializedAs("animationEndVelocity")]
    [field: BoxGroup("Velocity Windows")]
    [field: ShowIf(nameof(ApplyAnimationEndVelocity))]
    [field: LabelText("End Velocity")]
    [field: SerializeField] public Vector2 AnimationEndVelocity { get; private set; }

    [field: FormerlySerializedAs("animationEndVelocityDuration")]
    [field: BoxGroup("Velocity Windows")]
    [field: ShowIf(nameof(ApplyAnimationEndVelocity))]
    [field: MinValue(0f), SuffixLabel("s", true)]
    [field: LabelText("End Duration")]
    [field: SerializeField] public float AnimationEndVelocityDuration { get; private set; } = 0.15f;
}

/// <summary>
/// Named attack move made of one or more steps. The enemy's brain choose these by name.
/// </summary>
[Serializable]
public class EnemyMeleeAttackSequence
{
    [field: FormerlySerializedAs("sequenceName")]
    [field: BoxGroup("Attack Move")]
    [field: LabelText("Attack Move Name")]
    [field: SerializeField] public string SequenceName { get; private set; } = "Attack";

    [field: FormerlySerializedAs("steps")]
    [field: BoxGroup("Attack Move")]
    [field: ListDrawerSettings(DraggableItems = true, ShowFoldout = true, ListElementLabelName = nameof(EnemyMeleeAttackStep.StepName))]
    [field: SerializeField] public EnemyMeleeAttackStep[] Steps { get; private set; } = { new EnemyMeleeAttackStep() };
}

/// <summary>
/// Configurable melee attack module for enemies
/// </summary>
[RequireComponent(typeof(Enemy))]
public class EnemyMeleeAttack : MonoBehaviour, IEnemyAttack
{
    [BoxGroup("Attack Moves")]
    [LabelText("Attack Move Name")]
    [SerializeField] private string defaultSequenceName = "DashAttack";

    [BoxGroup("Attack Moves")]
    [LabelText("Attack Steps")]
    [ListDrawerSettings(DraggableItems = true, ShowFoldout = true, ListElementLabelName = nameof(EnemyMeleeAttackStep.StepName))]
    [SerializeField] private EnemyMeleeAttackStep[] steps = { new EnemyMeleeAttackStep() };

    [BoxGroup("Attack Moves")]
    [ListDrawerSettings(DraggableItems = true, ShowFoldout = true, ListElementLabelName = nameof(EnemyMeleeAttackSequence.SequenceName))]
    [SerializeField] private EnemyMeleeAttackSequence[] extraSequences;

    [field: FormerlySerializedAs("recoveryTime")]
    [field: BoxGroup("Timing")]
    [field: MinValue(0f), SuffixLabel("s", true)]
    [field: SerializeField] public float RecoveryTime { get; private set; } = 0.5f;

    [field: FormerlySerializedAs("cooldown")]
    [field: BoxGroup("Timing")]
    [field: MinValue(0f), SuffixLabel("s", true)]
    [field: SerializeField] public float Cooldown { get; private set; } = 2f;

    [field: FormerlySerializedAs("useAnimationEvents")]
    [field: BoxGroup("Timing")]
    [field: ToggleLeft]
    [field: LabelText("Use Animation Events")]
    [field: SerializeField] public bool UsesAnimationEvents { get; private set; } = true;

    [BoxGroup("Timing")]
    [ShowIf(nameof(UsesAnimationEvents))]
    [InfoBox("Add animation events that call AnimEvent_StartHitbox, AnimEvent_EndHitbox, and AnimEvent_CompleteStep. Turn this off for enemies that need timer-driven hitboxes.")]
    [SerializeField, HideLabel, ReadOnly] private string animationEventHelp;

    [BoxGroup("Timing")]
    [HideIf(nameof(UsesAnimationEvents))]
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

    [field: FormerlySerializedAs("range")]
    [field: BoxGroup("Combat")]
    [field: MinValue(0f)]
    [field: LabelText("Attack Range")]
    [field: SerializeField] public float Range { get; private set; } = 1.5f;

    [field: FormerlySerializedAs("parryable")]
    [field: BoxGroup("Combat")]
    [field: ToggleLeft]
    [field: LabelText("Parryable")]
    [field: SerializeField] public bool IsParryable { get; private set; } = true;

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

    public float TelegraphTime => CurrentStep != null ? CurrentStep.TelegraphTime : 0f;
    public float ActiveTime => CurrentStep != null ? CurrentStep.ActiveTime : 0f;
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

    /// <summary>
    /// Selects the attack move that the next Enemy_AttackState run should use.
    /// </summary>
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

    /// <summary>
    /// Resets step index, hit state, animation events, and timed velocity for a new attack move.
    /// </summary>
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

    /// <summary>
    /// Handles phase entry side effects such as firing the step animation trigger
    /// and starting animation-start velocity.
    /// </summary>
    public void OnPhaseEnter(Enemy self, EnemyAttackPhase phase)
    {
        string trigger = null;
        EnemyMeleeAttackStep step = CurrentStep;

        if (phase == EnemyAttackPhase.Telegraph && step != null)
        {
            trigger = step.AttackAnimTrigger;
        }
        if (!string.IsNullOrWhiteSpace(trigger)
            && self.animator != null
            && self.animator.runtimeAnimatorController != null)
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

    /// <summary>
    /// Checks all hitboxes on the active step until one valid hit or guard interaction resolves.
    /// </summary>
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

    /// <summary>
    /// Applies the highest-priority attack velocity for the current physics step:
    /// timed animation velocity first, then phase velocity, then optional stop.
    /// </summary>
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

    /// <summary>
    /// Consumes the animation event that opens the active hitbox window.
    /// </summary>
    public bool ConsumeActiveStarted()
    {
        if (!activeStartedEvent)
        {
            return false;
        }

        activeStartedEvent = false;
        return true;
    }

    /// <summary>
    /// Checks the animation event that closes the active hitbox window.
    /// </summary>
    public bool ConsumeActiveEnded()
    {
        if (!activeEndedEvent)
        {
            return false;
        }

        activeEndedEvent = false;
        return true;
    }

    /// <summary>
    /// Checks the animation event that marks the current attack step as complete.
    /// </summary>
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
        if (UsesAnimationEvents)
        {
            activeStartedEvent = true;
        }
    }

    public void AnimEvent_EndHitbox()
    {
        if (UsesAnimationEvents)
        {
            activeEndedEvent = true;
        }
    }

    public void AnimEvent_CompleteStep()
    {
        if (UsesAnimationEvents)
        {
            StartAnimationEndVelocity(CurrentStep);
            stepCompletedEvent = true;
        }
    }

    /// <summary>
    /// Advances to the next step in the active attack move. Returns false when the sequence is finished.
    /// </summary>
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

    /// <summary>
    /// Resolves guard checks before damage so parries/blocks can consume the hit.
    /// </summary>
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

    /// <summary>
    /// Reads the base velocity configured for the current attack phase.
    /// </summary>
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

    /// <summary>
    /// Starts a fixed-duration velocity window when the step animation begins.
    /// </summary>
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

    /// <summary>
    /// Starts a fixed-duration velocity window after the step completion event.
    /// </summary>
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

    /// <summary>
    /// Returns active timed velocity, including early dash stopping when the target enters the step hitbox reach.
    /// </summary>
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

    /// <summary>
    /// Checks whether the target is already inside any active step hitbox or its forward reach.
    /// </summary>
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

    /// <summary>
    /// Updates duration timers for animation-start and animation-end velocity windows.
    /// </summary>
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

    /// <summary>
    /// Draws hitbox previews according to the selected inspector preview mode.
    /// </summary>
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
