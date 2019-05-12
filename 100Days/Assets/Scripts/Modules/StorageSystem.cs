
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

[System.Serializable]
public class SavefileHeaderJsonData {
    public string name;
    public string school;
    public string saveTime;
    public int restDays;
    public IntArray lastScore;
    public IntArray lastMaxScore;
    public IntArray subjectIds;
    public bool empty = true;
}

[System.Serializable]
public class StorageJsonData {
    public SavefileHeaderJsonDataArray data;
}

public static class StorageSystem {
    const bool NeedEncode = true;
    const string FilePath = "/SaveData/";
    const string FileNameRoot = "savefile";
    const string FileExtendName = ".sav";

    const string FileHeaderPath = "/SaveData/";
    const string FileHeaderNameRoot = "savefile";
    const string FileHeaderExtendName = ".sav";

    const string CachePath = "/CacheData/";
    const string CacheNameRoot = "cache";
    const string CacheExtendName = ".ca";

    const string DefaultSalt = "R0cDovMzgwLTE2Lm1pZAl";

    const string DefaultSavingTipsText = "上传数据中...";
    const string DefaultNewGameTipsText = "创建角色中...";

    public const int MaxSaveFileCount = 3;

    static StorageJsonData savefileInfo;

    static string resendData;

    public static void initialize() {
        Debug.Log("hasStorageData: " + hasStorageData());
        if (hasStorageData())
            savefileInfo = loadStorageData();
        else {
            savefileInfo = new StorageJsonData();
            savefileInfo.data = new SavefileHeaderJsonDataArray(MaxSaveFileCount);
            for (int i = 0; i < MaxSaveFileCount; i++)
                savefileInfo.data.Add(new SavefileHeaderJsonData());
        }
        Debug.Log("savefileInfo: " + savefileInfo);
        Debug.Log("savefileInfo[0]: " + savefileInfo.data[0]);
        Debug.Log("savefileInfo.data.count: " + savefileInfo.data.Count);
        Debug.Log("savefileInfo.JSON: " + JsonUtility.ToJson(savefileInfo));
    }

    #region 文件操作

    // 存储数据到文件
    static void saveDataIntoFile(string data, string path, string name) {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        StreamWriter streamWriter = new StreamWriter(path + name, false);
        streamWriter.Write(data);
        streamWriter.Close();
        streamWriter.Dispose();
    }

    // 从文件读取数据
    static string loadDataFromFile(string path) {
        if (!File.Exists(path)) return "";
        StreamReader streamReader = new StreamReader(path);
        string data = streamReader.ReadToEnd();
        Debug.Log("loadFromFile: " + data);
        streamReader.Close();
        streamReader.Dispose();
        return data;
    }

    // 删除文件
    static void deleteFile(string path) {
        if (File.Exists(path)) File.Delete(path);
    }
    #endregion

    #region 存档信息

    // 获取特定存档信息
    public static SavefileHeaderJsonData getSaveHeaderData(int index) {
        return savefileInfo.data[index];
    }

    // 获取存档信息信息
    public static string getStorageDataInfo() {
        return JsonUtility.ToJson(savefileInfo);
    }
    // 保存存档信息
    public static void saveStorageData() {
        string json = getStorageDataInfo();
        Debug.Log(json);
        if (NeedEncode) json = base64Encode(json);
        saveStorageDataIntoFile(json);
    }
    // 读取存档信息
    public static StorageJsonData loadStorageData() {
        if (!hasStorageData()) return null;
        string json = loadStorageDataFromFile();
        if (NeedEncode) json = base64Decode(json);
        Debug.Log(json);
        return JsonUtility.FromJson<StorageJsonData>(json);
    }
    // 加入存档信息
    public static void addStorageData(int index, GameJsonData saveData) {
        savefileInfo.data[index] = makeHeader(saveData);
        saveStorageData();
    }
    // 删除存档信息
    public static void deleteStorageData(int index) {
        deleteStorageDataFromServer(index);
        savefileInfo.data[index] = null;
        saveStorageData();
    }
    // 删除服务器存档信息
    public static void deleteStorageDataFromServer(int index) {
        string name = savefileInfo.data[index].name;
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        NetworkSystem.clear();
        NetworkSystem.setTipsText("正在删除...");
        NetworkSystem.postRequest(NetworkSystem.DeleteRoute, form);
    }
    // 存档存档信息到文件
    static void saveStorageDataIntoFile(string data) {
        string path = Application.dataPath + FileHeaderPath;
        string name = FileHeaderNameRoot + FileHeaderExtendName;
        saveDataIntoFile(data, path, name);
    }

    // 从文件读取存档信息
    static string loadStorageDataFromFile() {
        string path = Application.dataPath + FileHeaderPath;
        string name = FileHeaderNameRoot + FileHeaderExtendName;
        return loadDataFromFile(path + name);
    }
    // 是否存在存档信息
    public static bool hasStorageData() {
        string path = Application.dataPath + FileHeaderPath +
            FileHeaderNameRoot + FileHeaderExtendName;
        return File.Exists(path);
    }

    // 获取存档信息
    static SavefileHeaderJsonData makeHeader(GameJsonData saveData) {
        SavefileHeaderJsonData data = new SavefileHeaderJsonData();
        DateTime cd = Convert.ToDateTime(saveData.curDate);
        DateTime fd = Convert.ToDateTime(saveData.finalDate);
        TimeSpan span = fd - cd;
        data.name = saveData.player.name;
        data.school = saveData.player.school;
        data.saveTime = saveData.curTime;
        data.restDays = span.Days;
        data.lastScore = new IntArray();
        data.lastMaxScore = new IntArray();
        int cnt = saveData.record.examSetRec.Count;
        ExamSetJsonData examSet = saveData.record.examSetRec[cnt - 1];
        Debug.Log("GameJsonData.LastExamSet:" + JsonUtility.ToJson(examSet));
        Debug.Log("saveData:" + JsonUtility.ToJson(saveData));
        ExamJsonDataArray exams = examSet.exams;
        foreach (ExamJsonData edt in exams) {
            int score = edt.finalScore;
            int maxScore = Subject.MaxScores[edt.subjectId];
            data.lastScore.Add(score);
            data.lastMaxScore.Add(maxScore);
        }
        data.subjectIds = examSet.subjectIds;
        data.empty = false;
        Debug.Log("GameJsonData.LastExamSet:" + JsonUtility.ToJson(examSet));
        Debug.Log("saveData:" + JsonUtility.ToJson(saveData));
        return data;
    }

    #endregion

    #region 存档
    #region 本地层
    // 获取存档信息（JSON字符串）
    public static string getSaveInfo() {
        return JsonUtility.ToJson(GameSystem.toJsonData());
    }
    // 读取存档信息
    public static GameJsonData loadSaveInfo(string json) {
        return JsonUtility.FromJson<GameJsonData>(json);
    }
    // 保存游戏
    public static void saveGame(RequestObject.ErrorAction onError = null) {
        int index = GameSystem.getSaveIndex();
        string json = getSaveInfo(); Debug.Log(json);
        addStorageData(index, GameSystem.toJsonData());
        Debug.Log("Ori:" + json);
        if (NeedEncode) json = base64Encode(json);
        
        Debug.Log("Encode:" + json);
        if (NeedEncode) json = base64Decode(json);
        Debug.Log("Decode:" + json);
        if (NeedEncode) json = base64Encode(json);
        Debug.Log("Encode:" + json);
        saveIntoServer(json, onError);
        saveIntoFile(json, index);
    }

    // 读取游戏
    public static bool loadGame(int index) {
        if (!hasSaveFile(index)) return false;
        string json = loadFromFile(index);
        if (NeedEncode) json = base64Decode(json);
        Debug.Log(json);
        GameSystem.fromJsonData(loadSaveInfo(json));
        return true;
    }
    // 删除存档
    public static void deleteSave(int index) {
        if (!hasSaveFile(index)) return;
        deleteStorageData(index);
        deleteSavefile(index);
    }
    // 存档到文件
    static void saveIntoFile(string data, int index) {
        string path = Application.dataPath + FilePath;
        string name = FileNameRoot + index + FileExtendName;
        saveDataIntoFile(data, path, name);
    }

    // 从文件读取
    static string loadFromFile(int index) {
        string path = Application.dataPath + FilePath;
        string name = FileNameRoot + index + FileExtendName;
        return loadDataFromFile(path + name);
    }

    // 删除存档文件
    public static void deleteSavefile(int index) {
        string path = Application.dataPath + FilePath;
        string name = FileNameRoot + index + FileExtendName;
        deleteFile(path + name);
    }

    // 判断存档文件是否存在
    public static bool hasSaveFile(int index) {
        string path = Application.dataPath + FilePath +
            FileNameRoot + index + FileExtendName;
        return File.Exists(path);
    }
    #endregion

    #region 网络层

    // 保存游戏到服务器
    static void saveIntoServer(string data,
        RequestObject.ErrorAction onError = null) {
        resendData = data;
        onError = onError ?? onSaveFail;
        Player player = GameSystem.getPlayer();
        WWWForm form = new WWWForm();
        form.AddField("save", data);
        form.AddField("name", player.getName());

        NetworkSystem.setErrorHandler(onSaveFail);
        NetworkSystem.setShowLoading(true);
        NetworkSystem.setTipsText(DefaultSavingTipsText);
        NetworkSystem.postRequest(NetworkSystem.SavefileRoute, form);
    }
    // 创建角色到服务器
    public static void registerGameToServer(string name, string school,
        RequestObject.ErrorAction onError = null) {
        onError = onError ?? onRegisterFail;
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("school", school);

        NetworkSystem.setErrorHandler(onError);
        NetworkSystem.setShowLoading(true);
        NetworkSystem.setTipsText(DefaultNewGameTipsText);
        NetworkSystem.postRequest(NetworkSystem.NewGameRoute, form);
    }
    // 保存失败
    static void onSaveFail(RespondStatus status, string errmsg) {
        Debug.LogError(status + " : " + errmsg);
        GameUtils.alert("上传数据失败：" + errmsg,
            new string[] { null, "重试", "取消" },
            new UnityAction[] { null, () => { saveIntoServer(resendData); }, null });
    }
    // 创建角色失败
    static void onRegisterFail(RespondStatus status, string errmsg) {
        Debug.LogError(status + " : " + errmsg);
        GameUtils.alert("用户注册失败：" + errmsg,
            new string[] { null, "关闭" },
            new UnityAction[] { null, null });
    }
    #endregion
    #endregion

    #region 缓存
    // 获取缓存信息（JSON字符串）
    public static string getCacheInfo() {
        return JsonUtility.ToJson(DataSystem.toJsonData());
    }
    // 读取存档信息
    public static CacheJsonData loadCacheInfo(string json) {
        return JsonUtility.FromJson<CacheJsonData>(json);
    }
    // 保存缓存
    public static void saveCache() {
        string json = getCacheInfo();
        Debug.Log(json);
        if (NeedEncode) json = base64Encode(json);
        saveCacheIntoFile(json);
    }
    // 读取缓存
    public static bool loadCache() {
        if (!hasCacheFile()) return false;
        string json = loadCacheFromFile();
        if (NeedEncode) json = base64Decode(json);
        Debug.Log(json);
        DataSystem.fromJsonData(loadCacheInfo(json));
        return true;
    }
    // 储存缓存信息到文件
    static void saveCacheIntoFile(string data) {
        string path = Application.dataPath + CachePath;
        string name = CacheNameRoot + CacheExtendName;
        saveDataIntoFile(data, path, name);
    }
    // 从文件读取缓存信息
    static string loadCacheFromFile() {
        string path = Application.dataPath + CachePath;
        string name = CacheNameRoot + CacheExtendName;
        return loadDataFromFile(path + name);
    }
    // 删除缓存文件
    public static void deteCacheFile() {
        string path = Application.dataPath + CachePath;
        string name = CacheNameRoot + CacheExtendName;
        deleteFile(path + name);
    }
    // 是否存在缓存文件
    public static bool hasCacheFile() {
        string path = Application.dataPath + CachePath +
            CacheNameRoot + CacheExtendName;
        return File.Exists(path);
    }
    #endregion

    #region 编码解码
    // base64编码
    public static string base64Encode(string ori, string salt = DefaultSalt) {
        byte[] bytes = Encoding.UTF8.GetBytes(ori);
        string code = Convert.ToBase64String(bytes, 0, bytes.Length);
        float pos = UnityEngine.Random.Range(0.1f, 0.9f);
        Debug.Log("base64Encode:");
        Debug.Log(code);
        code = code.Insert((int)(code.Length * pos), salt);
        code = randString(DefaultSalt.Length) + code;
        Debug.Log(code);
        return code;
    }
    // base64解码
    public static string base64Decode(string code, string salt = DefaultSalt) {
        Debug.Log("base64Decode:");
        Debug.Log(code);
        code = code.Substring(salt.Length);
        code = code.Replace(salt, "");
        Debug.Log(code);
        byte[] bytes = Convert.FromBase64String(code);
        return Encoding.UTF8.GetString(bytes);
    }
    // 获取随机字符串
    static string randString(int len) {
        string s = "";
        for (int i = 0; i < len; i++) {
            char c = (char)UnityEngine.Random.Range('A', 'Z');
            s += (UnityEngine.Random.Range(0, 2) >= 1) ? Char.ToLower(c) : c;
        }
        return s;
    }
    #endregion
}
