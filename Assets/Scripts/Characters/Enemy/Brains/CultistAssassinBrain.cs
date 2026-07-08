using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Enemy brain for the cultist assassin. It chases into a dash attack,
/// then chooses between backstab, teleport-away, or jump-away follow-ups.
/// </summary>
public class CultistAssassinBrain : EnemyBrain
{
    [BoxGroup("Attack Moves")]
    [SerializeField] private string dashAttackMove = "DashAttack";

    [BoxGroup("Attack Moves")]
    [SerializeField] private string backstabAttackMove = "BackstabAttack";

    [BoxGroup("Follow-Up Decision")]
    [Range(0f, 1f)]
    [LabelText("Backstab Chance")]
    [FormerlySerializedAs("defufuBackstabChance")]
    [SerializeField] private float backstabChance = 0.5f;

    [BoxGroup("Follow-Up Decision")]
    [Range(0f, 1f)]
    [LabelText("Teleport Away Chance")]
    [SerializeField] private float defufuAwayChance = 0.5f;

    [BoxGroup("Follow-Up Decision")]
    [ToggleLeft]
    [LabelText("Force Escape If Too Close")]
    [FormerlySerializedAs("jumpAwayIfTooClose")]
    [SerializeField] private bool forceEscapeIfTooClose = true;

    [BoxGroup("Follow-Up Decision")]
    [ShowIf(nameof(forceEscapeIfTooClose))]
    [MinValue(0f)]
    [LabelText("Too Close Distance")]
    [SerializeField] private float tooCloseDistance = 1f;

    [BoxGroup("Follow-Up Decision")]
    [ShowIf(nameof(forceEscapeIfTooClose))]
    [MinValue(0f), SuffixLabel("s", true)]
    [LabelText("Too Close Wait")]
    [SerializeField] private float tooCloseWaitBeforeEscape = 1f;

    [BoxGroup("Vanish And Reappear")]
    [LabelText("Disappear Trigger")]
    [SerializeField] private string disappearAnimTrigger = "Furtif";

    [BoxGroup("Vanish And Reappear")]
    [LabelText("Appear Trigger")]
    [SerializeField] private string appearAnimTrigger = "Defufu";

    [BoxGroup("Vanish And Reappear")]
    [ToggleLeft]
    [LabelText("Use Vanish Animation Events")]
    [SerializeField] private bool useVanishAnimationEvents = true;

    [BoxGroup("Vanish And Reappear")]
    [HideIf(nameof(useVanishAnimationEvents))]
    [MinValue(0f), SuffixLabel("s", true)]
    [LabelText("Disappear Duration")]
    [FormerlySerializedAs("defufuDuration")]
    [SerializeField] private float disappearDuration = 0.45f;

    [BoxGroup("Vanish And Reappear")]
    [ToggleLeft]
    [LabelText("Use Appear Animation Events")]
    [SerializeField] private bool useAppearAnimationEvents = true;

    [BoxGroup("Vanish And Reappear")]
    [HideIf(nameof(useAppearAnimationEvents))]
    [MinValue(0f), SuffixLabel("s", true)]
    [LabelText("Appear Duration")]
    [SerializeField] private float appearDuration = 0.25f;

    [BoxGroup("Backstab Teleport")]
    [MinValue(0f)]
    [SerializeField] private float teleportBehindPlayerDistance = 1.25f;

    [BoxGroup("Backstab Teleport")]
    [FormerlySerializedAs("teleportOffset")]
    [SerializeField] private Vector2 teleportBehindOffset;

    [BoxGroup("Escape Teleport")]
    [MinValue(0f)]
    [SerializeField] private float teleportAwayDistance = 3f;

    [BoxGroup("Escape Teleport")]
    [SerializeField] private Vector2 teleportAwayOffset;

    [BoxGroup("Jump Away")]
    [MinValue(1)]
    [SerializeField] private int minJumpAwayCount = 1;

    [BoxGroup("Jump Away")]
    [MinValue(1)]
    [SerializeField] private int maxJumpAwayCount = 2;

    [BoxGroup("Jump Away")]
    [SerializeField] private Vector2 jumpAwayVelocity = new Vector2(5f, 6f);

    [BoxGroup("Jump Away")]
    [MinValue(0.01f), SuffixLabel("s", true)]
    [SerializeField] private float jumpAwayDuration = 0.28f;

    [BoxGroup("Jump Away")]
    [MinValue(0f), SuffixLabel("s", true)]
    [SerializeField] private float pauseBetweenJumps = 0.08f;

    /// <summary>
    /// Builds the assassin's state graph
    /// </summary>
    protected override void Build()
    {
        Enemy_PatrolState patrol = new Enemy_PatrolState(enemy);
        Enemy_InCombatState chase = new Enemy_InCombatState(enemy);
        Enemy_AttackState dashAttack = new Enemy_AttackState(enemy, dashAttackMove);
        FollowUpDecisionState followUpDecision = new FollowUpDecisionState(enemy, this);
        WaitBeforeEscapeState waitBeforeEscape = new WaitBeforeEscapeState(enemy, this);
        DisappearTeleportState vanishBehindPlayer = new DisappearTeleportState(enemy, this, TeleportDestination.BehindPlayer);
        Enemy_AttackState backstabAttack = new Enemy_AttackState(enemy, backstabAttackMove);
        DisappearTeleportAppearState defufuAway = new DisappearTeleportAppearState(enemy, this, TeleportDestination.AwayFromPlayer);
        JumpAwayState jumpAway = new JumpAwayState(enemy, this);

        AddTransition(patrol, chase, () => enemy.perception.CanSeeTarget(), "Saw target");
        AddTransition(chase, patrol, () => enemy.perception.HasLostTarget(), "Lost target");
        AddTransition(chase, dashAttack, () => enemy.CanAttack, "Dash attack ready");
        AddTransition(dashAttack, followUpDecision, () => dashAttack.IsFinished, "Choose follow-up");
        AddTransition(followUpDecision, waitBeforeEscape, () => followUpDecision.WaitThenEscape, "Too close wait");
        AddTransition(followUpDecision, vanishBehindPlayer, () => followUpDecision.UseBackstab, "Choose backstab");
        AddTransition(followUpDecision, defufuAway, () => followUpDecision.UseDefufuAway, "Choose defufu away");
        AddTransition(followUpDecision, jumpAway, () => followUpDecision.UseJumpAway, "Choose jump away");
        AddTransition(waitBeforeEscape, defufuAway, () => waitBeforeEscape.IsFinished && waitBeforeEscape.UseDefufuAway, "Defufu away after wait");
        AddTransition(waitBeforeEscape, jumpAway, () => waitBeforeEscape.IsFinished && waitBeforeEscape.UseJumpAway, "Jump away after wait");
        AddTransition(vanishBehindPlayer, backstabAttack, () => vanishBehindPlayer.IsFinished, "Backstab after vanish");
        AddTransition(backstabAttack, chase, () => backstabAttack.IsFinished, "Backstab finished");
        AddTransition(defufuAway, chase, () => defufuAway.IsFinished, "Defufu away finished");
        AddTransition(jumpAway, chase, () => jumpAway.IsFinished, "Jump away finished");

        SetInitial(patrol);
    }

    private bool IsTooCloseToTarget()
    {
        return enemy.HasTarget && enemy.HorizontalDistanceToTarget <= tooCloseDistance;
    }

    private bool ChooseBackstab()
    {
        return Random.value <= backstabChance;
    }

    private bool ChooseDefufuAway()
    {
        return Random.value <= defufuAwayChance;
    }

    private int GetJumpAwayCount()
    {
        int min = Mathf.Max(1, minJumpAwayCount);
        int max = Mathf.Max(min, maxJumpAwayCount);
        return Random.Range(min, max + 1);
    }

    private enum TeleportDestination
    {
        BehindPlayer,
        AwayFromPlayer
    }

    private bool vanishCompleted;
    private bool appearCompleted;

    public override void AnimEvent_Teleport()
    {
    }

    /// <summary>
    /// Receives the vanish animation event that means the assassin has fully vanished.
    /// Teleporting happens after this event
    /// </summary>
    public override void AnimEvent_CompleteVanish()
    {
        vanishCompleted = true;
    }

    /// <summary>
    /// Receives the teleport in animation event that means the appear animation is complete.
    /// </summary>
    public override void AnimEvent_CompleteAppear()
    {
        appearCompleted = true;
    }

    private bool ConsumeVanishCompleted()
    {
        if (!vanishCompleted)
        {
            return false;
        }

        vanishCompleted = false;
        return true;
    }

    private bool ConsumeAppearCompleted()
    {
        if (!appearCompleted)
        {
            return false;
        }

        appearCompleted = false;
        return true;
    }

    private void ClearAnimationEventRequests()
    {
        vanishCompleted = false;
        appearCompleted = false;
    }

    private class FollowUpDecisionState : EnemyState
    {
        private readonly CultistAssassinBrain brain;

        public FollowUpDecisionState(Enemy enemy, CultistAssassinBrain brain) : base(enemy)
        {
            this.brain = brain;
        }

        public bool WaitThenEscape { get; private set; }
        public bool UseBackstab { get; private set; }
        public bool UseDefufuAway { get; private set; }
        public bool UseJumpAway { get; private set; }

        /// <summary>
        /// Decision after making an attack. Backstab has first chance, then too-close escape,
        /// then the normal escape choice.
        /// </summary>
        public override void Enter()
        {
            enemy.Stop();

            if (brain.ChooseBackstab())
            {
                UseBackstab = true;
                return;
            }

            if (brain.forceEscapeIfTooClose && brain.IsTooCloseToTarget())
            {
                WaitThenEscape = true;
                return;
            }

            ChooseEscape();
        }

        public override void Exit()
        {
            WaitThenEscape = false;
            UseBackstab = false;
            UseDefufuAway = false;
            UseJumpAway = false;
        }

        private void ChooseEscape()
        {
            UseDefufuAway = brain.ChooseDefufuAway();
            UseJumpAway = !UseDefufuAway;
        }
    }

    private class WaitBeforeEscapeState : EnemyState
    {
        private readonly CultistAssassinBrain brain;
        private float timer;

        public WaitBeforeEscapeState(Enemy enemy, CultistAssassinBrain brain) : base(enemy)
        {
            this.brain = brain;
        }

        public bool IsFinished { get; private set; }
        public bool UseDefufuAway { get; private set; }
        public bool UseJumpAway { get; private set; }

        /// <summary>
        /// Holds the assassin in place briefly before picking a defensive escape.
        /// </summary>
        public override void Enter()
        {
            enemy.Stop();
            timer = brain.tooCloseWaitBeforeEscape;
            IsFinished = false;
            UseDefufuAway = brain.ChooseDefufuAway();
            UseJumpAway = !UseDefufuAway;
        }

        public override void Update()
        {
            if (BlockedByStatus())
            {
                IsFinished = true;
                return;
            }

            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                IsFinished = true;
            }
        }

        public override void FixedUpdate()
        {
            enemy.Stop();
            if (enemy.HasTarget)
            {
                enemy.locomotion.FaceCombatTarget(enemy.Target.position);
            }
        }

        public override void Exit()
        {
            IsFinished = false;
        }
    }

    private class DisappearTeleportState : EnemyState
    {
        private readonly CultistAssassinBrain brain;
        private readonly TeleportDestination destinationType;
        private float timer;

        public DisappearTeleportState(Enemy enemy, CultistAssassinBrain brain, TeleportDestination destinationType) : base(enemy)
        {
            this.brain = brain;
            this.destinationType = destinationType;
        }

        public bool IsFinished { get; private set; }

        /// <summary>
        /// Plays vanish animation, waits until vanish completion, teleports, then finishes.
        /// The following attack state is responsible for the reappear attack animation.
        /// </summary>
        public override void Enter()
        {
            enemy.perception?.SetCombatMode(true);
            enemy.Stop();
            IsFinished = false;
            timer = 0f;
            brain.ClearAnimationEventRequests();

            if (!string.IsNullOrWhiteSpace(brain.disappearAnimTrigger) && enemy.animator != null)
            {
                enemy.animator.SetTrigger(brain.disappearAnimTrigger);
            }
        }

        public override void Update()
        {
            if (BlockedByStatus())
            {
                IsFinished = true;
                return;
            }

            timer += Time.deltaTime;

            if (brain.useVanishAnimationEvents)
            {
                if (brain.ConsumeVanishCompleted())
                {
                    Teleport();
                    IsFinished = true;
                }

                return;
            }

            if (timer >= brain.disappearDuration)
            {
                Teleport();
                IsFinished = true;
            }
        }

        public override void FixedUpdate()
        {
            enemy.Stop();
        }

        private void Teleport()
        {
            if (!enemy.HasTarget)
            {
                return;
            }

            Vector2 targetPosition = enemy.Target.position;
            Vector2 destination = destinationType == TeleportDestination.BehindPlayer
                ? GetBehindPlayerDestination(targetPosition)
                : GetAwayFromPlayerDestination(targetPosition);

            enemy.transform.position = destination;
            enemy.FaceDirection(targetPosition.x - enemy.transform.position.x);
            enemy.Stop();
        }

        private Vector2 GetBehindPlayerDestination(Vector2 targetPosition)
        {
            Player player = Player.instance;
            float playerFacing = player != null ? player.getFacingDirection() : Mathf.Sign(enemy.Target.localScale.x);
            return targetPosition - new Vector2(playerFacing * brain.teleportBehindPlayerDistance, 0f) + brain.teleportBehindOffset;
        }

        private Vector2 GetAwayFromPlayerDestination(Vector2 targetPosition)
        {
            float awayDirection = GetDirectionAwayFromTarget(targetPosition);
            return (Vector2)enemy.transform.position + new Vector2(awayDirection * brain.teleportAwayDistance, 0f) + brain.teleportAwayOffset;
        }

        private float GetDirectionAwayFromTarget(Vector2 targetPosition)
        {
            float xDelta = enemy.transform.position.x - targetPosition.x;
            if (Mathf.Abs(xDelta) <= 0.01f)
            {
                return -enemy.FacingDirection;
            }

            return Mathf.Sign(xDelta);
        }
    }

    private class DisappearTeleportAppearState : EnemyState
    {
        private readonly CultistAssassinBrain brain;
        private readonly TeleportDestination destinationType;
        private float timer;
        private float appearTimer;
        private bool appeared;

        public DisappearTeleportAppearState(Enemy enemy, CultistAssassinBrain brain, TeleportDestination destinationType) : base(enemy)
        {
            this.brain = brain;
            this.destinationType = destinationType;
        }

        public bool IsFinished { get; private set; }

        /// <summary>
        /// Plays vanish animation, teleports after vanish completion, then plays teleport in and waits for appear completion.
        /// </summary>
        public override void Enter()
        {
            enemy.perception?.SetCombatMode(true);
            enemy.Stop();
            IsFinished = false;
            appeared = false;
            timer = 0f;
            appearTimer = 0f;
            brain.ClearAnimationEventRequests();

            if (!string.IsNullOrWhiteSpace(brain.disappearAnimTrigger) && enemy.animator != null)
            {
                enemy.animator.SetTrigger(brain.disappearAnimTrigger);
            }
        }

        public override void Update()
        {
            if (BlockedByStatus())
            {
                IsFinished = true;
                return;
            }

            timer += Time.deltaTime;

            if (brain.useVanishAnimationEvents)
            {
                if (!appeared && brain.ConsumeVanishCompleted())
                {
                    Teleport();
                    StartAppear();
                }
            }
            else
            {
                if (!appeared && timer >= brain.disappearDuration)
                {
                    Teleport();
                    StartAppear();
                }
            }

            if (!appeared)
            {
                return;
            }

            if (brain.useAppearAnimationEvents)
            {
                if (brain.ConsumeAppearCompleted())
                {
                    IsFinished = true;
                }

                return;
            }

            appearTimer += Time.deltaTime;
            if (appearTimer >= brain.appearDuration)
            {
                IsFinished = true;
            }
        }

        /// <summary>
        /// Starts the appear animation after the teleport destination has been applied.
        /// </summary>
        private void StartAppear()
        {
            appeared = true;
            appearTimer = 0f;
            brain.ClearAnimationEventRequests();
            if (!string.IsNullOrWhiteSpace(brain.appearAnimTrigger) && enemy.animator != null)
            {
                enemy.animator.SetTrigger(brain.appearAnimTrigger);
            }
        }

        public override void FixedUpdate()
        {
            enemy.Stop();
        }

        private void Teleport()
        {
            if (!enemy.HasTarget)
            {
                return;
            }

            Vector2 targetPosition = enemy.Target.position;
            Vector2 destination = destinationType == TeleportDestination.BehindPlayer
                ? GetBehindPlayerDestination(targetPosition)
                : GetAwayFromPlayerDestination(targetPosition);

            enemy.transform.position = destination;
            enemy.FaceDirection(targetPosition.x - enemy.transform.position.x);
            enemy.Stop();
        }

        private Vector2 GetBehindPlayerDestination(Vector2 targetPosition)
        {
            Player player = Player.instance;
            float playerFacing = player != null ? player.getFacingDirection() : Mathf.Sign(enemy.Target.localScale.x);
            return targetPosition - new Vector2(playerFacing * brain.teleportBehindPlayerDistance, 0f) + brain.teleportBehindOffset;
        }

        private Vector2 GetAwayFromPlayerDestination(Vector2 targetPosition)
        {
            float awayDirection = GetDirectionAwayFromTarget(targetPosition);
            return (Vector2)enemy.transform.position + new Vector2(awayDirection * brain.teleportAwayDistance, 0f) + brain.teleportAwayOffset;
        }

        private float GetDirectionAwayFromTarget(Vector2 targetPosition)
        {
            float xDelta = enemy.transform.position.x - targetPosition.x;
            if (Mathf.Abs(xDelta) <= 0.01f)
            {
                return -enemy.FacingDirection;
            }

            return Mathf.Sign(xDelta);
        }
    }

    private class JumpAwayState : EnemyState
    {
        private readonly CultistAssassinBrain brain;
        private int jumpsRemaining;
        private float jumpTimer;
        private float pauseTimer;
        private float jumpDirection;

        public JumpAwayState(Enemy enemy, CultistAssassinBrain brain) : base(enemy)
        {
            this.brain = brain;
        }

        public bool IsFinished { get; private set; }

        /// <summary>
        /// Starts one or more backward jumps away from the target.
        /// </summary>
        public override void Enter()
        {
            enemy.perception?.SetCombatMode(true);
            IsFinished = false;
            jumpsRemaining = brain.GetJumpAwayCount();
            jumpTimer = 0f;
            pauseTimer = 0f;
            StartJump();
        }

        /// <summary>
        /// Applies the current jump velocity, waits between jumps, then finishes when all jumps are spent.
        /// </summary>
        public override void FixedUpdate()
        {
            if (BlockedByStatus())
            {
                IsFinished = true;
                return;
            }

            if (jumpTimer > 0f)
            {
                jumpTimer -= Time.fixedDeltaTime;
                enemy.SetVelocity(jumpDirection * brain.jumpAwayVelocity.x, enemy.rb.linearVelocity.y);
                return;
            }

            if (pauseTimer > 0f)
            {
                pauseTimer -= Time.fixedDeltaTime;
                enemy.Stop();
                return;
            }

            if (jumpsRemaining > 0)
            {
                StartJump();
                return;
            }

            enemy.Stop();
            IsFinished = true;
        }

        private void StartJump()
        {
            jumpsRemaining--;
            jumpDirection = GetDirectionAwayFromTarget();
            enemy.FaceDirection(-jumpDirection);
            enemy.SetVelocity(jumpDirection * brain.jumpAwayVelocity.x, brain.jumpAwayVelocity.y);
            jumpTimer = brain.jumpAwayDuration;
            pauseTimer = jumpsRemaining > 0 ? brain.pauseBetweenJumps : 0f;
        }

        private float GetDirectionAwayFromTarget()
        {
            if (!enemy.HasTarget)
            {
                return -enemy.FacingDirection;
            }

            float xDelta = enemy.transform.position.x - enemy.Target.position.x;
            if (Mathf.Abs(xDelta) <= 0.01f)
            {
                return -enemy.FacingDirection;
            }

            return Mathf.Sign(xDelta);
        }
    }
}
