
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;


[System.Serializable]
public class CacheJsonData {
    public QuestionJsonDataArray questions;
}

public static class DataSystem {
	static List<Question>	questions;	// 题库（缓存）

	public const int SMAX = Subject.SubjectCount;
	public const int LMAX = Question.MaxLevel;

    public static readonly string QuestionPath = "Questions/";

    static bool initialized = false;
    // 题目分发帮助信息
    public static class QuestionDistribution {
		public enum Type {
			Normal, // 无优先级
			OccurFirst, NotOccurFirst, // 已做优先, 未做优先
			WorngFirst, CorrFirst, // 错题优先, 正确优先
			SimpleFirst,	// 简单题优先
			MiddleFirst,	// 中等题优先
			DifficultFirst	// 难题优先
		}
        public static readonly string[] TypeText = {
            "普通模式", "已做优先", "未做优先", "错题优先", "对题优先",
            "简单题优先", "中档题优先", "难题优先"
        };

		public const int DifficulterRate = 10; // 抽到更难题目几率
		public const int OFFactor = 75; // OccurFirst 因数
		public const int NFFactor = 75; // NotOccurFirst 因数
		public const int WFFactor = 85; // WorngFirst 因数
		public const int CFFactor = 85; // CorrFirst 因数
		public const int SFFactor = 85; // SimpleFirst 因数
		public const int MFFactor = 80; // MiddleFirst 因数
		public const int DFFactor = 75; // DifficultFirst 因数

		// 每星级每科目题目开始ID
		public static int[,] startIndex = new int[SMAX,LMAX]; 
		public static int qmax = 0;
		// [sid, lid]	-> 科目sid, 难度lid的开始索引

		public static int getStart(int sid, int lid=0){
			if(lid >= LMAX) return getStart(sid+1);
			if(sid >= SMAX) return qmax;
			return startIndex[sid, lid];
		}
		public static int getEnd(int sid, int lid=-1){
			if(lid == -1) // 获取全部科目的End
				return getStart(sid+1);
			else return getStart(sid, lid+1);
		}
	}
    
    public static void initialize(){
        if (initialized) return;
        if (StorageSystem.hasCacheFile()) loadQuestions();
        else questions = new List<Question>();
        initialized = true;
    }

    #region 缓存基本操作
    // 加入一个 Question 缓存
    public static Question addQuestion(Question q, bool arr = false) {
        Question oq = getQuestionById(q.getId());
        if (oq == null) questions.Add(q);
        else oq.update(q);
        if (arr) arrangeQuestions();
        return q;
    }
    public static Question addQuestion(string title, int level, string desc, 
		int score, int sid, Question.Type type=Question.Type.Single, bool arr=false){
		return addQuestion(new Question(title, level, desc, score, sid, type), arr);
	}
    // 加入一组 Question 缓存
    public static void addQuestions(Question[] qs, bool arr = false) {
        foreach (Question q in qs) addQuestion(q);
        if (arr) arrangeQuestions();
    }
    // 获取 Question 缓存数量
    public static int getQuestionCount() {
        return questions.Count;
    }
    // 通过 数组下表 获取 Question （用于遍历）
    public static Question getQuestion(int qid) {
        return questions[qid];
    }
    // 通过 getId() 获取 Question
    public static Question getQuestionById(int qid) {
        return questions.Find((q) => q.getId() == qid);
    }
    public static bool hasQuestion(int qid) {
        return questions.Exists((q) => q.getId() == qid);
    }
    #endregion

    #region 本地题目缓存处理
    // 储存缓存
    public static void saveQuestions() {
        StorageSystem.saveCache();
    }
    public static CacheJsonData toJsonData() {
        CacheJsonData data = new CacheJsonData();
        data.questions = new QuestionJsonDataArray();
        foreach (Question q in questions)
            data.questions.Add(q.toJsonData());
        return data;
    }
    public static void fromJsonData(CacheJsonData data) {
        questions = new List<Question>();
        foreach (QuestionJsonData qdt in data.questions)
            addQuestion(new Question(qdt));
    }

    // 读取缓存
    public static void loadQuestions() {
        Debug.Log("loadQuestions");
        StorageSystem.loadCache();
        // 整理题目
        arrangeQuestions();
    }

    // 整理数据库中题目
    public static void arrangeQuestions() {
        questions.Sort();
        generateDtbInfo();
    }
    // 生成题目分发信息
    static void generateDtbInfo() {
        int sid = 0, lid = 0;
        int qmax = questions.Count;
        QuestionDistribution.qmax = qmax;
        for (int s = 0; s < SMAX; s++) for (int l = 0; l < LMAX; l++)
                QuestionDistribution.startIndex[s, l] = qmax;
        for (int i = 0; i < questions.Count; i++) {
            Question q = questions[i];
            // 要求：每科每个难度必须要有题目！
            if (q.getSubjectId() == sid &&
                q.getLevel() == lid) {
                QuestionDistribution.startIndex[sid, lid] = i;
                if (++lid >= LMAX) {
                    lid = 0;
                    if (++sid >= SMAX) return;
                }
            }
        }
    }
    #endregion

    #region 旧的题目处理方法
    /*
    static bool loadQuestion(int s, int l, int cnt) {
        string sbj = Subject.SubjectName[s], lvl = (l + 1).ToString();
        string path = QuestionPath + sbj + "/" + lvl + "/" + cnt.ToString();

        TextAsset file = Resources.Load(path) as TextAsset;
        if (file == null) return false;

        string input = file.text;
        if (input == "") {
            Debug.Log("Loading: " + path);
            Debug.Log("无效数据！");
            return false;
        }
        // 预处理（规范化） → 正则表达式
        if (input.IndexOf("DES: ") == -1) input += "\nDES: 无";
        string titleReg = @"TIT: ?\n?(.+)\n*";
        string choicesReg = @"CHO: ?\n?(.+)\n*";
        string answersReg = @"ANS: ?\n?(.+)\n*";
        string descReg = @"DES: ?\n?(.+)$";
        string choiceReg = @"^\w[．. :：]+(.+) *\n";
        string answerReg = @"(\w)";

        string regTxt = titleReg + choicesReg + answersReg + descReg;

        Regex reg = new Regex(regTxt, RegexOptions.Singleline);
        Regex creg = new Regex(choiceReg, RegexOptions.Multiline);
        Regex areg = new Regex(answerReg);

        string title, desc, choices, answers;
        List<string> choiceArr;
        List<int> answerArr;
        choiceArr = new List<string>();
        answerArr = new List<int>();

        bool success = false;
        foreach (Match match in reg.Matches(input)) {
            title = match.Groups[1].Value;
            choices = match.Groups[2].Value;
            answers = match.Groups[3].Value;
            desc = match.Groups[4].Value;
            foreach (Match choice in creg.Matches(choices)) 
                choiceArr.Add(choice.Groups[1].Value);
            foreach (Match answer in areg.Matches(answers)) 
                answerArr.Add(answer.Groups[1].Value[0] - 'A');

            Question.Type type = answerArr.Count > 1 ?
                Question.Type.Multiple : Question.Type.Single;

            Question q = addQuestion(title, l, desc, Question.DefaultScore, s, type);
            foreach (string c in choiceArr) q.addChoice(c);
            foreach (int a in answerArr) q.changeChoiceAnswer(a);
            success = true;
        }
        return success;
    }*/
    /*
    public static void addQuestionsForTest(){
		int cnt = 0;
		for(int n=0;n<1;n++) for(int s=0;s<SMAX;s++)
			for(int l=0;l<LMAX;l++){
				int t = UnityEngine.Random.Range(0,2);
				int len = UnityEngine.Random.Range(200,500);
				string txt = "测试 "+(++cnt).ToString()+" ";
				for(int i=0;i<len;i++)
					txt += (char)UnityEngine.Random.Range(32,126);
				Question q = addQuestion(txt, l, "这是题解", Question.DefaultScore, 
					s, t==0 ? Question.Type.Single : Question.Type.Multiple);
				if(t==0) {// 单选
					int c = UnityEngine.Random.Range(3,6);
					int ans = UnityEngine.Random.Range(0,c);
					for(int i=0;i<c;i++)
						q.addChoice(ans==i?"正确":"测试",ans==i);
				} else {
					int c = UnityEngine.Random.Range(3,6);
					for(int i=0;i<c;i++){
						bool ans = UnityEngine.Random.Range(0,2)==1;
						q.addChoice(ans?"正确":"测试",ans);
					}
				}
			}
	}*/

    #endregion

    #region 本地缓存题目分配算法
    // 根据玩家当前状态获取题目
    static int generateQuestionByType(int sid, Player p, 
		QuestionDistribution.Type type) {
		int value = p.getSubjectParamValue(sid);
		int ml = getMaxLevel(value);
		if(UnityEngine.Random.Range(0,100)<
			QuestionDistribution.DifficulterRate) 
			ml = Mathf.Min(ml+1, LMAX);
		int l = QuestionDistribution.getStart(sid);
		int r = QuestionDistribution.getEnd(sid,ml); 
		int index = 0, tmp = 0, dt;
		// 题目分配算法
		switch(type){
			case QuestionDistribution.Type.Normal:
			index = UnityEngine.Random.Range(l, r); break;
			case QuestionDistribution.Type.OccurFirst:
			do{ index = UnityEngine.Random.Range(l, r); }
			while(!questions[index].haveOccurred() 
				&& UnityEngine.Random.Range(0,100) < 
				QuestionDistribution.OFFactor);
			break;
			case QuestionDistribution.Type.NotOccurFirst:
			do{ index = UnityEngine.Random.Range(l, r); }
			while(questions[index].haveOccurred() 
				&& UnityEngine.Random.Range(0,100) < 
				QuestionDistribution.NFFactor);
			break;
			case QuestionDistribution.Type.WorngFirst:
			do{ index = UnityEngine.Random.Range(l, r); }
			while(questions[index].haveDone() 
				&& UnityEngine.Random.Range(0,100) < 
				QuestionDistribution.WFFactor);
			break;
			case QuestionDistribution.Type.CorrFirst:
			do{ index = UnityEngine.Random.Range(l, r); }
			while(!questions[index].haveDone() 
				&& UnityEngine.Random.Range(0,100) < 
				QuestionDistribution.CFFactor);
			break;
			case QuestionDistribution.Type.SimpleFirst:
			do{ index = UnityEngine.Random.Range(l, r); 
				dt = Mathf.Abs(questions[index].getLevel()-tmp);
			}while(dt > ml/2 && UnityEngine.Random.Range(0,100) < 
				QuestionDistribution.SFFactor);
			break;
			case QuestionDistribution.Type.MiddleFirst:
			do{ index = UnityEngine.Random.Range(l, r); tmp = ml/2;
				dt = Mathf.Abs(questions[index].getLevel()-tmp);
			}while(dt > ml/2 && UnityEngine.Random.Range(0,100) < 
				QuestionDistribution.MFFactor);
			break;
			case QuestionDistribution.Type.DifficultFirst:
			do{ index = UnityEngine.Random.Range(l, r); tmp = ml;
				dt = Mathf.Abs(questions[index].getLevel()-tmp);
			}while(dt > ml/2 && UnityEngine.Random.Range(0,100) < 
				QuestionDistribution.DFFactor);
			break;
		}
		return index;
	}

	static int generateQuestionByLevel(int sid, Player p, 
		QuestionDistribution.Type type, int level) {
		int l = QuestionDistribution.getStart(sid, level);
		int r = QuestionDistribution.getEnd(sid, level); 
		int index = 0;
		// 题目分配算法
		switch(type){
			case QuestionDistribution.Type.OccurFirst:
			do{ index = UnityEngine.Random.Range(l, r); }
			while(!questions[index].haveOccurred() 
				&& UnityEngine.Random.Range(0,100) < 
				QuestionDistribution.OFFactor);
			break;
			case QuestionDistribution.Type.NotOccurFirst:
			do{ index = UnityEngine.Random.Range(l, r); }
			while(questions[index].haveOccurred() 
				&& UnityEngine.Random.Range(0,100) < 
				QuestionDistribution.NFFactor);
			break;
			case QuestionDistribution.Type.WorngFirst:
			do{ index = UnityEngine.Random.Range(l, r); }
			while(questions[index].haveDone() 
				&& UnityEngine.Random.Range(0,100) < 
				QuestionDistribution.WFFactor);
			break;
			case QuestionDistribution.Type.CorrFirst:
			do{ index = UnityEngine.Random.Range(l, r); }
			while(!questions[index].haveDone() 
				&& UnityEngine.Random.Range(0,100) < 
				QuestionDistribution.CFFactor);
			break;
			default: index = UnityEngine.Random.Range(l, r); break;
		}
		return index;
	}
	// 通过本地缓存计算方法获取一个 Question
    public static int generateQuestion(int sid, Player p, 
		QuestionDistribution.Type type, int level=-1) {
		return level < 0 ? generateQuestionByType(sid, p, type) : 
			generateQuestionByLevel(sid, p, type, level);
    }
    // 通过本地缓存计算方法获取一组 Question
    public static List<int> generateQuestions(int sid, Player p, 
		int cnt, QuestionDistribution.Type type, int level=-1){
		List<int> res = new List<int>();
		List<int> ques = new List<int>(); int index,ltd=0;
		for(int i=0;i<cnt;i++) {
			do{index = generateQuestion(sid, p, type, level);}
			while(ltd++<100&&ques.Exists(id=>id==index));
            Debug.Log(questions.Count+", index="+index);
			ques.Add(index); res.Add(questions[index].getId());
		}
		return res;
    }
    // 获取可刷的最大难度
    public static int getMaxLevel(int value) {
        for (int i = 0; i < Question.MaxLevel; i++)
            if (value < Question.EntryValue[i])
                return i - 1;
        return Question.MaxLevel - 1;
    }
    #endregion
    /*
	public static List<Question>[] getExamQuestions(int[] subjectIds, Player p, 
		int[] levelDtb, QuestionDistribution.Type type,
        RequestObject.SuccessAction success = null,
        RequestObject.ErrorAction error = null) {
        Debug.Log("getExamQuestions");
        Debug.Log(Application.internetReachability);
        if (Application.internetReachability == NetworkReachability.NotReachable) 
            return getExamQuestionsFromCache(subjectIds, p, levelDtb, type);
        else getExamQuestionsFromServer(subjectIds, p, levelDtb, type);
        return new List<Question>[subjectIds.Length];
    }
    
    static List<Question>[] getExamQuestionsFromCache(int[] subjectIds, Player p,
        int[] levelDtb, QuestionDistribution.Type type) {
        return new List<Question>[subjectIds.Length];
    }*/
    // 能否根据缓存生成题目
    public static bool isCacheGenerateSupport(int sid, Player p,
        int cnt, QuestionDistribution.Type type) {
        return false;
    }
    public static bool isCacheGenerateSupport(int[] subjectIds, Player p,
        int[] levelDtb, QuestionDistribution.Type type) {
        return false;
    }

    public static void getExerciseQuestions(int sid, Player p,
        int cnt, QuestionDistribution.Type type) {

        WWWForm form = new WWWForm();
        form.AddField("sid", sid);
        form.AddField("dtb_type", (int)type);
        form.AddField("count", cnt);
        form.AddField("name", p.getName());

        NetworkSystem.postRequest(NetworkSystem.ExerciseRoute, form);
    }
    public static void getExamQuestions(int[] subjectIds, Player p,
        int[] levelDtb, QuestionDistribution.Type type) {
        string subTxt = "[" + String.Join(",", subjectIds) + "]";
        string dtbTxt = "[" + String.Join(",", levelDtb) + "]";

        WWWForm form = new WWWForm();
        form.AddField("sids", subTxt);
        form.AddField("dtb_type", (int)type);
        form.AddField("level_dtb", dtbTxt);
        form.AddField("name", p.getName());

        NetworkSystem.postRequest(NetworkSystem.ExamRoute, form);
    }
    public static void getQuestionsFromServer(int[] qids) {
        string qidsTxt = "[" + String.Join(",", qids) + "]";

        WWWForm form = new WWWForm();
        form.AddField("ids", qidsTxt);

        NetworkSystem.postRequest(NetworkSystem.QueryIdsRoute, form);
    }
}
