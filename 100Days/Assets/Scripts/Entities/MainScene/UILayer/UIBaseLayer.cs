using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBaseLayer : MonoBehaviour {
    public DateLayer dateLayer;
    public ScheduleLayer scheduleLayer;
    public MenuLayer menuLayer;
    public AnimatableLayer night;
	// Use this for initialization
	void Start () {
        GameSystem.initialize();
        dateLayer.refresh();
        showUILayer();
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
        dateLayer.refresh();
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

    public void showUILayer() {
        dateLayer.layerEnter();
        scheduleLayer.layerEnter();
        menuLayer.layerEnter();
    }
    public void hideUILayer() {
        dateLayer.layerOut();
        scheduleLayer.layerOut();
        menuLayer.layerOut();
    }
}
