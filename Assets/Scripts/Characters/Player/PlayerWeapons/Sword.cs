using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Sword", fileName = "Scriptable Objects/Sword")]
public class SwordWeapon : Weapon
{
    [SerializeField] private float parryWindow = 0.2f;
    [TitleGroup("Ability")]
    [SerializeField] private bool showAbilityHitbox;
    [SerializeField] private AttackStep freezeAbility;
    [SerializeField] private float freezeTime;
    
    // For future? I think adding flags for weapon modules that apply multiplier is a 
    // good approach here like mutliplying a freeze time stat before sending it to the constructor
    // of FreezeAttackAction

    public override bool OnSpecialAttackPressed(Player p)
    {
        if (p.actions == null) return false;
        if (p.actions.currentState is BlockAction) return false;

        p.actions.Enter(new BlockAction(p.actions.machine, p, parryWindow));
        return true;
    }

    public override bool OnAbilityPressed(Player p)
    {
        if (p.actions == null) return false;
        
        p.actions.Enter(new FreezeAttackAction(p.actions.machine,  p, freezeAbility, freezeTime));
        return true;
    }

    public override void DrawHitboxPreview(Player p)
    {
        base.DrawHitboxPreview(p);

        if (showAbilityHitbox)
        {
            DrawStep(p, freezeAbility);
        }
    }
    
}
