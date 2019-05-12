using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ExamScheduleJsonData {
    public string date;
    public string examType;
}

[System.Serializable]
public class GameJsonData {
    public string version;
    public PlayerJsonData player;
    public RecordJsonData record;
    public long playTime;
    public string createDate;
    public string curDate;
    public string finalDate;
    public int saveIndex;
    public string curTime;
    public ExamSetJsonData finalExam;
    public ExamScheduleJsonDataArray examSchedules;
}

[System.Serializable]
public class SchoolRespondData {
    public StringArray data;

    public static SchoolRespondData fromJson(string json) {
        return JsonUtility.FromJson<SchoolRespondData>(json);
    }
}

public class ExamSchedule {
    DateTime date;
    string examType;
    public ExamSchedule(DateTime date, string examType) {
        this.date = date; this.examType = examType;
    }
    public ExamSchedule(int days, string examType) {
        this.date = GameSystem.getFinalDate().AddDays(-(GameSystem.getDeltaDays() - days));
        this.examType = examType;
    }
    public ExamSchedule(ExamScheduleJsonData data) {
        fromJsonData(data);
    }
    public DateTime getDate() {
        return date;
    }
    public ExamSet getExam() {
        Player player = GameSystem.getPlayer();
        return getExam(player.getSubjectIds(), player);
    }
    public ExamSet getExam(int[] subjectIds, Player player = null) {
        switch (examType) {
            case "FirstExam":
                return new FirstExam(subjectIds, player, date);
            case "FinalExam":
                return new FinalExam(subjectIds, player, date);
            case "FirstLunarExam":
                return new FirstLunarExam(subjectIds, player, date);
            case "SecondLunarExam":
                return new SecondLunarExam(subjectIds, player, date);
            case "HunderSchoolExam":
                return new HunderSchoolExam(subjectIds, player, date);
            case "HunderDaysExam":
                return new HunderDaysExam(subjectIds, player, date);
            case "FirstSimExam":
                return new FirstSimExam(subjectIds, player, date);
            case "SecondSimExam":
                return new SecondSimExam(subjectIds, player, date);
            case "ThirdSimExam":
                return new ThirdSimExam(subjectIds, player, date);
            default:
                return null;
        }
    }
    public ExamScheduleJsonData toJsonData() {
        ExamScheduleJsonData data = new ExamScheduleJsonData();
        data.date = date.ToString();
        data.examType = examType;
        return data;
    }
    public void fromJsonData(ExamScheduleJsonData data) {
        date = DateTime.Parse(data.date);
        examType = data.examType;
    }
}

public static class GameSystem {
	public const string version = "0.1.2.190501";

	static Player player;
    static string[] schools;
    static TimeSpan playTime;
    static DateTime createTime;
    static DateTime startTime;
    static DateTime curDate;
    static DateTime finalDate;
    static int saveIndex = 0;
    static int dailyExeCnt = 0;

    static FinalExam finalExam;

    static List<ExamSchedule> examSchedules = new List<ExamSchedule>();

    static bool first = true;

    const int DeltaDays = 30;

    public static int getDeltaDays() { return DeltaDays; }
    public static DateTime getCreateTime() { return createTime; }
    public static TimeSpan getPlayTime() {
        DateTime now = DateTime.Now;
        return playTime + (now - startTime);
    }
    public static DateTime getCurDate() { return curDate; }
	public static DateTime getFinalDate() { return finalDate; }
	public static Player getPlayer() { return player; }
    public static int getDailyExeCnt() { return dailyExeCnt; }
    public static void addDailyExeCnt() { dailyExeCnt++; }

    public static void setFinalExam(FinalExam e) { finalExam = e; }
    public static FinalExam getFinalExam() { return finalExam; }

    public static bool isFirst() { return first; }
    public static void setFirst(bool val) { first = val; }

    public static int getSaveIndex() { return saveIndex; }
    public static void setSaveIndex(int index) { saveIndex = index; }

    public static void setSchools(string[] schools) { GameSystem.schools = schools; }
    public static void setSchools(SchoolRespondData data) {
        setSchools(data.data.ToArray());
    }
    public static string[] getSchools() { return schools; }

    public static GameJsonData toJsonData() {
        GameJsonData data = new GameJsonData();
        data.player = player.toJsonData();
        data.record = RecordSystem.toJsonData();
        data.version = version;
        data.playTime = getPlayTime().Ticks;
        data.createDate = createTime.ToString();
        data.curDate = curDate.ToString();
        data.finalDate = finalDate.ToString();
        data.saveIndex = saveIndex;
        data.curTime = DateTime.Now.ToString();
        if(finalExam != null)
            data.finalExam = finalExam.toJsonData();
        data.examSchedules = new ExamScheduleJsonDataArray();
        foreach (ExamSchedule es in examSchedules)
            data.examSchedules.Add(es.toJsonData());
        return data;
    }
    public static bool fromJsonData(GameJsonData data) {
        //if (version != data.version) return false;
        Debug.Log("fromJsonData:" + data);
        player = new Player(data.player);
        RecordSystem.fromJsonData(data.record);
        playTime = new TimeSpan(data.playTime);
        createTime = Convert.ToDateTime(data.createDate);
        curDate = Convert.ToDateTime(data.curDate);
        finalDate = Convert.ToDateTime(data.finalDate);
        saveIndex = data.saveIndex;
        examSchedules = new List<ExamSchedule>();
        foreach (ExamScheduleJsonData es in data.examSchedules)
            examSchedules.Add( new ExamSchedule(es));
        first = false;
        return true;
    }

    public static void initialize() {
        schools = new string[0];
        initializeDataSystem();
        initializeStorageSystem();
        initializeNetworkSystem();
        //initializeGameUtils();
        Debug.Log("GameSystem initialized!");
    }
    static void initializeExamSchedules() {
        examSchedules = new List<ExamSchedule>();
        examSchedules.Add(new ExamSchedule(0, "FirstExam"));
        examSchedules.Add(new ExamSchedule(4, "FirstLunarExam"));
        examSchedules.Add(new ExamSchedule(8, "SecondLunarExam"));
        examSchedules.Add(new ExamSchedule(DeltaDays / 3, "HunderSchoolExam"));
        examSchedules.Add(new ExamSchedule(DeltaDays / 2, "HunderDaysExam"));
        examSchedules.Add(new ExamSchedule(DeltaDays - 10, "FirstSimExam"));
        examSchedules.Add(new ExamSchedule(DeltaDays - 5, "SecondSimExam"));
        examSchedules.Add(new ExamSchedule(DeltaDays - 2, "ThirdSimExam"));
        examSchedules.Add(new ExamSchedule(DeltaDays, "FinalExam"));
    }
    static void initializeDataSystem() {
        DataSystem.initialize();
    }
    static void initializeStorageSystem() {
        StorageSystem.initialize();
    }
    static void initializeNetworkSystem() {
        NetworkSystem.initialize();
    }
    static void initializeRecordSystem() {
        RecordSystem.initialize();
    }
    static void initializeDate() {
        createTime = DateTime.Now;
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
    public static ExamSet getNextExam() {
        foreach (ExamSchedule es in examSchedules)
            if (curDate <= es.getDate())
                return es.getExam();
        return null;
    }
    public static ExamSet getTodaysExam() {
        foreach (ExamSchedule es in examSchedules)
            if (curDate == es.getDate())
                return es.getExam();
        return null;
    }
    public static void removeSchedule() {
        examSchedules.RemoveAt(0);
    }
    public static bool isEmptySchedule() {
        return examSchedules.Count == 0;
    }

    public static void newGame(int index) {
        first = true;
        initializeDate();
        initializeExamSchedules();
        initializeRecordSystem();
        setSaveIndex(index);
        RecordSystem.setOnLoadFinished(() => {
            SceneManager.LoadScene("StartAnimationScene");
        });
        RecordSystem.loadAllQuestionCount();
    }
    public static void continueGame(int index) {
        RecordSystem.setOnLoadFinished(startGame);
        if (!StorageSystem.loadGame(index)) {
            GameUtils.alert("存档错误！"); return;
        };
    }

    public static void startGame() {
        startTime = DateTime.Now;
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

		player.onNextDay();
	}

    public static void onGameEnd() {
        StorageSystem.saveCache();
        Application.Quit();
    }
}
