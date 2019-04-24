using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class AlertLayer : MonoBehaviour {

	int alertTextPaddingButtom = 24;
	int okButtonPaddingButtom = 24;

	float scaleSpeed = 0.2f;
	float stopScaleDist = 0.02f;

	public GameObject background;

	public RectTransform alertText, buttons;
	public RectTransform alertWindow;

    public GameObject okButton, leftButton, rightButton;

    GameObject[] buttonObjs;
    new string animation;

	// Use this for initialization
	void Awake () {
    }
	
	void Start () {
    }

	// Update is called once per frame
	void Update () {
		updateAnimation();
		updateWindowSize();
	}
    

	void updateAnimation(){
		Vector3 ts;
		switch(animation){
			case "show":
				ts = new Vector3(1,1,1);
				transform.localScale += (ts-
					transform.localScale)*scaleSpeed;
				if(isAniStopping(ts)){
					transform.localScale = ts;
					stopAnimation();
				}
				break;
			case "hide":
				ts = new Vector3(0,0,0);
				transform.localScale += (ts-
					transform.localScale)*scaleSpeed;
				if(isAniStopping(ts)){
					transform.localScale = ts;
					gameObject.SetActive(false);
					background.SetActive(false);
					stopAnimation();
				}
				break;
		}
	}

	bool isAniStopping(Vector3 ts){
		return Vector3.Distance(ts,transform.localScale)<stopScaleDist;
	}

	void stopAnimation(){
		animation = null;
	}

	void updateWindowSize(){
		float height = -alertText.rect.y+alertText.rect.height+
			alertTextPaddingButtom+buttons.rect.height+
			okButtonPaddingButtom;

		alertWindow.SetSizeWithCurrentAnchors(
			RectTransform.Axis.Vertical, height);
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

	public void showWindow(){
		background.SetActive(true);
		gameObject.SetActive(true);
		animation = "show";
	}
	public void hideWindow(){
		animation = "hide";
	}
}
