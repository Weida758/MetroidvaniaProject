using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public enum FlyingPatrolMode
{
    Hover,
    Waypoints,
    WanderInTerritory
}

[RequireComponent(typeof(Enemy))]
public class FlyingLocomotion : MonoBehaviour, ILocomotion
{
    [BoxGroup("Movement")]
    [MinValue(0f)]
    [SerializeField] private float patrolSpeed = 2f;

    [BoxGroup("Movement")]
    [MinValue(0f)]
    [SerializeField] private float combatMoveSpeed = 3.5f;

    [BoxGroup("Movement")]
    [MinValue(0f), SuffixLabel("units/s2", true)]
    [SerializeField] private float acceleration = 18f;

    [BoxGroup("Movement")]
    [MinValue(0.01f)]
    [SerializeField] private float slowingDistance = 0.75f;

    [BoxGroup("Movement")]
    [MinValue(0.01f)]
    [SerializeField] private float arrivalDistance = 0.12f;

    [BoxGroup("Patrol")]
    [FormerlySerializedAs("useWaypointPatrol")]
    [SerializeField] private FlyingPatrolMode patrolMode = FlyingPatrolMode.WanderInTerritory;

    [BoxGroup("Patrol")]
    [ShowIf(nameof(UsesWaypointPatrol))]
    [SerializeField] private Transform[] patrolPoints = new Transform[0];

    [BoxGroup("Patrol")]
    [ShowIf(nameof(UsesWaypointPatrol))]
    [MinValue(0f), SuffixLabel("s", true)]
    [SerializeField] private float patrolPointPause = 0.35f;

    [BoxGroup("Patrol")]
    [ShowIf(nameof(UsesTerritoryWander))]
    [MinMaxSlider(0f, 3f, true)]
    [LabelText("Wander Pause Range")]
    [SerializeField] private Vector2 wanderPauseRange = new Vector2(0.2f, 0.65f);

    [BoxGroup("Patrol")]
    [ShowIf(nameof(UsesTerritoryWander))]
    [MinValue(0f)]
    [SerializeField] private float wanderEdgePadding = 0.5f;

    [BoxGroup("Territory")]
    [ToggleLeft]
    [SerializeField] private bool useFlightTerritory = true;

    [BoxGroup("Territory")]
    [ShowIf(nameof(useFlightTerritory))]
    [SerializeField] private Vector2 territoryCenterOffset;

    [BoxGroup("Territory")]
    [ShowIf(nameof(useFlightTerritory))]
    [MinValue(0.01f)]
    [SerializeField] private Vector2 territorySize = new Vector2(12f, 7f);

    [BoxGroup("Obstacle Steering")]
    [ToggleLeft]
    [SerializeField] private bool useObstacleSteering = true;

    [BoxGroup("Obstacle Steering")]
    [ShowIf(nameof(useObstacleSteering))]
    [SerializeField] private LayerMask obstacleLayers;

    [BoxGroup("Obstacle Steering")]
    [ShowIf(nameof(useObstacleSteering))]
    [MinValue(0f)]
    [SerializeField] private float obstacleProbeDistance = 0.4f;

    [BoxGroup("Obstacle Steering")]
    [ShowIf(nameof(useObstacleSteering))]
    [MinValue(0.01f)]
    [SerializeField] private float obstacleProbeRadius = 0.2f;

    [BoxGroup("Obstacle Steering")]
    [ShowIf(nameof(useObstacleSteering))]
    [MinValue(0f)]
    [SerializeField] private float obstacleAvoidanceClearance = 0.35f;

    [BoxGroup("Preview")]
    [SerializeField] private Color patrolPathColor = new Color(0.2f, 0.8f, 1f, 0.9f);

    [BoxGroup("Preview")]
    [ShowIf(nameof(useFlightTerritory))]
    [SerializeField] private Color territoryColor = new Color(0.2f, 1f, 0.5f, 0.8f);

    private readonly HashSet<Collider2D> obstacleContacts = new HashSet<Collider2D>();

    private Enemy enemy;
    private Vector2 homePosition;
    private int patrolPointIndex;
    private float patrolPauseTimer;
    private Vector2 wanderTarget;
    private bool hasWanderTarget;
    private Vector2 detourPosition;
    private bool hasDetour;
    private bool detourUsesTerritoryConstraint;

    public bool IsTouchingObstacle => obstacleContacts.Count > 0;

    private bool UsesWaypointPatrol => patrolMode == FlyingPatrolMode.Waypoints;
    private bool UsesTerritoryWander => patrolMode == FlyingPatrolMode.WanderInTerritory;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        homePosition = transform.position;
        GetComponent<Rigidbody2D>().gravityScale = 0f;

        ValidateConfiguration();
    }

    public void Patrol()
    {
        if (patrolMode == FlyingPatrolMode.Hover)
        {
            PatrolHover();
            return;
        }

        if (patrolMode == FlyingPatrolMode.WanderInTerritory)
        {
            PatrolTerritory();
            return;
        }

        PatrolWaypoints();
    }

    private void PatrolHover()
    {
        Vector2 returnPosition = ClampToTerritory(transform.position);
        if (HasReached(returnPosition, false))
        {
            Stop();
            return;
        }

        MoveTowards(returnPosition, patrolSpeed, true, true);
    }

    private void PatrolWaypoints()
    {
        if (patrolPauseTimer > 0f)
        {
            patrolPauseTimer -= Time.fixedDeltaTime;
            Stop();
            return;
        }

        if (!TryGetCurrentPatrolPoint(out Vector2 patrolPosition))
        {
            Stop();
            return;
        }

        if (HasReached(patrolPosition, true))
        {
            Stop();
            AdvancePatrolPoint();
            patrolPauseTimer = patrolPointPause;
            return;
        }

        MoveTowards(patrolPosition, patrolSpeed, true, true);
    }

    private void PatrolTerritory()
    {
        if (!useFlightTerritory)
        {
            Stop();
            return;
        }

        if (patrolPauseTimer > 0f)
        {
            patrolPauseTimer -= Time.fixedDeltaTime;
            Stop();
            return;
        }

        if (!hasWanderTarget)
        {
            wanderTarget = GetRandomTerritoryPosition();
            hasWanderTarget = true;
        }

        if (HasReached(wanderTarget, true))
        {
            Stop();
            hasWanderTarget = false;
            float minPause = Mathf.Min(wanderPauseRange.x, wanderPauseRange.y);
            float maxPause = Mathf.Max(wanderPauseRange.x, wanderPauseRange.y);
            patrolPauseTimer = Random.Range(minPause, maxPause);
            return;
        }

        MoveTowards(wanderTarget, patrolSpeed, true, true);
    }

    public void InCombatMovement(Vector2 targetPosition)
    {
        MoveToCombatPosition(targetPosition);
        FaceCombatTarget(targetPosition);
    }

    public void MoveToCombatPosition(Vector2 destination)
    {
        MoveTowards(destination, combatMoveSpeed, false, false);
    }

    private bool HasReached(Vector2 destination, bool constrainToTerritory)
    {
        Vector2 resolvedDestination = constrainToTerritory
            ? ClampToTerritory(destination)
            : destination;
        return Vector2.Distance(transform.position, resolvedDestination) <= arrivalDistance;
    }

    public Vector2 ClampToTerritory(Vector2 position)
    {
        if (!useFlightTerritory)
        {
            return position;
        }

        Bounds bounds = GetTerritoryBounds();
        return new Vector2(
            Mathf.Clamp(position.x, bounds.min.x, bounds.max.x),
            Mathf.Clamp(position.y, bounds.min.y, bounds.max.y));
    }

    public bool HasClearPathTo(Vector2 destination)
    {
        if (!useObstacleSteering || obstacleLayers.value == 0)
        {
            return true;
        }

        return IsPathClear(transform.position, destination);
    }

    public void FaceCombatTarget(Vector2 targetPosition)
    {
        float xDelta = targetPosition.x - transform.position.x;
        if (Mathf.Abs(xDelta) > 0.01f)
        {
            enemy.FaceDirection(xDelta);
        }
    }

    public void Stop()
    {
        enemy.SetVelocity(0f, 0f);
    }

    private void MoveTowards(
        Vector2 destination,
        float speed,
        bool faceMovement,
        bool constrainToTerritory)
    {
        Vector2 resolvedDestination = constrainToTerritory
            ? ClampToTerritory(destination)
            : destination;
        Vector2 currentPosition = transform.position;
        float finalDistance = Vector2.Distance(currentPosition, resolvedDestination);

        if (finalDistance <= arrivalDistance || speed <= 0f)
        {
            hasDetour = false;
            Stop();
            return;
        }

        Vector2 movementDestination = ResolveMovementDestination(
            resolvedDestination,
            constrainToTerritory);
        Vector2 delta = movementDestination - currentPosition;
        float distance = delta.magnitude;
        if (distance <= 0.001f)
        {
            Stop();
            return;
        }

        Vector2 direction = delta / distance;
        direction = GetSteeredDirection(direction, distance);
        if (direction.sqrMagnitude <= 0.001f)
        {
            Stop();
            return;
        }

        float desiredSpeed = speed * Mathf.Clamp01(distance / slowingDistance);
        Vector2 desiredVelocity = direction.normalized * desiredSpeed;
        Vector2 nextVelocity = Vector2.MoveTowards(
            enemy.rb.linearVelocity,
            desiredVelocity,
            acceleration * Time.fixedDeltaTime);

        enemy.SetVelocity(nextVelocity.x, nextVelocity.y);

        if (faceMovement && Mathf.Abs(nextVelocity.x) > 0.01f)
        {
            enemy.FaceDirection(nextVelocity.x);
        }
    }

    private Vector2 GetSteeredDirection(Vector2 direction, float destinationDistance)
    {
        if (!useObstacleSteering || obstacleLayers.value == 0)
        {
            return direction;
        }

        float speedProbe = enemy.rb.linearVelocity.magnitude * Time.fixedDeltaTime;
        float castDistance = Mathf.Min(destinationDistance, obstacleProbeDistance + speedProbe);
        RaycastHit2D hit = Physics2D.CircleCast(
            transform.position,
            obstacleProbeRadius,
            direction,
            castDistance,
            obstacleLayers);

        if (!hit)
        {
            return direction;
        }

        return direction - Vector2.Dot(direction, hit.normal) * hit.normal;
    }

    private Vector2 ResolveMovementDestination(Vector2 destination, bool constrainToTerritory)
    {
        if (!useObstacleSteering || obstacleLayers.value == 0)
        {
            hasDetour = false;
            return destination;
        }

        Vector2 currentPosition = transform.position;
        if (IsPathClear(currentPosition, destination))
        {
            hasDetour = false;
            return destination;
        }

        if (hasDetour && detourUsesTerritoryConstraint != constrainToTerritory)
        {
            hasDetour = false;
        }

        if (hasDetour)
        {
            float detourReachDistance = Mathf.Max(arrivalDistance * 2f, 0.15f);
            if (Vector2.Distance(currentPosition, detourPosition) > detourReachDistance
                && IsPathClear(currentPosition, detourPosition))
            {
                return detourPosition;
            }

            hasDetour = false;
        }

        RaycastHit2D blockingHit = CastPath(currentPosition, destination);
        if (blockingHit && TryFindDetour(
                destination,
                blockingHit,
                constrainToTerritory,
                out Vector2 nextDetour))
        {
            detourPosition = nextDetour;
            hasDetour = true;
            detourUsesTerritoryConstraint = constrainToTerritory;
            return detourPosition;
        }

        return destination;
    }

    private bool TryFindDetour(
        Vector2 destination,
        RaycastHit2D blockingHit,
        bool constrainToTerritory,
        out Vector2 bestDetour)
    {
        bestDetour = transform.position;
        if (blockingHit.collider == null)
        {
            return false;
        }

        Bounds bounds = blockingHit.collider.bounds;
        float clearance = obstacleProbeRadius + obstacleAvoidanceClearance;
        Vector2[] candidates =
        {
            new Vector2(bounds.min.x - clearance, bounds.min.y - clearance),
            new Vector2(bounds.min.x - clearance, bounds.max.y + clearance),
            new Vector2(bounds.max.x + clearance, bounds.min.y - clearance),
            new Vector2(bounds.max.x + clearance, bounds.max.y + clearance),
            new Vector2(bounds.center.x, bounds.min.y - clearance),
            new Vector2(bounds.center.x, bounds.max.y + clearance),
            new Vector2(bounds.min.x - clearance, bounds.center.y),
            new Vector2(bounds.max.x + clearance, bounds.center.y)
        };

        Vector2 currentPosition = transform.position;
        float bestScore = Mathf.Infinity;
        bool found = false;

        foreach (Vector2 rawCandidate in candidates)
        {
            Vector2 candidate = constrainToTerritory
                ? ClampToTerritory(rawCandidate)
                : rawCandidate;
            if (Vector2.Distance(currentPosition, candidate) <= arrivalDistance
                || Physics2D.OverlapCircle(candidate, obstacleProbeRadius, obstacleLayers) != null
                || !IsPathClear(currentPosition, candidate))
            {
                continue;
            }

            bool reachesDestination = IsPathClear(candidate, destination);
            float score = Vector2.Distance(currentPosition, candidate)
                + Vector2.Distance(candidate, destination);
            if (!reachesDestination)
            {
                score += 1000f;
            }

            if (score >= bestScore)
            {
                continue;
            }

            bestScore = score;
            bestDetour = candidate;
            found = true;
        }

        return found;
    }

    private bool IsPathClear(Vector2 origin, Vector2 destination)
    {
        return !CastPath(origin, destination);
    }

    private RaycastHit2D CastPath(Vector2 origin, Vector2 destination)
    {
        Vector2 delta = destination - origin;
        float distance = delta.magnitude;
        if (distance <= arrivalDistance)
        {
            return default;
        }

        float castDistance = Mathf.Max(0f, distance - obstacleProbeRadius * 0.5f);
        return Physics2D.CircleCast(
            origin,
            obstacleProbeRadius,
            delta / distance,
            castDistance,
            obstacleLayers);
    }

    private Vector2 GetRandomTerritoryPosition()
    {
        Bounds bounds = GetTerritoryBounds();
        float padding = Mathf.Max(wanderEdgePadding, obstacleProbeRadius);
        float minX = bounds.min.x + padding;
        float maxX = bounds.max.x - padding;
        float minY = bounds.min.y + padding;
        float maxY = bounds.max.y - padding;

        if (minX > maxX || minY > maxY)
        {
            return bounds.center;
        }

        return new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
    }

    private bool TryGetCurrentPatrolPoint(out Vector2 position)
    {
        position = transform.position;
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            return false;
        }

        for (int offset = 0; offset < patrolPoints.Length; offset++)
        {
            int index = (patrolPointIndex + offset) % patrolPoints.Length;
            if (patrolPoints[index] == null)
            {
                continue;
            }

            patrolPointIndex = index;
            position = patrolPoints[index].position;
            return true;
        }

        return false;
    }

    private void AdvancePatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            return;
        }

        patrolPointIndex = (patrolPointIndex + 1) % patrolPoints.Length;
    }

    private Bounds GetTerritoryBounds()
    {
        Vector2 size = new Vector2(
            Mathf.Max(0.01f, Mathf.Abs(territorySize.x)),
            Mathf.Max(0.01f, Mathf.Abs(territorySize.y)));
        return new Bounds(homePosition + territoryCenterOffset, size);
    }

    private void ValidateConfiguration()
    {
        if (UsesWaypointPatrol && !HasValidPatrolPoint())
        {
            Debug.LogError($"{name} has no valid flying patrol points.", this);
        }

        if (UsesTerritoryWander && !useFlightTerritory)
        {
            Debug.LogError($"{name} uses territory wander without a flight territory.", this);
        }

        if (useObstacleSteering && obstacleLayers.value == 0)
        {
            Debug.LogWarning($"{name} has flying obstacle steering enabled with an empty layer mask.", this);
        }
    }

    private bool HasValidPatrolPoint()
    {
        if (patrolPoints == null)
        {
            return false;
        }

        foreach (Transform patrolPoint in patrolPoints)
        {
            if (patrolPoint != null)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsObstacleLayer(int layer)
    {
        return (obstacleLayers.value & (1 << layer)) != 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsObstacleLayer(collision.collider.gameObject.layer))
        {
            obstacleContacts.Add(collision.collider);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        obstacleContacts.Remove(collision.collider);
    }

    private void OnDisable()
    {
        obstacleContacts.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        if (UsesWaypointPatrol && patrolPoints != null)
        {
            Gizmos.color = patrolPathColor;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                Transform current = patrolPoints[i];
                Transform next = patrolPoints[(i + 1) % patrolPoints.Length];
                if (current == null)
                {
                    continue;
                }

                Gizmos.DrawWireSphere(current.position, arrivalDistance);
                if (next != null && patrolPoints.Length > 1)
                {
                    Gizmos.DrawLine(current.position, next.position);
                }
            }
        }

        if (Application.isPlaying && UsesTerritoryWander && hasWanderTarget)
        {
            Gizmos.color = patrolPathColor;
            Gizmos.DrawWireSphere(wanderTarget, arrivalDistance);
            Gizmos.DrawLine(transform.position, wanderTarget);
        }

        if (Application.isPlaying && hasDetour)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(detourPosition, obstacleProbeRadius);
            Gizmos.DrawLine(transform.position, detourPosition);
        }

        if (!useFlightTerritory)
        {
            return;
        }

        Vector2 previewHome = Application.isPlaying ? homePosition : (Vector2)transform.position;
        Vector2 size = new Vector2(
            Mathf.Max(0.01f, Mathf.Abs(territorySize.x)),
            Mathf.Max(0.01f, Mathf.Abs(territorySize.y)));

        Gizmos.color = territoryColor;
        Gizmos.DrawWireCube(previewHome + territoryCenterOffset, size);
    }
}
