using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecordJsonData {
    public QuestionStatJsonData[] questionRec;
    public ExerciseJsonData[] exerciseRec;
    public ExamSetJsonData[] examSetRec;
    public int[] questionCollection;
    public int[] questionWrong;
}

public static class RecordSystem {
    static List<Question>   questionRec;    // 题目记录
    static List<Exercise>	exerciseRec;    // 刷题记录
    static List<ExamSet>	examSetRec;     // 考试记录
    static List<Question>	questionCollection; // 题目收藏
    static List<Question>	questionWrong;      // 错题记录

    static bool initialized = false;

    public static RecordJsonData toJsonData() {
        if (!initialized) initialize();
        RecordJsonData data = new RecordJsonData();
        int cnt = questionRec.Count;
        data.questionRec = new QuestionStatJsonData[cnt];
        for (int i = 0; i < cnt; i++)
            data.questionRec[i] = questionRec[i].getStatData();
        cnt = exerciseRec.Count;
        data.exerciseRec = new ExerciseJsonData[cnt];
        for (int i = 0; i < cnt; i++)
            data.exerciseRec[i] = exerciseRec[i].toJsonData();
        cnt = examSetRec.Count;
        data.examSetRec = new ExamSetJsonData[cnt];
        for (int i = 0; i < cnt; i++)
            data.examSetRec[i] = examSetRec[i].toJsonData();
        cnt = questionCollection.Count;
        data.questionCollection = new int[cnt];
        for (int i = 0; i < cnt; i++)
            data.questionCollection[i] = questionCollection[i].getId();
        cnt = questionWrong.Count;
        data.questionWrong = new int[cnt];
        for (int i = 0; i < cnt; i++)
            data.questionWrong[i] = questionWrong[i].getId();

        return data;
    }
    public static bool fromJsonData(RecordJsonData data) {
        if (!initialized) initialize();
        else {
            questionRec.Clear();
            exerciseRec.Clear();
            examSetRec.Clear();
            questionCollection.Clear();
            questionWrong.Clear();
        }
        int cnt = data.questionRec.Length;
        for (int i = 0; i < cnt; i++) {
            QuestionStatJsonData d = data.questionRec[i];
            Question q = DataSystem.getQuestionById(d.id);
            q.loadStatData(d);
            questionRec.Add(q);
        }
        cnt = data.exerciseRec.Length;
        for (int i = 0; i < cnt; i++)
            exerciseRec.Add(new Exercise(data.exerciseRec[i]));
        cnt = data.examSetRec.Length;
        for (int i = 0; i < cnt; i++)
            examSetRec.Add(new ExamSet(data.examSetRec[i]));
        cnt = data.questionCollection.Length;
        for (int i = 0; i < cnt; i++)
            questionCollection.Add(DataSystem.getQuestionById(
                data.questionCollection[i]));
        cnt = data.questionWrong.Length;
        for (int i = 0; i < cnt; i++)
            questionWrong.Add(DataSystem.getQuestionById(
                data.questionWrong[i]));
        return true;
    }

    static public void initialize(){
        questionRec = new List<Question>();
        exerciseRec = new List<Exercise>();
		examSetRec = new List<ExamSet>();
		questionWrong = new List<Question>();
		questionCollection = new List<Question>();
        initialized = true;
    }

    static public void recordQuestion(Question q) {
        addQuestionList(questionRec, q);
    }
    static public void recordExercise(Exercise e) {
        exerciseRec.Add(e); recordQuestionSet(e);
    }
    static public void recordExamSet(ExamSet e){
        examSetRec.Add(e);
        int cnt = e.getExamCount();
        for (int i = 0; i < cnt; i++)
            recordQuestionSet(e.getExamById(i));
    }

    static public int questionCount() { return questionRec.Count; }
    static public int exerciseCount() { return exerciseRec.Count; }
    static public int examSetCount(){ return examSetRec.Count; }

    static public Question getQuestion(int id) { return questionRec[id]; }
    static public Exercise getExercise(int id) { return exerciseRec[id]; }
    static public ExamSet getExamSet(int id){ return examSetRec[id]; }

    static public void addQuestionCollect(Question q){ addQuestionList(questionCollection, q);}
    static public Question getQuestionCollect(int id){return questionCollection[id];}
    static public void deleteQuestionCollect(int id){questionCollection.RemoveAt(id);}
    static public void deleteQuestionCollect(Question q){questionCollection.Remove(q);}

    static public void addQuestionWrong(Question q){ addQuestionList(questionWrong, q);}
    static public Question getQuestionWrong(int id){return questionWrong[id];}
    static public void deleteQuestionWrong(int id){questionWrong.RemoveAt(id);}
    static public void deleteQuestionWrong(Question q){questionWrong.Remove(q);}

    static void addQuestionList(List<Question> list, Question q) {
        if (!list.Contains(q)) list.Add(q);
    }
    static void recordQuestionSet(QuestionSet qs) {
        int cnt = qs.getQuestionCount();
        for (int i = 0; i < cnt; i++)
            recordQuestion(qs.getQuestion(i));
    }
}
