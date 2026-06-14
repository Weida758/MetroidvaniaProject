using UnityEngine;

[System.Serializable]
public class AttackStep
{
    [field: Header("Identity")]
    [field: SerializeField] public string StepName { get; private set; } = "Swing";

    [field: Header("Damage")]
    [field: SerializeField] public int Damage { get; private set; } = 1;

    [field: Header("Timing")]
    [field: Tooltip("Duration of the hitbox being active")]
    [field: SerializeField] public float ActiveTime { get; private set; } = 0.12f;
    [field: Tooltip("Time you have to advance to next attack")]
    [field: SerializeField] public float ComboInputWindow { get; private set; } = 0.5f;

    [Header("Targeting")]
    [SerializeField] private LayerMask targetLayers;

    [field: Header("Hitboxes")]
    [field: SerializeField] public AttackHitbox[] Hitboxes { get; private set; } = { new AttackHitbox() };

    [field: Header("Animation")]
    [field: SerializeField] public string AnimTrigger { get; private set; }

    [field: Header("Debug")]
    [field: SerializeField] public bool DrawDebug { get; private set; } = true;
    [field: SerializeField] public Color DebugColor { get; private set; } = Color.red;
    [field: SerializeField] public float DebugDrawDuration { get; private set; } = 0.15f;

    public int GetTargetLayerMask()
    {
        if (targetLayers.value != 0) return targetLayers.value;

        int enemyLayer = LayerMask.NameToLayer("Enemy");
        return enemyLayer >= 0 ? 1 << enemyLayer : Physics2D.AllLayers;
    }
}