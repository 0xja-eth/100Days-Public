using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ExerciseLayer : QuestionDisplayer {
    int quesPaddingButtom = 24;
    int choicesSpacing = 8;

    public ExerciseResultLayer exerciseResultLayer;

    public Transform nextBtn, quitBtn;

    public GameObject choicePrefab;

    public Text subject, timer;
    public RectTransform question, choices;
    public RectTransform content;

    public AnswerLayer answerLayer;

	int quesPointer;
	int quesCount;
    
    DateTime quesTime;

    Exercise exercise;

    bool doing = false;

    // Use this for initialization
    void Awake () {
        base.Awake();
        //GameSystem.createPlayer();
    }

    void Start() {
        base.Start();
        /*
		DataSystem.QuestionDistribution.Type type = 
			(DataSystem.QuestionDistribution.Type) 
			UnityEngine.Random.Range(0,8);
        GameSystem.getPlayer().showSubjectParams();
        int sbjId = UnityEngine.Random.Range(0,3);
		setExercise(new Exercise(2, sbjId,
			DataSystem.QuestionDistribution.Type.Normal));*/

    }

    // Update is called once per frame
    void Update () {
        base.Update();
        if (doing) {
            updateTimer();
            updateChoicesPosition();
            updateContentHeight();
        }
    }

    void updateTimer() {
        DateTime now = DateTime.Now;
        TimeSpan span = now - quesTime;
        timer.text = "用时\n" + GameUtils.time2Str(span);
    }

    void updateChoicesPosition() {
        float y = -question.rect.height - quesPaddingButtom;
        choices.anchoredPosition = new Vector2(0, y); y = 0;
        foreach (RectTransform child in choices) {
            RectTransform label = GameUtils.find<RectTransform>(child, "Label");
            setRectHeight(child, label.rect.height);
            child.anchoredPosition = new Vector2(0, y);
            y -= child.rect.height + choicesSpacing;
        }
        setRectHeight(choices, -y + quesPaddingButtom);
    }
    void updateContentHeight() {
        setRectHeight(content, -choices.anchoredPosition.y + choices.rect.height);
    }
	void disableNextPageButton(){
		Button btn = GameUtils.button(nextBtn);
		btn.interactable = false;
	}
	void changeNextPageButton(){
		Text btnTxt = GameUtils.find<Text> (nextBtn,"Text");
		btnTxt.text = "提交";
	}

	public void nextPage(){
        if (!pushSelection()) return;
        if (quesPointer >= quesCount - 1) finishExercise();
        else {
            Question nq = exercise.getQuestion(quesPointer + 1);
            if (checkExerciseEnable(nq)) setPointer(quesPointer + 1);
        }
	}

	public void setPointer(int index){
		quesPointer = Mathf.Clamp(index,0,quesCount-1);
		setQuestion(quesPointer, exercise.getQuestion(quesPointer));
        if (quesPointer >= quesCount - 1) changeNextPageButton();
    }

	public void setExercise(Exercise e) {
        player = GameSystem.getPlayer();
        //paperPositionReset()
        gameObject.SetActive(true);
		exercise = e;
		quesCount = e.getQuestionCount();
		playStartAni();
		setPointer(0);
        e.start();
    }
    bool exerciseEnable(Question q) {
        int val = q.getEnergyCost();
        return player.getEnergy() >= val;
    }
    bool checkExerciseEnable(Question q) {
        bool res = exerciseEnable(q);
        if (!res) onExerciseExhausted();
        return res;
    }
    public void setQuestion(int index, Question q) {
        quesTime = DateTime.Now;
        LinkImageText queText = GameUtils.get<LinkImageText>(question);

        subject.text = getSubjectText(q);
        queText.text = getQuestionTextInExercise(index, q);

        int cnt = q.getChoiceCount(); clearChoices();
        for (int i = 0; i < cnt; i++) createChoice(i, q);

        content.anchoredPosition = new Vector2(0, 0);
        doing = true;
    }

    void createChoice(int index, Question q) {
        GameObject choiceObj = Instantiate(choicePrefab, choices, false);
        RectTransform choice = (RectTransform)choiceObj.transform;
        Text label = GameUtils.find<Text>(choice, "Label");
        Toggle toggle = GameUtils.get<Toggle>(choice);
        
        string choiceText = getQuestionChoiceTextWithAdjust(index, q);
        
        if (q.getType() != Question.Type.Multiple)
            toggle.group = GameUtils.get<ToggleGroup>(choices);
        label.text = choiceText;
    }

    void clearChoices() {
        List<Transform> list = new List<Transform>();
        foreach (Transform child in choices) list.Add(child);
        for (int i = 0; i < list.Count; i++) Destroy(list[i].gameObject);
    }

    public int[] getSelection() {
        List<int> selection = new List<int>(); int i = 0;
        foreach (Transform child in choices) {
            Toggle toggle = GameUtils.get<Toggle>(child);
            if (toggle.isOn) selection.Add(i); i++;
        }
        return selection.ToArray();
    }

    bool pushSelection() {
        int[] selection = getSelection();
        if (selection.Length <= 0) {
            GameUtils.alert("未选择选项！"); return false;
        } else {
            TimeSpan span = DateTime.Now - quesTime;
            exercise.answerQuestion(quesPointer, selection, span);
            return true;
        }
    }
    void forcePushSelection() {
        int[] selection = getSelection();
        TimeSpan span = DateTime.Now - quesTime;
        exercise.answerQuestion(quesPointer, selection, span);
    }

    public void onExerciseExhausted() {
        GameUtils.alert("当前精力值过低 ( "+player.getEnergy()+" )，不足以继续刷题。\n点击确认结束刷题并进入结果界面。",
            new string[] { null, "确认" }, new UnityAction[] { null, onExerciseQuit });
    }
    public void exerciseQuit() {
        GameUtils.alert("刷题尚未完成，确定要退出吗？",
            new string[] { null, "是", "否" },
            new UnityAction[] { null, onExerciseQuit, null });
    }

    void finishExercise() {
        exercise.terminate();
        GameSystem.addDailyExeCnt();
        RecordSystem.recordExercise(exercise);

        exerciseResultLayer.setExercise(exercise);
        StorageSystem.saveGame();
    }

    void onExerciseQuit() {
        forcePushSelection();
        finishExercise();
        //backSccene();
    }
    public void backSccene() {
        uiBaseLayer.backToUILayer();
        paperPositionReset();
    }
}
