using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(FlyingLocomotion))]
[RequireComponent(typeof(EnemyMeleeAttack))]
public class FlyingChargerBrain : EnemyBrain
{
    [BoxGroup("Attack")]
    [SerializeField] private string chargeAttackMove = "ChargeAttack";

    [BoxGroup("Charge Positioning")]
    [MinValue(0f)]
    [LabelText("Launch Distance")]
    [SerializeField] private float chargeStartDistance = 3f;

    [BoxGroup("Charge Positioning")]
    [MinValue(0f)]
    [LabelText("Minimum Launch Distance")]
    [SerializeField] private float minimumChargeDistance = 2f;

    [BoxGroup("Charge Positioning")]
    [MinValue(0f)]
    [LabelText("Launch Distance Tolerance")]
    [SerializeField] private float chargeDistanceTolerance = 0.35f;

    [BoxGroup("Charge Positioning")]
    [LabelText("Height Offset From Target")]
    [SerializeField] private float chargeHeightOffset;

    [BoxGroup("Charge Positioning")]
    [MinValue(0.01f)]
    [SerializeField] private float verticalAlignmentTolerance = 0.25f;

    [BoxGroup("Cooldown Hover")]
    [MinValue(0f)]
    [LabelText("Vertical Offset From Target")]
    [SerializeField] private float hoverVerticalOffset = 0.6f;

    [BoxGroup("Cooldown Hover")]
    [MinValue(0f)]
    [SerializeField] private float hoverHorizontalRadius = 1.1f;

    [BoxGroup("Cooldown Hover")]
    [MinValue(0f)]
    [SerializeField] private float hoverVerticalRadius = 0.45f;

    [BoxGroup("Cooldown Hover")]
    [MinValue(0.01f)]
    [LabelText("Anchor Follow Responsiveness")]
    [SerializeField] private float hoverAnchorResponsiveness = 2.5f;

    [BoxGroup("Cooldown Hover")]
    [MinValue(0.05f), SuffixLabel("s", true)]
    [SerializeField] private float hoverPointDuration = 0.35f;

    private FlyingLocomotion flyingLocomotion;

    protected override void Build()
    {
        flyingLocomotion = GetComponent<FlyingLocomotion>();

        FlyingPatrolState patrol = new FlyingPatrolState(enemy, flyingLocomotion);
        ChargePositionState positionForCharge = new ChargePositionState(enemy, this, flyingLocomotion);
        FlyingChargeAttackState chargeAttack = new FlyingChargeAttackState(enemy, chargeAttackMove);
        CombatHoverState cooldownHover = new CombatHoverState(enemy, this, flyingLocomotion);

        AddTransition(
            patrol,
            positionForCharge,
            () => enemy.perception != null && enemy.perception.CanSeeTarget(),
            "Saw target");
        AddTransition(
            positionForCharge,
            patrol,
            () => enemy.perception == null || enemy.perception.HasLostTarget(),
            "Lost target");
        AddTransition(positionForCharge, chargeAttack, () => CanBeginCharge(positionForCharge), "Charge lane ready");
        AddTransition(chargeAttack, cooldownHover, () => chargeAttack.IsFinished, "Charge finished");
        AddTransition(
            chargeAttack,
            cooldownHover,
            () => chargeAttack.IsAttackActive && flyingLocomotion.IsTouchingObstacle,
            "Charge hit obstacle");
        AddTransition(
            cooldownHover,
            patrol,
            () => enemy.perception == null || enemy.perception.HasLostTarget(),
            "Lost target after charge");
        AddTransition(cooldownHover, positionForCharge, () => enemy.attackCooldown <= 0f, "Charge cooldown ready");

        SetInitial(patrol);
    }

    public override void Begin()
    {
        if (enemy.perception == null)
        {
            Debug.LogError($"{name} has no perception module for FlyingChargerBrain.", this);
        }

        base.Begin();
    }

    private bool CanBeginCharge(ChargePositionState positionState)
    {
        if (!enemy.CanAct
            || !enemy.HasTarget
            || enemy.attack == null
            || enemy.attackCooldown > 0f
            || !enemy.IsFacingTarget)
        {
            return false;
        }

        float verticalError = Mathf.Abs(transform.position.y - positionState.ChargeHeight);
        if (verticalError > verticalAlignmentTolerance)
        {
            return false;
        }

        float horizontalDistance = enemy.HorizontalDistanceToTarget;
        if (horizontalDistance < minimumChargeDistance
            || horizontalDistance > chargeStartDistance + chargeDistanceTolerance)
        {
            return false;
        }

        Vector2 chargeLaneEnd = new Vector2(enemy.Target.position.x, positionState.ChargeHeight);
        return flyingLocomotion.HasClearPathTo(chargeLaneEnd);
    }

    private Vector2 GetChargeStartPosition(float side, float targetX, float chargeHeight)
    {
        return new Vector2(targetX + side * chargeStartDistance, chargeHeight);
    }

    private float GetSideAwayFromTarget(Vector2 currentPosition, Vector2 targetPosition)
    {
        float xDelta = currentPosition.x - targetPosition.x;
        if (Mathf.Abs(xDelta) > 0.01f)
        {
            return Mathf.Sign(xDelta);
        }

        return -enemy.FacingDirection;
    }

    private class FlyingPatrolState : EnemyState
    {
        private readonly FlyingLocomotion locomotion;

        public FlyingPatrolState(Enemy enemy, FlyingLocomotion locomotion) : base(enemy)
        {
            this.locomotion = locomotion;
        }

        public override void Enter()
        {
            enemy.perception?.SetCombatMode(false);
        }

        public override void FixedUpdate()
        {
            if (!enemy.CanAct)
            {
                locomotion.Stop();
                return;
            }

            locomotion.Patrol();
        }
    }

    private class ChargePositionState : EnemyState
    {
        private readonly FlyingChargerBrain brain;
        private readonly FlyingLocomotion locomotion;
        private float chargeSide;

        public float ChargeHeight { get; private set; }

        public ChargePositionState(
            Enemy enemy,
            FlyingChargerBrain brain,
            FlyingLocomotion locomotion) : base(enemy)
        {
            this.brain = brain;
            this.locomotion = locomotion;
        }

        public override void Enter()
        {
            enemy.perception?.SetCombatMode(true);
            chargeSide = enemy.HasTarget
                ? brain.GetSideAwayFromTarget(enemy.transform.position, enemy.Target.position)
                : -enemy.FacingDirection;
            ChargeHeight = enemy.HasTarget
                ? enemy.Target.position.y + brain.chargeHeightOffset
                : enemy.transform.position.y;
        }

        public override void FixedUpdate()
        {
            if (!enemy.CanAct || !enemy.HasTarget)
            {
                locomotion.Stop();
                return;
            }

            Vector2 targetPosition = enemy.Target.position;
            Vector2 chargePosition = brain.GetChargeStartPosition(chargeSide, targetPosition.x, ChargeHeight);
            locomotion.MoveToCombatPosition(chargePosition);
            locomotion.FaceCombatTarget(targetPosition);
        }
    }

    private class FlyingChargeAttackState : Enemy_AttackState
    {
        public FlyingChargeAttackState(Enemy enemy, string attackMoveName)
            : base(enemy, attackMoveName)
        {
        }

        public override void Enter()
        {
            enemy.locomotion.Stop();
            base.Enter();
        }
    }

    private class CombatHoverState : EnemyState
    {
        private static readonly Vector2[] HoverDirections =
        {
            Vector2.left,
            Vector2.down,
            Vector2.right,
            Vector2.up
        };

        private readonly FlyingChargerBrain brain;
        private readonly FlyingLocomotion locomotion;

        private Vector2 hoverAnchor;
        private float hoverSide;
        private int hoverPointIndex;
        private float hoverPointTimer;

        public CombatHoverState(
            Enemy enemy,
            FlyingChargerBrain brain,
            FlyingLocomotion locomotion) : base(enemy)
        {
            this.brain = brain;
            this.locomotion = locomotion;
        }

        public override void Enter()
        {
            enemy.perception?.SetCombatMode(true);
            enemy.attackCooldown = Mathf.Max(enemy.attackCooldown, enemy.attack.Cooldown);
            locomotion.Stop();

            if (enemy.HasTarget)
            {
                Vector2 targetPosition = enemy.Target.position;
                hoverSide = brain.GetSideAwayFromTarget(enemy.transform.position, targetPosition);
                hoverAnchor = GetDesiredAnchor(targetPosition);
            }
            else
            {
                hoverSide = -enemy.FacingDirection;
                hoverAnchor = enemy.transform.position;
            }

            hoverPointIndex = 0;
            hoverPointTimer = brain.hoverPointDuration;
        }

        public override void FixedUpdate()
        {
            if (!enemy.CanAct)
            {
                locomotion.Stop();
                return;
            }

            if (!enemy.HasTarget)
            {
                locomotion.Stop();
                return;
            }

            Vector2 targetPosition = enemy.Target.position;
            Vector2 desiredAnchor = GetDesiredAnchor(targetPosition);
            float followT = 1f - Mathf.Exp(-brain.hoverAnchorResponsiveness * Time.fixedDeltaTime);
            hoverAnchor = Vector2.Lerp(hoverAnchor, desiredAnchor, followT);

            hoverPointTimer -= Time.fixedDeltaTime;
            if (hoverPointTimer <= 0f)
            {
                hoverPointIndex = (hoverPointIndex + 1) % HoverDirections.Length;
                hoverPointTimer = brain.hoverPointDuration;
            }

            Vector2 direction = HoverDirections[hoverPointIndex];
            float availableHorizontalMovement = Mathf.Max(
                0f,
                brain.chargeStartDistance - brain.minimumChargeDistance);
            float horizontalMovement = Mathf.Min(
                brain.hoverHorizontalRadius,
                availableHorizontalMovement);
            Vector2 hoverOffset = new Vector2(
                direction.x * horizontalMovement,
                direction.y * brain.hoverVerticalRadius);
            Vector2 hoverPosition = hoverAnchor + hoverOffset;

            locomotion.MoveToCombatPosition(hoverPosition);
            locomotion.FaceCombatTarget(targetPosition);
        }

        private Vector2 GetDesiredAnchor(Vector2 targetPosition)
        {
            return targetPosition + new Vector2(
                hoverSide * brain.chargeStartDistance,
                brain.hoverVerticalOffset);
        }
    }
}
