
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class StorageSystem {
    const bool NeedEncode = true;
    const string FilePath = "/SaveData/";
    const string FileNameRoot = "savefile";
    const string FileExtendName = ".sav";
    const string DefaultSalt = "R0cDovMzgwLTE2Lm1pZAl";

    public const int MaxSaveFileCount = 3;

    public static string getSaveInfo() {
        return JsonUtility.ToJson(GameSystem.toJsonData());
    }
    public static GameJsonData loadSaveInfo(string json) {
        return JsonUtility.FromJson<GameJsonData>(json);
    }
    public static void saveGame() {
        string json = getSaveInfo();
        Debug.Log(json);
        if (NeedEncode) json = base64Encode(json);
        saveIntoFile(json, GameSystem.getSaveIndex());
    }
    public static void loadGame(int index) {
        if (!hasSaveFile(index)) return;
        string json = loadFromFile(index);
        if (NeedEncode) json = base64Decode(json);
        Debug.Log(json);
        GameSystem.fromJsonData(loadSaveInfo(json));
    }

    public static void saveIntoFile(string data, int index) {
        string path = Application.dataPath + FilePath;
        string name = FileNameRoot + index + FileExtendName;
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        StreamWriter streamWriter = new StreamWriter(path + name, false);
        streamWriter.Write(data);
        streamWriter.Close();
        streamWriter.Dispose();
        //AssetDatabase.Refresh();
    }
    public static string loadFromFile(int index) {
        string path = Application.dataPath + FilePath;
        string name = FileNameRoot + index + FileExtendName;
        if (!File.Exists(path+name)) return "";
        StreamReader streamReader = new StreamReader(path+name);
        string data = streamReader.ReadToEnd();
        Debug.Log("loadFromFile: "+data);
        streamReader.Close();
        streamReader.Dispose();
        return data;
    }
    public static bool hasSaveFile(int index) {
        string path = Application.dataPath + FilePath +
            FileNameRoot + index + FileExtendName;
        return File.Exists(path);
    }

    public static string base64Encode(string ori, string salt = DefaultSalt) {
        byte[] bytes = Encoding.UTF8.GetBytes(ori);
        string code = Convert.ToBase64String(bytes, 0, bytes.Length);
        float pos = UnityEngine.Random.Range(0.1f, 0.9f);
        Debug.Log("base64Encode:");
        Debug.Log(code);
        code.Insert((int)(code.Length * pos), salt);
        Debug.Log(code);
        return code;
    }
    public static string base64Decode(string code, string salt = DefaultSalt) {
        Debug.Log("base64Decode:");
        Debug.Log(code);
        code = code.Replace(salt, "");
        Debug.Log(code);
        byte[] bytes = Convert.FromBase64String(code);
        return Encoding.UTF8.GetString(bytes);
    }
}
