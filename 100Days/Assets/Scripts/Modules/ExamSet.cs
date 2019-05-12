using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[System.Serializable]
public class SubjectQuestionsJsonData {
    public QuestionJsonData[][] data;
}
*/
[System.Serializable]
public class ExamRespondJsonData {
    public QuestionJsonDataArray2D data;

    public static ExamRespondJsonData fromJson(string json) {
        Debug.Log("fromJson: "+json);
        return JsonUtility.FromJson<ExamRespondJsonData>(json);
    }
}

[System.Serializable]
public class ExamSetJsonData {
    public string name;      
    public string date;    
    public IntArray subjectIds; 
    public ExamJsonDataArray exams; 
                                
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
    protected bool finished;    // 考试结束
    protected int timeLtd;      // 每科时限
    protected string spoken;   // 老师寄语

    protected List<int>[] questions; // 问题
    protected int[] levelDtb;
    protected DataSystem.QuestionDistribution.Type type;

    protected double difficulty;    // 难度系数
    protected int baseValue;    // 科目点数基数
    protected double dependence;    // 科目点数依赖率

    public const int Dispersion = 7;   // 分数离散度
    const double FactorK1 = 0.75;
    const double FactorK2 = 3;
    const double FactorK3 = 66;
    const double FactorD = 0.2;
    const double FactorB1 = 0.1;
    const double FactorB2 = 1;

    public int CompareTo(ExamSet e) {
        return e.date.CompareTo(date);
    }

    public double getDifficulty() {
        return difficulty;
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
        data.name = name;
        data.date = date.ToString();
        data.subjectIds = new IntArray(subjectIds);

        data.finished = finished;
        data.difficulty = difficulty;
        data.baseValue = baseValue;
        data.dependence = dependence;
        data.exams = new ExamJsonDataArray();
        foreach(Exam e in exams)
            data.exams.Add(e.toJsonData());
        return data;
    }
    public bool fromJsonData(ExamSetJsonData data) {
        int cnt = data.exams.Count;
        name = data.name;
        date = Convert.ToDateTime(data.date);
        subjectIds = data.subjectIds.ToArray();

        finished = data.finished;
        difficulty = data.difficulty;
        baseValue = data.baseValue;
        dependence = data.dependence;
        exams = new List<Exam>();
        foreach (ExamJsonData e in data.exams)
            exams.Add(new Exam(e, this, player));
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
        this.questions = new List<int>[subjectIds.Length];
        this.levelDtb = levelDtb;
        this.type = type;
        this.timeLtd = timeLtd;
        foreach (int sid in subjectIds)
            addExam(new Exam(generateName(sid), sid,
                timeLtd, type, levelDtb, this, player, date));
        finished = false;
        //this.result = new ExamsResult();
    }
    public ExamSet(ExamSetJsonData data, Player player = null) {
        this.player = player ?? GameSystem.getPlayer();
        fromJsonData(data);
    }

    public void generateQuestions() {
        DataSystem.getExamQuestions(subjectIds, player, levelDtb, type);
    }

    public void loadQuestions(ExamRespondJsonData data) {
        for (int i = 0; i < questions.Length; i++) {
            Debug.Log(i);
            Debug.Log(data.data[i]);
            questions[i] = new List<int>();
            foreach (QuestionJsonData d in data.data[i]) {
                Question q = DataSystem.addQuestion(new Question(d));
                questions[i].Add(q.getId());
            }
        }
        DataSystem.arrangeQuestions();
        setExamsQuestions();
    }

    void setExamsQuestions() {
        for (int i = 0; i < exams.Count; i++)
            exams[i].setQuestions(questions[i].ToArray());
    }

    public Question[] getQuestions(Exam e) {
        int index = exams.IndexOf(e);
        if (index == -1) return null;
        return getQuestions(index); 
    }
    public Question[] getQuestions(int index) {
        if (questions[index] == null)
            return new Question[0];
        List<Question> res = new List<Question>();
        foreach (int qid in questions[index])
            res.Add(DataSystem.getQuestionById(qid));
        return res.ToArray();
    }
    public Question[] getAllQuestions(Exam e) {
        int index = exams.IndexOf(e);
        if (index == -1) return null;
        return getQuestions(index);
    }

    protected virtual string generateName(int sid = -1) {
        return "考试";
    }
    protected virtual string generateSpoken() {
        return "无";
    }

    public string getName() { return name; }
    public DateTime getDate() { return date; }
    public string getSpoken() { return spoken = spoken ?? generateSpoken(); }
    public int getTimeLtd() { return timeLtd; }
    public int[] getLevelDtb() { return levelDtb; }

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
            Question q = e.getQuestionObject(i);
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
        score_ *= (rate = player.calcPressureEffect());
        Debug.Log("rate_pressure = " + rate);
        score_ = calcFuncEffect(score_, sbjMaxScore);
        Debug.Log("temp_score = " + score_);
        int res = Mathf.RoundToInt((float)score_);
        Debug.Log("result = " + res);
        Debug.Log("=============================");
        e.setSubFinalScore(sid, res);
        return res;
    }
    double getBaseScore(int sbjMaxScore, double totScore, double maxTotScore) {
        return getBaseScore(sbjMaxScore, totScore, maxTotScore, difficulty);
    }
    double calcValueEffect(int value) {
        return calcValueEffect(value, baseValue);
    }
    double calcSumEffect(double wAvg, double mWAvg, int value) {
        return calcSumEffect(wAvg, mWAvg, value, dependence, baseValue);
    }

    protected static double getBaseScore(int sbjMaxScore, double totScore, double maxTotScore, double difficulty) {
        return Mathf.Max((float)(sbjMaxScore * (totScore / maxTotScore) * difficulty), (float)FactorB2);
    }
    protected static double calcAvgScoreEffect(double wAvg, double mWAvg) {
        return Mathf.Max((float)(wAvg / mWAvg * FactorK1), (float)FactorB1);
    }
    protected static double calcValueEffect(int value, int baseValue) {
        return Mathf.Log((float)(value * 1.0 / baseValue * FactorK2 + 1), 2f);
    }
    protected static double calcSumEffect(double wAvg, double mWAvg, int value, double dependence, int baseValue) {
        double rate1 = calcAvgScoreEffect(wAvg, mWAvg);
        Debug.Log("rate1 = " + rate1);
        double rate2 = calcValueEffect(value, baseValue);
        Debug.Log("rate2 = " + rate2);
        return rate1 * (1 - dependence) + rate2 * dependence;
    }
    protected static double calcDisperEffect() {
        return UnityEngine.Random.Range(100 - Dispersion, 100 + Dispersion) / 100.0;
    }
    protected static double sigmoid(double x) {
        return 1 / (1 + Mathf.Exp(-(float)x));
    }
    protected static double calcFuncEffect(double score, int sbjMaxScore) {
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
        return player.getSchool() + " " + curDate.Year.ToString() + " 学年第一次摸底考试";
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
            Question q = e.getQuestionObject(i);
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

    static readonly string[] Spokens = {
        "No matter excellent or out of control,that＇s your own life.Wish you strong both in body and spirit!",
        "回首，青春无悔；前望，前程似锦，愿同学们面对未来，乐观、坚强、永不言弃。",
        "正如故乡是用来怀念的，青春就是用来追忆的，当你怀揣着它时，它一文不值，只有将它耗尽后，再回过头来看，一切才有了意义。——珍惜青春。",
        "清露晨流，新桐初引。高飞为念，美丽同行。",
        "If you do not think about your future,you cannot have one.(如果你不思索你的未来，你不会有未来)",
        "以你的自信，以你的开朗，以你的毅力，还有我的祝福，你一定能够驶向理想的彼岸。",
        "你的天赋好比一朵火花，假如你用勤勉辛劳去助燃，它一定会变成熊熊烈火，放出无比的光和热来。",
        "你用才智和学识取得今天的收获，又将以明智和果敢接受明天的挑战。愿你永葆一往无前精神。",
        "假如生活是一条河流，愿你是一叶执著向前的小舟；假如生活是一叶小舟，愿你是个风雨无阻的水手。",
        "现在的你站在什么位置并不重要，重要的是前进的方向。坚定地迈出每一步，都会让你更接近梦想。",
        "只有一个忠告给你--做你自己的主人。拼搏无极限。",
        "世界是你们的，也是我们的，但是归根结底是你们的，你们青年人朝气蓬勃，正在兴旺时期，好像早晨八九点钟的太阳，希望寄托在你们身上。",
        "你若想得到这世界最好的东西，先得让世界看到最好的你。",
        "成功会迟到，但不会缺席。我们终究会踏上各自的路，天涯海角 温暖共存。",
        "用拼搏的汗水灌注无悔的高三路。考前不怕，考后不悔。",
        "考好考坏，爸妈都等你回家吃饭。",
        "有一种经历叫高考；有一种变化叫成长；有一种力量叫坚韧；有一种责任叫付出；有一种幸福叫安康；有一种胸怀叫忍让；有一种快乐叫工作；有一种关心叫鞭策；有一种收获叫舍弃。",
        "人生有无数种可能，希望大家都能坦然面对，稳中求胜，努力成长。彼时当年少，莫负好时光呀！",
        "愿你合上笔盖的刹那，有着侠客收剑入鞘的骄傲。",
        "高考只是人生的一段历程，即不是全部，也不是终点，哭过，笑过，累过，冲过，努力过，拼搏过，就不是辜负，而是凯旋。",
        "一直坚信这句话，乘风破浪会有时，直挂云帆济沧海。高考不是终点，而是一个新的起点，学习是无止境的，愿你脚踏实地地走好每一步，旗开得胜，考入理想学府！",
        "高考永远不是终点，而是人生长路中一段你们老去时依然会觉得璀璨又有意义的岁月。愿你们得偿所愿，也愿你们今后的路越来越好。"
    };

    public FinalExam(int[] subjectIds, Player player = null,
        DateTime date = default(DateTime)) : base(subjectIds,
        TimeLtd, BaseValue, Difficulty, Dependence,
        Type, LevelDtb, player, date) { }

    protected override string generateName(int sid = -1) {
        DateTime curDate = GameSystem.getCurDate();
        return curDate.Year.ToString() + " 年普通高等学校招生全国统一考试";
    }
    protected override string generateSpoken() {
        int id = UnityEngine.Random.Range(0, Spokens.Length);
        return Spokens[id];
    }

    public static int[] evaluateScore(int sid, Player p, double avg, double floatRate) {
        if (sid == DataSystem.SMAX) {
            int[] res = new int[] { 0, 0 }, tmp;
            foreach (int id in p.getSubjectIds()) {
                tmp = evaluateScore(id, p, RecordSystem.getExamAvgScore(id),
                    RecordSystem.getExamFloatRate(id));
                res[0] += tmp[0]; res[1] += tmp[1];
            }
            return res;
        }
        int value = p.getSubjectParamValue(sid);
        return evalExamScoreForSubject(sid, value, avg, floatRate);
    }
    static int[] evalExamScoreForSubject(int sid, int value, double avg, double floatRate) {
        int[] res = new int[2];
        int sbjMaxScore = Subject.MaxScores[sid];
        double mWAvg = sbjMaxScore, rate;
        mWAvg *= (1 - Difficulty) * 2;
        double score_ = getBaseScore(sbjMaxScore, avg, mWAvg, Difficulty);
        score_ *= (rate = calcSumEffect(avg, mWAvg, value, Dependence, BaseValue));
        //score_ *= (rate = calcDisperEffect());
        score_ = calcFuncEffect(score_, sbjMaxScore);
        //score_ = Mathf.RoundToInt((float)score_);
        double fr = Dispersion / 100.0;
        fr += Mathf.Sqrt((float)floatRate) / sbjMaxScore;
        res[0] = Mathf.RoundToInt((float)(score_ * (1 - fr)));
        res[1] = Mathf.RoundToInt((float)(score_ * (1 + fr)));
        return res;
    }
}

// 第一次月考
public class FirstLunarExam : ExamSet {
    const int TimeLtd = 15;
    const int BaseValue = 233;
    const double Difficulty = 0.75;
    const double Dependence = 0.65;
    const DataSystem.QuestionDistribution.Type Type =
        DataSystem.QuestionDistribution.Type.NotOccurFirst;
    static readonly int[] LevelDtb = { 0, 1, 1, 1, 0, 0 };

    public FirstLunarExam(int[] subjectIds, Player player = null,
        DateTime date = default(DateTime)) : base(subjectIds,
        TimeLtd, BaseValue, Difficulty, Dependence,
        Type, LevelDtb, player, date) { }

    protected override string generateName(int sid = -1) {
        DateTime curDate = GameSystem.getCurDate();
        return player.getSchool() + " " + curDate.Year.ToString() + " 学年第一次月考";
    }
    protected override string generateSpoken() {
        return "这学期第一次正式的考试，难度不大，希望能够有所进步。";
    }
}

// 第二次月考
public class SecondLunarExam : ExamSet {
    const int TimeLtd = 20;
    const int BaseValue = 666;
    const double Difficulty = 0.6;
    const double Dependence = 0.7;
    const DataSystem.QuestionDistribution.Type Type =
        DataSystem.QuestionDistribution.Type.NotOccurFirst;
    static readonly int[] LevelDtb = { 0, 1, 1, 1, 1, 0 };

    public SecondLunarExam(int[] subjectIds, Player player = null,
        DateTime date = default(DateTime)) : base(subjectIds,
        TimeLtd, BaseValue, Difficulty, Dependence,
        Type, LevelDtb, player, date) { }

    protected override string generateName(int sid = -1) {
        DateTime curDate = GameSystem.getCurDate();
        return player.getSchool() + " " + curDate.Year.ToString() + " 学年第二次月考";
    }
    protected override string generateSpoken() {
        return "希望大家能把握住这次机会，好好检查自己知识上的漏洞。";
    }
}

// 百校联考
public class HunderSchoolExam : ExamSet {
    const int TimeLtd = 30;
    const int BaseValue = 999;
    const double Difficulty = 0.5;
    const double Dependence = 0.7;
    const DataSystem.QuestionDistribution.Type Type =
        DataSystem.QuestionDistribution.Type.NotOccurFirst;
    static readonly int[] LevelDtb = { 0, 0, 0, 1, 2, 1 };

    public HunderSchoolExam(int[] subjectIds, Player player = null,
        DateTime date = default(DateTime)) : base(subjectIds,
        TimeLtd, BaseValue, Difficulty, Dependence,
        Type, LevelDtb, player, date) { }

    protected override string generateName(int sid = -1) {
        DateTime curDate = GameSystem.getCurDate();
        return curDate.Year.ToString() + " 年全国百校联考";
    }
    protected override string generateSpoken() {
        return "我们学校每年都会参加这个全国性的百校联考，题目可能会比较有挑战性。";
    }
}
// 百日高考特别考试
public class HunderDaysExam : ExamSet {
    const int TimeLtd = 20;
    const int BaseValue = 1440;
    const double Difficulty = 0.75;
    const double Dependence = 0.25;
    const DataSystem.QuestionDistribution.Type Type =
        DataSystem.QuestionDistribution.Type.NotOccurFirst;
    static readonly int[] LevelDtb = { 0, 0, 0, 2, 1, 0 };

    public HunderDaysExam(int[] subjectIds, Player player = null,
        DateTime date = default(DateTime)) : base(subjectIds,
        TimeLtd, BaseValue, Difficulty, Dependence,
        Type, LevelDtb, player, date) { }

    protected override string generateName(int sid = -1) {
        DateTime curDate = GameSystem.getCurDate();
        return "百日高考工作组 " + curDate.Year.ToString() + " 年特别考试";
    }
    protected override string generateSpoken() {
        return "我们还是第一次参加这个考试，不知道难度怎么样...";
    }
}

// 一模
public class FirstSimExam : ExamSet {
    const int TimeLtd = 20;
    const int BaseValue = 2000;
    const double Difficulty = 0.65;
    const double Dependence = 0.75;
    const DataSystem.QuestionDistribution.Type Type =
        DataSystem.QuestionDistribution.Type.NotOccurFirst;
    static readonly int[] LevelDtb = { 0, 1, 1, 1, 1, 0 };

    public FirstSimExam(int[] subjectIds, Player player = null,
        DateTime date = default(DateTime)) : base(subjectIds,
        TimeLtd, BaseValue, Difficulty, Dependence,
        Type, LevelDtb, player, date) { }

    protected override string generateName(int sid = -1) {
        DateTime curDate = GameSystem.getCurDate();
        return curDate.Year.ToString() + " 年普通高中毕业班综合测试（一）";
    }
    protected override string generateSpoken() {
        return "一模是最接近高考难度的一次考试，希望大家能够好好对待！加油！";
    }
}

// 二模
public class SecondSimExam : ExamSet {
    const int TimeLtd = 20;
    const int BaseValue = 2200;
    const double Difficulty = 0.7;
    const double Dependence = 0.7;
    const DataSystem.QuestionDistribution.Type Type =
        DataSystem.QuestionDistribution.Type.NotOccurFirst;
    static readonly int[] LevelDtb = { 2, 1, 1, 1, 0, 0 };

    public SecondSimExam(int[] subjectIds, Player player = null,
        DateTime date = default(DateTime)) : base(subjectIds,
        TimeLtd, BaseValue, Difficulty, Dependence,
        Type, LevelDtb, player, date) { }

    protected override string generateName(int sid = -1) {
        DateTime curDate = GameSystem.getCurDate();
        return curDate.Year.ToString() + " 年普通高中毕业班综合测试（二）";
    }
    protected override string generateSpoken() {
        return "二模就要来了！虽然题型与高考有点区别，但是还是可以跟一模做个对比，及时调整自己的学习计划！";
    }
}

// 三模
public class ThirdSimExam : ExamSet {
    const int TimeLtd = 20;
    const int BaseValue = 2000;
    const double Difficulty = 0.8;
    const double Dependence = 0.6;
    const DataSystem.QuestionDistribution.Type Type =
        DataSystem.QuestionDistribution.Type.NotOccurFirst;
    static readonly int[] LevelDtb = { 2, 2, 1, 0, 0, 0 };

    public ThirdSimExam(int[] subjectIds, Player player = null,
        DateTime date = default(DateTime)) : base(subjectIds,
        TimeLtd, BaseValue, Difficulty, Dependence,
        Type, LevelDtb, player, date) { }

    protected override string generateName(int sid = -1) {
        DateTime curDate = GameSystem.getCurDate();
        return player.getSchool() + " " + curDate.Year.ToString() + " 年普通高中毕业班综合测试（三）";
    }
    protected override string generateSpoken() {
        return "高考前最后的一次考试了。这只是学校举办的考前热身考试，题目比较简单，但是永远不要懈怠！";
    }
}