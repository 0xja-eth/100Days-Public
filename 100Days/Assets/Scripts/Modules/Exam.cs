using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExamJsonData : QuestionSetJsonData {
    //public ExamResultJsonData result;

    public int score;
    public int finalScore;
    public IntArray subFinScore;
    public IntArray2D selections;
    public long totSpan;

    public ExamJsonData(QuestionSetJsonData data):base(data) {}
}
/*
[System.Serializable]
public class ExamResultJsonData {
}
*/

public class Exam : QuestionSet {
	string		name;		// 考试名称
    ExamSet     examSet;    // 考试集
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
            if (questions[i] == q.getId())
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
        getResultData(data);
        Debug.Log("ExamJsonData:" + JsonUtility.ToJson(data));
        return data;
    }
    public override bool fromJsonData(QuestionSetJsonData data) {
        return fromJsonData((ExamJsonData)data);
    }
    public bool fromJsonData(ExamJsonData data) {
        if (!base.fromJsonData(data)) return false;
        return loadResultData(data);
    }
    public void getResultData(ExamJsonData data) {
        data.score = result.score;
        data.finalScore = result.finalScore;
        data.subFinScore = new IntArray(result.subFinScore);
        data.selections = new IntArray2D(result.selections);
        data.totSpan = result.totSpan.Ticks;
    }
    public bool loadResultData(ExamJsonData data) {
        result = new ExamResult();
        result.score = data.score;
        result.finalScore = data.finalScore;
        result.subFinScore = data.subFinScore.ToArray();
        result.selections = data.selections.ToArray2D();
        result.totSpan = new TimeSpan(data.totSpan);
        return true;
    }

    // timeLtd : 时间限制（单位：分钟）
    public Exam(string name, int subjectId, int timeLtd,
		DataSystem.QuestionDistribution.Type type, int[] levelDtb,
        ExamSet examSet=null, Player player = null, DateTime date = default(DateTime)):
		base(0, subjectId, type, levelDtb, player, date) {
		this.name = name;
        this.examSet = examSet;
        this.timeLtd = new TimeSpan(0, timeLtd, 0);
    }

    public Exam(ExamJsonData data, ExamSet examSet=null, 
        Player player = null) : base(data, player) { 
            this.examSet = examSet;
        }
    
    public void setQuestions(int[] questions) {
        this.questions = questions;
        initializeResult();
    }

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
            Question q = DataSystem.getQuestionById(questions[i]);
            result.score += q.processAnswer(selections[i], spans[i], date);
        }
    }

    protected override void initializeResult(){
		result = new ExamResult();
		result.score = 0;
		result.selections = new int[questions.Length][];
        if (subjectId >= 9) result.subFinScore = new int[3];
        else result.subFinScore = new int[0];
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
