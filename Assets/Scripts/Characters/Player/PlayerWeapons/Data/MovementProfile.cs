using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public struct MovementProfile
{
    [TitleGroup("Speeds")]
    public float baseSpeed;
    public float sprintSpeed;
    public float jumpVelocity;
    public float gravity;

    [TitleGroup("Capabilities")]
    public bool canJump;
    public bool canWallSlide;

    [TitleGroup("Animation")]
    public RuntimeAnimatorController animatorOverride;
}
