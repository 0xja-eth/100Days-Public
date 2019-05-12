using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuLayer : AnimatableLayer { 
    const int initialHeight = 88;
    const int extendHeight = 316;
    
    new const float resizeSpeed = 0.18f;
    new const float stopResizeDist = 1f;

    public GameObject arrow;
    public GameObject menuBtns;
    // Use this for initialization

    bool extended = false;

	void Start () {
        base.Start();
    }
	
	// Update is called once per frame
	void Update () {
        base.Update();
        //updatePointerEvents();
        //if (isMoving()) updateMove();
    }
    /*
    void updateMove() {
        float h = self.rect.height;
        float delta = (targetHeight - h) * moveSpeed;
        GameUtils.setRectHeight(self, h + delta);
        if (isStopping()) stopMove();
    }
    */
    void onResizeEnd() {
        //GameUtils.setRectHeight(self, targetHeight);
        extended = (targetSize.y == extendHeight);
        if (!extended) {
            arrow.SetActive(true);
            menuBtns.SetActive(false);
        }
    }
    /*
    public bool isMoving() {
        return self.rect.height != targetHeight;
    }
    public bool isStopping() {
        return Mathf.Abs(self.rect.height - targetHeight) < stopMoveDist;
    }*/
    public bool isExtended() {
        return extended;
    }

    public void toggleLayer() {
        if (extended) resetLayer();
        else extendLayer();
    }

    public void extendLayer() {
        Vector2 size = rectToSize();
        size.y = extendHeight;
        arrow.SetActive(false);
        menuBtns.SetActive(true);
        resizeTo(size, "extend");
    }
    public void resetLayer() {
        Vector2 size = rectToSize();
        size.y = initialHeight;
        resizeTo(size, "reset", onResizeEnd);
    }

    public void layerEnter() {
        showWindow();
        //rectTransform = (RectTransform)transform;
        float width = rectTransform.rect.width;
        moveDelta(new Vector3(-width, 0, 0), "enter");
    }
    public void layerOut() {
        float width = rectTransform.rect.width;
        moveDelta(new Vector3(width, 0, 0), "out");
        hideWindow(new Vector3(1, 1, 1));
    }
}
