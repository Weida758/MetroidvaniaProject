using UnityEngine;


[System.Serializable]
public class GameData
{
    public Vector2 playerPositionData;
    
    public GameData()
    {
        this.playerPositionData = new Vector2(0f, 0f);
    }
}
