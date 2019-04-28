using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class AlertLayer : AnimatableLayer {

    const int AlertTextPaddingTop = 36;
    const int TextButtonSpacing = 18;
    const int OkButtonPaddingButtom = 36;

	public RectTransform alertText, buttons;
	public RectTransform alertWindow;

    public GameObject okButton, leftButton, rightButton;

    GameObject[] buttonObjs;

	// Use this for initialization
	void Awake () {
        base.Awake();
    }
	
	void Start () {
        base.Start();
    }

	// Update is called once per frame
	void Update () {
        base.Update();
        updateWindowSize();
	}

	void updateWindowSize(){
        float height = AlertTextPaddingTop + alertText.rect.height +
            TextButtonSpacing + buttons.rect.height +
            OkButtonPaddingButtom;

        GameUtils.setRectHeight(alertWindow, height);
		//alertWindow.SetSizeWithCurrentAnchors(
		//	RectTransform.Axis.Vertical, height);
	}

    void resetButtons() {
        foreach (GameObject obj in buttonObjs)
            obj.SetActive(false);
    }

    public void setup(string text, string[] btns=null, UnityAction[] actions=null) {
        buttonObjs = buttonObjs ?? new GameObject[3] { leftButton, okButton, rightButton };

        btns = btns ?? new string[2] { null, "确认" };
        actions = actions ?? new UnityAction[2] { null, hideWindow };
        setText(text);
        resetButtons();
        for (int i = 0; i < btns.Length; i++) {
            string txt = btns[i];
            if (txt == null) continue;
            GameObject btn = buttonObjs[i];
            Button button = GameUtils.button(btn);
            Text label = GameUtils.find<Text>(btn, "Text");
            UnityAction act = actions[i] ?? hideWindow;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(()=> {
                button.interactable = false;
                act.Invoke(); hideWindow();
            });
            button.interactable = true;
            label.text = txt;
            btn.SetActive(true);
        }
        showWindow();
    }

	public void setText(string text) {
		Debug.Log(alertWindow);
		Text alert = GameUtils.text(alertText);
		alert.text = text;
	}
    
}
