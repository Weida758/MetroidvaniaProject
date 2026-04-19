using UnityEngine;

public class TestingHealthBarDisplay : MonoBehaviour
{
    [SerializeField] private HealthSystem health;
    [SerializeField] private RectTransform fillRect;

    private float maxWidth;

    void Start()
    {
        maxWidth = fillRect.sizeDelta.x;
    }

    void Update()
    {
        float percent = (float)health.GetCurrentHealth() / health.GetMaxHealth();

        fillRect.sizeDelta = new Vector2(
            maxWidth * percent,
            fillRect.sizeDelta.y
        );
    }
}
