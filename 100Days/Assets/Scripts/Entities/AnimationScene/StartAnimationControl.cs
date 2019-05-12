using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class StartAnimationControl : MonoBehaviour {
    float moveSpeed = 0.6f;
    float fadeSpeed = 0.015f;
    float stoppingY = 92f;

    public RectTransform textRect;
    public GameObject continueBtn;
    public GameObject skipBtn;
    public GameObject background;
    public InputController inputLayer;
    public SubjectController subjectLayer;

    Text text;
    Image buttonImg;

    string[] schools;

    bool speed = false, textAnimating = true;
    bool pause = false, pausable = true;
    bool btnAniType = true;
    string status = "ani";
    // Use this for initialization
    void Awake() {
        GameUtils.initialize(null,
            "Canvas/PromptLayer/AlertWindow",
            "Canvas/PromptLayer/LoadingScene");
        refreshSchoolData();
    }
    void Start () {
        text = GameUtils.text(textRect);
        buttonImg = GameUtils.get<Image>(continueBtn);
        text.text = generateStartText();
    }
    public void refreshSchoolData() {
        NetworkSystem.setSuccessHandler(onGetSchoolSuccess);
        NetworkSystem.setErrorHandler(onGetSchoolError);
        NetworkSystem.setTipsText("获取学校列表中...");
        NetworkSystem.postRequest(NetworkSystem.GetSchoolRoute);
    }
    void onGetSchoolSuccess(RespondJsonData data) {
        Debug.Log(data);
        SchoolRespondData sData = SchoolRespondData.fromJson(data.getJson());
        GameSystem.setSchools(sData);
        inputLayer.setSchools(GameSystem.getSchools());
    }
    void onGetSchoolError(RespondStatus status,string errmsg) {
        GameUtils.alert("学校数据读取失败："+errmsg,
            new string[] { null, "重试","取消"},
            new UnityAction[] { null, refreshSchoolData, null});
    }

	string generateStartText() {
        return String.Format(text.text, GameSystem.getDeltaDays());
    }
	
    // Update is called once per frame
	void Update () {
        updatePause();
        updateTextAnimation();
        updateButtonAnimation();
    }
    void updatePause() {
        if(pausable) pause = background.activeInHierarchy;
    }
    void updateTextAnimation() {
        if (!pause) textRect.position = textRect.position + Vector3.up * getMoveSpeed();
        if (isTextAnimationStopping() && textAnimating) onTextAnimationEnd();
    }
    void updateButtonAnimation() {
        if (!textAnimating) { 
            fadeOutText(); fadeInOutButon();
        }
    }
    void fadeOutText() {
        Color c = text.color; c.a -= fadeSpeed; text.color = c;
    }
    void fadeInOutButon() {
        Color c = buttonImg.color;
        if (btnAniType) btnAniType = (c.a += fadeSpeed) < 0.95;
        else btnAniType = (c.a -= fadeSpeed) < 0.5;
        buttonImg.color = c;
    }
    bool isTextAnimationStopping() {
        return textRect.position.y > textRect.rect.height + stoppingY;
    }
    void onTextAnimationEnd() {
        status = "name";
        pausable = pause = false;
        textAnimating = false;
        skipBtn.SetActive(false);
        continueBtn.SetActive(true);
        inputLayer.showWindow();
    }

    public void toggleAnimation() {
        if (pausable) pause = !pause;
    }
    public void toggleSpeedAnimation() {
        speed = !speed;
    }
    float getMoveSpeed() {
        return speed ? moveSpeed * 5 : moveSpeed;
    }

    public void skipAnimation() {
        moveSpeed *= 50;
    }

    public void onContinue() {
        switch (status) {
            case "name":
                onNameLayerContinue();
                break;
            case "subject":
                onSubjectLayerContinue();
                break;
        }
    }

    void onNameLayerContinue() {
        if (checkNameLayer()) pushPlayerInfo();
        else
            GameUtils.alert("创建玩家失败：信息输入有误",
                new string[] { null, "关闭" },
                new UnityAction[] { null, null });
    }
    bool checkNameLayer() {
        return inputLayer.check();
    }

    void pushPlayerInfo() {
        string name = inputLayer.getName();
        string school = inputLayer.getSchool();
        NetworkSystem.setSuccessHandler(onPushSuccess);
        StorageSystem.registerGameToServer(name, school, onPushError);
    }

    void onPushSuccess(RespondJsonData data) {
        status = "subject";
        Text btnTxt = GameUtils.find<Text>(continueBtn, "Text");
        btnTxt.text = "开始游戏";
        inputLayer.hideWindow(new Vector3(1, 0, 1));
        subjectLayer.showWindow();
    }
    void onPushError(RespondStatus status, string errmsg) {
        if (status == RespondStatus.PlayerExist)
            inputLayer.setNameExplainText(errmsg="该名称已存在");
        GameUtils.alert("创建玩家失败：" + errmsg,
            new string[] { null, "关闭" },
            new UnityAction[] { null, null});
    }

    void onSubjectLayerContinue() {
        int subjectSel = subjectLayer.getSubjectSelection();
        if (subjectSel < 0) GameUtils.alert("未选择分科！",
            new string[] { null, "关闭" }, new UnityAction[] { null, null });
        else {
            GameSystem.createPlayer(inputLayer.getName(), inputLayer.getSchool(), subjectSel);
            subjectLayer.hideWindow(new Vector3(1, 0, 1));
            startNewGame();
        }
    }

    void startNewGame() {
        GameSystem.startGame();
    }

}
