using UnityEngine;
using System.Collections;
public class OneWayPlatform : MonoBehaviour
{
    private GameObject CurentPlatform;
    [SerializeField] private Player player;
    private CapsuleCollider2D playerCollider; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCollider= player.GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetDownPressedInput() == true){
            if(CurentPlatform!=null){
                StartCoroutine(DisableCollision());
            }
        }
        
    }
    private void OnCollisionEnter2D(Collision2D collider){
        if(collider.gameObject.CompareTag("OneWayPlatform")){
            CurentPlatform=collider.gameObject;
        }

    }
    private void OnCollisionExit2D(Collision2D collider ){
        if(collider.gameObject.CompareTag("OneWayPlatform")){
            CurentPlatform=null;
        }

    }
    private IEnumerator DisableCollision(){
        BoxCollider2D platformCollider = CurentPlatform.GetComponent<BoxCollider2D>();
       Physics2D.IgnoreCollision(playerCollider,platformCollider);
       yield return new WaitForSeconds(1f);
       Physics2D.IgnoreCollision(playerCollider,platformCollider,false);

    }
}
