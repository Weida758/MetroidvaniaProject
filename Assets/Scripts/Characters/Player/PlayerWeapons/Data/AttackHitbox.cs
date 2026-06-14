using System;
using UnityEngine;

public enum AttackHitboxShape
{
    Box,
    Circle
}

[System.Serializable]
public class AttackHitbox
{
    [SerializeField] private bool enabled = true;
    [SerializeField] private AttackHitboxShape shape = AttackHitboxShape.Box;

    [Tooltip("Position relative to the player")] 
    [SerializeField]
    private Vector2 localOffset = new Vector2(1f, 0f);
    
    [SerializeField] private Vector2 boxSize = new Vector2(0.75f, 0.75f);
    [SerializeField] private float radius = 0.75f;
    [SerializeField] private float localAngle;
    
    // Where the hitbox is in the world corresponding to the player facing direction
    private Vector2 GetWorldCenter(Player player)
    {
        float facing = player.getFacingDirection();
        Vector2 offset = new Vector2(localOffset.x * facing, localOffset.y);
        return player.rb.position + offset;
    }

    
    private float GetWorldAngle(Player player)
    {
        //mirror the angle when facing left
        return player.getFacingDirection() > 0 ? localAngle : 180f - localAngle;
    }
    
    // Return hit colliders from the atack
    public Collider2D[] GetHits(Player player, int layerMask)
    {
        if(!enabled) return Array.Empty<Collider2D>();

        Vector2 center = GetWorldCenter(player);
        float angle = GetWorldAngle(player);

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
        if (!enabled) return;
        
        Vector2 center = GetWorldCenter(player);
        float angle = GetWorldAngle(player);

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
        {
            
        }
    }
}
