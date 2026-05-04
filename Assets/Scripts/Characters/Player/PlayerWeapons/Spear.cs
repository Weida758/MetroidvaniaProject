using Characters.Player.PlayerWeapons.Data;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Spear", fileName = "Scriptable Objects/SpearWeapon")]
public class SpearWeapon : Weapon
{
    public AttackStep[] attacks;
    public float comboWindow = 0.5f;

    public GameObject spearProjectilePrefab;
    public float throwCooldown = 2f;
    public float aimSlowmo = 0.25f;

    public float baseLungeSpeed = 10f;
    public float lungeHeldTimeMax = 1.5f;
    public float lungeHeldTimeMultiplier = 5f;

    public float lightningChainRadius = 2.5f;

    private float throwCDRemaining;

    public override void OnEquip(Player p)
    {
        base.OnEquip(p);
        throwCDRemaining = 0f;
    }

    public override void WeaponUpdate(ref Player p)
    {
        base.WeaponUpdate(ref p);
        if (throwCDRemaining > 0f) {throwCDRemaining -= Time.deltaTime;}
    }
    
    public override bool OnBasicAttack(Player p)              => false;
    public override bool OnSpecialAttackPressed(Player p)     => false;
    public override bool OnSpecialAttackReleased(Player p)    => false;
    public override bool OnMovementAbilityPressed(Player p)  => false;
    public override bool OnMovementAbilityReleased(Player p) => false;
    public override bool OnAbility(Player p)                  => false;

    public bool ThrowReady => throwCDRemaining <= 0f;
    public void StartThrowCooldown() => throwCDRemaining = throwCooldown;
}