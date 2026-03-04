using UnityEngine;


public abstract class CharacterBase : MonoBehaviour
{
    protected Animator animator;
    protected Rigidbody2D rb;


    protected virtual void Awake()
    {
        //animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    
    
}
