/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem {

    private static readonly string SAVE_FOLDER = Application.dataPath + "/Saves/";
    private const string SAVE_EXTENSION = "rkmn";

    public static void Init() {
        // Test if Save Folder exists
        if (!Directory.Exists(SAVE_FOLDER)) {
            // Create Save Folder
            Directory.CreateDirectory(SAVE_FOLDER);
        }
    }

    public static void Save(string fileName, string saveString, bool overwrite) {
        Init();
        string saveName = fileName;
        // Make sure the Save Number is unique so it doesnt overwrite a previous save file
        if(!overwrite){
            int saveNumber = 1;
            while (File.Exists(SAVE_FOLDER + saveName + "." + SAVE_EXTENSION)) {
                saveNumber++;
                saveName = fileName + "_" + saveNumber;
            }
            // saveNumber is unique
        }
        File.WriteAllText(SAVE_FOLDER + saveName + "." + SAVE_EXTENSION, saveString);
    }

    public static string Load(string fileName)
    {
        Init();
        if (File.Exists(SAVE_FOLDER + fileName + "." + SAVE_EXTENSION))
        {
            string saveString = File.ReadAllText(SAVE_FOLDER + fileName + "." + SAVE_EXTENSION);
            return saveString;
        }
        else
        {
            return null;
        }
    }

    public static string Load(string fileName, string Save_Folder)
    {
        Init();
        if (File.Exists(SAVE_FOLDER + fileName + "." + SAVE_EXTENSION))
        {
            string saveString = File.ReadAllText(Save_Folder + fileName + "." + SAVE_EXTENSION);
            return saveString;
        }
        else
        {
            return null;
        }
    }

    public static string LoadMostRecentFile() {
        DirectoryInfo directoryInfo = new DirectoryInfo(SAVE_FOLDER);
        // Get all save files
        FileInfo[] saveFiles = directoryInfo.GetFiles("*." + SAVE_EXTENSION);
        // Cycle through all save files and identify the most recent one
        FileInfo mostRecentFile = null;
        foreach (FileInfo fileInfo in saveFiles) {
            if (mostRecentFile == null) {
                mostRecentFile = fileInfo;
            } else {
                if (fileInfo.LastWriteTime > mostRecentFile.LastWriteTime) {
                    mostRecentFile = fileInfo;
                }
            }
        }

        // If theres a save file, load it, if not return null
        if (mostRecentFile != null) {
            string saveString = File.ReadAllText(mostRecentFile.FullName);
            return saveString;
        } else {
            return null;
        }
    }

    public static void SaveObject(object saveObject){
        SaveObject("save", saveObject, false);
    }

    public static void SaveObject(string fileName, object saveObject, bool overwrite){
        Init();
        string json = JsonUtility.ToJson(saveObject);
        Save(fileName, json, overwrite);
    }

    public static TSaveObject LoadMostRecentObject<TSaveObject>(){
        Init();
        string saveString = LoadMostRecentFile();
        if(saveString != null){
            TSaveObject saveObj = JsonUtility.FromJson<TSaveObject>(saveString);
            return saveObj;
        } else {
            return default(TSaveObject);
        }
    }

    public static TSaveObject LoadObject<TSaveObject>(string filename)
    {
        Init();
        string saveString = Load(filename);
        if (saveString != null)
        {
            TSaveObject saveObj = JsonUtility.FromJson<TSaveObject>(saveString);
            return saveObj;
        }
        else
        {
            return default(TSaveObject);
        }
    }

    public static TSaveObject LoadObject<TSaveObject>(string filename, string dir)
    {
        Init();
        string saveString = Load(filename, dir);
        if (saveString != null)
        {
            TSaveObject saveObj = JsonUtility.FromJson<TSaveObject>(saveString);
            return saveObj;
        }
        else
        {
            return default(TSaveObject);
        }
    }
}
