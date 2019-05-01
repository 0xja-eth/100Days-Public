using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScheduleLayer : AnimatableLayer {
    const int MinExerciseEnergy = 10;
    const int MaxNextDayEnergy = 100;

    public Button exercise, game, friend, end;

    Player player;
    // Use this for initialization
    void Awake() {
        base.Awake();
    }

    // Update is called once per frame
    void Update () {
        base.Update();
    }

    public void refresh() {
        player = GameSystem.getPlayer();
        exercise.interactable = exerciseEnable();
        game.interactable = gameEnable();
        friend.interactable = friendEnable();
        end.interactable = endEnable();
    }

    bool exerciseEnable() {
        return player.getEnergy() >= MinExerciseEnergy;
    }
    bool gameEnable() {
        return false;
    }
    bool friendEnable() {
        return false;
    }
    bool endEnable() {
        return player.getEnergy() <= MaxNextDayEnergy;
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
