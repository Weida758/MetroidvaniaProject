using UnityEngine;
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

    [TitleGroup("Basic Attack Preview")]
    [SerializeField] private bool showHitboxPreview = true;

    [TitleGroup("Basic Attack Preview")]
    [ShowIf(nameof(showHitboxPreview))]
    [SerializeField] private bool previewAllAttacks;

    [TitleGroup("Basic Attack Preview")]
    [LabelText("Preview Attack")]
    [ValueDropdown(nameof(GetPreviewAttackChoices))]
    [ShowIf("@showHitboxPreview && !previewAllAttacks")]
    [SerializeField] private int previewAttackIndex;

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

    public virtual void WeaponUpdate(ref Player p) { }

    public virtual bool OnBasicAttack(Player p)
    {
        if (p.actions.currentState is AttackAction) return false;

        p.actions.Enter(new AttackAction(p.actions.machine, p, basicAttacks));
        return true;
    }

    // Draws basic-attack hitboxes in the Scene view
    public virtual void DrawHitboxPreview(Player p)
    {
        if (!showHitboxPreview || basicAttacks == null || basicAttacks.Length == 0) return;

        if (previewAllAttacks)
        {
            foreach (AttackStep step in basicAttacks)
                DrawStep(p, step);
            return;
        }

        int index = Mathf.Clamp(previewAttackIndex, 0, basicAttacks.Length - 1);
        DrawStep(p, basicAttacks[index]);
    }

    protected static void DrawStep(Player p, AttackStep step)
    {
        Gizmos.color = step.DebugColor;
        foreach (AttackHitbox hitbox in step.Hitboxes)
            hitbox.DrawGizmos(p);
    }

    [TitleGroup("Basic Attack Preview")]
    [ButtonGroup("Basic Attack Preview/Nav")]
    [Button("◄ Previous")]
    [ShowIf("@showHitboxPreview && !previewAllAttacks")]
    private void PreviewPrevious() => StepPreview(-1);

    [ButtonGroup("Basic Attack Preview/Nav")]
    [Button("Next ►")]
    [ShowIf("@showHitboxPreview && !previewAllAttacks")]
    private void PreviewNext() => StepPreview(1);
    
    private void StepPreview(int dir)
    {
        if (basicAttacks == null || basicAttacks.Length == 0) return;
        int count = basicAttacks.Length;
        previewAttackIndex = (Mathf.Clamp(previewAttackIndex, 0, count - 1) + dir + count) % count;
    }
    
    private ValueDropdownList<int> GetPreviewAttackChoices()
    {
        ValueDropdownList<int> choices = new ValueDropdownList<int>();
        if (basicAttacks == null || basicAttacks.Length == 0)
        {
            choices.Add("No attacks configured", 0);
            return choices;
        }

        for (int i = 0; i < basicAttacks.Length; i++)
        {
            string label = string.IsNullOrWhiteSpace(basicAttacks[i].StepName) ? "Unnamed" : basicAttacks[i].StepName;
            choices.Add($"{i + 1}: {label}", i);
        }
        return choices;
    }

    public virtual bool OnSpecialAttackPressed(Player p) => false;
    public virtual bool OnSpecialAttackReleased(Player p) => false;
    public virtual bool OnAbilityPressed(Player p) => false;

    public virtual bool OnMovementAbilityPressed(Player p) => false;
    public virtual bool OnMovementAbilityReleased(Player p) => false;
}
