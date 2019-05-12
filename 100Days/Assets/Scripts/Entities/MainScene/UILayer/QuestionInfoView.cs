using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class QuestionInfoView : QuestionDisplayer {

    const float deleteWidth = 256;
    // Start is called before the first frame update
    public RectTransform quesText;
    public RectTransform content;

    public Toggle collect;

    public RecordLayer recordLayer;

    public RectTransform delete, remove, back;

    public QuestionInfoView fullScreenView;

    Question question;
    int index, mode = -1;

    // Use this for initialization
    void Awake() {
        base.Awake();
    }

    void Start() {
        base.Start();
    }
    // Update is called once per frame
    void Update() {
        base.Update();
        updateContentHeight();
    }

    void updateContentHeight() {
        setRectHeight(content, -quesText.anchoredPosition.y * 2 + quesText.rect.height);
    }
    
    public void setQuestion(int index, Question q, int mode) {
        question = q; 
        drawQuestionMain(index);
        drawQuestionDescription();
        content.anchoredPosition = new Vector2(0, 0);
        collect.isOn = RecordSystem.isInCollection(q);
        delete.gameObject.SetActive(mode >= 0);
        back.gameObject.SetActive(mode < 0);
        if (mode == 2) showRemoveButton();
        else hideRemoveButton();
        this.index = index;
        this.mode = mode;
    }

    public void clear() {
        TextExtend queText = GameUtils.get<TextExtend>(quesText);
        back.gameObject.SetActive(false);
        hideRemoveButton();
        collect.isOn = false;
        queText.text = "";
    }

    void showRemoveButton() {
        GameUtils.setRectWidth(delete, 120);
        remove.gameObject.SetActive(true);
    }
    void hideRemoveButton() {
        GameUtils.setRectWidth(delete, deleteWidth);
        remove.gameObject.SetActive(false);
    }

    void drawQuestionMain(int index) {
        TextExtend queText = GameUtils.get<TextExtend>(quesText);
        GameUtils.setTexturePool(question.getPictures());
        queText.text = getQuestionTextInAnswer(index, question, queText.fontSize);
    }
    void drawQuestionDescription() {
        TextExtend queText = GameUtils.get<TextExtend>(quesText);
        queText.text += getDescriptionText(question);
    }

    public void toggleCollect() {
        if (RecordSystem.isInCollection(question)) delCollect();
        else addCollect();
    }
    void delCollect() {
        RecordSystem.deleteQuestionCollect(question);
        collect.isOn = false;
    }
    void addCollect() {
        RecordSystem.addQuestionCollect(question);
        collect.isOn = true;
    }
    public void deleteRecord() {
        GameUtils.alert("确定删除该题目吗？\n删除后该题目将不会在你的做题记录中出现，直到下一次做到。",
            new string[] { null, "确定", "返回" }, new UnityAction[] { null, onDeleteRecordConfirm, null });
    }
    void onDeleteRecordConfirm() {
        delCollect(); delWrong();
        question.delete();
        recordLayer.refresh();
    }
    void delWrong() {
        RecordSystem.deleteQuestionWrong(question);
        Debug.Log("delWrong: " + RecordSystem.getWrongs().Count);
        Debug.Log("delWrong.include: " + RecordSystem.getWrongs(true).Count);
    }
    public void removeRecord() {
        delWrong(); recordLayer.refresh();
    }
    public new void hideWindow() {
        hideWindow(new Vector3(1, 0, 0));
    }
    public void fullScreen() {
        fullScreenView.showWindow(()=> {
            fullScreenView.setQuestion(index, question, mode);
        }, new Vector3(1, 1, 1));
        Debug.Log("fullScreenView");
    }
}
