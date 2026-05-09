using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Weapons/Sword", fileName = "Scriptable Objects/Sword")]
public class SwordWeapon : Weapon
{
    [TitleGroup("Sword")]
    [Tooltip("If true, Shift press toggles sprint on/off. If false, hold Shift to sprint.")]
    public bool tapSprint;

    public override bool OnMovementAbilityPressed(Player p)
    {
        if (!movement.canSprint) return false;

        if (tapSprint)
        {
            p.speed = (p.speed == movement.sprintSpeed)
                ? movement.baseSpeed
                : movement.sprintSpeed;
        }
        else
        {
            p.speed = movement.sprintSpeed;
        }
        return true;
    }

    public override bool OnMovementAbilityReleased(Player p)
    {
        if (!movement.canSprint) return false;
        if (tapSprint) return false;

        p.speed = movement.baseSpeed;
        return true;
    }
}