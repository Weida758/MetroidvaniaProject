using UnityEngine;

namespace Characters.Player.PlayerWeapons
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData", order = 0)]
    public class WeaponData : ScriptableObject
    {
        public Sprite weaponSprite;
        public float baseAttack;
        public float skillCooldown;
    }
}