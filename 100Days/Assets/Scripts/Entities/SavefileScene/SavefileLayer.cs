using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavefileLayer : AnimatableLayer{
    float targetY = 360;

    public Color normalSaveColor = new Color(1, 1, 1, 0.9f);
    public Color emptySaveColor = new Color(1, 1, 1, 0.5f);
    public Color errorSaveColor = new Color(1, 0.5f, 0.5f, 0.5f);
    public Color emptySaveTextColor = new Color(0, 0, 0, 1);
    public Color errorSaveTextColor = new Color(0.5f, 0, 0, 1);

    public Image selfBackground;
    public GameObject infoLayer, emptyLayer;
    public Text name, school, saveTime, days, lastExam, empty;

    public int index = -1;

    bool newEnable, continueEable, deleteEnable;

    void Awake() {
        base.Awake();
        refresh();
        Debug.Log(index + ":" + transform.position);
    }
    void Start() {
        base.Start();
        savefileEnter();
    }
    public bool isNewEnable() {
        return newEnable;
    }
    public bool isContinueEnable() {
        return continueEable;
    }
    public bool isDeleteEnable() {
        return deleteEnable;
    }

    public void refresh() {
        if (index == -1) return;
        SavefileHeaderJsonData data = StorageSystem.getSaveHeaderData(index);
        Debug.Log(index + ": " + data.empty + " , "+ StorageSystem.hasSaveFile(index));
        if (!data.empty)
            if (StorageSystem.hasSaveFile(index))
                drawSavefileInfo(data);
            else drawErrorSavefile();
        else if (StorageSystem.hasSaveFile(index))
            drawErrorSavefile();
        else drawEmptySavefile();
    }

    void drawSavefileInfo(SavefileHeaderJsonData data) {
        infoLayer.SetActive(true);
        emptyLayer.SetActive(false);
        name.text = data.name;
        school.text = data.school;
        saveTime.text = data.saveTime;
        days.text = "距离高考 " + data.restDays + " 天";
        lastExam.text = gengerateLastExamInfo(data);
        selfBackground.color = normalSaveColor;
        newEnable = continueEable = deleteEnable = true;
    }
    string gengerateLastExamInfo(SavefileHeaderJsonData data) {
        string text = ""; int sumScore = 0, sumMax = 0;
        for(int i = 0; i < data.subjectIds.Count; i++) {
            string subject = Subject.SubjectName[data.subjectIds[i]];
            int score = data.lastScore[i], max = data.lastMaxScore[i];
            text += subject + ": " + score + "/" + max + "\n";
            sumScore += score; sumMax += max;
        }
        text += "总分: " + sumScore + "/" + sumMax;
        return text;
    }
    void drawEmptySavefile() {
        infoLayer.SetActive(false);
        emptyLayer.SetActive(true);
        empty.text = "空存档";
        empty.color = emptySaveTextColor;
        selfBackground.color = emptySaveColor;
        continueEable = deleteEnable = false;
        newEnable = true;
    }
    void drawErrorSavefile() {
        infoLayer.SetActive(false);
        emptyLayer.SetActive(true);
        empty.text = "错误存档";
        empty.color = errorSaveTextColor;
        selfBackground.color = errorSaveColor;
        continueEable = newEnable = false;
        deleteEnable = true;
    }

    void savefileEnter() {
        Vector3 pos = transform.position;
        pos.y = targetY; moveTo(pos);
    }
}
