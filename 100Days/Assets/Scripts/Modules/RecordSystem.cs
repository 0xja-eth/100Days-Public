using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class RecordJsonData {
    public QuestionStatJsonDataArray questionRec;
    public ExerciseJsonDataArray exerciseRec;
    public ExamSetJsonDataArray examSetRec;
    public IntArray questionCollection;
    public IntArray questionWrong;
}

[System.Serializable]
public class RecordRespondJsonData {
    public QuestionJsonDataArray data;

    public static RecordRespondJsonData fromJson(string json) {
        Debug.Log("fromJson: " + json);
        return JsonUtility.FromJson<RecordRespondJsonData>(json);
    }
}

[System.Serializable]
public class QuestionCountRespondJsonData {
    public IntArray2D data;

    public static QuestionCountRespondJsonData fromJson(string json) {
        Debug.Log("fromJson: " + json);
        return JsonUtility.FromJson<QuestionCountRespondJsonData>(json);
    }
}

public static class RecordSystem {
    static List<Question>   questionRec;    // 题目记录
    static List<Exercise>	exerciseRec;    // 刷题记录
    static List<ExamSet>	examSetRec;     // 考试记录
    static List<Question>	questionCollection; // 题目收藏
    static List<Question>	questionWrong;      // 错题记录
    static List<QuestionBufferItem> questionBuffers;  // 缓冲加载题目

    static int[,] allQuestionCount = new int[DataSystem.SMAX+1,DataSystem.LMAX+1];

    static UnityAction onLoadFinished; // 读取完成（不管是否成功）

    public class QuestionBufferItem {
        public int id; public string type;
        public QuestionStatJsonData stat;
        public QuestionBufferItem(int id,string type,
            QuestionStatJsonData stat = null) {
            this.id = id; this.type = type; this.stat = stat;
        }
    }

    static bool initialized = false;

    public static RecordJsonData toJsonData() {
        if (!initialized) initialize();
        Debug.Log("toJsonData: "+getWrongs().Count);
        Debug.Log("toJsonData.include: " + getWrongs(true).Count);
        RecordJsonData data = new RecordJsonData();
        int cnt = questionRec.Count;
        data.questionRec = new QuestionStatJsonDataArray();
        for (int i = 0; i < cnt; i++)
            data.questionRec.Add(questionRec[i].getStatData());
        cnt = exerciseRec.Count;
        data.exerciseRec = new ExerciseJsonDataArray();
        for (int i = 0; i < cnt; i++)
            data.exerciseRec.Add(exerciseRec[i].toJsonData());
        cnt = examSetRec.Count;
        data.examSetRec = new ExamSetJsonDataArray();
        for (int i = 0; i < cnt; i++)
            data.examSetRec.Add(examSetRec[i].toJsonData());
        cnt = questionCollection.Count;
        data.questionCollection = new IntArray();
        for (int i = 0; i < cnt; i++)
            data.questionCollection.Add(questionCollection[i].getId());
        cnt = questionWrong.Count;
        data.questionWrong = new IntArray();
        for (int i = 0; i < cnt; i++)
            data.questionWrong.Add(questionWrong[i].getId());
        return data;
    }
    public static bool fromJsonData(RecordJsonData data) {
        clear();
        int cnt = data.questionRec.Count;
        for (int i = 0; i < cnt; i++) {
            QuestionStatJsonData d = data.questionRec[i];
            Question q = DataSystem.getQuestionById(d.id);
            if (q != default(Question)) {
                q.loadStatData(d);
                questionRec.Add(q);
            } else questionBuffers.Add(
                new QuestionBufferItem(d.id,"record",d));
        }
        cnt = data.exerciseRec.Count;
        for (int i = 0; i < cnt; i++)
            exerciseRec.Add(new Exercise(data.exerciseRec[i]));
        cnt = data.examSetRec.Count;
        for (int i = 0; i < cnt; i++)
            examSetRec.Add(new ExamSet(data.examSetRec[i]));
        cnt = data.questionCollection.Count;
        for (int i = 0; i < cnt; i++) {
            int id = data.questionCollection[i];
            Question q = DataSystem.getQuestionById(id);
            if (q != default(Question))
                questionCollection.Add(q);
            else questionBuffers.Add(
                new QuestionBufferItem(id, "collection"));
        }
        cnt = data.questionWrong.Count;
        for (int i = 0; i < cnt; i++) {
            int id = data.questionWrong[i];
            Question q = DataSystem.getQuestionById(id);
            if (q != default(Question))
                questionWrong.Add(q);
            else questionBuffers.Add(
                new QuestionBufferItem(id, "wrong"));
        }
        loadAllQuestionCount();
        return true;
    }

    public static void setOnLoadFinished(UnityAction action) {
        onLoadFinished = action;
    }

    public static void loadAllQuestionCount() {
        NetworkSystem.setup(
            onGetAllQuestionCountSuccess,
            onGetAllQuestionCountFail,
            true, "题库信息拉取中...");
        NetworkSystem.postRequest(NetworkSystem.QuertCountRoute);
    }
    static void onGetAllQuestionCountSuccess(RespondJsonData data) {
        QuestionCountRespondJsonData rData = QuestionCountRespondJsonData.fromJson(data.getJson());
        int sum = 0;
        int[] sums = new int[DataSystem.LMAX];
        for (int s = 0; s < DataSystem.SMAX; s++) {
            int suml = 0;
            for (int l = 0; l < DataSystem.LMAX; l++) {
                int cnt = (allQuestionCount[s, l] = rData.data[s][l]);
                sums[l] += cnt; suml += cnt; sum += cnt;
            }
            allQuestionCount[s, DataSystem.LMAX] = suml;
        }
        for (int l=0;l<DataSystem.LMAX; l++) 
            allQuestionCount[DataSystem.SMAX, l] = sums[l];
        allQuestionCount[DataSystem.SMAX, DataSystem.LMAX] = sum;

        for (int i = 0; i <= DataSystem.SMAX; i++)
            for (int j = 0; j <= DataSystem.LMAX; j++)
                Debug.Log("allQuestionCount[" + i + "," + j + "] = " + allQuestionCount[i, j]);

        if (questionBuffers.Count > 0)
            loadQuestionBuffers();
        else onLoadFinished.Invoke();
    }
    static void loadQuestionBuffers() {
        int cnt = questionBuffers.Count;
        int[] qids = new int[cnt];
        for (int i = 0; i < cnt; i++)
            qids[i] = questionBuffers[i].id;

        NetworkSystem.setup(
            onQuestionsBuffersLoadedSuccess,
            onQuestionsBuffersLoadedFail,
            true, "题目信息拉取中...");
        DataSystem.getQuestionsFromServer(qids);
    }
    static void onQuestionsBuffersLoadedSuccess(RespondJsonData data) {
        RecordRespondJsonData rData = RecordRespondJsonData.fromJson(data.getJson());
        Debug.Log("onQuestionsBuffersLoadedSuccess");
        foreach (QuestionJsonData qdt in rData.data)
            DataSystem.addQuestion(new Question(qdt));
        foreach (QuestionBufferItem qbi in questionBuffers) {
            Question q = DataSystem.getQuestionById(qbi.id);
            Debug.Log(qbi.id + ":" + q);
            if (q == null) continue;
            switch (qbi.type) {
                case "record":
                    q.loadStatData(qbi.stat);
                    questionRec.Add(q);
                    break;
                case "collection":
                    questionCollection.Add(q);
                    break;
                case "wrong":
                    questionWrong.Add(q);
                    break;
            }
        }
        questionBuffers.Clear();
        onLoadFinished?.Invoke();
    }
    static void onGetAllQuestionCountFail(RespondStatus status, string errmsg) {
        GameUtils.alert("获取题库信息失败！\n详细信息：" + errmsg,
            new string[] { null, "重试"},
            new UnityAction[] { null, loadAllQuestionCount });
    }
    static void onQuestionsBuffersLoadedFail(RespondStatus status, string errmsg) {
        GameUtils.alert("部分题目读取失败，仅显示本地缓存记录！\n详细信息：" + errmsg,
            new string[] { null, "重试", "关闭" },
            new UnityAction[] { null, loadQuestionBuffers, onLoadFinished });
    }

    static public void initialize(){
        if (!initialized) {
            questionRec = new List<Question>();
            exerciseRec = new List<Exercise>();
            examSetRec = new List<ExamSet>();
            questionWrong = new List<Question>();
            questionCollection = new List<Question>();
            questionBuffers = new List<QuestionBufferItem>();
        } else clear();
       initialized = true;
    }
    static public void clear() {
        if (!initialized) initialize();
        questionRec.Clear();
        exerciseRec.Clear();
        examSetRec.Clear();
        questionCollection.Clear();
        questionWrong.Clear();
        questionBuffers.Clear();
    }

    static public void recordQuestion(Question q) {
        addQuestionList(questionRec, q);
    }
    static public void recordExercise(Exercise e) {
        exerciseRec.Add(e); //recordQuestionSet(e);
    }
    static public void recordExamSet(ExamSet e){
        examSetRec.Add(e);
        /*int cnt = e.getExamCount();
        for (int i = 0; i < cnt; i++)
            recordQuestionSet(e.getExamById(i));*/
    }

    static public int getAllQuestionCount(int sid,int level) { return allQuestionCount[sid, level]; }
    static public int getQuestionCount(int sid, int level) {
        int cnt = 0;
        foreach (Question q in questionRec)
            if ((sid == DataSystem.SMAX && level == DataSystem.LMAX) || 
                (sid == DataSystem.SMAX && q.getLevel() == level) ||
                (level == DataSystem.LMAX && q.getSubjectId() == sid) ||
                (q.getSubjectId() == sid && q.getLevel() == level)) cnt++;
        return cnt;
    }
    static public double getQuestionCorrRate(int sid, int level) {
        int cnt = 0; double rate = 0;
        foreach (Question q in questionRec)
            if ((sid == DataSystem.SMAX && level == DataSystem.LMAX) ||
                (sid == DataSystem.SMAX && q.getLevel() == level) ||
                (level == DataSystem.LMAX && q.getSubjectId() == sid) ||
                (q.getSubjectId() == sid && q.getLevel() == level)) {
                cnt++; rate += q.getCrtRate();
            }
        return cnt == 0 ? -1 : rate / cnt;
    }
    static public TimeSpan getQuestionAvgTime(int sid, int level) {
        int cnt = 0; TimeSpan avg = new TimeSpan(0);
        foreach (Question q in questionRec)
            if ((sid == DataSystem.SMAX && level == DataSystem.LMAX) ||
                (sid == DataSystem.SMAX && q.getLevel() == level) ||
                (level == DataSystem.LMAX && q.getSubjectId() == sid) ||
                (q.getSubjectId() == sid && q.getLevel() == level)) {
                cnt++; avg += q.getAvgTime();
            }
        return new TimeSpan(cnt == 0 ? -1 : avg.Ticks / cnt);
    }

    static public double getExamAvgScore(int sid) {
        int sum = 0, cnt = 0;
        foreach (ExamSet es in examSetRec) {
            if (sid == DataSystem.SMAX) {
                sum += es.getSumFinalScore(); cnt++;
            } else {
                Debug.Log("getExamAvgScore: " + sid);
                sum += es.getExam(sid).getFinalScore(); cnt++;
            }
        }
        return cnt <= 0 ? -1 : sum / cnt;
    }
    static public double getExamFloatRate(int sid) {
        int cnt = 0, score; double sum = 0;
        double avg = getExamAvgScore(sid);
        foreach (ExamSet es in examSetRec) {
            score = -1;
            if (sid == DataSystem.SMAX)  score = es.getSumFinalScore();
            else score = es.getExam(sid).getFinalScore();
            if (score >= 0) {
                sum += (score - avg) * (score - avg); cnt++;
            }
        }
        return cnt <= 0 ? -1 : sum / cnt;
    }

    static public int questionCount() { return questionRec.Count; }
    static public int exerciseCount() { return exerciseRec.Count; }
    static public int examSetCount(){ return examSetRec.Count; }
    static public int questionCollectCount() { return questionCollection.Count; }
    static public int questionWrongCount() { return questionWrong.Count; }

    static public Question getQuestion(int id) { return questionRec[id]; }
    static public Exercise getExercise(int id) { return exerciseRec[id]; }
    static public ExamSet getExamSet(int id) { return examSetRec[id]; }
    static public ExamSet getLastExamSet() { return examSetRec[examSetRec.Count-1]; }

    static public void addQuestionCollect(Question q){ addQuestionList(questionCollection, q);}
    static public Question getQuestionCollect(int id){return questionCollection[id];}
    static public void deleteQuestionCollect(int id){questionCollection.RemoveAt(id);}
    static public void deleteQuestionCollect(Question q){questionCollection.Remove(q); }
    static public bool isInCollection(Question q) { return questionCollection.Contains(q); }

    static public void addQuestionWrong(Question q){ addQuestionList(questionWrong, q);}
    static public Question getQuestionWrong(int id){return questionWrong[id];}
    static public void deleteQuestionWrong(int id){questionWrong.RemoveAt(id);}
    static public void deleteQuestionWrong(Question q){questionWrong.Remove(q); }
    static public bool isInWrong(Question q) { return questionWrong.Contains(q); }

    static public List<Question> getQuestions(bool include = false) {
        return include ? questionRec : questionRec.FindAll(q=>!q.isDeleted());
    }
    static public List<Exercise> getExercises(bool include = false) {
        return include ? exerciseRec : exerciseRec.FindAll(e => !e.isDeleted());
    }
    static public List<ExamSet> getExamSets() { return examSetRec; }
    static public List<Question> getCollections(bool include = false) {
        return include ? questionCollection : questionCollection.FindAll(q => !q.isDeleted());
    }
    static public List<Question> getWrongs(bool include = false) {
        return include ? questionWrong : questionWrong.FindAll(q => !q.isDeleted());
    }

    static void addQuestionList(List<Question> list, Question q) {
        if (!list.Contains(q)) list.Add(q);
    }
    static void recordQuestionSet(QuestionSet qs) {
        int cnt = qs.getQuestionCount();
        for (int i = 0; i < cnt; i++)
            recordQuestion(qs.getQuestionObject(i));
    }
}
