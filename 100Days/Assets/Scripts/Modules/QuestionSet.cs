using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionSet : IComparable<QuestionSet> {
	protected Player		player;		// 玩家
	protected DateTime		date;		// 开始日期
	protected DateTime		startTime;	// 开始时间（用于计时）
	protected int			count;		// 题目数量
	protected int			subjectId;	// 科目代号（一次刷题仅刷一个科目）
	protected Question[]	questions;	// 题目
	protected bool			finished;	// 是否完成
	protected int[]			levelDtb;	// 题目难度分配

	protected DataSystem.QuestionDistribution.Type type; // 训练类型

  	public int CompareTo(QuestionSet e) {
  		return e.date.CompareTo(date);
    }

	public DateTime getDate() {return date;}
	public DateTime getStartTime() {return startTime;}
	public int getCount() {return count; }
    public int getSubjectId() { return subjectId; }
    public int getQuestionCount() {return questions.Length; }
    public Question getQuestion(int id) { return questions[id]; }

    public int getSumScore() {
        int score = 0;
        foreach (Question q in questions)
            score += q.getScore();
        return score;
    }

    public QuestionSet(int count,  int subjectId, 
		DataSystem.QuestionDistribution.Type type, int[] levelDtb = null,
		Player player = null, DateTime date = default(DateTime)){
		this.date = (date==default(DateTime) ? 
			GameSystem.getCurDate() : date);
		this.player = player ?? GameSystem.getPlayer();
		this.subjectId = subjectId; this.levelDtb = levelDtb; 
		this.count = (levelDtb==null ? count : clacDtbCount());
		this.type = type;
		finished = false;
		createQuestions();
		initializeResult();
	}

	int clacDtbCount(){
		int res = 0;
		foreach(int c in this.levelDtb)
			res += c;
		return res;
	}

	protected virtual void createQuestions(){
		List<Question> ql = new List<Question>();
		foreach(int sid in Subject.DefaultMultSubjectSet[subjectId])
			ql.AddRange(createQuestionsForSubject(sid));
		questions = ql.ToArray();
	}

	protected virtual List<Question> createQuestionsForSubject(int sid){
		List<Question> ql = new List<Question>();
		if(levelDtb == null) 
			ql = DataSystem.getQuestions(sid, player, count, type);
		else for(int i=0;i<levelDtb.Length;i++)
			ql.AddRange(DataSystem.getQuestions(
				sid, player, levelDtb[i], type, i));
		return ql;
	}

	protected virtual void initializeResult(){}

	public virtual void start(){
		startTime = DateTime.Now;
	}
	public virtual void terminate(){
		finished = true;
	}
}
