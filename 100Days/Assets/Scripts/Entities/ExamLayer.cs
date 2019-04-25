using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ExamLayer : QuestionDisplayer {
    int quesPaddingButtom = 24;
    int choicesSpacing = 8;

    public ExamResultLayer examResultLayer;

    public GameObject buttons;

    public Transform nextBtn, prevBtn, pushBtn;

    public GameObject choicePrefab;

    public Text title, subject, timer;
    public RectTransform question, choices;
    public RectTransform content;


    int quesPointer, examPointer;
    int quesCount, examCount;

    Player player;

    DateTime quesTime;
    TimeSpan[] timeSpans;
    int[][] selections;
    
    Exam exam;
    ExamSet examSet;

    bool doing = false;

    // Use this for initialization
    void Awake() {
        GameSystem.initialize();
        //GameSystem.createPlayer();
    }

    void Start() {
        player = GameSystem.getPlayer();
        player.showSubjectParams();
        ExamSet exams = new FirstExam(player.getSubjectIds());
        setExamSet(exams);
        //setExam(exams.getExamById(0));
    }

    // Update is called once per frame
    void Update() {
        if (doing && exam!=null) {
            updateTimer();
            updateChoicesPosition();
            updateContentHeight();
        }
    }

    void updateTimer() {
        DateTime now = DateTime.Now;
        TimeSpan span = now - exam.getStartTime();
        if(exam.getTimeLtd() <= span) {
            string[] txt = { null, "提交"};
            UnityAction[] act = { null, pushSelectionForce };
            GameUtils.alert("时间已到！", txt, act);
            doing = false;
        } else {
            span = exam.getTimeLtd() - span;
            timer.text = "剩余时间\n" + GameUtils.time2Str(span);
        }
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

    public void activateButtonsLayer() {
        buttons.SetActive(true);
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
    void disablePushButton() {
        Button btn = GameUtils.button(pushBtn);
        btn.interactable = false;
    }


    public void nextPage() {
        setPointer(quesPointer + 1);
    }
    public void prevPage() {
        setPointer(quesPointer - 1);
    }
    public void pushAnswer() {
        saveSelection();
        if (pushSelection()) {
            exam.terminate();
            nextExam();
        }
    }

    public void setPointer(int index) {
        saveSelection();
        int last = quesPointer;
        quesPointer = Mathf.Clamp(index, 0, quesCount - 1);
        if (last != quesPointer) onPageChange(last);
        content.anchoredPosition = new Vector2(0, 0);
    }
    public void setExamPointer(int index) {
        examPointer = Mathf.Clamp(index, 0, examCount - 1);
        setExam(examSet.getExamById(index));
    }
    public void nextExam() {
        if(examPointer >= examCount - 1){
            // 刷题完成
            examSet.terminate();
            RecordSystem.recordExamSet(examSet);
            //gameObject.SetActive(false);
            //disablePushButton();
            examResultLayer.setExamSet(examSet);
            player.showSubjectParams();

            StorageSystem.saveGame();
        } else setExamPointer(examPointer + 1);
    }

    public void onPageChange(int last) {
        recordTimeSpan(last);
        refreshQuestion();
        refreshButtonEnable();
    }

    public void recordTimeSpan(int last) {
        if (last < 0) return;
        TimeSpan span = DateTime.Now - quesTime;
        timeSpans[last] += span;
    }

    public void setExamSet(ExamSet e) {
        examSet = e;
        examCount = e.getExamCount();
        setExamPointer(0);
    }
    public void setExam(Exam e) {
        exam = e;
        quesPointer = -1;
        quesCount = e.getQuestionCount();
        beforeExamStart(); exam.start();
        setPointer(0);
    }
    void beforeExamStart() {
        selections = new int[quesCount][];
        timeSpans = new TimeSpan[quesCount];
        for (int i = 0; i < selections.Length; i++)
            selections[i] = new int[0];
        for (int i = 0; i < timeSpans.Length; i++)
            timeSpans[i] = new TimeSpan(0);
        title.text = exam.getName();
        playStartAni();
    }
    public void refreshQuestion() {
        quesTime = DateTime.Now;

        Question q = exam.getQuestion(quesPointer);
        LinkImageText queText = GameUtils.get<LinkImageText>(question);

        subject.text = getSubjectText(q);
        queText.text = getQuestionTextInExercise(quesPointer, q);

        int cnt = q.getChoiceCount(); clearChoices();
        for (int i = 0; i < cnt; i++) createChoice(i, q);

        loadSelection();
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
        //toggle.isOn = true;
        label.text = choiceText;
        Debug.Log(label.text);
    }
    void clearChoices() {
        List<Transform> list = new List<Transform>();
        foreach (Transform child in choices) list.Add(child);
        for (int i = 0; i < list.Count; i++) DestroyImmediate(list[i].gameObject);
    }

    public void saveSelection() {
        if (quesPointer < 0) return;
        List<int> selection = new List<int>(); int i = 0;
        foreach (Transform child in choices) {
            Toggle toggle = GameUtils.get<Toggle>(child);
            if (toggle.isOn) selection.Add(i); i++;
        }
        Debug.Log("saveSelection: " + selection.Count);
        selections[quesPointer] = selection.ToArray();
    }
    public void loadSelection() {
        if (quesPointer < 0) return;
        int[] sel = selections[quesPointer];
        Debug.Log("loadSelection: " + sel.Length);
        foreach (Transform child in choices) {
            Text label = GameUtils.find<Text>(child, "Label");
            Debug.Log(label.text);
        }
        foreach (int s in sel) {
            Transform child = choices.GetChild(s);
            Text label = GameUtils.find<Text>(child, "Label");
            Toggle toggle = GameUtils.get<Toggle>(child);
            toggle.isOn = true;
        }
    }

    void playStartAni() {
        CameraControl ctr = GameUtils.getCameraControl();
        ctr.moveTo(new Vector3(0, 15, 10), new Vector3(90, 0, 0));
        ctr.setCallback(() => { activateButtonsLayer(); });
    }

    public int checkIncompleteQuesId() {
        for (int i = 0; i < selections.Length; i++) {
            Debug.Log(i + ": " + selections[i].Length);
            if (selections[i].Length == 0) return i;
        }
        return -1;
    }
    void pushSelectionForce() {
        exam.answerQuestions(selections, timeSpans);
        exam.terminate();
        nextExam();
    }

    bool pushSelection() {
        int index = checkIncompleteQuesId();
        if (index >= 0) {
            string[] txt = { null, "是", "否" };
            UnityAction[] act = { null, pushSelectionForce, null};
            GameUtils.alert("还有问题未完成！是否继续提交？", txt, act);
            setPointer(index);
        } else exam.answerQuestions(selections, timeSpans);
        return index < 0;
    }
}
