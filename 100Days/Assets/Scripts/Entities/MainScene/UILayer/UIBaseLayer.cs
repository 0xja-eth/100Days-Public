using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UIBaseLayer : MonoBehaviour {

    static readonly Vector3 GameCameraPosition = new Vector3(36.4f, 16.2f, -96.5f);
    static readonly Vector3 GameCameraRotation = new Vector3(0, 180, 0);

    static Vector3 cameraPosition;
    static Vector3 cameraRotation;

    public ExamLayer examLayer;
    public ExerciseLayer exerciseLayer;

    public DateLayer dateLayer;
    public ShortStatusLayer statusLayer;
    public ScheduleLayer scheduleLayer;
    public MenuLayer menuLayer;
    public AnimatableLayer gameLayer;
    public AnimatableLayer night;

    Player player;
    ExamSet exams;
    Exercise exercise;

    bool isUIShown = false;
    // Use this for initialization
    void Awake() {
        GameUtils.initialize("Canvas2D/UILayer", 
            "Canvas2D/PromptLayer/AlertWindow",
            "Canvas2D/PromptLayer/LoadingScreen");
        cameraPosition = GameUtils.getCamera().position;
        cameraRotation = GameUtils.getCamera().eulerAngles;
        Debug.Log("cameraPosition:" + cameraPosition);
        Debug.Log("cameraRotation:" + cameraRotation);
        /*cameraPosition = new Vector3(127.7f, 33.9f, -80.3f);
        cameraRotation = new Vector3(8.3f, 297.0f, 0.0f);*/
        player = GameSystem.getPlayer();
        Debug.Log(player);
    }

    void Start () {
        refresh();
        if (GameSystem.isFirst()) startFirstExam();
        else if (checkExam()) showUILayer();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void nextDay() {
        GameSystem.nextDay();
        NetworkSystem.setSuccessHandler(null);
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
        if (checkExam()) showUILayer();
    }
    void deactivateNightMask() {
        night.colorTo(new Color(0, 0, 0, 0), "deactivate");
        night.hideWindow(new Vector3(1, 1, 1));
        night.stopGeneralAnimation();
    }

    public void refresh() {
        scheduleLayer.refresh();
        dateLayer.refresh();
        statusLayer.refresh();
    }
    // 是否不考试（
    bool checkExam() {
        if (GameSystem.isEmptySchedule() && GameSystem.getDeltaDays() <= 0) {
            onGameOver();
            return false;
        } else {
            ExamSet es = GameSystem.getTodaysExam();
            bool res = es == null;
            if (!res) generateExamHelp(es);
            return res;
        }
    }

    public void showUILayer() {
        refresh();
        if (isUIShown) return;
        isUIShown = true;
        dateLayer.layerEnter();
        scheduleLayer.layerEnter();
        menuLayer.layerEnter();
        statusLayer.layerEnter();
    }
    public void hideUILayer() {
        if (!isUIShown) return;
        isUIShown = false;
        dateLayer.layerOut();
        scheduleLayer.layerOut();
        menuLayer.layerOut();
        statusLayer.layerOut();
    }

    public void backToUILayer() {
        resetCamera();
        showUILayer();
    }

    public void resetCamera() {
        CameraControl ctr = GameUtils.getCameraControl();
        ctr.moveTo(cameraPosition, cameraRotation);
    }

    public void startExercise(Exercise e = null) {
        if (e != null) exercise = e;
        NetworkSystem.setup(onExerciseGenerateSuccess,
            onExerciseGenerateError, true, "读取题目中...");
        exercise.generateQuestions();
    }
    public void startExamSet(ExamSet e = null) {
        if (e != null) exams = e;
        NetworkSystem.setup(onExamGenerateSuccess,
            onExamGenerateError, true, "读取题目中...");
        exams.generateQuestions();
    }

    void startFirstExam() {
        GameSystem.setFirst(false);
        generateFirstExamHelp();
        //generateFirstExam();
    }
    void generateExamHelp(ExamSet e) {
        GameUtils.alert("现在将要进行 " + e.getName() + " ！\n" + e.getSpoken(),
            new string[] { null, "开始考试" }, new UnityAction[] { null,
                ()=> { startExamSet(e); } });
    }

    /*
    void generateFirstExam() {
        exams.generateQuestions(onFirstExamGenerateSuccess,
            onFirstExamGenerateError, true, "读取题目中...");
    }
    */
    void onExerciseGenerateSuccess(RespondJsonData data) {
        Debug.Log(data);
        ExerciseRespondJsonData eData = ExerciseRespondJsonData.fromJson(data.getJson());
        exercise.loadQuestions(eData);
        exerciseLayer.setExercise(exercise);
        hideUILayer();
    }
    void onExerciseGenerateError(RespondStatus status, string errmsg) {
        Debug.LogError(status + " : " + errmsg);
        GameUtils.alert("题目获取失败：" + errmsg,
            new string[] { null, "重试" }, new UnityAction[] { null, () => { startExercise(); } });
    }

    void onExamGenerateSuccess(RespondJsonData data) {
        Debug.Log(data);
        ExamRespondJsonData eData = ExamRespondJsonData.fromJson(data.getJson());
        exams.loadQuestions(eData);
        examLayer.setExamSet(exams);
        GameSystem.removeSchedule();
        hideUILayer();
    }
    void onExamGenerateError(RespondStatus status, string errmsg) {
        Debug.LogError(status+" : "+errmsg);
        GameUtils.alert("题目获取失败："+ errmsg,
            new string[] { null, "重试" }, new UnityAction[] { null, ()=> { startExamSet(); } });
    }

    void generateFirstExamHelp() {
        GameUtils.alert("现在将要进行摸底考！摸底考将用于生成玩家最初的科目点数，请认真进行摸底考试！",
            new string[] { null, "开始考试" }, new UnityAction[] { null,
                ()=> { startExamSet(new FirstExam(player.getSubjectIds())); } });
    }

    public void backToTitle() {
        GameUtils.alert("确定保存并返回主菜单吗？",
            new string[] { null, "是", "否" }, new UnityAction[] { null, onBackConfirm, null });
    }
    public void exitGame() {
        GameUtils.alert("确定保存并退出游戏吗？",
            new string[] { null, "是", "否" }, new UnityAction[] { null, onExitGameConfirm, null });
    }

    void onBackConfirm() {
        NetworkSystem.setSuccessHandler((data) => {
            SceneManager.LoadScene("GameTitleScene");
        });
        StorageSystem.saveGame();
    }
    void onExitGameConfirm() {
        NetworkSystem.setSuccessHandler((data) => {
            GameSystem.onGameEnd();
        });
        StorageSystem.saveGame();
    }

    public void onGameOver() {
        night.showWindow();
        night.colorTo(new Color(0, 0, 0, 1), "fadeOut", ()=> {
            SceneManager.LoadScene("GraduateScene");
        });
    }

    protected void playStartAni() {
    }
    public void showGameLayer() {
        CameraControl ctr = GameUtils.getCameraControl();
        ctr.moveTo(GameCameraPosition, GameCameraRotation);
        ctr.setCallback(gameLayer.showWindow);
        hideUILayer();
    }
    public void hideGameLayer() {
        gameLayer.hideWindow();
        resetCamera();
        showUILayer();
    }
    public void startGame(string name) {
        gameLayer.hideWindow(() => { SceneManager.LoadScene(name); }, Vector3.zero);
    }
}
