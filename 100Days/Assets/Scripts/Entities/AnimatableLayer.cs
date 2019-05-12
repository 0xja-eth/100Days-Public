using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class AnimatableLayer : MonoBehaviour {
    protected const float moveSpeed = 0.15f;
    protected const float rotateSpeed = 0.05f;
    protected const float scaleSpeed = 0.2f;
    protected const float resizeSpeed = 0.2f;
    protected const float colorSpeed = 0.075f;

    protected const float stopMoveDist = 0.1f;
    protected const float stopRotaDist = 1f;
    protected const float stopScaleDist = 0.01f;
    protected const float stopResizeDist = 0.02f;
    protected const float stopColorDist = 0.01f;

    const string TestObjectName = "DifficultyInfo";

    new protected string animation;

    public GameObject background;
    public Image image;

    public AnimatableLayer[] infos = new AnimatableLayer[0];

    UnityAction action;
    protected UIBaseLayer uiBaseLayer;
    protected RectTransform rectTransform;

    public bool posDisable, rotDisable, sclDisable, resDisable, colDisable;

    protected Vector2 targetSize;
    protected Vector3 targetPosition, targetRotation, targetScale;
    protected Vector4 targetColor;

    // Use this for initialization
    protected void Awake() {
        uiBaseLayer = GameUtils.getUIBaseLayer();
        rectTransform = (RectTransform)transform;
        initGeneralAnimationParams();
    }

    protected void Start () {}
	
	// Update is called once per frame
	protected virtual void Update () {
        updateBackground();
        updateGeneralAnimation();
        updateSpecificAnimation();
    }

    void OnDisable() {
        if (background) background.SetActive(false);
        foreach (AnimatableLayer obj in infos)
            obj.hideWindow();
    }

    void updateBackground() {
        if (background) background.SetActive(gameObject.activeSelf);
    }

    void initGeneralAnimationParams() {
        if (posAniEnable()) targetPosition = transform.position;
        if (rotAniEnable()) targetRotation = transform.eulerAngles;
        if (sclAniEnable()) targetScale = transform.localScale;
        if (resAniEnable()) targetSize = rectToSize();
        if (colAniEnable()) targetColor = image.color;
    }
    // General Animation
    void updateGeneralAnimation() {
        if (!isGeneralAniPlaying()) return;
        //if (gameObject.name == "NightMask")
        //    Debug.Log(targetColor+" ← "+(Vector4)image.color);
        if (posAniEnable()) updatePositionAnimation();
        if (rotAniEnable()) updateRotationAnimation();
        if (sclAniEnable()) updateScaleAnimation();
        if (resAniEnable()) updateSizeAnimation();
        if (colAniEnable()) updateColorAnimation();
        if (isGeneralAniStopping())
            stopGeneralAnimation();
    }
    void updatePositionAnimation() {
        if (gameObject.name == TestObjectName) 
            Debug.Log("Position: " + targetPosition + " ← " + transform.position + 
                " Distance = " + Vector3.Distance(transform.position, targetPosition) + 
                " StopMove = " + stopMoveDist);
        transform.position += (targetPosition - transform.position) * moveSpeed;
    }
    void updateRotationAnimation() {
        if (gameObject.name == TestObjectName)
            Debug.Log("Position: " + targetRotation + " ← " + transform.eulerAngles +
                " Distance = " + Vector3.Distance(transform.eulerAngles, targetRotation) +
                " StopMove = " + stopRotaDist);
        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.Euler(targetRotation), rotateSpeed);
    }
    void updateScaleAnimation() {
        if (gameObject.name == TestObjectName)
            Debug.Log("Position: " + targetScale + " ← " + transform.localScale +
                " Distance = " + Vector3.Distance(transform.localScale, targetScale) +
                " StopMove = " + stopScaleDist);
        transform.localScale += (targetScale - transform.localScale) * scaleSpeed;
    }
    void updateSizeAnimation() {
        Vector2 size = rectToSize();
        if (gameObject.name == TestObjectName)
            Debug.Log("Position: " + targetSize + " ← " + size +
                " Distance = " + Vector3.Distance(size, targetSize) +
                " StopMove = " + stopResizeDist);
        size += (targetSize - size) * resizeSpeed;
        GameUtils.setRectWidth(rectTransform, size.x);
        GameUtils.setRectHeight(rectTransform, size.y);
    }
    void updateColorAnimation() {
        if (gameObject.name == TestObjectName)
            Debug.Log("Color: " + targetColor + " ← " + (Vector4)image.color);
        image.color = (Vector4)image.color + (targetColor - (Vector4)image.color) * colorSpeed;
    }

    bool isGeneralAniPlaying() {
        Vector2 size = rectToSize();
        return !isGeneralAniStopping();/* (posAniEnable() && transform.position != targetPosition)
            || (rotAniEnable() && transform.eulerAngles != targetRotation)
            || (sclAniEnable() && transform.localScale != targetScale)
            || (resAniEnable() && size != targetSize)
            || (colAniEnable() && (Vector4)image.color != targetColor);*/
    }
    bool isGeneralAniStopping() {
        Vector2 size = rectToSize();
        return (!posAniEnable() || Vector3.Distance(transform.position, targetPosition) < stopMoveDist)
            && (!rotAniEnable() || Vector3.Distance(transform.eulerAngles, targetRotation) < stopRotaDist)
            && (!sclAniEnable() || Vector3.Distance(transform.localScale, targetScale) < stopScaleDist)
            && (!resAniEnable() || Vector2.Distance(size, targetSize) < stopResizeDist)
            && (!colAniEnable() || Vector4.Distance(image.color, targetColor) < stopColorDist);
    }

    protected bool posAniEnable() { return !posDisable; }
    protected bool rotAniEnable() { return !rotDisable; }
    protected bool sclAniEnable() { return !sclDisable; }
    protected bool resAniEnable() { return !resDisable; }
    protected bool colAniEnable() { return !colDisable && image; }

    public void stopGeneralAnimation(bool force = false) {
        if (gameObject.name == TestObjectName)
            Debug.Log("stopGeneralAnimation");
        resetGeneralAnimation();
        stopAnimation(force);
    }
    void resetGeneralAnimation() {
        if (posAniEnable()) transform.position = targetPosition;
        if (rotAniEnable()) transform.eulerAngles = targetRotation;
        if (sclAniEnable()) transform.localScale = targetScale; 
        if (resAniEnable()) {
            GameUtils.setRectWidth(rectTransform, targetSize.x);
            GameUtils.setRectHeight(rectTransform, targetSize.y);
        }
        if (colAniEnable()) image.color = targetColor;
    }

    public Vector2 rectToSize() {
        return GameUtils.rectToSize(rectTransform.rect);
    }
    // Specific Animation
    protected virtual void updateSpecificAnimation() {
        /*
        Vector3 ts;
        switch (animation) {
            case "show":
                ts = new Vector3(1, 1, 1);
                transform.localScale += (ts -
                    transform.localScale) * scaleSpeed;
                if (isAniStopping(ts)) {
                    transform.localScale = ts;
                    stopAnimation();
                }
                break;
            case "hide":
                ts = new Vector3(0, 0, 0);
                transform.localScale += (ts -
                    transform.localScale) * scaleSpeed;
                if (isAniStopping(ts)) {
                    transform.localScale = ts;
                    stopAnimation();
                }
                break;
        }*/
    }
    protected virtual bool isAniStopping(Vector3 ts) {
        return true;// Vector3.Distance(ts, transform.localScale) < stopScaleDist;
    }

    // Animation Operation
    void stopAnimation(bool force = false) {
        if (gameObject.name == TestObjectName)
            Debug.Log("Action: " + action);
        UnityAction lastAct = action;
        clearAnimation(); clearAction();
        if (lastAct != null && !force) lastAct.Invoke();
    }
    public string getAnimation() {
        return animation;  
    }
    public void setAnimation(string ani) {
        animation = ani;
    }
    public void clearAnimation() {
        animation = null;
    }
    public void setAction(UnityAction act) {
        action = act;
    }
    public void clearAction() {
        action = null;
    }

    // Base Animation
    public void moveTo(Vector3 pos, string ani = null, UnityAction act = null) {
        if (!posAniEnable()) return;
        if (gameObject.name == TestObjectName)
            Debug.Log("moveTo(" + pos + ")");
        targetPosition = pos;
        setAnimation(ani);
        setAction(act);
    }
    public void moveDelta(Vector3 pos, string ani = null, UnityAction act = null) {
        if (!posAniEnable()) return;
        if (gameObject.name == TestObjectName)
            Debug.Log("moveDelta(" + pos + ")");
        moveTo(transform.position + pos, ani, act);
    }
    public void rotateTo(Vector3 rot, string ani = null, UnityAction act = null) {
        if (!rotAniEnable()) return;
        if (gameObject.name == TestObjectName)
            Debug.Log("rotateTo(" + rot + ")");
        targetRotation = rot;
        setAnimation(ani);
        setAction(act);
    }
    public void rotateDelta(Vector3 rot, string ani = null, UnityAction act = null) {
        if (!rotAniEnable()) return;
        if (gameObject.name == TestObjectName)
            Debug.Log("rotateDelta(" + rot + ")");
        moveTo(transform.eulerAngles + rot, ani, act);
    }
    public void scaleTo(Vector3 scale, string ani = null, UnityAction act = null) {
        if (!sclAniEnable()) return;
        if (gameObject.name == TestObjectName)
            Debug.Log("scaleTo(" + scale + ")");
        targetScale = scale;
        setAnimation(ani);
        setAction(act);
    }
    public void scaleDelta(Vector3 scale, string ani = null, UnityAction act = null) {
        if (!sclAniEnable()) return;
        if (gameObject.name == TestObjectName)
            Debug.Log("scaleDelta(" + scale + ")");
        moveTo(transform.localScale + scale, ani, act);
    }
    public void resizeTo(Vector2 size, string ani = null, UnityAction act = null) {
        if (!resAniEnable()) return;
        if (gameObject.name == TestObjectName)
            Debug.Log("resizeTo(" + size + ")");
        targetSize = size;
        setAnimation(ani);
        setAction(act);
    }
    public void resizeDelta(Vector2 size, string ani = null, UnityAction act = null) {
        if (!resAniEnable()) return;
        if (gameObject.name == TestObjectName)
            Debug.Log("resizeDelta(" + size + ")");
        resizeTo(rectToSize() + size, ani, act);
    }
    public void colorTo(Color color, string ani = null, UnityAction act = null) {
        if (!colAniEnable()) return;
        if (gameObject.name == TestObjectName)
            Debug.Log("colorTo(" + color + ")");
        targetColor = color;
        setAnimation(ani);
        setAction(act);
    }
    public void colorDelta(Color color, string ani = null, UnityAction act = null) {
        if (!colAniEnable()) return;
        if (gameObject.name == TestObjectName)
            Debug.Log("colorDelta(" + color + ")");
        resizeTo((Vector4)image.color + (Vector4)color, ani, act);
    }

    // Default Animation
    protected virtual void onWindowShown() {
    }
    protected virtual void onWindowHidden() {
        gameObject.SetActive(false);
        //if (background) background.SetActive(false);
    }
    public void showWindow(UnityAction act, Vector3? scale) {
        scale = scale ?? new Vector3(1, 1, 1);
        //if (background) background.SetActive(true);
        gameObject.SetActive(true);
        scaleTo((Vector3)scale, "show", () => {
            onWindowShown(); if (act != null) act.Invoke();
        });
    }
    public void showWindow(Vector3? scale) {
        showWindow(null, scale);
    }
    public void showWindow() {
        showWindow(null);
    }

    public void hideWindow(UnityAction act, Vector3? scale){// = default(Vector3)) {
        scale = scale ?? new Vector3(0, 0, 0);
        //if (scale == default(Vector3)) scale = new Vector3(0, 0, 0);
        scaleTo((Vector3)scale, "hide", () => {
            onWindowHidden(); if (act != null) act.Invoke();
        });
        foreach (AnimatableLayer obj in infos)
            obj.hideWindow();
    }
    public void hideWindow(Vector3? scale) {
        hideWindow(null, scale);
    }
    public void hideWindow() {
        hideWindow(null);
    }
}
