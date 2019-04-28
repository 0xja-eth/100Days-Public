using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GameJsonData {
    public string version;
    public PlayerJsonData player;
    public RecordJsonData record;
    public string curDate;
    public string finalDate;
    public int saveIndex;
    public string curTime;
}

public static class GameSystem {
	public const string version = "0.1.1.190313";

	static Player player;
	static DateTime curDate;
	static DateTime finalDate;
    static int saveIndex = 0;
    static int dailyExeCnt = 0;

    static bool first = true;

    const int DeltaDays = 30;

    public static int getDeltaDays() { return DeltaDays; }
	public static DateTime getCurDate() { return curDate; }
	public static DateTime getFinalDate() { return finalDate; }
	public static Player getPlayer() { return player; }
    public static int getDailyExeCnt() { return dailyExeCnt; }
    public static void addDailyExeCnt() { dailyExeCnt++; }

    public static bool isFirst() { return first; }

    public static int getSaveIndex() { return saveIndex; }
    public static void setSaveIndex(int index) { saveIndex = index; }

    public static GameJsonData toJsonData() {
        GameJsonData data = new GameJsonData();
        data.player = player.toJsonData();
        data.record = RecordSystem.toJsonData();
        data.version = version;
        data.curDate = curDate.ToString();
        data.finalDate = finalDate.ToString();
        data.saveIndex = saveIndex;
        data.curTime = DateTime.Now.ToString();
        return data;
    }
    public static bool fromJsonData(GameJsonData data) {
        if (version != data.version) return false;
        Debug.Log("fromJsonData:" + data);
        player = new Player(data.player);
        RecordSystem.fromJsonData(data.record);
        curDate = Convert.ToDateTime(data.curDate);
        finalDate = Convert.ToDateTime(data.finalDate);
        saveIndex = data.saveIndex;
        first = false;
        return true;
    }

    public static void initialize(){
        initializeDataSystem();
        //initializeGameUtils();
		Debug.Log("GameSystem initialized!");
    }
    static void initializeDataSystem() {
        DataSystem.initialize();
    }
    static void initializeRecordSystem() {
        RecordSystem.initialize();
    }
    static void initializeDate() {
        curDate = DateTime.Today;
        int year = curDate.Year;
        finalDate = new DateTime(year, 6, 7);
        if (finalDate.CompareTo(curDate) < 0)
            // 最终日期早于当前时间
            finalDate = finalDate.AddYears(1);
        curDate = finalDate.AddDays(-DeltaDays);
    }

    /*
    static void initializeGameUtils(){
		GameUtils.initialize();
	}*/

    public static void newGame(int index) {
        first = true;
        initializeDate();
        initializeRecordSystem();
        setSaveIndex(index);
        createPlayer(); // 测试用
        SceneManager.LoadScene("StartAnimationScene");
    }
    public static void continueGame(int index) {
        if (!StorageSystem.loadGame(index)) return;
        SceneManager.LoadScene("GameMainScene");
    }

	public static Player createPlayer(string name, string school, int subjectsType) {
		return player = new Player(name, school, subjectsType);
	}
	public static Player createPlayer() {
        return player = new Player("俪菌俺", "滑男你攻附属中学", 1);
    }

    public static int getDays() {
        TimeSpan span = finalDate - curDate;
        return span.Days;
    }

	public static void nextDay(){
        dailyExeCnt = 0;
        curDate = curDate.AddDays(1);
		player.recoveryEnergy();
		player.reduceSubjectParams();
	}

}
