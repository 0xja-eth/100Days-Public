using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ExerciseInfoView : MonoBehaviour {
    const int MaxCol = 5;

    public Color correctTextColor = new Color(0, 0.5f, 0);
    public Color correctBackgroundColor = new Color(0.5f, 1, 0.5f);
    public Color wrongTextColor = new Color(0.5f, 0, 0);
    public Color wrongBackgroundColor = new Color(1, 0.5f, 0.5f);

    public RecordLayer recordLayer;

    public Text title, detail1, detail2;

    public RectTransform resultContent;

    public Button detail;

    Exercise exercise;

    public void setExercise(Exercise e) {
        clear();
        exercise = e;
        title.text = e.getName();
        //detail1.text = detail2.text = "";
        generateExerciseDetail();
        createQuestionResult();
        detail.interactable = true;
        detail.onClick.RemoveAllListeners();
        detail.onClick.AddListener(showQuestionsDetail);
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
        t2 += "正确率：" + Mathf.Round((float)crtcnt / cnt * 10000) / 100 + "%\n";
        t2 += "新题目：" + exercise.getNewQuestionCnt() + "\n";
        t2 += "压力增加：" + exercise.getPressurePlus();

        detail1.text = t1;
        detail2.text = t2;
    }
    void createQuestionResult() {
        float w = 0, h = 0;
        int max = exercise.getQuestionCount();
        for (int i = 0; i < max; i++) {
            Question q = exercise.getQuestionObject(i);
            string txt = "题目 " + (i+1);
            int[] sel = exercise.getSelections(i);
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
            rt.anchoredPosition = new Vector2(w * (i % MaxCol), -h * Mathf.FloorToInt(i / MaxCol));

            int tmpi = i;
            btn.onClick.AddListener(()=> {
                showQuestionsDetail(tmpi);
            });

            recordLayer.addQuestionResultObjs(rt);
        }
        GameUtils.setRectHeight(resultContent, h * Mathf.CeilToInt(max*1.0f / MaxCol));
    }
    public void showQuestionsDetail(int index) {
        Debug.Log("ShowQuestion " + index);
        recordLayer.showQuestionSetQuestions(exercise);
        recordLayer.setQuestion(index);
    }
    public void showQuestionsDetail() {
        showQuestionsDetail(0);
    }
    public void clear() {
        title.text = "";
        detail1.text = "";
        detail2.text = "";
        detail.interactable = false;
        recordLayer.clearQuestionResultObjs();
    }
    public void deleteRecord() {
        GameUtils.alert("确定删除该刷题记录吗？\n删除后该次刷题记录将不会再在你的刷题记录中出现。",
            new string[] { null, "确定", "返回" }, new UnityAction[] { null, onDeleteRecordConfirm, null });
    }
    void onDeleteRecordConfirm() {
        exercise.delete();
        recordLayer.refresh();
    }

}
