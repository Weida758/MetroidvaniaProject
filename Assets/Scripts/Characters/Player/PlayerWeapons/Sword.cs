using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Sword", fileName = "Scriptable Objects/Sword")]
public class SwordWeapon : Weapon
{
    [SerializeField] private float parryWindow = 0.2f;

    public override bool OnSpecialAttackPressed(Player p)
    {
        if (p.actions == null) return false;
        if (p.actions.currentState is BlockAction) return false;

        p.actions.Enter(new BlockAction(p.actions.machine, p, parryWindow));
        return true;
    }
}
