using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordLayer : AnimatableLayer{
    // Start is called before the first frame update
    const int TitleLength = 20;

    public Color highlightColor = new Color(0.5f, 0.5f, 1);
    public Color normalColor = new Color(1, 1, 1, 0.75f);

    public GameObject quesListPerfab;

    public GameObject resultListPerfab; // 题目结果预制体

    public Toggle collect;
    public Button delete, full;

    // List
    public Dropdown modeSelect;
    public RectTransform content, list;

    // View
    public GameObject questionLayer, questionSetLayer;
    
    // Question
    public QuestionInfoView questionInfo;

    // QuestionSet
    public GameObject examDetail, exerciseDetail;

    // ExamDetail
    public ExamSetInfoView examSetInfo;

    // ExerciseDetail
    public ExerciseInfoView exerciseInfo;

    List<RectTransform> quesListItems = new List<RectTransform>();
    List<RectTransform> quesResultItems = new List<RectTransform>();

    List<Question> questions = new List<Question>();
    List<Exercise> exercises = new List<Exercise>();
    List<ExamSet> examSets = new List<ExamSet>();

    Player player;

    int mode, curIndex = -1;
    int lastIndex = -1;

    void Start() {
        base.Start();
        player = GameSystem.getPlayer();
        selectMode(0);
    }

    // Update is called once per frame
    void Update(){
        base.Update();

    }

    public void openWindow() {
        showWindow(); clear();
    }

    public void onSelectChange() {
        selectMode(modeSelect.value);
        foreach (AnimatableLayer obj in infos)
            obj.hideWindow();
    }

    public void selectMode(int mode) {
        this.mode = mode;
        modeSelect.value = mode;
        switch (mode) {
            case 0: // 所有题目
                setQuestions(RecordSystem.getQuestions());
                break;
            case 1: // 收藏题目
                setQuestions(RecordSystem.getCollections());
                break;
            case 2: // 错题
                setQuestions(RecordSystem.getWrongs());
                break;
            case 3: // 刷题记录
                setExercises(RecordSystem.getExercises());
                break;
            case 4: // 考试记录
                setExams(RecordSystem.getExamSets());
                break;
            case -1: // 题集
                break;
        }
    }

    protected override void onWindowShown() {
        onSelectChange();
    }

    public void refresh() {
        onSelectChange();
    }

    void displayList() {
        float h = 0; clear();
        if (mode <= 2) {
            questionLayer.SetActive(true);
            for (int i = 0; i < questions.Count; i++)
                h += createQuestionListObj(i, questions[i]);
        } else if (mode == 3) {
            questionSetLayer.SetActive(true);
            exerciseDetail.SetActive(true);
            for (int i = 0; i < exercises.Count; i++)
                h += createQuestionListObj(i, exercises[i]);
        } else if (mode == 4) {
            questionSetLayer.SetActive(true);
            examDetail.SetActive(true);
            for (int i = 0; i < examSets.Count; i++)
                h += createQuestionListObj(i, examSets[i]);
        }
        GameUtils.setRectHeight(content, h);
        content.anchoredPosition = new Vector2(0, 0);
    }

    void clear() {
        curIndex = -1;
        clearQuestionListObjs();
        clearQuestionLayer();
        clearQuestionSetLayer();
    }
    void clearQuestionLayer() {
        questionLayer.SetActive(false);
        collect.interactable = false;
        delete.interactable = false;
        full.interactable = false;
        questionInfo.clear();
    }
    void clearQuestionSetLayer() {
        questionSetLayer.SetActive(false);
        exerciseDetail.SetActive(false);
        examDetail.SetActive(false);
        exerciseInfo.clear();
        examSetInfo.clear();
    }

    void clearQuestionListObjs() {
        List<RectTransform> list = new List<RectTransform>();
        foreach (RectTransform child in quesListItems) list.Add(child);
        for (int i = 0; i < list.Count; i++) DestroyImmediate(list[i].gameObject);
        quesListItems.Clear();
    }

    public void addQuestionResultObjs(RectTransform rt) {
        quesResultItems.Add(rt);
    }
    public void clearQuestionResultObjs() {
        List<RectTransform> list = new List<RectTransform>();
        foreach (RectTransform child in quesResultItems) list.Add(child);
        for (int i = 0; i < list.Count; i++) DestroyImmediate(list[i].gameObject);
        quesResultItems.Clear();
    }

    public void setQuestions(List<Question> q) {
        questions = q; 
        questions.Sort();
        displayList();
        setQuestion(0);
    }
    public void setExercises(List<Exercise> e) {
        exercises = e; displayList();
        setExercise(0);
    }
    public void setExams(List<ExamSet> e) {
        examSets = e; displayList();
        setExamSet(0);
    }

    float createQuestionListObj(int index, Question q) {
        string sname = Subject.SubjectName[q.getSubjectId()];
        return createQuestionListObj(index, sname+ GameUtils.spaceEncode + q.getTitle());
    }
    float createQuestionListObj(int index, Exercise e) {
        return createQuestionListObj(index, e.getName());
    }
    float createQuestionListObj(int index, ExamSet e) {
        return createQuestionListObj(index, e.getName());
    }
    float createQuestionListObj(int index, string name) {
        var go = Instantiate(quesListPerfab, content, false);
        RectTransform rt = (RectTransform)go.transform;
        Text text = GameUtils.find<Text>(go, "Text");
        Button btn = GameUtils.button(go);
        string title = name;
        float h = rt.rect.height;
        text.text = (index + 1) + "." + GameUtils.spaceEncode + getTitleString(title);
        btn.onClick.AddListener(() => { onSelect(index); });
        rt.anchoredPosition = new Vector2(0, -index * h);
        quesListItems.Add(rt);
        return h;
    }
    string getTitleString(string title) {
        Regex reg = new Regex(@"<.+?>", RegexOptions.Singleline);
        title = reg.Replace(title, string.Empty);
        int len = Mathf.Min(TitleLength, title.Length);
        return title.Substring(0, len) + "...";
    }

    public void onSelect(int index) {
        switch (mode) {
            case 3:
                setExercise(index); break;
            case 4:
                setExamSet(index); break;
            default:
                setQuestion(index); break;
        }
    }

    public void setQuestion(int index) {
        if (index < 0) return;
        if (index >= questions.Count) return;
        Question q = questions[index];
        questionInfo.setQuestion(index, q, mode);
        collect.interactable = true;
        delete.interactable = true;
        full.interactable = true;
        curIndex = index;
        hightlightItem();
    }
    public void setExercise(int index) {
        if (index < 0) return;
        if (index >= exercises.Count) return;
        Exercise e = exercises[index];
        exerciseInfo.setExercise(e);
        delete.interactable = true;
        curIndex = index;
        hightlightItem();
    }
    public void setExamSet(int index) {
        if (index >= examSets.Count) return;
        if (index >= examSets.Count) return;
        ExamSet e = examSets[index];
        examSetInfo.setExamSet(e);
        curIndex = index;
        hightlightItem();
    }

    void hightlightItem() {
        for (int i = 0; i < quesListItems.Count; i++) {
            RectTransform rt = quesListItems[i];
            Image img = GameUtils.get<Image>(rt);
            Text text = GameUtils.find<Text>(rt, "Text");
            img.color = (curIndex == i ? highlightColor : normalColor);
            text.fontStyle = (curIndex == i ? FontStyle.Bold : FontStyle.Normal);
        }
        RectTransform srt = quesListItems[curIndex];
        if (content.anchoredPosition.y + list.rect.height < -srt.anchoredPosition.y)
            content.anchoredPosition = -srt.anchoredPosition;
    }

    public void showQuestionSetQuestions(QuestionSet qs) {
        List<Question> q = new List<Question>();
        int max = qs.getQuestionCount();
        for (int i = 0; i < max; i++)
            q.Add(qs.getQuestionObject(i));
        showQuestionSetQuestions(q);
    }
    public void showQuestionSetQuestions(List<Question> qs) {
        lastIndex = curIndex;
        mode = -mode; setQuestions(qs);
    }

    public void onDelete() {
        switch (mode) {
            case 0:
            case 1:
            case 2:
                questionInfo.deleteRecord();
                break;
            case 3:
                exerciseInfo.deleteRecord();
                break;
            default:
                GameUtils.alert("无法删除！");
                break;
        }
    }
    public void backToQuestionSet() {
        if (mode >= 0) return;
        selectMode(-mode);
        if (mode == 3) setExercise(lastIndex);
        if (mode == 4) setExamSet(lastIndex);
        lastIndex = -1;
    }


}
