using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class AnswerLayer : QuestionDisplayer {

    public Transform nextBtn, prevBtn;
    
    public Text title, info, timer, score;
	public RectTransform question;
	public RectTransform content;

    Exercise exercise;
    ExamSet examSet;

    List<Question> questions;

    int quesPointer;
    int quesCount;

    string mode;

    // Use this for initialization
    void Start () {
        base.Start();
    }
	
	// Update is called once per frame
	void Update () {
        base.Update();
        updateContentHeight();
    }

    void updateContentHeight() {
        setRectHeight(content, -question.anchoredPosition.y*2+question.rect.height);
    }

    void refreshButtonEnable() {
        setNextPageButtonEnable(quesPointer < quesCount - 1);
        setPrevPageButtonEnable(quesPointer > 0);
    }

    void setNextPageButtonEnable(bool enable) {
        Button btn = GameUtils.button(nextBtn);
        btn.interactable = enable;
    }
    void setPrevPageButtonEnable(bool enable) {
        Button btn = GameUtils.button(prevBtn);
        btn.interactable = enable;
    }

    public void nextPage(){
        setPointer(quesPointer + 1);
    }
    public void prevPage() {
        setPointer(quesPointer - 1);
    }
    public void backSccene() {
        uiBaseLayer.backToUILayer();
        paperPositionReset();
    }
    public Question curQuestion() {
        Debug.Log(quesPointer);
        return questions[quesPointer];
    }
    public void setPointer(int index){
        quesPointer = Mathf.Clamp(index, 0, quesCount - 1);
        refreshQuestion();
        refreshButtonEnable();
        content.anchoredPosition = new Vector2(0, 0);
    }

    public void setExercise(Exercise e) {
        mode = "Exercise";
        exercise = e;
        paperEnter();
        gameObject.SetActive(true);
        loadQuestions();
        showBaseInfo();
        setPointer(0);
    }
    public void setExamSet(ExamSet e) {
        mode = "ExamSet";
        examSet = e;
        paperEnter();
        gameObject.SetActive(true);
        loadQuestions();
        showBaseInfo();
        setPointer(0);
    }
    void showBaseInfo() {
        switch (mode) {
            case "Exercise":
                timer.text = "用时 " + GameUtils.time2Str(exercise.getSpan());
                score.text = "得分 " + exercise.getScore() + "/" + exercise.getSumScore();
                break;
            case "ExamSet":
                timer.text = "";
                score.text = "";// 总分 " + examSet.getSumFinalScore() + "/" + examSet.getSumMaxScore();
                break;
        }
    }
    void loadQuestions() {
        questions = new List<Question>();
        switch (mode) {
            case "Exercise":
                quesCount = exercise.getQuestionCount();
                for (int i = 0; i < quesCount; i++)
                    questions.Add(exercise.getQuestion(i));
                break;
            case "ExamSet":
                quesCount = 0;
                for (int i = 0; i < examSet.getExamCount(); i++) {
                    Exam exam = examSet.getExamById(i);
                    int cnt = exam.getQuestionCount();
                    for (int j = 0; j < cnt; j++)
                        questions.Add(exam.getQuestion(j));
                    quesCount += cnt;
                }
                break;
        }
    }
    public void refreshQuestion(){
        drawQuestionMain();
        drawQuestionDescription();
        drawQuestionStat();
    }

    void drawQuestionMain() {
        Question q = curQuestion(); // exercise.getQuestion(quesPointer);
        LinkImageText queText = GameUtils.get<LinkImageText>(question);

        if (mode == "ExamSet") {
            Exam exam = examSet.getExam(q.getSubjectId());
            timer.text = "用时 " + GameUtils.time2Str(exam.getSpan());
            score.text = "总分 " + exam.getFinalScore() + "/" + exam.getMaxScore();
        }
        title.text = "参考答案-" + getSubjectText(q);
        queText.text = getQuestionTextInAnswer(quesPointer, q);
    }
    void drawQuestionDescription() {
        Question q = curQuestion();
        LinkImageText queText = GameUtils.get<LinkImageText>(question);

        int[] sels = null;
        switch (mode) {
            case "Exercise":
                sels = exercise.getSelections(quesPointer);
                break;
            case "ExamSet":
                Exam exam = examSet.getExam(q.getSubjectId());
                sels = exam.getSelections(q);
                break;
        }
        queText.text += getDescriptionText(quesPointer, q, sels);
    }
    void drawQuestionStat() {
        Question q = curQuestion();
        info.text = getQuestionStatText(q);
    }
}
