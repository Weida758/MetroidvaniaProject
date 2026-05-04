using UnityEngine;
using Sirenix.OdinInspector;
namespace Characters.Player.PlayerWeapons.Data
{
    [System.Serializable]
    public struct MovementProfile
    {
        [TitleGroup("Speeds") ]
        public float baseSpeed;
        public float sprintSpeed;
        public float jumpVelocity;
        public float gravity;

        [TitleGroup("Capabilities")] 
        public bool canJump;
        public bool canDoubleJump;
        public bool canWallSlide;
        public bool canSprint;

        [TitleGroup("Animation")] 
        public RuntimeAnimatorController animatorOverride;

    
    }
}