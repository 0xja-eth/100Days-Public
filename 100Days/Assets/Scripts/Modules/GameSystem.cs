using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSystem {
	static string version = "0.1.1.190313"; 
	static Player player;
	static DateTime curDate;
	static DateTime finalDate;

	const int DeltaDays = 30;

	public static DateTime getCurDate() {return curDate;}
	public static DateTime getFinalDate() {return finalDate;}
	public static Player getPlayer() {return player;}

	public static void initialize(){
        initializeDataSystem();
        initializeRecordSystem();
        initializeGameUtils();
		initializeDate();
		Debug.Log("GameSystem initialized!");
		Debug.Log(curDate);
		Debug.Log(finalDate);
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

	public static void nextDay(){
		curDate.AddDays(1);
		player.recoveryEnergy();
		player.reduceSubjectParams();
	}
}
