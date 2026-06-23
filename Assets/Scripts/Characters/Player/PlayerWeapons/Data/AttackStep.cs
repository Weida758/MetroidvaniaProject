using UnityEngine;

[System.Serializable]
public class AttackStep
{
    [Header("Identity")]
    [field: SerializeField] public string StepName { get; private set; } = "Swing";

    [Header("Damage")]
    [field: SerializeField] public int Damage { get; private set; } = 1;

    [Header("Timing")]
    [field: SerializeField] public float StartupTime { get; private set; }
    [field: SerializeField] public float ActiveTime { get; private set; } = 0.12f;
    [field: SerializeField] public float ComboInputWindow { get; private set; } = 0.5f;

    [Header("Targeting")]
    [SerializeField] private LayerMask targetLayers;

    [Header("Hitboxes")]
    [field: SerializeField] public AttackHitbox[] Hitboxes { get; private set; } = { new AttackHitbox() };

    [Header("Animation")]
    [field: SerializeField] public string AnimTrigger { get; private set; }

    [Header("Debug")]
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