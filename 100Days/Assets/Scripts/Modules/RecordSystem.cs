using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RecordSystem {
    static List<Exercise>	exerciseRec;    // 刷题记录
    static List<ExamSet>	examSetRec;     // 考试记录
    static List<Question>	questionCollection; // 题目收藏
    static List<Question>	questionWrong;      // 错题记录

    static public void initialize(){
		exerciseRec = new List<Exercise>();
		examSetRec = new List<ExamSet>();
		questionWrong = new List<Question>();
		questionCollection = new List<Question>();
	}

    static public void recordExercise(Exercise e){exerciseRec.Add(e);}
    static public void recordExam(ExamSet e){examSetRec.Add(e);}

    static public int exerciseCount(){return exerciseRec.Count;}
    static public int examSetCount(){return examSetRec.Count;}

    static public Exercise getExercise(int id){return exerciseRec[id];}
    static public ExamSet getExamSet(int id){return examSetRec[id];}

    static public void addQuestionCollect(Question q){questionCollection.Add(q);}
    static public Question getQuestionCollect(int id){return questionCollection[id];}
    static public void deleteQuestionCollect(int id){questionCollection.RemoveAt(id);}
    static public void deleteQuestionCollect(Question q){questionCollection.Remove(q);}

    static public void addQuestionWrong(Question q){questionWrong.Add(q);}
    static public Question getQuestionWrong(int id){return questionWrong[id];}
    static public void deleteQuestionWrong(int id){questionWrong.RemoveAt(id);}
    static public void deleteQuestionWrong(Question q){questionWrong.Remove(q);}
}
