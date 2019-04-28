using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartAnimationControl : MonoBehaviour {
    float moveSpeed = 0.75f;
    float fadeSpeed = 0.015f;

    //public string startText = "你是一名高三学生";

    public RectTransform textRect;
    public GameObject buttonObj;

    Text text;
    Image buttonImg;

    bool pause = false, pausable = true;
    bool btnAniType = true;
	// Use this for initialization
	void Start () {
        text = GameUtils.text(textRect);
        buttonImg = GameUtils.get<Image>(buttonObj);
        text.text = generateStartText();
    }

	string generateStartText() {
        return String.Format(text.text, GameSystem.getDeltaDays());
    }
	
    // Update is called once per frame
	void Update () {
        updateTextAnimation();
        updateButtonAnimation();
    }

    void updateTextAnimation() {
        if (!pause) textRect.position = textRect.position + Vector3.up * moveSpeed;
    }
    void updateButtonAnimation() {
        if(textRect.position.y > textRect.rect.height+92) {
            pausable = pause = false;
            Color c = text.color;
            c.a -= fadeSpeed;
            text.color = c;
            c = buttonImg.color;
            buttonObj.SetActive(true);
            if (btnAniType) btnAniType = (c.a += fadeSpeed) < 0.95;
            else btnAniType = (c.a -= fadeSpeed) < 0.5;
            buttonImg.color = c;
        }
    }

    public void toggleAnimation() {
        if(pausable) pause = !pause;
    }

    public void startNewGame() {
        SceneManager.LoadScene("GameMainScene");
    }

}
