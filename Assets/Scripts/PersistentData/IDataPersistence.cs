using UnityEngine;

public interface IDataPersistence
{
    public void SaveData(ref GameData gameData);
    public void LoadData(GameData gameData);
}
