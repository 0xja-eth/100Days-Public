using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionDisplayer : MonoBehaviour {
    static readonly Vector3 targetPosition = new Vector3(0, 14.2f, 8.1f);
    static readonly Vector3 targetRotation = new Vector3(80, 0, 0);

    public GameObject buttons;

    public const string spaceIdentifier = "&S&";
    public const string starIdentifier = "<quad" +
        spaceIdentifier + "name=Star" + spaceIdentifier + "size=13" +
        spaceIdentifier + "width=1" + spaceIdentifier + "/>";
    const string spaceEncode = "\u00A0";
    const string lineSpliter = "_______________________________\n\n";
    protected string getLevelText(int level) {
        string text = "";
        for (int i = 0; i <= level; i++)
            text += starIdentifier;
        return text;
    }
    protected string getQuestionTextInExercise(int index, Question q) {
        string typeText = Question.TypeText[(int)q.getType()];
        string text = index+1 + ". (" + typeText + ")  ";
        text += getLevelText(q.getLevel()) + "\n";
        text += q.getTitle();
        return adjustText(text);
    }
    protected string getQuestionTextInAnswer(int index, Question q) {
        string typeText = Question.TypeText[(int)q.getType()];
        string text = index+1 + ". (" + typeText + ")  ";
        text += getLevelText(q.getLevel()) + "\n";
        text += q.getTitle() + "\n";

        int cnt = q.getChoiceCount();
        for (int i = 0; i < cnt; i++)
            text += getQuestionChoiceText(i, q)+"\n";
        text += lineSpliter;
        return adjustText(text);
    }
    protected string getDescriptionText(int index, Question q, int[] sels) {
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
        return adjustText(text);
    }
    protected string getQuestionChoiceTextWithAdjust(int index, Question q) {
        string choiceText = q.getChoiceText(index);
        string text = (char)('A' + index) + ". " + choiceText;
        return adjustText(text);
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
        return adjustText(Subject.SubjectName[q.getSubjectId()]);
    }
    protected string getQuestionStatText(Question q) {
        string text = "做题次数  " + q.getCount() + "\n";
        text += "正确次数  " + q.getCrtCnt() + "\n";
        text += "正确率　  " + Mathf.Round((float)q.getCrtRate()*10000)/100 + "%\n";
        text += "平均用时  " + GameUtils.time2Str(q.getAvgTime());
        return adjustText(text);
    }
    protected void setRectWidth(RectTransform rt, float w) {
        GameUtils.setRectWidth(rt, w);
    }
    protected void setRectHeight(RectTransform rt, float h) {
        GameUtils.setRectHeight(rt, h);
    }
    protected string adjustText(string text) {
        text = text.Replace(" ", spaceEncode);
        text = text.Replace(spaceIdentifier, " ");
        return text.ToString();
    }

    public void activateButtonsLayer() {
        buttons.SetActive(true);
    }

    protected void playStartAni() {
        CameraControl ctr = GameUtils.getCameraControl();
        ctr.moveTo(targetPosition, targetRotation);
        ctr.setCallback(() => { activateButtonsLayer(); });
    }
}
