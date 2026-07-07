using System;
using UnityEngine;
using Sirenix.OdinInspector;

public enum AttackHitboxShape
{
    Box,
    Circle
}

[System.Serializable]
public class AttackHitbox
{
    [ToggleLeft]
    [SerializeField] private bool enabled = true;

    [EnableIf(nameof(enabled))]
    [EnumToggleButtons]
    [SerializeField] private AttackHitboxShape shape = AttackHitboxShape.Box;

    [EnableIf(nameof(enabled))]
    [Tooltip("Position relative to the attacker")]
    [LabelText("Local Offset")]
    [SerializeField] private Vector2 localOffset = new Vector2(1f, 0f);

    [EnableIf(nameof(enabled))]
    [ShowIf("@shape == AttackHitboxShape.Box")]
    [MinValue(0f)]
    [LabelText("Box Size")]
    [SerializeField] private Vector2 boxSize = new Vector2(0.75f, 0.75f);

    [EnableIf(nameof(enabled))]
    [ShowIf("@shape == AttackHitboxShape.Circle")]
    [MinValue(0f)]
    [LabelText("Radius")]
    [SerializeField] private float radius = 0.75f;

    [EnableIf(nameof(enabled))]
    [ShowIf("@shape == AttackHitboxShape.Box")]
    [SuffixLabel("deg", true)]
    [LabelText("Local Angle")]
    [SerializeField] private float localAngle;

    private Vector2 GetWorldCenter(Transform origin, float facing)
    {
        facing = NormalizeFacing(facing);
        Vector2 offset = new Vector2(localOffset.x * facing, localOffset.y);
        return (Vector2)origin.position + offset;
    }

    private float GetWorldAngle(float facing)
    {
        // Mirror the angle when facing left.
        return NormalizeFacing(facing) > 0f ? localAngle : 180f - localAngle;
    }

    private static float GetFacing(Player player)
    {
        if (Application.isPlaying) return player.getFacingDirection();
        return player.transform.localScale.x < 0f ? -1f : 1f;
    }

    private static float NormalizeFacing(float facing)
    {
        return facing < 0f ? -1f : 1f;
    }

    // Return hit colliders from the attack
    public Collider2D[] GetHits(Player player, int layerMask)
    {
        return GetHits(player.transform, GetFacing(player), layerMask);
    }

    public Collider2D[] GetHits(Transform origin, float facing, int layerMask)
    {
        if (!enabled) return Array.Empty<Collider2D>();

        Vector2 center = GetWorldCenter(origin, facing);
        float angle = GetWorldAngle(facing);

        switch (shape)
        {
            case AttackHitboxShape.Circle:
                return Physics2D.OverlapCircleAll(center, radius, layerMask);
            default:
                return Physics2D.OverlapBoxAll(center, boxSize, angle, layerMask);
        }
    }

    public void DrawGizmos(Player player)
    {
        DrawGizmos(player.transform, GetFacing(player));
    }

    public void DrawGizmos(Transform origin, float facing)
    {
        if (!enabled) return;

        Vector2 center = GetWorldCenter(origin, facing);
        float angle = GetWorldAngle(facing);

        switch (shape)
        {
            case AttackHitboxShape.Circle:
                Gizmos.DrawWireSphere(center, radius);
                break;
            default:
                // Store original gizmos coordinate space
                Matrix4x4 previous = Gizmos.matrix;
                // move and rotate the gizmos coordinate space to the hitbox
                Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.Euler(0f, 0f, angle), Vector3.one);
                Gizmos.DrawWireCube(Vector3.zero, boxSize);
                // After drawing restore the coordinate space
                Gizmos.matrix = previous;
                break;
        }
    }
}
