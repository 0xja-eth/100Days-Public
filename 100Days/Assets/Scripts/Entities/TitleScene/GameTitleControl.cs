using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameTitleControl : MonoBehaviour {

    public ShowLogo logo;
    public AnimatableLayer mainLayer;

    const int WindowWidth = 1280;
    const int WindowHeight = 720;

    bool isStarted = false;

    // Use this for initialization
    void Awake() {
        Screen.SetResolution(WindowWidth, WindowHeight, false);
        GameUtils.initialize(null,
            "Canvas/PromptLayer/AlertWindow",
            "Canvas/PromptLayer/LoadingScene");
        GameSystem.initialize();
    }

    void testCodes() { 
        Debug.Log("TEST");
        QuestionJsonData d1 = new QuestionJsonData();
        d1.title = "测试";
        d1.id = 3;
        QuestionJsonData d2 = new QuestionJsonData();
        d2.title = "测试2";
        d2.id = 5;
        QuestionJsonData d3 = new QuestionJsonData();
        d3.title = "测试daf2";
        d3.id = 8;
        d3.choices = new QuestionChoiceJsonDataArray();//[2];
        d3.choices.Add(new Question.QuestionChoice("ss"));
        d3.choices.Add(new Question.QuestionChoice("sA", true));
        QuestionJsonDataArray ary1 = new QuestionJsonDataArray();
        QuestionJsonDataArray ary2 = new QuestionJsonDataArray();
        ary1.Add(d1);
        ary1.Add(d2);
        ary2.Add(d1);
        ary2.Add(d3);
        QuestionJsonDataArray2D ary2D = new QuestionJsonDataArray2D();
        ary2D.Add(ary1);
        ary2D.Add(ary2);
        string json = JsonUtility.ToJson(ary2D);
        Debug.Log(json);
        QuestionJsonDataArray2D ary2D2 = JsonUtility.FromJson<QuestionJsonDataArray2D>(json);
        Debug.Log(ary2D2.Count);
        Debug.Log(ary2D2[0][0]);
    }

    // Update is called once per frame
    void Update () {
        if (logo.getShown() && !isStarted) {
            isStarted = true;
            mainLayer.showWindow();
        }
    }

    public void enterGame() {
        mainLayer.hideWindow(()=> {
            SceneManager.LoadScene("StartAndSaveScene");
        }, new Vector3(1,0,1));
    }

    public void exitGame() {
        GameSystem.onGameEnd();
    }
}
