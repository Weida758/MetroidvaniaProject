using UnityEngine;

/// <summary>Default action state. Routes input to the equipped weapon's hooks.</summary>
public class NoneAction : ActionState
{
    public NoneAction(StateMachine stateMachine, Player player)
        : base(stateMachine, "ActionNone", player) { }

    public override void Update()
    {
        var weapon = player.inventory != null ? player.inventory.currentWeapon : null;
        if (weapon == null) return;

        // Order matter
        if (player.GetSpecialAttackPressedInput()) weapon.OnSpecialAttackPressed(player);
        if (player.GetSpecialAttackReleasedInput()) weapon.OnSpecialAttackReleased(player);
        if (player.GetAttackPressedInput()) weapon.OnBasicAttack(player);
        // Ability key
        if (player.inputs.magicAttackPressed) weapon.OnAbilityPressed(player);
    }
}


