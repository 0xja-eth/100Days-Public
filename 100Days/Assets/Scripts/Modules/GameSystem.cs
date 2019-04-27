using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameJsonData {
    public string version;
    public PlayerJsonData player;
    public RecordJsonData record;
    public string curDate;
    public string finalDate;
    public int saveIndex;
}

public static class GameSystem {
	public const string version = "0.1.1.190313"; 

	static Player player;
	static DateTime curDate;
	static DateTime finalDate;
    static int saveIndex = 0;

    const int DeltaDays = 30;

	public static DateTime getCurDate() {return curDate;}
	public static DateTime getFinalDate() {return finalDate;}
	public static Player getPlayer() {return player;}

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
        return true;
    }

    public static void initialize(){
        initializeDataSystem();
        initializeRecordSystem();
        initializeGameUtils();
		initializeDate();
		Debug.Log("GameSystem initialized!");
        StorageSystem.loadGame(0);
    }
    static void initializeDataSystem() {
        DataSystem.initialize();
    }
    static void initializeRecordSystem() {
        RecordSystem.initialize();
    }
    static void initializeGameUtils(){
		GameUtils.initialize();
	}

	static void initializeDate(){
		curDate = DateTime.Today;
		int year = curDate.Year;
		finalDate = new DateTime(year, 6, 7);
		if(finalDate.CompareTo(curDate)<0)
		// 最终日期早于当前时间
			finalDate = finalDate.AddYears(1);
		curDate = finalDate.AddDays(-DeltaDays);
	}

	public static Player createPlayer(string name, int subjectsType) {
		return player = new Player(name, subjectsType);
	}
	public static Player createPlayer() {
        return player = new Player("俪菌俺", 1);
    }

    public static int getDays() {
        TimeSpan span = finalDate - curDate;
        return span.Days;
    }

	public static void nextDay(){
        curDate = curDate.AddDays(1);
		player.recoveryEnergy();
		player.reduceSubjectParams();
	}

}
