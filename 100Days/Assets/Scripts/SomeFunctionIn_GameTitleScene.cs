using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SomeFunctionIn_GameTitleScene : MonoBehaviour
{
    //切换到ChangeToStartAndSave
    public string sceneName_01 = null;
    public void ChangeToStartAndSave()
    {
        SceneManager.LoadScene(sceneName_01);
    }//end function ChangeToStartAndSave

    //退出游戏
    public void exitGame()
    {
        Application.Quit();
    }//end function exitGame
}//end class
