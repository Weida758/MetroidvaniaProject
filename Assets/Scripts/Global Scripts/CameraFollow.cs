using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] 
    private GameObject player;
    
    private Transform target;
    private Vector3 velocity = Vector3.zero;
    [Range(0, 1)] public float smoothTime;

    public Vector3 positionOffSet;
    
    void Start()
    {
        target = player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = target.position+positionOffSet;
        transform.position =
            Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

    }
}