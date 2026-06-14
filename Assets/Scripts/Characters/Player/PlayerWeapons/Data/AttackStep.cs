using UnityEngine;


[System.Serializable]
public class AttackStep
{
    public float radius = 1f;
    public float range = 2f;

    public int damage;

    public float comboInputWindow = 0.5f;

    public string animTrigger;
}