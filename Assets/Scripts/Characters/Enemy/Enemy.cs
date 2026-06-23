using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [DisplayOnly] public float lightningCooldown;
    [DisplayOnly] public bool isSpeared;
    [DisplayOnly] public bool isFreezed;
    public string weight;
    [DisplayOnly] public bool stunned=false;
    public Rigidbody2D rb { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(lightningCooldown>0){
            lightningCooldown-=Time.deltaTime;
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
