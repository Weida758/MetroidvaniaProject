using UnityEngine;

public class SecretCoin : MonoBehaviour
{
    [SerializeField] private string id;
    [SerializeField] bool canCollect = false;


    public void Start()
    {
        if (GameManager.instance.secretCoinTracker.ContainsSecret(id))
        {
            Debug.Log("Secret already Collected");
            gameObject.SetActive(false);
        }
    }
    
    
    public void Collect()
    {
        GameManager.instance.secretCoinTracker.CollectSecret(id);
        gameObject.SetActive(false);
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (canCollect)
            {
                Collect();
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        canCollect = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        canCollect = false;
    }
    
    
}
