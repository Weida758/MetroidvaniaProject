using UnityEngine;
using System.Collections;

public class TESTENEMY : MonoBehaviour
{
    [DisplayOnly] public float lightningCooldown;
    [DisplayOnly] public bool isSpeared;
    [DisplayOnly] public bool isFreezed;
    [DisplayOnly] public bool isMarked;
    public string weight;
    [DisplayOnly] public bool stunned = false;
    public Rigidbody2D rb { get; private set; }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (lightningCooldown > 0)
        {
            lightningCooldown -= Time.deltaTime;
        }
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
    }

    public IEnumerator Stun(float time)
    {
        stunned = true;
        yield return new WaitForSeconds(time);
        stunned = false;
    }

    public IEnumerator Freeze(float time)
    {
        isFreezed = true;
        yield return new WaitForSeconds(time);
        isFreezed = false;
    }
}
