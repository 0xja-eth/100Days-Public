using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestionJsonData {
    public int id;
    public string title;       
    public int level;          
    public string description; 
    public int score;          
    public int type;          
    public int subjectId;      
    public Question.QuestionChoice[] choices;
    //public Question.QuestionStatistics stat;
}

[System.Serializable]
public class QuestionStatJsonData {
    public int id;
    public int count;         
    public int crtCnt;        
    public string lastDate; 
    public long lastTime; 
    public long avgTime;
}

//注： crt 为 correct 缩写
public class Question : IComparable<Question> {
	public enum Type{ Single, Multiple, Judgment, Others };

	public static readonly string[] TypeText = {"单选题","多选题","判断题","其他题"};

    static int ID = 0;

    int     id;             // 题目ID
	string	title;			// 题目题干
	int		level;			// 题目星数
	string	description;	// 题目题解
	int		score;			// 题目分值
	Type	type;			// 题目类型
	int		subjectId;		// 所属科目ID
	List<QuestionChoice>	choices;	// 题目选项
	QuestionStatistics		stat;		// 题目统计

	public const int MaxLevel = 6;	// 总星数
	public static readonly int[] EnergyCost = {6,9,12,16,20,25};	// 每级精力消耗
	public static readonly int[] LevelWeight= {5,12,25,50,75,100};	// 每级初始基数
	public static readonly int[] IncreBase 	= {3,6,15,35,75,100};	// 每级增益基数
	public static readonly int[] EntryValue = {0,0,80,250,750,2000};// 科目点数等级限制
	public static readonly int[] LevelMinute= {1,3,5,6,8,10};		// 难度标准时间
	public const int DefaultScore = 6;		// 选择题分值
	const double UnderSelectFactor = 0.5;	// 多选题少选得分因数

  	public int CompareTo(Question q) {
  		int res = this.subjectId - q.subjectId;
        return res==0 ? this.level - q.level : res;
    }

    public int getId() { return id; }
	public string getTitle() {return title;}
	public int getLevel() {return level;}
	public string getDesc() {return description;}
	public int getScore() {return score;}
	public Type getType() {return type;}
	public int getSubjectId() {return subjectId;}
	public int getChoiceCount() {return choices.Count;}
	public string getChoiceText(int idx) {return choices[idx].text;}
	public bool isChoiceCrt(int idx) {return choices[idx].answer;}

	public int getCrtChoiceCount() {
		int res = 0;
		foreach(QuestionChoice c in choices)
			if(c.answer) res++;
		return res;
	}
	public int[] getCrtSelection() {
		List<int> res = new List<int>();
		for(int i=0;i<choices.Count;i++)
			if(choices[i].answer) res.Add(i);
		return res.ToArray();
	}

	public int getCount() {return stat.count;}
	public int getCrtCnt() {return stat.crtCnt;}
	public double getCrtRate() {return stat.crtRate();}
	public DateTime getLastDate() {return stat.lastDate;}
	public TimeSpan getLastTime() {return stat.lastTime;}
	public TimeSpan getAvgTime() {return stat.avgTime;}

	// 是否做过
	public bool haveOccurred() {return stat.count > 0;}
	// 是否做对过
	public bool haveDone() {return stat.crtCnt > 0;}
	public TimeSpan lastTimeDelta(DateTime date=default(DateTime)) {
		date = (date==default(DateTime) ? 
			GameSystem.getCurDate() : date);
		return date - stat.lastDate;
	}

	public int getEnergyCost() {return EnergyCost[level];}

	public int calcScore(int[] selection){
        if (selection.Length <= 0) return 0;
		if(type == Type.Multiple){ // 是否多选题
			int crtcnt = getCrtChoiceCount();
			foreach(int s in selection)			// 遍历答案选择
				if(isChoiceCrt(s)) crtcnt--;	// 若选择正确，计数-1
				else return 0;	// 选择错误，直接为0分
			// 判断是否少选，计算最后分数
			return crtcnt>0 ? (int)(score*UnderSelectFactor) : score;
		}
		// 不是多选题，直接判断选择答案
		return isChoiceCrt(selection[0]) ? score : 0;
	}

	public bool isCorrect(int[] selection){
		return calcScore(selection) == score;
	}

	public int processAnswer(int[] selection, TimeSpan time, 
		DateTime date=default(DateTime)) {
		date = (date==default(DateTime) ? 
			GameSystem.getCurDate() : date);
		int score = calcScore(selection);
		bool correct = (score == this.score);
		stat.addRecord(date, time, correct);
		return score;
	}

    [System.Serializable]
    public class QuestionChoice {
		public string	text;	// 选项文本
		public bool		answer;	// 是否正确答案
		public QuestionChoice(string text, bool answer=false){
			this.text = text; this.answer = answer;
		}
	}

    public class QuestionStatistics {
		public int count;			// 做题次数
		public int crtCnt;			// 正确次数
		public DateTime lastDate;	// 上次做题日期
		public TimeSpan lastTime;	// 上次用时
		public TimeSpan avgTime;	// 平均用时

		public QuestionStatistics(){
			count = crtCnt = 0;
		}

		public double crtRate() {return crtCnt*1.0/count;}
		public void addRecord(DateTime date, 
			TimeSpan time, bool correct){
			lastDate = date;
			addTimeRecord(time); count++;
			if(correct) crtCnt++; 
		}
		public void addTimeRecord(TimeSpan time){
			lastTime = time;
			long tick = avgTime.Ticks*count;
			tick += time.Ticks; tick /= count+1;
			avgTime = new TimeSpan(tick);
		}
	}
    
    public string toJson() {
        return JsonUtility.ToJson(toJsonData());
    }
    public QuestionJsonData toJsonData() {
        QuestionJsonData data = new QuestionJsonData();
        data.id = id;
        data.title = title;
        data.level = level;
        data.description = description;
        data.score = score;
        data.type = (int)type;
        data.subjectId = subjectId;
        data.choices = choices.ToArray();
        //data.stat = stat;
        return data;
    }
    public bool fromJsonData(QuestionJsonData data) {
        id = data.id;
        title = data.title;
        level = data.level;
        description = data.description;
        score = data.score;
        type = (Type)data.type;
        subjectId = data.subjectId;
        choices = new List<QuestionChoice>(data.choices);
        //stat = data.stat;
        return true;
    }
    public QuestionStatJsonData getStatData() {
        QuestionStatJsonData data = new QuestionStatJsonData();
        data.id = id;
        data.count = stat.count;
        data.crtCnt = stat.crtCnt;
        data.lastDate = stat.lastDate.ToString();
        data.lastTime = stat.lastTime.Ticks;
        data.avgTime = stat.avgTime.Ticks;
        return data;
    }
    public bool loadStatData(QuestionStatJsonData data) {
        stat = new QuestionStatistics();
        stat.count = data.count;
        stat.crtCnt = data.crtCnt;
        stat.lastDate = Convert.ToDateTime(data.lastDate);
        stat.lastTime = new TimeSpan(data.lastTime);
        stat.avgTime = new TimeSpan(data.avgTime);
        return true;
    }

    public Question(string title, int level, string desc, 
		int score, int sid, Type type=Type.Single){
        this.id = ++ID;
        this.title = title;
		this.level = level;
		this.description = desc;
		this.score = score;
		this.subjectId = sid;
		this.type = type;
		this.choices = new List<QuestionChoice>();
		this.stat = new QuestionStatistics();
	}

	public void addChoice(QuestionChoice choice){
		choices.Add(choice);
    }
    public void addChoice(string text, bool answer = false) {
        addChoice(new QuestionChoice(text, answer));
    }
    public void changeChoiceAnswer(int index, bool answer = true) {
        choices[index].answer = answer;
    }
}


