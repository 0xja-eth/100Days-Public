using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExerciseResultLayer : AnimatableLayer {

    public Text title, name_, detail1, detail2;

    public ExerciseLayer exerciseLayer;
    public AnswerLayer answerLayer;

    Player player;
    Exercise exercise;

    // Use this for initialization
    void Awake() {
        base.Awake();
        player = GameSystem.getPlayer();
    }

    // Update is called once per frame
    void Update() {
        base.Update();
    }

    public void setExercise(Exercise e) {
        exercise = e;
        player = GameSystem.getPlayer();
        title.text = e.getName();
        name_.text = "姓名：<size=30>" + player.getName() + "</size>";
        //detail1.text = detail2.text = "";
        generateExerciseDetail();
        showWindow();
    }
    void generateExerciseDetail() {
        int crtcnt = exercise.getCrtCnt();
        int cnt = exercise.getQuestionCount();
        int type = (int)exercise.getType();
        string t1 = "", t2 = "\n";
        t1 += "刷题模式：" + DataSystem.QuestionDistribution.TypeText[type] + "\n";
        t1 += "题量：" + cnt + "\n";
        t1 += "得分：" + exercise.getScore() + "/" + exercise.getSumScore() + "\n";
        t1 += "用时：" + GameUtils.time2Str(exercise.getSpan()) + "\n";
        t1 += "精力消耗：" + exercise.getEnergyCost() + "\n\n";
        t1 += "自我感觉：" + exercise.generateExerciseFeel();

        t2 += "正确数：" + crtcnt + "\n";
        t2 += "正确率：" + Mathf.Round((float)crtcnt/cnt * 10000) / 100 + "%\n";
        t2 += "新题目：" + exercise.getNewQuestionCnt() + "\n";
        t2 += "压力增加：" + exercise.getPressurePlus();

        detail1.text = t1;
        detail2.text = t2;
    }
    public void backScene() {
        hideWindow(new Vector3(1, 0, 0));
        uiBaseLayer.backToUILayer();
        exerciseLayer.paperPositionReset();
        //exerciseLayerObject.SetActive(false);
    }
    public void showDescription() {
        hideWindow(new Vector3(1, 0, 0));
        exerciseLayer.paperOut();
        answerLayer.setExercise(exercise);
    }
}
