using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExamSetDispalyLayer : AnimatableLayer{
    public Text name, difficulty, time, 
        subjects, timeLtd, level, teacher;

    ExamSet examSet;

    private void Awake() {
        base.Awake();
        //clear();
    }

    public void setExamSet(ExamSet e) {
        clear();
        examSet = e;
        refresh();
        showWindow();
    }
    public void clear() {
        examSet = null;
        name.text = "";
        difficulty.text = "";
        subjects.text = "";
        time.text = "";
        timeLtd.text = "";
        level.text = "";
        teacher.text = "";
    }
    public void refresh() {
        Debug.Log(examSet);
        if (examSet == null) return;
        name.text = examSet.getName();
        time.text = "考试日期：" + examSet.getDate().ToString();
        difficulty.text = "难度系数：" + examSet.getDifficulty();
        subjects.text = "考试科目：" + generateSubjects();
        timeLtd.text = "每科时限：" + examSet.getTimeLtd() + "分钟";
        level.text = "每科题目分配：\n" + generateLevelDtb();
        teacher.text = "老师寄语：" + examSet.getSpoken();
    }
    string generateSubjects() {
        string res = "";
        int cnt = examSet.getExamCount();
        for (int i = 0; i < cnt; i++) {
            Exam e = examSet.getExamById(i);
            res += Subject.SubjectName[e.getSubjectId()] + " ";
        }
        return res;
    }
    string generateLevelDtb() {
        string res = "";
        int cnt, sum = 0, time, sumt = 0;
        int[] dtb = examSet.getLevelDtb();
        for(int i = 0; i < dtb.Length; i++) {
            sum += (cnt = dtb[i]);
            if (cnt <= 0) continue;
            sumt += (time = cnt * Question.LevelMinute[i]);
            res += "\t- " + (i + 1) + " 星题目：" + cnt +" 条";
            res += "\t\t预计用时：" + time + " 分钟\n";
        }
        res += "总计：" + sum + " 条\t\t\t\t\t预计用时：" + sumt + " 分钟\n";
        return res;
    }

    public void closeWindow() {
        hideWindow(new Vector3(1, 0, 0));
    }
}
