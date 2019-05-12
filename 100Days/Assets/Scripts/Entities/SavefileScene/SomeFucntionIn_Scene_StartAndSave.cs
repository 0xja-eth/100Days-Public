using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

public class SomeFucntionIn_Scene_StartAndSave : MonoBehaviour
{
    public string sceneName_01;

    public Button newGame, continueGame, deleteGame;

    public SavefileLayer[] savefileLayers;

    int index = 0;
    int[] SaveName = new int[] { 0, 1, 2 };

    public void ChangeToGameTitleScene(){
        SceneManager.LoadScene(sceneName_01);
    }//end function ChangeToGameTitleScene

    public void onSavefileClick(int index) {
        this.index = SaveName[index];
    }

    //新游戏
    public void NewGame()
    {
        HideUI();
        if (StorageSystem.hasSaveFile(index)) 
            //弹出提示框：已有存档，请读取存档
            GameUtils.alert("该位置已有存档，是否覆盖？",
                new string[] { null, "是", "否" },
                new UnityAction[] { null, onNewGame, null });
        else onNewGame();
    }//end function NewGame

    void onNewGame() {
        GameSystem.newGame(index);
    }

    //继续游戏
    public void ContinueGame() {
        HideUI();
        if (StorageSystem.hasSaveFile(index))
            GameSystem.continueGame(index);
        else GameUtils.alert("该存档不存在！");
    }//end function ContinueGame
    //删除存档
    public void DeleteSave(){
        HideUI();
        if (StorageSystem.hasSaveFile(index)) 
            GameUtils.alert("删除后存档将不可恢复，是否删除？",
                new string[] { null, "是", "否" },
                new UnityAction[] { null, onDelete, null });
        else GameUtils.alert("该存档不存在！");
    }//end function DeleteGame

    void onDelete() {
        StorageSystem.deleteSave(index);
        refresh();
    }
    
    public void refresh() {
        foreach (SavefileLayer sl in savefileLayers)
            sl.refresh();
    }

    //需要显示的对象
    public AnimatableLayer clickMenu;

    //隐藏控件
    public void HideUI() {
        clickMenu.hideWindow(new Vector3(1, 0, 1));
        clickMenu.stopGeneralAnimation();
    }
    public void SlideHideUI() {
        clickMenu.hideWindow(new Vector3(1, 0, 1));
    }//end function HideUI
    //显示控件
    public void DisplayUI()
    {
        clickMenu.showWindow();
        SavefileLayer sl = savefileLayers[index];
        newGame.interactable = sl.isNewEnable();
        continueGame.interactable = sl.isContinueEnable();
        deleteGame.interactable = sl.isDeleteEnable();
    }//end function DisplayUI

    void Awake() {
        GameUtils.initialize(null,
            "Canvas/PromptLayer/AlertWindow",
            "Canvas/PromptLayer/LoadingScene");
    }
    void Start() {
        HideUI(); refresh();
    }//end function Start

    public void Test()
    {
        HideUI();
        /*
        if (Input.GetMouseButtonUp(0))
        {
        */
        float x = 0, y = 0;
        bool posInfo = false;
        if (SystemInfo.deviceType == DeviceType.Desktop) {
            x = Input.mousePosition.x;
            y = Input.mousePosition.y;
            posInfo = true;
        } else if (Input.touches.Length > 0) {
            x = Input.touches[0].position.x;
            y = Input.touches[0].position.y;
            posInfo = true;
        }         //float gx = 200;
        //float gy = 300;
        if (posInfo) {
            clickMenu.transform.position = new Vector3(x, y, 0);
            DisplayUI();
        }
        // }//end if
    }//
    void Update()
    {
        
    }//end function Update
}//end class
