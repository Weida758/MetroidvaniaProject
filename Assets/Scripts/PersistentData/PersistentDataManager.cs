using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PersistentDataManager : MonoBehaviour
{

    [Header("File Storage Config")]
    [SerializeField] private string filename;
    public static PersistentDataManager instance { get; private set; }
    private GameData gameData;
    private GameDataFileHandler dataFileHandler;

    private List<IDataPersistence> persistentDataObjects;

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        dataFileHandler = new GameDataFileHandler(Application.persistentDataPath, filename);
        persistentDataObjects = GetAllPersistentDataGameObjects();
        LoadGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SaveGame();
        }
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void SaveGame()
    {
        foreach (IDataPersistence dataObject in persistentDataObjects)
        {
            dataObject.SaveData(ref gameData);
        }

        dataFileHandler.Save(gameData);
    }

    public void LoadGame()
    {

        gameData = dataFileHandler.Load();
        
        if (gameData == null)
        {
            Debug.Log("No game data to load from");
            NewGame();
            Debug.Log("New game created at: " + Application.persistentDataPath + "/" + filename);
        }

        foreach (IDataPersistence dataObject in persistentDataObjects)
        {
            dataObject.LoadData(gameData);
        }
        
        Debug.Log("Game Data Loaded with player position: " + gameData.playerPositionData);
    }


    /* private void OnApplicationQuit()
    {
        SaveGame();
    }
    */

    public List<IDataPersistence> GetAllPersistentDataGameObjects()
    {
        IEnumerable<IDataPersistence> dataObjects = 
            FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDataPersistence>();
        
        Debug.Log("Number of persistent data: "  + dataObjects.Count());
        
        return new List<IDataPersistence>(dataObjects);
    }
    
    
}
