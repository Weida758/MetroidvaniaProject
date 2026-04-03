using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GameData
{
    public Vector2 playerPositionData;
    public string sceneName;
    public List<string> secretCoinsCollected;
    
    public GameData()
    {
        playerPositionData = new Vector2(0f, 0f);
        //Starting scene
        sceneName = "TestScene";
        secretCoinsCollected = new List<string>();
    }
}
