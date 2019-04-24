using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExamResultLayer : MonoBehaviour {
    float scaleSpeed = 0.2f;
    float stopScaleDist = 0.02f;

    public Text title, name_, detail;

    public GameObject background;

    public GameObject examLayerObject;
    public AnswerLayer answerLayer;

    Player player;
    ExamSet examSet;

    string animation;
	// Use this for initialization
	void Awake () {
        player = GameSystem.getPlayer();
	}
	
	// Update is called once per frame
	void Update () {
        updateAnimation();
    }

    void updateAnimation() {
        Vector3 ts;
        switch (animation) {
            case "show":
                ts = new Vector3(1, 1, 1);
                transform.localScale += (ts -
                    transform.localScale) * scaleSpeed;
                if (isAniStopping(ts)) {
                    transform.localScale = ts;
                    stopAnimation();
                }
                break;
            case "hide":
                ts = new Vector3(1, 0, 0);
                transform.localScale += (ts -
                    transform.localScale) * scaleSpeed;
                if (isAniStopping(ts)) {
                    transform.localScale = ts;
                    gameObject.SetActive(false);
                    background.SetActive(false);
                    stopAnimation();
                }
                break;
        }
    }

    bool isAniStopping(Vector3 ts) {
        return Vector3.Distance(ts, transform.localScale) < stopScaleDist;
    }

    void stopAnimation() {
        animation = null;
    }

    public void setExamSet(ExamSet es) {
        examSet = es;
        player = GameSystem.getPlayer();
        title.text = es.getName()+" 成绩单";
        name_.text = "姓名：<size=18>" + 
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

    public void showWindow() {
        background.SetActive(true);
        gameObject.SetActive(true);
        animation = "show";
    }
    public void hideWindow() {
        animation = "hide";
    }
    public void backScene() {
        hideWindow();
    }
    public void showDescription() {
        hideWindow();
        examLayerObject.SetActive(false);
        answerLayer.setExamSet(examSet);

    }
}
