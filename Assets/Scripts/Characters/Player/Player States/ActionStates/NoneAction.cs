using UnityEngine;


        
/// <summary>
/// Default action state. The action FSM is here when the player isn't
/// attacking, aiming, charging, etc. Routes input to the equipped weapon's hooks.
/// </summary>
public class NoneAction : ActionState
{
    public NoneAction(StateMachine stateMachine, Player player)
        : base(stateMachine, "ActionNone", player) { }

    public override void Update()
    {
        var weapon = player.inventory != null ? player.inventory.Current : null;
        if (weapon == null) return;

        // Order matter
        if (player.GetSpecialAttackPressedInput()) weapon.OnSpecialAttackPressed(player);
        if (player.GetSpecialAttackReleasedInput()) weapon.OnSpecialAttackReleased(player);
        if (player.GetAttackPressedInput()) weapon.OnBasicAttack(player);
        if (player.GetShiftPressedInput()) weapon.OnMovementAbilityPressed(player);
        if (player.GetShiftReleasedInput()) weapon.OnMovementAbilityReleased(player);
        // Ability key
        if (player.inputs.magicAttackPressed) weapon.OnAbility(player);
    }
}


