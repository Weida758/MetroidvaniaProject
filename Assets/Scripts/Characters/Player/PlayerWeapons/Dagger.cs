using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Weapons/Dagger", fileName = "Scriptable Objects/Dagger")]
public class DaggerWeapon : Weapon
{
    [TitleGroup("Dash")]
    public float dashSpeed = 20f;
    public float dashTime = 0.2f;
    public float dashCooldown = 1f;

    private float dashCDRemaining;

    public override void OnEquip(Player p)
    {
        base.OnEquip(p);
        dashCDRemaining = 0f;
    }

    public override void WeaponUpdate(ref Player p)
    {
        base.WeaponUpdate(ref p);
        if (dashCDRemaining > 0f) dashCDRemaining -= Time.deltaTime;
    }

    public override bool OnMovementAbilityPressed(Player p)
    {
        if (!DashReady) return false;
        if (p.isDashing) return false;
        if (p.actions == null) return false;
        if (p.actions.currentState is DashAction) return false;

        p.actions.Enter(new DashAction(p.actions.machine, p, dashSpeed, dashTime));
        StartDashCooldown();
        return true;
    }

    public bool DashReady => dashCDRemaining <= 0f;
    public void StartDashCooldown() => dashCDRemaining = dashCooldown;
}