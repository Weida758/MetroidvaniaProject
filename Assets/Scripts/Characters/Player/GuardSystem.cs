using UnityEngine;

public class GuardSystem : MonoBehaviour
{
    [Header("Prototype Visuals")]
    [SerializeField] private Color parryColor = Color.cyan;
    [SerializeField] private Color blockColor = Color.blue;

    public bool IsBlocking { get; private set; }
    public bool IsParrying { get; private set; }

    private float parryTimer;
    private SpriteRenderer spriteRenderer;
    private Color defaultColor;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
            defaultColor = spriteRenderer.color;
    }

    private void Update()
    {
        if (!IsParrying) return;

        parryTimer -= Time.deltaTime;
        if (parryTimer <= 0f)
        {
            IsParrying = false;
            parryTimer = 0f;
            SetSpriteColor(blockColor);
        }
    }

    public void StartGuard(float parryWindow)
    {
        IsBlocking = true;
        IsParrying = parryWindow > 0f;
        parryTimer = Mathf.Max(0f, parryWindow);
        SetSpriteColor(IsParrying ? parryColor : blockColor);

        Debug.Log(IsParrying ? $"{name} parry window started" : $"{name} started blocking");
    }

    public void StopGuard()
    {
        IsBlocking = false;
        IsParrying = false;
        parryTimer = 0f;
        SetSpriteColor(defaultColor);

        Debug.Log($"{name} stopped guarding");
    }

    public bool TryGuard(SimpleEnemyAttackPattern attack)
    {
        if (attack == null) return false;

        if (IsParrying && attack.IsParryable)
        {
            Debug.Log($"{name} parried {attack.name}");
            attack.OnParried();
            return true;
        }

        if (IsBlocking && attack.IsAttackActive)
        {
            Debug.Log($"{name} blocked {attack.name}");
            return true;
        }

        return false;
    }

    private void SetSpriteColor(Color color)
    {
        if (spriteRenderer != null)
            spriteRenderer.color = color;
    }
}
