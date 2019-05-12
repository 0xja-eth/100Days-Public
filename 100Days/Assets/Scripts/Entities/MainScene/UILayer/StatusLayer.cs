using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusLayer : AnimatableLayer {
    public Text name, school, energy, pressure, curDate, finalDate, rest, diff;
    public AnimatableLayer energyBar, pressureBar;
    public RectTransform[] subjects;
    public RadarSetting radar;

    Player player;

    // Start is called before the first frame update
    void Awake() {
        base.Awake();
        player = GameSystem.getPlayer();
    }

    public void openWindow() {
        showWindow();
        refresh();
    }

    public void refresh() {
        refreshBaseInfo();
        refreshDateInfo();
        refreshExamInfo();
    }

    void refreshBaseInfo() {
        name.text = player.getName();
        school.text = "学校：" + player.getSchool();

        refreshEnergy();
        refreshPressure();
    }
    void refreshEnergy() {
        int max = player.getMaxEnergy();
        int cur = player.getEnergy();
        float rate = cur * 1.0f / max;
        energy.text = cur + "/" + max;
        energyBar.scaleTo(new Vector3(rate, 1, 1));
    }
    void refreshPressure() {
        int max = player.getMaxPressure();
        int cur = player.getPressure();
        float rate = cur * 1.0f / max;
        pressure.text = cur + "/" + max;
        pressureBar.scaleTo(new Vector3(rate, 1, 1));
        pressureBar.colorTo(new Color(rate, 1 - rate, 1 - rate));
    }

    void refreshDateInfo() {
        curDate.text = "当前日期：" + GameSystem.getCurDate().ToString("yyyy年MM月dd日");
        finalDate.text = "高考日期：" + GameSystem.getFinalDate().ToString("yyyy年MM月dd日");
        rest.text = "高考倒计时\n<size=48>" + GameSystem.getDays() + "</size> ";
    }
    void refreshExamInfo() {
        setExams(RecordSystem.getLastExamSet());
    }

    public void setExams(ExamSet exams) {
        List<int> scores = new List<int>();
        List<int> maxs = new List<int>();
        List<float> values = new List<float>();
        List<string> names = new List<string>();

        int cnt = exams.getExamCount();
        for (int i = 0; i < cnt; i++) {
            Exam e = exams.getExamById(i);
            int score = e.getFinalScore();
            int sid = e.getSubjectId();
            string sname = Subject.SubjectName[sid];
            int max = Subject.MaxScores[sid];
            scores.Add(score); maxs.Add(max);
            values.Add(score * 1.0f / max);
            names.Add(sname);
        }

        setRadar(values, names);

        setContents((float)exams.getDifficulty(),
            scores, maxs, values, names);
    }
    void setRadar(List<float> values, List<string> names) {
        radar.setNames(names);
        radar.setValues(values);
    }
    void setContents(float difficulty, List<int> scores,
        List<int> maxs, List<float> values, List<string> names) {
        diff.text = "难度系数：" + difficulty;
        for(int i = 0; i < scores.Count; i++) {
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
}
