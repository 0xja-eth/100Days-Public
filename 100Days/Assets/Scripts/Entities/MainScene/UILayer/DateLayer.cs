using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DateLayer : AnimatableLayer {
    Player player;

    public ExamSetDispalyLayer examDisplay;

    public Text date, day, next;
    // Use this for initialization
    void Awake() {
        base.Awake();
    }
    void Start () {
        base.Start();
    }

    // Update is called once per frame
    void Update() {
        base.Update();
    }
    
    public void showNextExam() {
        ExamSet es = GameSystem.getNextExam();
        examDisplay.setExamSet(es);
    }

    public void refresh() {
        player = GameSystem.getPlayer();
        refreshDate();
        refreshNextExam();
    }
    void refreshDate() {
        date.text = GameSystem.getCurDate().ToString("yyyy 年 MM 月 dd 日");
        day.text = GameSystem.getDays() + " 天";
    }
    void refreshNextExam() {
        next.text = "";
        ExamSet es = GameSystem.getNextExam();
        if (es == null) return;
        TimeSpan ts = es.getDate() - GameSystem.getCurDate();
        next.text = "下一次考试在 "+Mathf.FloorToInt((float)ts.TotalDays)+" 天后 >>";
    }

    public void layerEnter() {
        showWindow();
        //rectTransform = (RectTransform)transform;
        float width = rectTransform.rect.width;
        moveDelta(new Vector3(width, 0, 0), "enter");
    }
    public void layerOut() {
        float width = rectTransform.rect.width;
        moveDelta(new Vector3(-width, 0, 0), "out");
        hideWindow(new Vector3(1, 1, 1));
    }
}
