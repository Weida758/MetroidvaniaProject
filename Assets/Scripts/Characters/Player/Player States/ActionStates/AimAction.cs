using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Spear throw aim phase. Slows time, clamps the cursor near screen center, rotates the aim indicator.</summary>
public class AimAction : ActionState
{
    private readonly float slowMotionScale;
    private readonly float warpRadius; 
    private readonly float initialOffset; 

    public AimAction(StateMachine sm, Player player,
                     float slowmoScale = 0.25f,
                     float warpRadius = 40f,
                     float initialOffset = 15f)
        : base(sm, "Aim", player)
    {
        
        this.slowMotionScale = slowmoScale;
        this.warpRadius = warpRadius;
        this.initialOffset = initialOffset;
    }

    public override void Enter()
    {
        base.Enter();
        Time.timeScale = slowMotionScale;
        player.isAiming = true;
        WarpCursorInitial();
    }

    public override void Update()
    {
        base.Update();
        UpdateCursorAndAimRotation();

        if (player.GetSpecialAttackReleasedInput())
            player.inventory.currentWeapon?.OnSpecialAttackReleased(player);
    }

    public override void Exit()
    {
        base.Exit();
        Time.timeScale = 1f;
        player.isAiming = false;
        player.aim.SetActive(false);
    }
    
    public Vector2 GetAimDirection()
    {
        Vector2 center = new Vector2(Screen.width / 2f, Screen.height / 2f);
        return player.GetMousePosition() - center;
    }

    private void WarpCursorInitial()
    {
        Vector2 center = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 warp;
        if (player.GetDownCurrentlyPressed())
            warp = new Vector2(center.x, center.y - initialOffset);
        else if (player.GetUpCurrentlyPressed())
            warp = new Vector2(center.x, center.y + initialOffset);
        else
            warp = new Vector2(center.x + initialOffset * player.getFacingDirection(), center.y);
        Mouse.current.WarpCursorPosition(warp);
    }

    
    private void UpdateCursorAndAimRotation()
    {
        Vector2 center = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 mouse = player.GetMousePosition();
        Vector2 direction = mouse - center;
        
        if (mouse.y > center.y + warpRadius || mouse.x > center.x + warpRadius ||
            mouse.y < center.y - warpRadius || mouse.x < center.x - warpRadius)
        {
            Vector2 warp = new Vector2(center.x + direction.x / 2f, center.y + direction.y / 2f);
            Mouse.current.WarpCursorPosition(warp);
        }

        float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) - 90f;
        if (angle < 0) angle += 360f;
        var e = player.aim.transform.eulerAngles;
        player.aim.transform.eulerAngles = new Vector3(e.x, e.y, angle);
        
        player.aim.SetActive(true);
    }
}