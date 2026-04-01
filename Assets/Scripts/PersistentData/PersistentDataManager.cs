using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PersistentDataManager : MonoBehaviour
{
    public static PersistentDataManager instance { get; private set; }
    private GameData gameData;

    public List<IDataPersistence> persistentDataObjects;

    public void Awake()
    {
        if (instance == null && instance != this)
        {
            Destroy(gameObject);
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        persistentDataObjects = GetAllPersistentDataGameObjects();
        LoadGame();
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
        
        Debug.Log("Game Data Saved");
    }

    public void LoadGame()
    {
        if (gameData == null)
        {
            Debug.Log("No game data to load from");
            NewGame();
            Debug.Log("New game created");
        }

        foreach (IDataPersistence dataObject in persistentDataObjects)
        {
            dataObject.LoadData(gameData);
        }
        
        Debug.Log("Game Data Loaded with player position: " + gameData.playerPositionData);
    }


    private void OnApplicationQuit()
    {
        SaveGame();
    }


    public List<IDataPersistence> GetAllPersistentDataGameObjects()
    {
        IEnumerable<IDataPersistence> dataObjects = 
            FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDataPersistence>();
        
        return new List<IDataPersistence>(dataObjects);
    }
}
