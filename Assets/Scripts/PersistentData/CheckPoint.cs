using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class CheckPoint : MonoBehaviour
{
    [DisplayOnly] [field: SerializeField] public string sceneName { get; private set; }
    [DisplayOnly] [field: SerializeField] public Vector2 checkPointPosition { get; private set; }

    private bool canSave = false;


    private void Awake()
    {
        sceneName = SceneManager.GetActiveScene().name;
        checkPointPosition = transform.position;
    }


    private void Update()
    {
        if (canSave)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                PersistentDataManager.instance.SaveGame();
                Debug.Log("Saved Game");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        canSave = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        canSave = false;
    }
}
