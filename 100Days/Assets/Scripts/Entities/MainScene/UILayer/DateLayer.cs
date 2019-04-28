using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DateLayer : AnimatableLayer {
    Player player;

    public Text date, day;
    public AnimatableLayer bar;
    // Use this for initialization
    void Awake() {
        base.Awake();
        bar.stopGeneralAnimation(true);
    }
    void Start () {
        base.Start();
    }

    // Update is called once per frame
    void Update() {
        base.Update();
    }

    public void refresh() {
        player = GameSystem.getPlayer();
        refreshDate();
        refreshEnergy();
    }
    void refreshDate() {
        date.text = GameSystem.getCurDate().ToString("yyyy 年 MM 月 dd 日");
        day.text = GameSystem.getDays() + " 天";
    }
    void refreshEnergy() {
        int max = player.getMaxEnergy();
        int cur = player.getEnergy();
        float rate = cur*1.0f / max;
        Debug.Log(rate);
        bar.scaleTo(new Vector3(rate, 1, 1));
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
