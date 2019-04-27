using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScheduleLayer : AnimatableLayer {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        base.Update();
    }

    public void layerEnter() {
        showWindow();
        //rectTransform = (RectTransform)transform;
        float width = rectTransform.rect.width;
        moveDelta(new Vector3(width, 0, 0), "enter");
    }
    public void layerOut() {
        float width = rectTransform.rect.width;
        moveDelta(new Vector3(-width, 0, 0), "out");
        hideWindow(new Vector3(1, 1, 1));
    }

}
