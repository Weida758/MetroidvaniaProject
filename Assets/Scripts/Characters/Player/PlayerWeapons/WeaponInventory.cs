using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    [Header("Weapon Switch Bindings")]
    [SerializeField] private Weapon slot1;
    [SerializeField] private Weapon slot2;
    [SerializeField] private Weapon slot3;
    [SerializeField] private Weapon slot4;

    [Header("Unlock Flags")] 
    [SerializeField] private bool slot2Unlocked;
    [SerializeField] private bool slot3Unlocked;
    [SerializeField] private bool slot4Unlocked;

    [DisplayOnly] [SerializeField] private string equippedDebug;

    public Weapon currentWeapon { get; private set; }
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        if (slot1 != null) Equip(slot1);
    }

    public void Equip(Weapon weapon)
    {
        if (weapon == null || weapon == currentWeapon) return;
        currentWeapon?.OnUnequip(player);
        currentWeapon = weapon;
        currentWeapon.OnEquip(player);
        equippedDebug = weapon.displayName;
    }

    public bool TryEquipSlot(int slot)
    {
        switch (slot)
        {
            case 1:
                Equip(slot1);
                return slot1 != null;
            case 2:
                if (slot2Unlocked)
                {
                    Equip(slot2);
                    return slot2 != null;
                }
                break;
            case 3 :
                if (slot3Unlocked)
                {
                    Equip(slot3);
                    return slot3 != null;
                }
                break;
            case 4 :
                if (slot4Unlocked)
                {
                    Equip(slot4);
                    return slot4 != null;
                }

                break;
        }

        return false;
    }

    public void SetUnlocked(int slot, bool value)
    {
        switch (slot)
        {
            case 2: slot2Unlocked = value; break;
            case 3: slot3Unlocked = value; break;
            case 4: slot4Unlocked = value; break;
        }
    }

    // No equipped weapon in editor mode yet, so use slot1 for hitbox previews
    public Weapon GetPreviewWeapon() => currentWeapon != null ? currentWeapon : slot1;
}
