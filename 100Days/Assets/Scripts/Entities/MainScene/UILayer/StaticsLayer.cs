using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaticsLayer : AnimatableLayer {

    public Color mainTabSelectColor;
    public Color mainTabDeselectColor;
    public Color subTabSelectColor;
    public Color subTabDeselectColor;

    public RectTransform mainTabs, subTabs;
    public RectTransform barsView;
    public Text info, createTime, totalTIme;

    public RectTransform[] bars;

    Player player;

    int mainId, subId;
    // Start is called before the first frame update
    void Awake() {
        base.Awake();
        player = GameSystem.getPlayer();
        Debug.Log("Awake!");
        reset();
    }
    void Start() {
        base.Start();
        reset();
    }

    // Update is called once per frame
    void Update() {
        base.Update();
        updateTotalTime();
    }

    void updateTotalTime() {
        totalTIme.text = "游戏时长：" + GameUtils.time2StrWithHour(GameSystem.getPlayTime());
    }

    public void reset() {
        setupSubjects();
        resetMainTabs();
        resetSubTabs();
        onMainToggle(0);
        onSubToggle(0);
    }
    public void resetMainTabs() {
        bool first = true; Toggle tg;
        foreach (RectTransform rt in mainTabs) {
            tg = GameUtils.get<Toggle>(rt);
            tg.isOn = false; first = false;
            Debug.Log(mainTabs.gameObject.name + "." + rt.gameObject.name + " = " + tg.isOn);
        }
    }
    public void resetSubTabs() {
        bool first = true; Toggle tg;
        foreach (RectTransform rt in subTabs) {
            tg = GameUtils.get<Toggle>(rt);
            tg.isOn = false; first = false;
            Debug.Log(subTabs.gameObject.name + "." + rt.gameObject.name + " = " + tg.isOn);
        }
    }
    public void refresh() {
        drawInfo(); drawContent();
        createTime.text = "开始时间：" + GameSystem.getCreateTime().ToString();
    }

    void setupSubjects() {
        int cnt = player.getSubjectCount();
        int[] sids = player.getSubjectIds();
        for (int i = 0; i < cnt; i++) {
            var rt = mainTabs.GetChild(i + 1);
            var txt = GameUtils.find<Text>(rt, "Text");
            var sname = Subject.SubjectName[sids[i]];
            txt.text = sname;
        }
    }

    public void onMainToggle(int index) {
        var rt = mainTabs.GetChild(index);
        var tg = GameUtils.get<Toggle>(rt);
        tg.isOn = true; selectMainTab(index);
    }
    public void onSubToggle(int index) {
        var rt = subTabs.GetChild(index);
        var tg = GameUtils.get<Toggle>(rt);
        tg.isOn = true; selectSubTab(index);
        //selectSubTab(index);
    }

    void selectMainTab(int index) {
        mainId = index;
        updateToggleGroup(mainTabs, mainTabSelectColor, mainTabDeselectColor);
        if (player != null) refresh();
    }
    void selectSubTab(int index) {
        subId = index;
        updateToggleGroup(subTabs, subTabSelectColor, subTabDeselectColor);
        if (player != null) refresh();
    }
    void updateToggleGroup(RectTransform rt,Color sel,Color dsel) {
        foreach (RectTransform child in rt) {
            var tg = GameUtils.get<Toggle>(child);
            var img = GameUtils.get<Image>(child);
            img.color = tg.isOn ? sel : dsel;
        }
    }

    int getSubjectId(int id = -1) {
        if (id == -1) id = mainId;
        if (id == 0) return DataSystem.SMAX;
        return player.getSubjectParamById(id-1).getId();
    }

    void drawInfo() {
        List<Subject> mins, maxs;
        int lmax = DataSystem.LMAX;
        int sid = getSubjectId();
        int maxScore = Subject.MaxScores[sid];
        int unlockCnt = RecordSystem.getQuestionCount(sid, lmax);
        float avgScore = (float)RecordSystem.getExamAvgScore(sid);
        float floatRate = (float)RecordSystem.getExamFloatRate(sid);
        int[] eval = FinalExam.evaluateScore(sid, player, avgScore, floatRate);
        mins = player.getMinSubjects(); maxs = player.getMaxSubjects();

        string minsName = "", maxsName = "";
        foreach (Subject s in mins) minsName += s.getName() + " ";
        foreach (Subject s in maxs) maxsName += s.getName() + " ";
        string text = "解锁数量：" + unlockCnt + "\n";
        text += "\n考试均分：" + (Mathf.Round(avgScore * 100) / 100) + "/" + maxScore;
        text += "\n波动程度：" + floatRate + "\n";
        text += "\n当前估分：" + eval[0] + "~" + eval[1] + "/" + maxScore + "\n";
        text += "\n强势科目：" + maxsName + "\n弱势科目：" + minsName;

        info.text = text;
    }
    void drawContent() {
        switch (subId) {
            case 0: drawUnlockRate(); break;
            case 1: drawCorrRate(); break;
            case 2: drawAvgTime(); break;
        }
    }
    public void clear() {
        clearInfo(); clearBars();
    }
    void clearInfo() {
        info.text = "";
    }
    void clearBars() {
        for (int i = 0; i < bars.Length; i++) {
            RectTransform rt = bars[i];
            Text val = GameUtils.find<Text>(rt, "Value");
            AnimatableLayer bar = GameUtils.find<AnimatableLayer>(rt, "Bar/Bar");
            val.text = ""; bar.scaleTo(new Vector3(0, 1, 1));
            bar.stopGeneralAnimation();
        }
    }
    void drawUnlockRate() {
        int sid = getSubjectId();
        for (int i = 0; i < bars.Length; i++) {
            int qc = RecordSystem.getQuestionCount(sid, i);
            int aqc = RecordSystem.getAllQuestionCount(sid, i);
            Debug.Log("Level " + i + " qc/aqc = " + qc + "/" + aqc);
            float rate = qc * 1.0f / aqc;
            RectTransform rt = bars[i];
            Text val = GameUtils.find<Text>(rt, "Value");
            AnimatableLayer bar = GameUtils.find<AnimatableLayer>(rt, "Bar/Bar");
            Image img = GameUtils.find<Image>(rt, "Bar/Bar"); bar.image = img;
            val.text = Mathf.Round(rate * 10000) / 100 + "%";
            bar.scaleTo(new Vector3(rate, 1, 1));
            bar.colorTo(new Color(1 - rate, rate, 0));
        }
    }
    void drawCorrRate() {
        int sid = getSubjectId();
        for (int i = 0; i < bars.Length; i++) {
            float rate = (float)RecordSystem.getQuestionCorrRate(sid, i);
            RectTransform rt = bars[i];
            Text val = GameUtils.find<Text>(rt, "Value");
            AnimatableLayer bar = GameUtils.find<AnimatableLayer>(rt, "Bar/Bar");
            Image img = GameUtils.find<Image>(rt, "Bar/Bar"); bar.image = img;
            if (rate < 0) {
                val.text = "--";
                bar.scaleTo(new Vector3(0, 1, 1));
                bar.colorTo(new Color(1, 0, 0));
            } else {
                val.text = Mathf.Round(rate * 10000) / 100 + "%";
                bar.scaleTo(new Vector3(rate, 1, 1));
                bar.colorTo(new Color(1 - rate, rate, 0));
            }
        }
    }
    void drawAvgTime() {
        int sid = getSubjectId(), avgStdMin = 0;
        for (int i = 0; i < bars.Length; i++) {
            TimeSpan ts = RecordSystem.getQuestionAvgTime(sid, i);
            int stdMin = ((i == bars.Length - 1) ? avgStdMin / i :
                Question.LevelMinute[i]);
            float avgMin = (float)ts.TotalMinutes;
            float rate = (avgMin / stdMin - 0.5f)*10;
            avgStdMin += stdMin;
            rate = sigmoid(rate);
            RectTransform rt = bars[i];
            Text val = GameUtils.find<Text>(rt, "Value");
            AnimatableLayer bar = GameUtils.find<AnimatableLayer>(rt, "Bar/Bar");
            Image img = GameUtils.find<Image>(rt, "Bar/Bar"); bar.image = img;
            if (ts.Ticks <= 0) {
                val.text = "--";
                bar.scaleTo(new Vector3(0, 1, 1));
                bar.colorTo(new Color(0, 1, 0));
            } else {
                val.text = GameUtils.time2Str(ts);
                bar.scaleTo(new Vector3(rate, 1, 1));
                bar.colorTo(new Color(rate, 1 - rate, 0));
            }
        }
    }

    float sigmoid(float x) {
        return 1 / (1 + Mathf.Exp(-x));
    }
}
