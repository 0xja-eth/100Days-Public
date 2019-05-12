using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExamSetInfoView : MonoBehaviour {
    const int MaxCol = 5;

    public Color correctTextColor = new Color(0, 0.5f, 0);
    public Color correctBackgroundColor = new Color(0.5f, 1, 0.5f);
    public Color wrongTextColor = new Color(0.5f, 0, 0);
    public Color wrongBackgroundColor = new Color(1, 0.5f, 0.5f);

    public RecordLayer recordLayer;

    public RectTransform resultContent;
    public AnimatableLayer questionsResult;

    public Text title, difficulty;
    public RectTransform[] subjects; // 包括 Sum
    public RadarSetting radar;
    public Button detail, expendResult;

    ExamSet examSet;
    List<Question> questions;

    Vector2 oriResultSize = new Vector2(588,144);
    bool expended = false;

    private void Awake() {
        /*
        RectTransform rt = (RectTransform)questionsResult.transform;
        oriResultSize = rt.rect.size;
        Debug.Log("oriResultSize = " + oriResultSize);*/
    }

    public void setExamSet(ExamSet e) {
        examSet = e;
        clear(false);
        title.text = e.getName();
        difficulty.text = "难度系数：" + e.getDifficulty();
        //detail1.text = detail2.text = "";
        drawExamScoreInfo();
        createQuestionResult();
        detail.interactable = true;
        detail.onClick.RemoveAllListeners();
        detail.onClick.AddListener(showQuestionsDetail);
    }
    void drawExamScoreInfo() {
        List<int> scores = new List<int>();
        List<int> maxs = new List<int>();
        List<float> values = new List<float>();
        List<string> names = new List<string>();

        int sum = 0, sumMax = 0;
        int cnt = examSet.getExamCount();
        for (int i = 0; i < cnt; i++) {
            Exam e = examSet.getExamById(i);
            int score = e.getFinalScore();
            int sid = e.getSubjectId();
            string sname = Subject.SubjectName[sid];
            int max = Subject.MaxScores[sid];
            sum += score; sumMax += max;
            scores.Add(score); maxs.Add(max);
            values.Add(score * 1.0f / max);
            names.Add(sname);
        }
        scores.Add(sum);
        maxs.Add(sumMax);
        values.Add(sum * 1.0f / sumMax);
        names.Add("总分");

        drawExamBar(scores, maxs, values, names);
        drawExamRadar(values, names);
    }
    void drawExamBar(List<int> scores, List<int> maxs, 
        List<float> values, List<string> names) {
        for (int i = 0; i < subjects.Length; i++) {
            Text name = GameUtils.find<Text>(subjects[i], "Name");
            Text value = GameUtils.find<Text>(subjects[i], "Value");
            AnimatableLayer bar = GameUtils.find<AnimatableLayer>(subjects[i], "Bar/Bar");
            Image img = GameUtils.find<Image>(subjects[i], "Bar/Bar");
            bar.image = img;
            name.text = names[i];
            value.text = scores[i] + "/" + maxs[i];
            bar.scaleTo(new Vector3(values[i], 1, 1));
            bar.colorTo(new Color(1 - values[i], values[i], 0));
        }
    }
    void drawExamRadar(List<float> values, List<string> names) {
        radar.setNames(names);
        radar.setValues(values);
    }
    void createQuestionResult() {
        float w = 0, h = 0;
        questions = new List<Question>();
        int cnt = 0;
        for (int i = 0; i < examSet.getExamCount(); i++) {
            Exam exam = examSet.getExamById(i);
            int qcnt = exam.getQuestionCount();
            string sname = Subject.SubjectName[exam.getSubjectId()];
            for (int j = 0; j < qcnt; j++) {
                Question q = exam.getQuestionObject(j);
                string txt = sname + " " + (j+1);
                int[] sel = exam.getSelections(j);
                bool corr = q.isCorrect(sel);
                Color tc = corr ? correctTextColor : wrongTextColor;
                Color bgc = corr ? correctBackgroundColor : wrongBackgroundColor;
                txt += corr ? "\n答案正确" : "\n答案错误";

                var go = Instantiate(recordLayer.resultListPerfab, resultContent);
                RectTransform rt = (RectTransform)go.transform;
                Button btn = GameUtils.button(go);
                Image img = GameUtils.get<Image>(go);
                Text text = GameUtils.find<Text>(go, "Text");
                text.text = txt; text.color = tc; img.color = bgc;

                w = rt.rect.width; h = rt.rect.height;
                rt.anchoredPosition = new Vector2(w * (cnt % MaxCol), 
                    -h * Mathf.FloorToInt(cnt / MaxCol));

                Debug.Log(cnt + " sel : " + string.Join(",", sel));
                Debug.Log(cnt + " : " + rt.anchoredPosition);

                go.name = cnt.ToString();

                btn.onClick.AddListener(() => {
                    showQuestionsDetail(int.Parse(go.name));
                });

                recordLayer.addQuestionResultObjs(rt);
                questions.Add(q); cnt++;
            }
        }
        expendResult.gameObject.SetActive(true);
        expendResult.interactable = (cnt > 10);
        GameUtils.setRectHeight(resultContent, h * Mathf.CeilToInt(cnt * 1.0f / MaxCol));
    }
    public void clear(bool all = true) {
        title.text = "";
        closeQuestionResult();
        detail.interactable = false;
        expendResult.gameObject.SetActive(false);
        recordLayer.clearQuestionResultObjs();
        if (all) clearExamScoreInfo();
    }
    void clearExamScoreInfo() {
        clearExamBar(); clearExamRadar();
    }
    void clearExamRadar() {
        radar.clear();
    }
    void clearExamBar() {
        for (int i = 0; i < subjects.Length; i++) {
            Text value = GameUtils.find<Text>(subjects[i], "Value");
            AnimatableLayer bar = GameUtils.find<AnimatableLayer>(subjects[i], "Bar/Bar");
            value.text = ""; bar.scaleTo(new Vector3(0, 1, 1));
            bar.stopGeneralAnimation();
        }
    }
    public void showQuestionsDetail(int index) {
        Debug.Log("ShowQuestion " + index);

        recordLayer.showQuestionSetQuestions(questions);
        recordLayer.setQuestion(index);
    }
    public void showQuestionsDetail() {
        showQuestionsDetail(0);
    }

    public void toggleQuestionResult() {
        if (expended) closeQuestionResult();
        else expendQuestionResult();
    }
    public void expendQuestionResult() {
        expended = true;
        Vector2 ts = oriResultSize;
        ts.y = Mathf.Max(ts.y, resultContent.rect.height);
        questionsResult.resizeTo(ts);
        Text txt = GameUtils.find<Text>(expendResult.transform, "Text");
        txt.text = "收起答题结果";
    }
    public void closeQuestionResult() {
        expended = false;
        questionsResult.resizeTo(oriResultSize);
        Text txt = GameUtils.find<Text>(expendResult.transform, "Text");
        txt.text = "展开答题结果";
    }
}
