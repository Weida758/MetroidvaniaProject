using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class PersistentDataManager : MonoBehaviour
{

    [Header("File Storage Config")]
    [SerializeField] private string filename;
    
    public static PersistentDataManager instance { get; private set; }
    
    
    private GameData gameData;
    private GameDataFileHandler dataFileHandler;

    private List<IDataPersistence> persistentDataObjects;

    [SerializeField] private bool isTesting;

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        dataFileHandler = new GameDataFileHandler(Application.persistentDataPath, filename);
    }

    private void Start()
    {
        if (isTesting)
        {
            LoadGame();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void NewGame()
    {
        gameData = new GameData();
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
            NewGame();
            Debug.Log("New game created at: " + Application.persistentDataPath + "/" + filename);
        }

        SceneManager.LoadSceneAsync(gameData.sceneName);


        //Debug.Log("Game Data Loaded with player position: " + gameData.playerPositionData +
        //"in Scene: " + gameData.sceneName);
    }
    
    /// <summary>
    /// Find all objects in the scene that implements IDataPersistence
    /// </summary>
    /// <returns></returns>
    public List<IDataPersistence> GetAllPersistentDataGameObjects()
    {
        IEnumerable<IDataPersistence> dataObjects = 
            FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDataPersistence>();
        
        return new List<IDataPersistence>(dataObjects);
    }


    public void OnSceneLoaded(Scene scene, LoadSceneMode sceneLoadMode)
    {
        
        persistentDataObjects = GetAllPersistentDataGameObjects();
        Debug.Log("Finding Persistent data objects");
        
        foreach (IDataPersistence dataObject in persistentDataObjects)
        {
            dataObject.LoadData(gameData);
        }
    }
    
    
}
