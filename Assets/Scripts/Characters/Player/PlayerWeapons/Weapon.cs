using Characters.Player.PlayerWeapons;
using UnityEngine;
using Characters.Player.PlayerWeapons.Data;
using Sirenix.OdinInspector;

/// <summary>Base ScriptableObject for weapons. Override input hooks and configure the MovementProfile.</summary>
public abstract class Weapon : ScriptableObject
{
    [TitleGroup("Identity")]
    public string weaponID;
    public string displayName;

    [TitleGroup("Movement")]
    public MovementProfile movement;
    
    [TitleGroup("Basic Attack")]
    [SerializeField] private AttackStep[] basicAttacks;

    //-----life cycle------
    public virtual void OnEquip(Player p)
    {
        if (movement.animatorOverride != null)
        {
            p.animator.runtimeAnimatorController = movement.animatorOverride;
        }

        p.speed = movement.baseSpeed;
    }

    public virtual void OnUnequip(Player p) { }

    public virtual void WeaponUpdate(ref Player p){ }

    public virtual bool OnBasicAttack(Player p)
    {
        if (p.actions.currentState is AttackAction) return false;
        
        p.actions.Enter(new AttackAction(p.actions.machine, p, basicAttacks));
        return true;
    }
    public virtual bool OnSpecialAttackPressed(Player p) => false;
    public virtual bool OnSpecialAttackReleased(Player p) => false;
    public virtual bool OnAbility(Player p) => false;

    public virtual bool OnMovementAbilityPressed(Player p) => false;
    public virtual bool OnMovementAbilityReleased(Player p) => false;

}
