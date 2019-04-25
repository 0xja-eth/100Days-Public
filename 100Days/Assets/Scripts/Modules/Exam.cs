using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExamJsonData : QuestionSetJsonData {
    public ExamResultJsonData result;
    public ExamJsonData(QuestionSetJsonData data):base(data) {}
}
[System.Serializable]
public class ExamResultJsonData {
    public int score;          
    public int finalScore;     
    public int[] subFinScore;  
    public int[][] selections; 
    public long totSpan;
}

public class Exam : QuestionSet {
	string		name;		// 考试名称
	TimeSpan	timeLtd;	// 时间限制
	ExamResult	result;		// 考试结果

	public string getName() {return name;}
	public TimeSpan getTimeLtd() {return timeLtd;}

	public int getScore() {
		return finished ? result.score : 0;
	}
	public int getSubScore(int sid) {
		if(!finished) return 0;
		int bias = 0;
		if(subjectId==9)  bias = 3;
		if(subjectId==10) bias = 6;
		if(bias == 0) return 0;
		return result.subFinScore[sid-bias];
	}
	public int getFinalScore() {
		return finished ? result.finalScore : 0;
	}
	public int getMaxScore() {
		return Subject.MaxScores[subjectId];
	}
    public int[] getSelections(int id) {
        return finished ? result.selections[id] : null;
    }
    public int[] getSelections(Question q) {
        if(!finished) return null;
        int id = findQuestionIndex(q);
        return id >= 0 ? getSelections(id) : null;
    }
    public int findQuestionIndex(Question q) {
        for (int i = 0; i < questions.Length; i++)
            if (questions[i].getId() == q.getId())
                return i;
        return -1;
    }
    public TimeSpan getSpan() {
		if(!finished) return default(TimeSpan);
		return result.totSpan;
	}

	public class ExamResult {
		public int 		score;		// 总分
		public int		finalScore;	// 最终分数
		public int[]	subFinScore;// 子科目分数
		public int[][] 	selections;	// 各题选择记录
		public TimeSpan	totSpan;	// 考试时间
	}

    public new ExamJsonData toJsonData() {
        ExamJsonData data = new ExamJsonData(base.toJsonData());
        data.result = getResultData();
        return data;
    }
    public override bool fromJsonData(QuestionSetJsonData data) {
        return fromJsonData((ExamJsonData)data);
    }
    public bool fromJsonData(ExamJsonData data) {
        if (!base.fromJsonData(data)) return false;
        return loadStatData(data.result);
    }
    public ExamResultJsonData getResultData() {
        ExamResultJsonData data = new ExamResultJsonData();
        data.score = result.score;
        data.finalScore = result.finalScore;
        data.subFinScore = result.subFinScore;
        data.selections = result.selections;
        data.totSpan = result.totSpan.Ticks;
        return data;
    }
    public bool loadStatData(ExamResultJsonData data) {
        result = new ExamResult();
        result.score = data.score;
        result.finalScore = data.finalScore;
        result.subFinScore = data.subFinScore;
        result.selections = data.selections;
        result.totSpan = new TimeSpan(data.totSpan);
        return true;
    }

    // timeLtd : 时间限制（单位：分钟）
    public Exam(string name, int subjectId, int timeLtd,
		DataSystem.QuestionDistribution.Type type, int[] levelDtb,
		Player player = null, DateTime date = default(DateTime)):
		base(0, subjectId, type, levelDtb, player, date) {
			this.name = name;
			this.timeLtd = new TimeSpan(0, timeLtd, 0);
		}

    public Exam(ExamJsonData data, Player player = null) : base(data, player) { }

    public void timeOut(){
		terminate();
		result.totSpan = timeLtd;
	}
	public override void terminate(){
		DateTime now = DateTime.Now;
		TimeSpan span = now - startTime;
		result.totSpan = span;
		base.terminate();
	}

    public void answerQuestions(int[][] selections, TimeSpan[] spans) {
        //DateTime now = DateTime.Now;
        //TimeSpan span = now - startTime;
        for(int i = 0; i < questions.Length; i++) {
            result.selections[i] = selections[i];
            result.score += questions[i].processAnswer(selections[i], spans[i], date);
        }
    }

    protected override void initializeResult(){
		result = new ExamResult();
		result.score = 0;
		result.selections = new int[questions.Length][];
		if(subjectId>=9) result.subFinScore = new int[3];
	}

	public void setFinalScore(int score){
		result.finalScore = score;
	}
	public void setSubFinalScore(int sid,int score){
		int bias = 0;
		if(subjectId==9)  bias = 3;
		if(subjectId==10) bias = 6;
		if(bias == 0) return;
		result.subFinScore[sid-bias] = score;
    }

}
