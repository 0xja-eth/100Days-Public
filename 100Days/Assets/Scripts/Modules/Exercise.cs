using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExerciseJsonData : QuestionSetJsonData {
    public bool deleted;
    //public ExerciseResultJsonData result;
    public SubjectJsonDataArray increment;
    public int score;
    public IntArray2D selections;
    public LongArray spans;
    public long totSpan;
    public string name;
    public ExerciseJsonData(QuestionSetJsonData data) : base(data) { }
}
/*
[System.Serializable]
public class ExerciseResultJsonData {
}
*/
[System.Serializable]
public class ExerciseRespondJsonData {
    public QuestionJsonDataArray data;

    public static ExerciseRespondJsonData fromJson(string json) {
        Debug.Log("fromJson: " + json);
        return JsonUtility.FromJson<ExerciseRespondJsonData>(json);
    }
    /*
    public ExamRespondJsonData(RespondJsonData data) {
        Debug.Log("ExamRespondJsonData");
        Debug.Log(data.getJson());
        ExamRespondJsonData self = 
        this.data = self.data;
        Debug.Log(this.data);
        Debug.Log("JSON:" + JsonUtility.ToJson(this.data));
    }*/
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
	bool		   deleted;	// 是否删除
    string         name;    // 刷题命名
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

    public void delete() { deleted = true; }
    public void refresh() { deleted = false; }
    public bool isDeleted() { return deleted; }

    public string generateName() {
        return name = GameSystem.getCurDate().ToString("yyyy 年 MM 月 dd 日\n") + 
            "第 " + GameSystem.getDailyExeCnt() + " 次刷题记录 —— " +
             Subject.SubjectName[getSubjectId()];
    }
    public string getName() { return name; }

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
            Question q = DataSystem.getQuestionById(questions[i]);
            int[] sel = result.selections[i];
            res += (q.isCorrect(sel) ? 1 : 0);
        }
        return res;
    }
    public int getNewQuestionCnt() {
        int res = 0;
        foreach (int qid in questions) {
            Question q = DataSystem.getQuestionById(qid);
            res += q.haveOccurredWhenTerminated() ? 0 : 1;
        }
        return res;
    }
    public int getEnergyCost() {
        int res = 0;
        foreach (int qid in questions) {
            Question q = DataSystem.getQuestionById(qid);
            res += q.getEnergyCost();
        }
        return res;
    }
    public int getPressurePlus() {
        if (!finished) return 0;
        int res = 0;
        for (int i = 0; i < questions.Length; i++) {
            Question q = DataSystem.getQuestionById(questions[i]);
            int[] sel = result.selections[i];
            bool corr = q.isCorrect(sel);
            res += q.getPressurePlus(corr);
        }
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
        data.deleted = deleted;
        getResultData(data);
        return data;
    }
    public override bool fromJsonData(QuestionSetJsonData data) {
        return fromJsonData((ExerciseJsonData)data);
    }
    public bool fromJsonData(ExerciseJsonData data) {
        if (!base.fromJsonData(data)) return false;
        deleted = data.deleted;
        return loadResultData(data);
    }
    public void getResultData(ExerciseJsonData data) {
        int cnt = result.increment.Length;
        data.name = name;
        data.score = result.score;
        data.selections = new IntArray2D(result.selections);
        data.totSpan = result.totSpan.Ticks;
        data.increment = new SubjectJsonDataArray();
        for (int i = 0; i < cnt; i++)
            data.increment.Add(result.increment[i].toJsonData());
        cnt = result.spans.Length;
        data.spans = new LongArray();
        for (int i = 0; i < cnt; i++)
            data.spans.Add(result.spans[i].Ticks);
    }
    public bool loadResultData(ExerciseJsonData data) {
        int cnt = data.increment.Count;
        name = data.name;
        result = new ExerciseResult();
        result.score = data.score;
        result.selections = data.selections.ToArray2D();
        result.totSpan = new TimeSpan(data.totSpan);
        result.increment = new Subject[cnt];
        for (int i = 0; i < cnt; i++)
            result.increment[i] = new Subject(data.increment[i]);
        cnt = data.spans.Count;
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

    public void generateQuestions() {
        Debug.Log("generateQuestions");
        DataSystem.getExerciseQuestions(subjectId, player, count, type);
    }

    // 从数据库中获取到的数据
    public void loadQuestions(ExerciseRespondJsonData data) {

        Debug.Log("loadQuestions");
        Debug.Log(data.data.Count);
        questions = new int[data.data.Count];
        for (int i = 0; i < questions.Length; i++) {
            Question q = DataSystem.addQuestion(new Question(data.data[i]));
            questions[i] = q.getId();
        }
        initializeResult();
    }

    public override void terminate(){
		DateTime now = DateTime.Now;
		TimeSpan span = now - startTime;
		result.totSpan = span;
        filterQuestions();
		dealIncrement();
		base.terminate();
	}
    void filterQuestions() {
        List<int> qs = new List<int>();
        int cnt = questions.Length;
        for (int i = 0; i < cnt; i++) {
            Debug.Log("i = " + i);
            Debug.Log(questions[i]);
            if (result.selections[i] != null)
                qs.Add(questions[i]);
        }
        questions = qs.ToArray();
        Debug.Log("QS:"+qs[0]);
        Debug.Log(questions[questions.Length-1]);
        Debug.Log(questions[0]);
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
        Question q = DataSystem.getQuestionById(questions[qid]);
		result.score += q.processAnswer(selection, span, date);
        dealEnergyCost(qid);
        dealPressurePlus(qid, selection);
    }

    void dealEnergyCost(int qid) {
        Question q = DataSystem.getQuestionById(questions[qid]);
        player.changeEnergy(-q.getEnergyCost());
    }
    void dealPressurePlus(int qid, int[] sel) {
        Question q = DataSystem.getQuestionById(questions[qid]);
        bool corr = q.isCorrect(sel);
        player.changePressure(q.getPressurePlus(corr));
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
        Debug.Log("====== calcIncrement ======");
		int value = player.getSubjectParamValue(subjectId);
        Debug.Log("Value of " + subjectId + " = " + value);
		for(int i=0;i<questions.Length;i++){
			Question q = DataSystem.getQuestionById(questions[i]);
			int[] sel = result.selections[i];
            TimeSpan span = result.spans[i];
            Debug.Log("Question " + i + ": Span: " + span);
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
        Debug.Log("base = " + base_ + ", entry = " + entry);
		base_ *= calcCountEffect(count);
        Debug.Log("calcCountEffect("+count+") = " + base_);
        base_ *= calcValueEffect(value,entry);
        Debug.Log("calcValueEffect(" + value + "," + entry + ") = " + base_);
        base_ *= calcCorrEffect(corr);
        Debug.Log("calcCorrEffect(" + corr + ")  = " + base_);
        base_ *= calcDisperEffect();
        Debug.Log("calcDisperEffect = " + base_);
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
        if (value < entry) return 1;
		return (1-sigmoid(Mathf.Sqrt((value-entry))/FactorK2))*2;
	}
	double calcCorrEffect(bool corr){
		return corr ? FactorC : FactorW;
	}
	double calcDisperEffect(){
		return UnityEngine.Random.Range(100-Dispersion, 100+Dispersion)/100.0;
	}
}
