using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour, IDataPersistence
{
    public static GameManager instance { get; private set; }
    public SecretTracker secretCoinTracker { get; private set; }
    

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        secretCoinTracker = new  SecretTracker();
    }


    public void SaveData(ref GameData gameData)
    {
        gameData.secretCoinsCollected = new List<string>(secretCoinTracker.secrets);
    }

    public void LoadData(GameData gameData)
    {
        foreach(string id in  gameData.secretCoinsCollected)
        {
            secretCoinTracker.CollectSecret(id);
        }
        Debug.Log("SecretCoinTracker has: " +  secretCoinTracker.secrets.Count);
    }
}
