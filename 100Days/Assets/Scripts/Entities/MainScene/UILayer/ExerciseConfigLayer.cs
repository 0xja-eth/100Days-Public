using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExerciseConfigLayer : AnimatableLayer {
    const int MinCount = 1;
    const int MaxCount = 10;

    public ExerciseLayer exerciseLayer;

    public UIBaseLayer uiBaseLayer;

    public Text time, count, cost;
    public Scrollbar countBar;
    public Dropdown subject, mode;

    Player player;
    // Use this for initialization
    void Awake() {
        base.Awake();
        player = GameSystem.getPlayer();
        initializeSubjects();
    }
    void Start () {
        base.Start();
        refreshEvaluate();
    }
	
	// Update is called once per frame
	void Update () {
        base.Update();
    }

    int getCount(float rate) {
        return (int)(MinCount + (MaxCount - MinCount) * rate);
    }
    int getCurCount() {
        return getCount(countBar.value);
    }

    void initializeSubjects() {
        subject.options.Clear();
        subject.options.Add(new Dropdown.OptionData("随机"));
        for (int i = 0; i < player.getSubjectCount(); i++) 
            subject.options.Add(new Dropdown.OptionData(
                player.getSubjectParam(i).getName()));
        subject.value = 0;
    }
    public void refreshEvaluate() {
        int sid = subject.value;
        int cnt = getCurCount();
        int eng = player.getEnergy();
        int value = 0;
        if (sid == 0) { // 随机
            int scnt = player.getSubjectCount();
            for (int i = 0; i < scnt; i++)
                value += player.getSubjectParamValueById(i) / scnt;
        } else value = player.getSubjectParamValueById(sid - 1);
        int avgLevel = DataSystem.getMaxLevel(value) / 2;
        int min = Question.LevelMinute[avgLevel] * cnt;
        int cot = Question.EnergyCost[avgLevel] * cnt;
        cost.color = cot > eng ? Color.red : Color.black;
        count.text = cnt.ToString();
        cost.text = cot + "/" + eng;
        time.text = min + " min";
    }
    public void startExercise() {
        exerciseLayer.setExercise(produceExercise());
        uiBaseLayer.hideUILayer();
        hideWindow(new Vector3(1, 0, 0));
    }
    public void closeWindow() {
        hideWindow(new Vector3(1, 0, 0));
    }

    Exercise produceExercise() {
        int scnt = player.getSubjectCount();
        int sid = subject.value;
        int type = mode.value;
        int cnt = getCurCount();
        if (sid == 0) sid = Random.Range(1, scnt+1);
        return new Exercise(cnt, sid-1, (DataSystem.QuestionDistribution.Type)type, null, player);
    }
}
