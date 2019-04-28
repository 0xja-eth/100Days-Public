using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UIBaseLayer : MonoBehaviour {
    static Vector3 cameraPosition;
    static Vector3 cameraRotation;

    public ExamLayer examLayer;
    public ExerciseLayer exerciseLayer;

    public DateLayer dateLayer;
    public ScheduleLayer scheduleLayer;
    public MenuLayer menuLayer;
    public AnimatableLayer night;

    bool isUIShown = false;
    // Use this for initialization
    void Awake() {
        GameUtils.initialize("Canvas2D/UILayer", "Canvas2D/PromptLayer/AlertWindow");
        cameraPosition = GameUtils.getCamera().position;
        cameraRotation = GameUtils.getCamera().eulerAngles;
    }

    void Start () {
        refresh();
        if (GameSystem.isFirst()) startFirstExam();
        else showUILayer();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void nextDay() {
        GameSystem.nextDay();
        StorageSystem.saveGame();
        hideUILayer();
        night.showWindow();
        night.colorTo(new Color(0, 0, 0, 1),"fadeOut", dayAnimation);
    }
    void dayAnimation() {
        night.colorTo(new Color(1, 1, 1, 1), "animation", dayBegin);
    }
    void dayBegin() {
        night.colorTo(new Color(1, 1, 1, 0), "fadeIn", deactivateNightMask);
        showUILayer();
    }
    void deactivateNightMask() {
        night.colorTo(new Color(0, 0, 0, 0), "deactivate");
        night.hideWindow();
        night.stopGeneralAnimation();
    }

    public void refresh() {
        scheduleLayer.refresh();
        dateLayer.refresh();
    }

    public void showUILayer() {
        refresh();
        if (isUIShown) return;
        isUIShown = true;
        dateLayer.layerEnter();
        scheduleLayer.layerEnter();
        menuLayer.layerEnter();
    }
    public void hideUILayer() {
        if (!isUIShown) return;
        isUIShown = false;
        dateLayer.layerOut();
        scheduleLayer.layerOut();
        menuLayer.layerOut();
    }

    public void backToUILayer() {
        resetCamera();
        showUILayer();
    }

    public void resetCamera() {
        CameraControl ctr = GameUtils.getCameraControl();
        ctr.moveTo(cameraPosition, cameraRotation);
    }

    public void startExercise(Exercise e) {
        exerciseLayer.setExercise(e);
        hideUILayer();
    }
    public void startExamSet(ExamSet e) {
        examLayer.setExamSet(e);
        hideUILayer();
    }

    void startFirstExam() {
        Player player = GameSystem.getPlayer();
        ExamSet exams = new FirstExam(player.getSubjectIds());
        generateFirstExamHelp();
        startExamSet(exams);
    }
    void generateFirstExamHelp() {
        GameUtils.alert("现在进行摸底考！摸底考将用于生成最初的科目点数，请好好考试！",
            new string[] { null, "知道了" }, new UnityAction[] { null, null });
    }

    public void backToTitle() {
        GameUtils.alert("确定返回主菜单吗？当前进度将不会保存！\n（存档每天结束自动保存）",
            new string[] { null, "是", "否" }, new UnityAction[] { null,
            ()=> { SceneManager.LoadScene("GameTitleScene"); }, null });
    }
    public void exitGame() {
        GameUtils.alert("确定退出游戏吗？当前进度将不会保存！\n（存档每天结束自动保存）",
            new string[] { null, "是", "否" }, new UnityAction[] { null, Application.Quit, null });
    }
}
