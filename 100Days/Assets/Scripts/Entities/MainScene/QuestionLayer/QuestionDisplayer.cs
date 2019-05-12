using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionDisplayer : AnimatableLayer {
    static readonly Vector3 cameraPosition = new Vector3(0, 15.2f, 12.5f);
    static readonly Vector3 cameraRotation = new Vector3(80, 0, 0);

    public Vector3 initPosition = new Vector3(0, 6.8f, 14.4f);

    public GameObject buttons;

    public const string starIdentifier = "<quad name=Star size={0} width=1 />";
    const string lineSpliter = "_______________________________\n\n";

    protected Player player;

    void Awake() {
        base.Awake();
        player = GameSystem.getPlayer();
        //GameSystem.createPlayer();
    }

    protected string getLevelText(int level) {
        string text = "";
        for (int i = 0; i <= level; i++)
            text += starIdentifier;
        return text;
    }
    protected string getQuestionTextInExercise(int index, Question q, int fsize = 14) {
        string typeText = Question.TypeText[(int)q.getType()];
        string text = index+1 + ". (" + typeText + ")  ";
        text += getLevelText(q.getLevel()) + "\n";
        text = string.Format(text, fsize + 2);
        text += q.getTitle();
        return GameUtils.adjustText(text);
    }
    protected string getQuestionTextInAnswer(int index, Question q, int fsize = 14) {
        string typeText = Question.TypeText[(int)q.getType()];
        string text = index + 1 + ". (" + typeText + ")  ";
        text += getLevelText(q.getLevel()) + "\n";
        text = string.Format(text, fsize + 2);
        text += q.getTitle() + "\n";

        int cnt = q.getChoiceCount();
        for (int i = 0; i < cnt; i++)
            text += getQuestionChoiceText(i, q) + "\n";
        text += lineSpliter;
        return GameUtils.adjustText(text);
    }
    protected string getDescriptionText(Question q, int[] sels) {
        int[] corr = q.getCrtSelection();
        int score = q.calcScore(sels);
        bool correct = score == q.getScore();
        string color = correct ? "green" : "red";
        string text = getSelectionText("你的答案：", sels);
        text += getSelectionText("\n正确答案：", corr);
        text += "\n本题得分：" + score + "/" + q.getScore();
        text += "\n本题用时：" + GameUtils.time2Str(q.getLastTime());
        text = "<color=" + color + ">" + text + "</color>";
        text += "\n题解：\n" + q.getDesc();
        return GameUtils.adjustText(text);
    }
    protected string getDescriptionText(Question q) {
        int[] corr = q.getCrtSelection();
        string text = getSelectionText("\n正确答案：", corr);
        text += "\n做题次数：" + q.getCount();
        text += "\n上次用时：" + GameUtils.time2Str(q.getLastTime());
        text += "\n平均用时：" + GameUtils.time2Str(q.getAvgTime());
        text += "\n正确次数：" + q.getCrtCnt();
        text += "\n正确率　：" + Mathf.Round((float)q.getCrtRate() * 10000) / 100 + "%";
        text += "\n题解：\n" + q.getDesc();
        return GameUtils.adjustText(text);
    }
    protected string getQuestionChoiceTextWithAdjust(int index, Question q) {
        string choiceText = q.getChoiceText(index);
        string text = (char)('A' + index) + ". " + choiceText;
        return GameUtils.adjustText(text);
    }
    protected string getQuestionChoiceText(int index, Question q) {
        string choiceText = q.getChoiceText(index);
        string text = (char)('A' + index) + ". " + choiceText;
        return text;
    }
    protected string getSelectionText(string title, int[] sels) {
        foreach (int sel in sels) title += (char)('A' + sel);
        return title;
    }
    protected string getSubjectText(Question q) {
        return GameUtils.adjustText(Subject.SubjectName[q.getSubjectId()]);
    }
    protected string getQuestionStatText(Question q) {
        string text = "做题次数  " + q.getCount() + "\n";
        text += "正确次数  " + q.getCrtCnt() + "\n";
        text += "正确率　  " + Mathf.Round((float)q.getCrtRate()*10000)/100 + "%\n";
        text += "平均用时  " + GameUtils.time2Str(q.getAvgTime());
        return GameUtils.adjustText(text);
    }
    protected void setRectWidth(RectTransform rt, float w) {
        GameUtils.setRectWidth(rt, w);
    }
    protected void setRectHeight(RectTransform rt, float h) {
        GameUtils.setRectHeight(rt, h);
    }


    public void activateButtonsLayer() {
        buttons.SetActive(true);
    }
    public void deactivateButtonsLayer() {
        buttons.SetActive(false);
    }

    protected void playStartAni() {
        CameraControl ctr = GameUtils.getCameraControl();
        ctr.moveTo(cameraPosition, cameraRotation);
        ctr.setCallback(activateButtonsLayer);// () => { activateButtonsLayer(); });
    }

    public virtual void paperPositionReset() {
        Debug.Log("paperPositionReset: " + gameObject.name + " → " + initPosition);
        deactivateButtonsLayer();
        moveTo(initPosition, "enter");
        hideWindow(new Vector3(1, 1, 1));
        stopGeneralAnimation();
    }
    public virtual void paperEnter() {
        showWindow();
        deactivateButtonsLayer();
        Debug.Log("paperEnter: "+gameObject.name);
        moveTo(new Vector3(0, 6.8f, 14.4f), "enter", activateButtonsLayer);
    }
    public virtual void paperOut() {
        deactivateButtonsLayer();
        Debug.Log("paperOut: " + gameObject.name);
        hideWindow(new Vector3(1, 1, 1));
        moveTo(new Vector3(0, 6.8f, 0), "out", paperPositionReset);
    }
}
