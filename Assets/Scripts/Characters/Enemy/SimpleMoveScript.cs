using UnityEngine;

public class SimpleMoveScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Rigidbody2D rb { get; private set; }
    private int direction = 1;
    private Enemy enemydata;

    void Start()
    {
        

        rb = GetComponent<Rigidbody2D>();
        enemydata = GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D floor = Physics2D.Raycast(rb.transform.position + new Vector3(direction, 0,0), Vector2.down, 1.75f, 1 << LayerMask.NameToLayer("Ground"));
        RaycastHit2D wall =Physics2D.Raycast(
            rb.transform.position,
            new Vector2(direction, 0),
            1f,
            1 << LayerMask.NameToLayer("Wall"));
        

        if(!floor || wall){
            direction*=-1;
        }
        if (!enemydata.isSpeared){
            SetVelocity( 5f * direction, rb.linearVelocity.y);
        }
        else{ SetVelocity(0, 0);}
    }
    public void SetVelocity(float xVelocity, float yVelocity)
    {
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
    }

}
