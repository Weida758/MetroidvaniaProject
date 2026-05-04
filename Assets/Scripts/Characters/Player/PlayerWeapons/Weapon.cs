using Characters.Player.PlayerWeapons;
using UnityEngine;
using Characters.Player.PlayerWeapons.Data;

public abstract class Weapon : ScriptableObject
{
    [Header("Identity")] 
    public string weaponID;
    public string displayName;

    [Header("Movement")] 
    public MovementProfile movement;
    
    //-----life cycle------
    public virtual void OnEquip(Player p)
    {
        if (movement.animatorOverride != null)
        {
            p.animator.runtimeAnimatorController = movement.animatorOverride;
        }
    }

    public virtual void OnUnequip(Player p) { }
    
    /// <summary>
    /// Runs every frame
    /// </summary>
    /// <param name="p"></param>
    public virtual void WeaponUpdate(ref Player p){ }

    public virtual bool OnBasicAttack(Player p) => false;
    public virtual bool OnSpecialAttackPressed(Player p) => false;
    public virtual bool OnSpecialAttackReleased(Player p) => false;
    public virtual bool OnAbility(Player p) => false;

    public virtual bool OnMovementAbilityPressed(Player p) => false;
    public virtual bool OnMovementAbilityReleased(Player p) => false;

}
