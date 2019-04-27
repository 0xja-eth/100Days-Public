
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class DataSystem {
	static List<Question>	questions;	// 题库

	public const int SMAX = Subject.SubjectCount;
	public const int LMAX = Question.MaxLevel;

    public static readonly string QuestionPath = "Questions/";

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
		addQuestions();
	}

	// 整理数据库中题目
	static void arrangeQuestions(){
		questions.Sort();
		generateDtbInfo();
	}
	// 生成题目分发信息
	static void generateDtbInfo(){
		int sid = 0, lid = 0;
		int qmax = questions.Count;
		QuestionDistribution.qmax = qmax;
		for(int s=0;s<SMAX;s++) for(int l=0;l<LMAX;l++)
			QuestionDistribution.startIndex[s,l]=qmax;
		for(int i=0;i<questions.Count;i++){
			Question q = questions[i];
			// 要求：每科每个难度必须要有题目！
			if(q.getSubjectId() == sid && 
				q.getLevel() == lid) {
				QuestionDistribution.startIndex[sid,lid]=i;
				if(++lid >= LMAX){ lid = 0;
					if(++sid >= SMAX) return;
				}
			}
		}
	}

	public static Question addQuestion(Question q, bool arr=false){
		questions.Add(q);
		if(arr) arrangeQuestions();
		return q;
	}
	public static Question addQuestion(string title, int level, string desc, 
		int score, int sid, Question.Type type=Question.Type.Single, bool arr=false){
		return addQuestion(new Question(title, level, desc, score, sid, type), arr);
	}
	public static void addQuestions(){
		questions = new List<Question>();

        Debug.Log("loadQuestions");
        loadQuestions();
        addQuestionsForTest(); // 测试题目

		// 读取数据库
		arrangeQuestions();
	}
    public static void loadQuestions() {
        for(int s = 0; s < SMAX; s++) for(int l=0; l < LMAX; l++) {
            int cnt = 1; while (loadQuestion(s, l, cnt++)) ;
        }
    }
    static bool loadQuestion(int s, int l, int cnt) {
        string sbj = Subject.SubjectName[s], lvl = (l + 1).ToString();
        string path = QuestionPath + sbj + "/" + lvl + "/" + cnt.ToString();
        //if (!File.Exists(path)) return false;

        TextAsset file = Resources.Load(path) as TextAsset;
        if (file == null) return false;

        //string input = "选出下列加点字注音全对的一项（）\nA．吮吸(shǔn) 涎皮（yán） 敕造（chì） 百无聊赖(lài)\nB．讪讪(shàn) 庠序（xiáng） 俨然(yǎn) 少不更事(jīng)\nC．折本（shé） 干瘪（biě) 谬种(miù) 沸反盈天(fèi)\nD．蹙缩（cù） 驯熟(xùn) 两靥（yàn） 鸡豚狗彘（zhì）\n\n解析：C\n无";
        string input = file.text;
        //
        if (input == "") {
            Debug.Log("Loading: " + path);
            Debug.Log("无效数据！");
            return false;
        }
        //Debug.Log("源数据：\n" + input); // 选项不能有换行
        // 预处理（规范化） → 正则表达式
        if (input.IndexOf("DES: ") == -1) input += "\nDES: 无";
        //input = Regex.Replace(input, @"(?<str>\w)([．. ]+)", "\n${str}. ");
        //Debug.Log("规范化：\n" + input);
        /*byte[] bytes = file.bytes;
         string input = System.Text.Encoding.UTF8.GetString(bytes);
         */
        /*input = input.Replace("答案", "XSOLX");
        input = input.Replace("解析", "XSOLX");
        input = input.Replace("：", ":");
        */
        //string[] ips = input.Split('\n');
        /*
        string titleReg = @"((.|\n)+?)";
        string choicesReg = @"((\w[．. ]+([^ ]+) *\n*){2,})";
        string choiceReg = @"\w[．. ]+([^ ]+) *\n*";
        string answersReg = @"((\w.*)+)";
        string answerReg = @"(\w)";
        string descReg = @"((.|\n)+?)";
        */
        string titleReg = @"TIT: ?\n?(.+)\n*";
        string choicesReg = @"CHO: ?\n?(.+)\n*";
        string answersReg = @"ANS: ?\n?(.+)\n*";
        string descReg = @"DES: ?\n?(.+)$";
        string choiceReg = @"^\w[．. :：]+(.+) *\n";
        string answerReg = @"(\w)";
        /*
        string test1 = @"^" + titleReg + @"\n*" + choicesReg;
        string test2 = @"\n+解析：" + answersReg;
        //string test = "\n*解析：";
        Regex testReg = new Regex(test1);
        Debug.Log(testReg.IsMatch(input));
        foreach (Match match in testReg.Matches(input))
            foreach (Capture cp in match.Groups)
                Debug.Log("CP: " + cp.Value);
        testReg = new Regex(test2);
        Debug.Log(testReg.IsMatch(input));
        foreach (Match match in testReg.Matches(input))
            foreach (Capture cp in match.Groups)
                Debug.Log("CP: " + cp.Value);
        /*foreach (string str in ips) {
            Debug.Log("Split: " + str);
            Debug.Log(testReg.IsMatch(str));
            foreach (Match match in testReg.Matches(str)) 
                Debug.Log(match.Groups[1].Value);
        }*/
        //return false;


        string regTxt = titleReg + choicesReg + answersReg + descReg;
        //string regTxt2 = @"\n*解析：" + answersReg + @" *\n*" + descReg + @"?$";
        Regex reg = new Regex(regTxt, RegexOptions.Singleline);
        //Regex reg2 = new Regex(regTxt2);
        Regex creg = new Regex(choiceReg, RegexOptions.Multiline);
        Regex areg = new Regex(answerReg);

        string title, desc, choices, answers;
        List<string> choiceArr;
        List<int> answerArr;
        choiceArr = new List<string>();
        answerArr = new List<int>();

        bool success = false;// && reg2.IsMatch(input);
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
    }
    public static void addQuestionsForTest(){
		int cnt = 0;
		for(int n=0;n<1;n++) for(int s=0;s<SMAX;s++)
			for(int l=0;l<LMAX;l++){
				int t = Random.Range(0,2);
				int len = Random.Range(200,500);
				string txt = "测试 "+(++cnt).ToString()+" ";
				for(int i=0;i<len;i++)
					txt += (char)Random.Range(32,126);
				Question q = addQuestion(txt, l, "这是题解", Question.DefaultScore, 
					s, t==0 ? Question.Type.Single : Question.Type.Multiple);
				if(t==0) {// 单选
					int c = Random.Range(3,6);
					int ans = Random.Range(0,c);
					for(int i=0;i<c;i++)
						q.addChoice(ans==i?"正确":"测试",ans==i);
				} else {
					int c = Random.Range(3,6);
					for(int i=0;i<c;i++){
						bool ans = Random.Range(0,2)==1;
						q.addChoice(ans?"正确":"测试",ans);
					}
				}
			}
        /*
		for(int i=0;i<30;i++){
			string title = "如程序框图所示，其作用是输入x的值，输出相应的y的值.若要使输入的x的值与输出的y的值相等，则这样的x的值有（   ）\n\n<quad"+
                QuestionDisplayer.spaceIdentifier+"name=Test"+QuestionDisplayer.spaceIdentifier+"size=128"+QuestionDisplayer.spaceIdentifier+"width=1"+QuestionDisplayer.spaceIdentifier+"/>";
			Question q = addQuestion(title, 0, "无", 6, 1);
			q.addChoice("1个");
			q.addChoice("2个");
			q.addChoice("3个", true);
			q.addChoice("4个");			
		}*/
	}
	// 根据玩家当前状态获取题目
	static int getQuestionByType(int sid, Player p, 
		QuestionDistribution.Type type) {
		int value = p.getSubjectParamValue(sid);
		int ml = getMaxLevel(value);
		if(Random.Range(0,100)<
			QuestionDistribution.DifficulterRate) 
			ml = Mathf.Min(ml+1, LMAX);
		int l = QuestionDistribution.getStart(sid);
		int r = QuestionDistribution.getEnd(sid,ml); 
		int index = 0, tmp = 0, dt;
		// 题目分配算法
		switch(type){
			case QuestionDistribution.Type.Normal:
			index = Random.Range(l, r); break;
			case QuestionDistribution.Type.OccurFirst:
			do{ index = Random.Range(l, r); }
			while(!questions[index].haveOccurred() 
				&& Random.Range(0,100) < 
				QuestionDistribution.OFFactor);
			break;
			case QuestionDistribution.Type.NotOccurFirst:
			do{ index = Random.Range(l, r); }
			while(questions[index].haveOccurred() 
				&& Random.Range(0,100) < 
				QuestionDistribution.NFFactor);
			break;
			case QuestionDistribution.Type.WorngFirst:
			do{ index = Random.Range(l, r); }
			while(questions[index].haveDone() 
				&& Random.Range(0,100) < 
				QuestionDistribution.WFFactor);
			break;
			case QuestionDistribution.Type.CorrFirst:
			do{ index = Random.Range(l, r); }
			while(!questions[index].haveDone() 
				&& Random.Range(0,100) < 
				QuestionDistribution.CFFactor);
			break;
			case QuestionDistribution.Type.SimpleFirst:
			do{ index = Random.Range(l, r); 
				dt = Mathf.Abs(questions[index].getLevel()-tmp);
			}while(dt > ml/2 && Random.Range(0,100) < 
				QuestionDistribution.SFFactor);
			break;
			case QuestionDistribution.Type.MiddleFirst:
			do{ index = Random.Range(l, r); tmp = ml/2;
				dt = Mathf.Abs(questions[index].getLevel()-tmp);
			}while(dt > ml/2 && Random.Range(0,100) < 
				QuestionDistribution.MFFactor);
			break;
			case QuestionDistribution.Type.DifficultFirst:
			do{ index = Random.Range(l, r); tmp = ml;
				dt = Mathf.Abs(questions[index].getLevel()-tmp);
			}while(dt > ml/2 && Random.Range(0,100) < 
				QuestionDistribution.DFFactor);
			break;
		}
		return index;
	}

	static int getQuestionByLevel(int sid, Player p, 
		QuestionDistribution.Type type, int level) {
		int l = QuestionDistribution.getStart(sid, level);
		int r = QuestionDistribution.getEnd(sid, level); 
		int index = 0;
		// 题目分配算法
		switch(type){
			case QuestionDistribution.Type.OccurFirst:
			do{ index = Random.Range(l, r); }
			while(!questions[index].haveOccurred() 
				&& Random.Range(0,100) < 
				QuestionDistribution.OFFactor);
			break;
			case QuestionDistribution.Type.NotOccurFirst:
			do{ index = Random.Range(l, r); }
			while(questions[index].haveOccurred() 
				&& Random.Range(0,100) < 
				QuestionDistribution.NFFactor);
			break;
			case QuestionDistribution.Type.WorngFirst:
			do{ index = Random.Range(l, r); }
			while(questions[index].haveDone() 
				&& Random.Range(0,100) < 
				QuestionDistribution.WFFactor);
			break;
			case QuestionDistribution.Type.CorrFirst:
			do{ index = Random.Range(l, r); }
			while(!questions[index].haveDone() 
				&& Random.Range(0,100) < 
				QuestionDistribution.CFFactor);
			break;
			default: index = Random.Range(l, r); break;
		}
		return index;
	}
	
	public static int getQuestionCount(){
		return questions.Count;
    }
    public static Question getQuestion(int qid) {
        return questions[qid];
    }
    public static Question getQuestionById(int qid) {
        foreach (Question q in questions)
            if (q.getId() == qid) return q;
        return null;
    }
    public static int getQuestion(int sid, Player p, 
		QuestionDistribution.Type type, int level=-1) {
		return level < 0 ? getQuestionByType(sid, p, type) : 
			getQuestionByLevel(sid, p, type, level);
	}
	public static List<Question> getQuestions(int sid, Player p, 
		int cnt, QuestionDistribution.Type type, int level=-1){
		List<Question> res = new List<Question>();
		List<int> ques = new List<int>(); int index,ltd=0;
		for(int i=0;i<cnt;i++) {
			do{index = getQuestion(sid, p, type, level);}
			while(ltd++<100&&ques.Exists(id=>id==index));
            Debug.Log(questions.Count+", index="+index);
			ques.Add(index); res.Add(questions[index]);
		}
		return res;
	}
    // 获取可刷的最大难度
    public static int getMaxLevel(int value){
		for(int i=0;i<Question.MaxLevel;i++)
			if(value<Question.EntryValue[i])
				return i-1;
		return Question.MaxLevel-1;
	}
}
