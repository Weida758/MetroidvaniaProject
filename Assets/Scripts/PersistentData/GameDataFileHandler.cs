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
}
