using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exercise : QuestionSet {
	/*
	Player		player;		// 玩家
	DateTime	date;		// 开始日期
	DateTime	startTime;	// 开始时间（用于计时）
	int			count;		// 题目数量
	int			subjectId;	// 科目代号（一次刷题仅刷一个科目）
	Question[]	questions;	// 题目
	bool		finished;	// 是否完成
	*/	

	ExerciseResult result;	// 刷题结果

	// 计算因数
	public const double FactorA = 0.7;
	public const int FactorK1 = 5;
	public const int FactorK2 = 25;
	public const double FactorC = 1;
	public const double FactorW = 0.8;		
	public const int Dispersion = 10;	// 增益离散度

  	public int CompareTo(Exercise e) {
  		return e.date.CompareTo(date);
    }

	public Subject[] getIncr() {return finished ? result.increment : null;}
	public int getScore() {return finished ? result.score : 0;}
	public int[] getSelections(int id) {
		return finished ? result.selections[id] : null;
	}
	public TimeSpan getSpan(int id=-1) {
		if(!finished) return default(TimeSpan);
		return id == -1 ? result.totSpan : result.spans[id];
	}

	public class ExerciseResult {
		public Subject[]	increment;	// 科目点数增量
		public int 			score;		// 刷题总分
		public int[][] 		selections;	// 各题选择记录
		public TimeSpan[]	spans;		// 做题时间
		public TimeSpan		totSpan;	// 刷题时间
	}

	public Exercise(int count, int subjectId, 
		DataSystem.QuestionDistribution.Type type, int[] levelDtb = null,
		Player player = null, DateTime date = default(DateTime)):
		base(count, subjectId, type, levelDtb, player, date) {
		/*
		date = (date==default(DateTime) ? 
			GameSystem.getCurDate() : date);
		this.player = player ?? GameSystem.getPlayer();
		this.count = count;
		this.date = date;
		this.type = type;
		finished = false;
		createQuestions();
		initializeReault();
		*/
	}

	public override void terminate(){
		DateTime now = DateTime.Now;
		TimeSpan span = now - startTime;
		result.totSpan = span;
		dealIncrement();
		base.terminate();
	}

	protected override void initializeResult(){
		this.result = new ExerciseResult();
		result.score = 0;
		result.increment = Subject.getStandardSubjects(
			Subject.DefaultMultSubjectSet[subjectId]);
		result.selections = new int[questions.Length][];
		result.spans = new TimeSpan[questions.Length];
	}

	public void answerQuestion(int qid, int[] selection, TimeSpan span){
		//DateTime now = DateTime.Now;
		//TimeSpan span = now - startTime;
		result.spans[qid] = span;
		result.selections[qid] = selection;
		result.score += questions[qid].processAnswer(selection, span, date);
		dealEnergyCost(qid);
	}

	void dealEnergyCost(int qid){
		player.changeEnergy(questions[qid].getEnergyCost());
	}	
	void dealIncrement(){
		resetIncrement(); calcIncrement();
		int[] set = Subject.DefaultMultSubjectSet[subjectId];
		for(int i=0;i<set.Length;i++)
			player.getSubjectParam(set[i]).
				addPoint(result.increment[i]);
	}	
	void resetIncrement(){
		for(int i=0;i<result.increment.Length;i++)
			result.increment[i].resetPoint();
	}
	// 计算科目点数增量
	void calcIncrement(){	
		int value = player.getSubjectParamValue(subjectId);
		for(int i=0;i<questions.Length;i++){
			Question q = questions[i];
			int[] sel = result.selections[i];
			Subject incre = calcQuestionIncre(value, q, sel);
			for(int j=0;j<result.increment.Length;j++)
				result.increment[j].addPoint(incre);
		}
	}
	Subject calcQuestionIncre(int value, Question q, int[] sel){
		Subject sbj = Subject.getStandardSubject(q.getSubjectId());		
		int level = q.getLevel();
		int count = q.getCount();
		bool corr = q.isCorrect(sel);
		double base_ = Question.IncreBase[level];
		int entry = Question.EntryValue[level];
		base_ *= calcCountEffect(count);
		base_ *= calcValueEffect(value,entry);
		base_ *= calcCorrEffect(corr);
		base_ *= calcDisperEffect();
		sbj.addPoint(Mathf.RoundToInt((float)base_));
		return sbj;
	}
	double sigmoid(double x){
		return 1/(1+Mathf.Exp(-(float)x));
	}
	double calcCountEffect(int count){
		return Mathf.Pow((float)FactorA, (float)(count*1.0/FactorK1));
	}
	double calcValueEffect(int value, int entry){
		return (1-sigmoid(Mathf.Sqrt((float)(value-entry))/FactorK2))*2;
	}
	double calcCorrEffect(bool corr){
		return corr ? FactorC : FactorW;
	}
	double calcDisperEffect(){
		return UnityEngine.Random.Range(100-Dispersion, 100+Dispersion)/100.0;
	}
}
