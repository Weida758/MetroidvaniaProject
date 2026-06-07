using UnityEngine;
using Characters.Player.PlayerWeapons.Data;

/// <summary>Base class for locomotion states. Exposes the equipped weapon's MovementProfile and ticks WeaponUpdate.</summary>
public abstract class LocomotionState : CharacterBaseState
{
    protected readonly Player player;

    protected LocomotionState(StateMachine stateMachine, string animBoolName, Player player)
        : base(stateMachine, animBoolName)
    {
        this.player = player;
    }

    /// <summary>Profile from the equipped weapon, or a fallback if none equipped.</summary>
    protected MovementProfile Profile
    {
        get
        {
            if (player.inventory != null && player.inventory.Current != null)
                return player.inventory.Current.movement;
            return new MovementProfile {
                baseSpeed = 7f, sprintSpeed = 12f, jumpVelocity = 10f,
                canJump = true, canWallSlide = true
            };
        }
    }

    public override void Enter()
    {
        base.Enter();
        player.animator.SetBool(animBoolName, true);
    }

    public override void Exit()
    {
        base.Exit();
        player.animator.SetBool(animBoolName, false);
    }

    public override void Update()
    {
        base.Update();
        HandleWeaponSwitch();
        player.speed = player.GetShiftCurrentlyPressed() ? Profile.sprintSpeed : Profile.baseSpeed;
        if (player.walljumptime > 0) player.walljumptime -= Time.deltaTime;
        if (player.inventory != null && player.inventory.Current != null)
            player.inventory.Current.WeaponUpdate(ref RefSelf());
        player.animator.SetFloat("xInput", player.inputs.moveInput.x);
    }

    private Player _selfRef;
    private ref Player RefSelf() { _selfRef = player; return ref _selfRef; }

    private void HandleWeaponSwitch()
    {
        if (player.inventory == null) return;
        if (player.GetOnePressedInput()) player.inventory.TryEquipSlot(1);
        if (player.GetTwoPressedInput()) player.inventory.TryEquipSlot(2);
        if (player.GetThreePressedInput()) player.inventory.TryEquipSlot(3);
        if (player.GetFourPressedInput()) player.inventory.TryEquipSlot(4);
    }

    protected bool WallCheck()
    {
        return Physics2D.Raycast(
            player.rb.transform.position,
            new Vector2(player.getFacingDirection(), 0),
            0.75f,
            1 << LayerMask.NameToLayer("Wall"));
    }
}
