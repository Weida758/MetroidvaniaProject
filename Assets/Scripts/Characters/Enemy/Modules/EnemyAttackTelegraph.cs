using UnityEngine;

public class EnemyAttackTelegraph : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color telegraphColor = Color.yellow;
    [SerializeField] private Color activeColor = Color.red;
    [SerializeField] private Color recoveryColor = Color.gray;

    private Color defaultColor;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            defaultColor = spriteRenderer.color;
        }
    }

    public void ShowTelegraph()
    {
        SetColor(telegraphColor);
    }

    public void ShowActive()
    {
        SetColor(activeColor);
    }

    public void ShowRecovery()
    {
        SetColor(recoveryColor);
    }

    public void ResetVisual()
    {
        SetColor(defaultColor);
    }

    private void SetColor(Color color)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }
}
