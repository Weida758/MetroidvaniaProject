using UnityEngine;
using System.IO;
using System;
public class GameDataFileHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";


    public GameDataFileHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                    
                }
                
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
                
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to load data from file: " + fullPath +
                               "\n" + e);
            }
        }

        return loadedData;

    }

    public void Save(GameData gameData)
    {
        // Path.Combine for compatibility across different os
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            // Create the directory the file will be written to if it doesn't already exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            
            //Serialize c# game data object into JSON, true for readable format of the json file.
            string dataToStore = JsonUtility.ToJson(gameData, true);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
            
        }
        catch (Exception e)
        {
            Debug.LogError("Error occurred when trying to save data to file: " + fullPath + "\n" + e);
        }
    }
    
    
    
    
    
}
