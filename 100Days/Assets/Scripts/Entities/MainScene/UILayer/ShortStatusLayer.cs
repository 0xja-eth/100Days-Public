using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortStatusLayer : AnimatableLayer {
    public AnimatableLayer energy, pressure;
    Player player;
    // Start is called before the first frame update
    void Awake() {
        base.Awake();
        energy.stopGeneralAnimation(true);
        pressure.stopGeneralAnimation(true);
    }
    void Start() {
        base.Start();
    }

    // Update is called once per frame
    void Update() {
        base.Update();
    }

    public void refresh() {
        player = GameSystem.getPlayer();
        refreshEnergy();
        refreshPressure();
    }
    void refreshEnergy() {
        int max = player.getMaxEnergy();
        int cur = player.getEnergy();
        float rate = cur * 1.0f / max;
        energy.scaleTo(new Vector3(rate, 1, 1));
    }
    void refreshPressure() {
        int max = player.getMaxPressure();
        int cur = player.getPressure();
        float rate = cur * 1.0f / max;
        pressure.scaleTo(new Vector3(rate, 1, 1));
        pressure.colorTo(new Color(rate, 1 - rate, 1 - rate));
    }

    public void layerEnter() {
        showWindow();
        //rectTransform = (RectTransform)transform;
        float height = rectTransform.rect.height;
        moveDelta(new Vector3(0, -height, 0), "enter");
    }
    public void layerOut() {
        float height = rectTransform.rect.height;
        moveDelta(new Vector3(0, height, 0), "out");
        hideWindow(new Vector3(1, 1, 1));
    }
}
