using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExamResultLayer : AnimatableLayer {

    public Text title, name_, detail;

    public ExamLayer examLayer;
    public AnswerLayer answerLayer;

    Player player;
    ExamSet examSet;

	// Use this for initialization
	void Awake () {
        base.Awake();
        player = GameSystem.getPlayer();
	}
	
	// Update is called once per frame
	void Update () {
        base.Update();
    }

    public void setExamSet(ExamSet es) {
        examSet = es;
        player = GameSystem.getPlayer();
        title.text = es.getName()+" 成绩单";
        name_.text = "姓名：<size=30>" + 
            player.getName() + "</size>";
        detail.text = "";
        int cnt = es.getExamCount();
        for (int i = 0; i < cnt; i++)
            drawExamScore(es.getExamById(i));
        detail.text += "总分: " + es.getSumFinalScore() + "/" + es.getSumMaxScore();
        showWindow();
    }
    void drawExamScore(Exam e) {
        string sbj = Subject.SubjectName[e.getSubjectId()];
        int score = e.getFinalScore(), maxScore = e.getMaxScore();
        detail.text += sbj + ": " + score + "/" + maxScore + "\n";
    }
    public void backScene() {
        hideWindow(new Vector3(1, 0, 0));
        uiBaseLayer.backToUILayer();
        examLayer.paperPositionReset();
    }
    public void showDescription() {
        hideWindow(new Vector3(1, 0, 0));
        examLayer.paperOut();
        answerLayer.setExamSet(examSet);
        /*
        hideWindow();
        examLayerObject.SetActive(false);
        answerLayer.setExamSet(examSet);
        */
    }
}
