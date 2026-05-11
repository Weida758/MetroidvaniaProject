using UnityEngine;

public class Enemy : MonoBehaviour
{
    [DisplayOnly] public float lightningCooldown;
    [DisplayOnly] public bool isSpeared;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(lightningCooldown>0){
            lightningCooldown-=Time.deltaTime;
        }
        
    }
    
}
