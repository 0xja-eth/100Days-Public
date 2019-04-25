using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExamSetJsonData {
    public string name;      
    public string date;    
    public int[] subjectIds; 
    public ExamJsonData[] exams; 
                                
    public bool finished;    
    public double difficulty;
    public int baseValue;    
    public double dependence;
}

public class ExamSet : IComparable<ExamSet> {
    protected Player player;        // 玩家
    protected string name;      // 考试总名称
    protected DateTime date;        // 开始日期	
    protected int[] subjectIds; // 科目代号
    protected List<Exam> exams;     // 子测验
                                    //ExamsResult	result;		// 考试结果
    protected bool finished;    // 考试结束
    protected double difficulty;    // 难度系数
    protected int baseValue;    // 科目点数基数
    protected double dependence;    // 科目点数依赖率

    const int Dispersion = 7;   // 分数离散度
    const double FactorK1 = 0.75;
    const double FactorK2 = 3;
    const double FactorK3 = 66;
    const double FactorD = 0.2;
    const double FactorB1 = 0.1;
    const double FactorB2 = 1;

    public int CompareTo(ExamSet e) {
        return e.date.CompareTo(date);
    }

    public int getSumScore() {
        if (!finished) return 0;
        int score = 0;
        foreach (Exam e in exams)
            score += e.getScore();
        return score;
    }
    public int getSumFinalScore() {
        if (!finished) return 0;
        int score = 0;
        foreach (Exam e in exams)
            score += e.getFinalScore();
        return score;
    }
    public int getSumMaxScore() {
        int score = 0;
        foreach (Exam e in exams)
            score += e.getMaxScore();
        return score;
    }

    public ExamSetJsonData toJsonData() {
        ExamSetJsonData data = new ExamSetJsonData();
        int cnt = exams.Count;
        data.name = name;
        data.date = date.ToString();
        data.subjectIds = subjectIds;

        data.finished = finished;
        data.difficulty = difficulty;
        data.baseValue = baseValue;
        data.dependence = dependence;
        data.exams = new ExamJsonData[cnt];
        for (int i = 0; i < cnt; i++)
            data.exams[i] = exams[i].toJsonData();
        return data;
    }
    public bool fromJsonData(ExamSetJsonData data) {
        int cnt = data.exams.Length;
        name = data.name;
        date = Convert.ToDateTime(data.date);
        subjectIds = data.subjectIds;

        finished = data.finished;
        difficulty = data.difficulty;
        baseValue = data.baseValue;
        dependence = data.dependence;
        exams = new List<Exam>();
        for (int i = 0; i < cnt; i++) 
            exams.Add(new Exam(data.exams[i], player));
        return true;
    }

    public ExamSet(int[] subjectIds, int timeLtd,
        int baseValue, double difficulty, double dependence,
        DataSystem.QuestionDistribution.Type type, int[] levelDtb,
        Player player = null, DateTime date = default(DateTime)) {
        this.date = (date == default(DateTime) ?
            GameSystem.getCurDate() : date);
        this.player = player ?? GameSystem.getPlayer();
        this.difficulty = difficulty;
        this.subjectIds = subjectIds;
        this.baseValue = baseValue;
        this.name = generateName();
        this.exams = new List<Exam>();
        foreach (int sid in subjectIds)
            addExam(new Exam(generateName(sid), sid,
                timeLtd, type, levelDtb, player, date));
        finished = false;
        //this.result = new ExamsResult();
    }
    public ExamSet(ExamSetJsonData data, Player player = null) {
        this.player = player ?? GameSystem.getPlayer();
        fromJsonData(data);
    }

    protected virtual string generateName(int sid = -1) {
        return "考试";
    }

    public string getName() { return generateName(); }

    public void addExam(Exam e) { exams.Add(e); }
    public int getExamCount() { return exams.Count; }
    public Exam getExam(int subjectId) {
        return exams.Find(s => s.getSubjectId() == subjectId);
    }
    public Exam getExamById(int id) {
        return exams[id];
    }

    public void terminate() {
        finished = true;
        afterTerminate();
        calcFinalScores();
    }
    protected virtual void afterTerminate() { }
    void calcFinalScores() {
        foreach (Exam e in exams) calcExamScore(e);
    }
    void calcExamScore(Exam e) {
        // calc avgs score
        int[] sbjs = Subject.DefaultMultSubjectSet[e.getSubjectId()];
        int score = 0;
        foreach (int s in sbjs)
            score += calcExamScoreForSubject(e, s);
        e.setFinalScore(score);
    }
    int calcExamScoreForSubject(Exam e, int sid) {
        int value = player.getSubjectParamValue(sid);
        int cnt = e.getQuestionCount();
        int sbjMaxScore = Subject.MaxScores[sid];
        int totScore = 0;
        double maxTotScore = 0, mWAvg = 0;
        double sumWeight = 0, wAvg = 0;
        for (int i = 0; i < cnt; i++) {
            Question q = e.getQuestion(i);
            if (q.getSubjectId() != sid) continue;
            int[] sel = e.getSelections(i);
            int score = q.calcScore(sel);
            int maxScore = q.getScore();
            int level = q.getLevel();
            int weight = Question.LevelWeight[level];
            totScore += score;
            maxTotScore += maxScore;
            wAvg += score * weight;
            mWAvg += maxScore * weight;
            sumWeight += weight;
        }
        Debug.Log("sid = " + sid + ", value = " + value);
        wAvg /= sumWeight; mWAvg /= sumWeight;
        mWAvg *= (1 - difficulty) * 2;
        Debug.Log("wAvg = " + wAvg + ", mWAvg = " + mWAvg);
        double score_ = getBaseScore(sbjMaxScore, totScore, maxTotScore);
        Debug.Log("base_score = " + score_);
        double rate;
        score_ *= (rate = calcSumEffect(wAvg, mWAvg, value));
        Debug.Log("rate_sum = " + rate);
        score_ *= (rate = calcDisperEffect());
        Debug.Log("rate_disper = " + rate);
        score_ = calcFuncEffect(score_, sbjMaxScore);
        Debug.Log("temp_score = " + score_);
        int res = Mathf.RoundToInt((float)score_);
        Debug.Log("result = " + res);
        Debug.Log("=============================");
        e.setSubFinalScore(sid, res);
        return res;
    }
    double getBaseScore(int sbjMaxScore, double totScore, double maxTotScore) {
        return Mathf.Max((float)(sbjMaxScore * (totScore / maxTotScore) * difficulty), (float)FactorB2);
    }
    double calcAvgScoreEffect(double wAvg, double mWAvg) {
        return Mathf.Max((float)(wAvg / mWAvg * FactorK1), (float)FactorB1);
    }
    double calcValueEffect(int value) {
        return Mathf.Log((float)(value * 1.0 / baseValue * FactorK2 + 1), 2f);
    }
    double calcSumEffect(double wAvg, double mWAvg, int value) {
        double rate1 = calcAvgScoreEffect(wAvg, mWAvg);
        Debug.Log("rate1 = " + rate1);
        double rate2 = calcValueEffect(value);
        Debug.Log("rate2 = " + rate2);
        return rate1 * (1 - dependence) + rate2 * dependence;
    }
    double calcDisperEffect() {
        return UnityEngine.Random.Range(100 - Dispersion, 100 + Dispersion) / 100.0;
    }
    double sigmoid(double x) {
        return 1 / (1 + Mathf.Exp(-(float)x));
    }
    double calcFuncEffect(double score, int sbjMaxScore) {
        return (sigmoid(score / FactorK3) - FactorD) / (1 - FactorD) * sbjMaxScore;
    }
}

// 摸底考
public class FirstExam : ExamSet {
    const int TimeLtd = 15;
    const int BaseValue = 30;
    const double Difficulty = 0.8;
    const double Dependence = 0.25;
    const DataSystem.QuestionDistribution.Type Type =
        DataSystem.QuestionDistribution.Type.Normal;
    static readonly int[] LevelDtb = { 1, 1, 1, 0, 0, 0 };

    const int IDispersion = 3;  // 初始化离散度
    const double FactorA = 1.12;
    const double FactorK = 0.85;
    const double FactorB = 0.15;
    const int FactorM = 5;

    public FirstExam(int[] subjectIds, Player player = null,
        DateTime date = default(DateTime)) : base(subjectIds,
        TimeLtd, BaseValue, Difficulty, Dependence,
        Type, LevelDtb, player, date) { }

    protected override string generateName(int sid = -1) {
        DateTime curDate = GameSystem.getCurDate();
        return "XX中学" + curDate.Year.ToString() + "学年第一次摸底考";
    }
    // 考试结束后，生成科目点数
    protected override void afterTerminate() {
        Subject[] sbjs = Subject.getStandardSubjects(subjectIds);
        for (int i = 0; i < exams.Count; i++)
            setupSubjectValue(exams[i], sbjs[i]);
    }
    void setupSubjectValue(Exam e, Subject s) {
        int cnt = e.getQuestionCount();
        int stdSpan = 0; double base_ = 0;
        for (int i = 0; i < cnt; i++) {
            Question q = e.getQuestion(i);
            int[] sel = e.getSelections(i);
            int score = q.calcScore(sel);
            int maxScore = q.getScore();
            int level = q.getLevel();
            base_ += Question.LevelWeight[level] *
                calcScoreEffect(maxScore, score);
            stdSpan += Question.LevelMinute[level];
        }
        base_ *= calcTimeEffect(e.getSpan(), stdSpan);
        base_ *= calcIDisperEffect();
        s.addPoint(Mathf.RoundToInt((float)base_));
        player.increaseSubjectParam(s);
    }
    double calcScoreEffect(int maxScore, int score) {
        return FactorK * (score / maxScore) + FactorB;
    }
    double calcTimeEffect(TimeSpan span, int stdSpan) {
        TimeSpan std = new TimeSpan(0, stdSpan, 0);
        double stdMin = std.TotalMinutes;
        double min = span.TotalMinutes;
        return Mathf.Pow((float)FactorA, Mathf.Clamp(
            (float)(stdMin - min), -(float)FactorM, (float)FactorM));
    }
    double calcIDisperEffect() {
        return UnityEngine.Random.Range(100 - IDispersion, 100 + IDispersion) / 100.0;
    }
}

// 高考
public class FinalExam : ExamSet {
    const int TimeLtd = 20;
    const int BaseValue = 2333;
    const double Difficulty = 0.6;
    const double Dependence = 0.8;
    const DataSystem.QuestionDistribution.Type Type =
        DataSystem.QuestionDistribution.Type.NotOccurFirst;
    static readonly int[] LevelDtb = { 0, 1, 1, 1, 1, 0 };

    public FinalExam(int[] subjectIds, Player player = null,
        DateTime date = default(DateTime)) : base(subjectIds,
        TimeLtd, BaseValue, Difficulty, Dependence,
        Type, LevelDtb, player, date) { }

    protected override string generateName(int sid = -1) {
        DateTime curDate = GameSystem.getCurDate();
        return curDate.Year.ToString() + "年普通高等学校招生全国统一考试";
    }
}