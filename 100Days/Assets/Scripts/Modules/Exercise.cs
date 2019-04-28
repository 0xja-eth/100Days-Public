using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExerciseJsonData : QuestionSetJsonData {
    public ExerciseResultJsonData result;
    public ExerciseJsonData(QuestionSetJsonData data) : base(data) { }
}
[System.Serializable]
public class ExerciseResultJsonData {
    public SubjectJsonData[] increment; 
    public int score;
    public int[][] selections;
    public long[] spans;
    public long totSpan;
}

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

    static readonly string[][] FeelingTexts = {
        new string[] { "感觉没什么提升...", "我是在做无用功吧？","可能得再做多一点题才有效果..."},
        new string[] { "有一点提升，但是总觉得哪里不够...","慢慢进步，总会到达成功的！","这些题目可能效果一般啊~"},
        new string[] { "不错不错，感觉良好。","稳步提升，下次考试能进前十吧？","有点意思~","挺不错的，明天继续！"},
        new string[] { "提升了很多，感觉整个人都充实了。","学到了新知识！","很有意思，下次也要这样做！","太好了！又学到了新知识！"},
        new string[] { "提升很大！再刷一套！","这样下去，下次考试就要考第一名了。怎么办？","爱上刷题了，每次都有那么大收获~"},
        new string[] { "醍醐灌顶！","感觉打开了新世界的大门！","下次考试能考750！","清华北大等我~"}
    };

    // 计算因数
    const double FactorA = 0.7;
	const int FactorK1 = 5;
	const int FactorK2 = 25;
	const double FactorC = 1;
	const double FactorW = 0.8;		
	const int Dispersion = 10;	// 增益离散度
    const double ShortTimeRate = 0.15;

  	public int CompareTo(Exercise e) {
  		return e.date.CompareTo(date);
    }

	public Subject[] getIncr() {return finished ? result.increment : null; }
    public int getScore() { return finished ? result.score : 0; }
    public int[] getSelections(int id) {
		return finished ? result.selections[id] : null;
	}
	public TimeSpan getSpan(int id=-1) {
		if(!finished) return default(TimeSpan);
		return id == -1 ? result.totSpan : result.spans[id];
    }
    public int getCrtCnt() {
        int res = 0;
        for(int i = 0; i < questions.Length; i++) {
            Question q = questions[i];
            int[] sel = result.selections[i];
            res += (q.isCorrect(sel) ? 1 : 0);
        }
        return res;
    }
    public int getNewQuestionCnt() {
        int res = 0;
        foreach (Question q in questions)
            res += q.haveOccurredWhenTerminated() ? 0 : 1;
        return res;
    }
    public int getEnergyCost() {
        int res = 0;
        foreach (Question q in questions)
            res += q.getEnergyCost();
        return res;
    }
    public string generateExerciseFeel() {
        int value = 0, type = 0;
        foreach (Subject s in result.increment)
            value += s.getValue() / result.increment.Length;
        if (value > 10) type = 1;
        if (value > 20) type = 2;
        if (value > 50) type = 3;
        if (value > 100) type = 4;
        if (value > 300) type = 5;
        string[] texts = FeelingTexts[type];
        int id = UnityEngine.Random.Range(0, texts.Length);
        return texts[id];
    }


    public class ExerciseResult {
		public Subject[]	increment;	// 科目点数增量
		public int 			score;		// 刷题总分
		public int[][] 		selections;	// 各题选择记录
		public TimeSpan[]	spans;		// 做题时间
		public TimeSpan		totSpan;	// 刷题时间
	}

    public new ExerciseJsonData toJsonData() {
        ExerciseJsonData data = new ExerciseJsonData(base.toJsonData());
        data.result = getResultData();
        return data;
    }
    public override bool fromJsonData(QuestionSetJsonData data) {
        return fromJsonData((ExerciseJsonData)data);
    }
    public bool fromJsonData(ExerciseJsonData data) {
        if (!base.fromJsonData(data)) return false;
        return loadStatData(data.result);
    }
    public ExerciseResultJsonData getResultData() {
        ExerciseResultJsonData data = new ExerciseResultJsonData();
        int cnt = result.increment.Length;
        data.score = result.score;
        data.selections = result.selections;
        data.totSpan = result.totSpan.Ticks;
        data.increment = new SubjectJsonData[cnt];
        for (int i = 0; i < cnt; i++)
            data.increment[i] = result.increment[i].toJsonData();
        cnt = result.spans.Length;
        data.spans = new long[cnt];
        for (int i = 0; i < cnt; i++)
            data.spans[i] = result.spans[i].Ticks;
        return data;
    }
    public bool loadStatData(ExerciseResultJsonData data) {
        int cnt = data.increment.Length;
        result = new ExerciseResult();
        result.score = data.score;
        result.selections = data.selections;
        result.totSpan = new TimeSpan(data.totSpan);
        result.increment = new Subject[cnt];
        for (int i = 0; i < cnt; i++)
            result.increment[i] = new Subject(data.increment[i]);
        cnt = data.spans.Length;
        result.spans = new TimeSpan[cnt];
        for (int i = 0; i < cnt; i++)
            result.spans[i] = new TimeSpan(data.spans[i]);
        return true;
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
    public Exercise(ExerciseJsonData data, Player player = null) : base(data, player) { }

    public override void terminate(){
		DateTime now = DateTime.Now;
		TimeSpan span = now - startTime;
		result.totSpan = span;
        filterQuestions();
		dealIncrement();
		base.terminate();
	}
    void filterQuestions() {
        List<Question> qs = new List<Question>();
        int cnt = questions.Length;
        for (int i = 0; i < cnt; i++) {
            Debug.Log("i = " + i);
            if (result.selections[i] != null)
                qs.Add(questions[i]);
        }
        questions = qs.ToArray();
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
        if (result.selections[qid] != null) return;
		//DateTime now = DateTime.Now;
		//TimeSpan span = now - startTime;
		result.spans[qid] = span;
		result.selections[qid] = selection;
		result.score += questions[qid].processAnswer(selection, span, date);
		dealEnergyCost(qid);
	}

	void dealEnergyCost(int qid){
		player.changeEnergy(-questions[qid].getEnergyCost());
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
            TimeSpan span = result.spans[i];
			Subject incre = calcQuestionIncre(span, value, q, sel);
			for(int j=0;j<result.increment.Length;j++)
				result.increment[j].addPoint(incre);
		}
	}
	Subject calcQuestionIncre(TimeSpan span, int value, Question q, int[] sel){
		Subject sbj = Subject.getStandardSubject(q.getSubjectId());
        if (sel.Length == 0) return sbj;
        int level = q.getLevel();
        double min = span.TotalMinutes;
		int count = q.getCount();
		bool corr = q.isCorrect(sel);
		double base_ = Question.IncreBase[level];
		int entry = Question.EntryValue[level];
		base_ *= calcCountEffect(count);
		base_ *= calcValueEffect(value,entry);
		base_ *= calcCorrEffect(corr);
		base_ *= calcDisperEffect();
        if (min <= Question.LevelMinMinute[level]) base_ *= ShortTimeRate;
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
