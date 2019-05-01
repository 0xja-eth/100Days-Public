using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class SomeFucntionIn_Scene_StartAndSave : MonoBehaviour
{
    //初始化
    const int WindowWidth = 1248;
    const int WindowHeight = 702;

    int index=0;
    private int[] SaveName = new int[] { 0, 1, 2 };

    public void ClickA()
    {
        index = SaveName[0];
       
    }//end function ClickA
    public void ClickB()
    {
        index = SaveName[1];
       
    }//end function ClickB
    public void ClickC()
    {
        index = SaveName[2];
        
    }//end function ClickC

    const string FilePath = "/SaveData/";
    const string FileNameRoot = "savefile";
    const string FileExtendName = ".sav";
    
 
    //切换到GameTitleScene
    public string sceneName_01;
    public void ChangeToGameTitleScene()
    {
        SceneManager.LoadScene(sceneName_01);
    }//end function ChangeToGameTitleScene

   
    
    //存档文件是否存在
    bool FileExists(int index)
    {
        string path = Application.dataPath + FilePath;
        string name = FileNameRoot + index + FileExtendName;
        return File.Exists(path + "\\" + name);
    }//end function  FileExists

    //新游戏
    public void NewGame()
    {
        if (FileExists(index))
        {
            //弹出提示框：已有存档，请读取存档
            
        }//end if
        else
            GameSystem.newGame(index);
    }//end function NewGame
    //继续游戏
    public void ContinueGame()
    {
        GameSystem.continueGame(index);
    }//end function ContinueGame
    //删除存档
    public void DeleteSave()
    {

        string path = Application.dataPath + FilePath;
        string name = FileNameRoot + index + FileExtendName;
        if (FileExists(index))
        {
            //弹出提示框是否删除存档
            File.Delete(path + "\\" + name);
            if (!FileExists(index))
            {
                HideUI();
            }
        }//end if
        else
        {
            //empty
        }//end else
    }//end function DeleteGame

    //如果有存档，则显示信息
    public void DisplayMessageFromSave(int _index,GameObject gameO)
    {
        //如果存在存档
        if (FileExists(_index))
        {
            //打开存档文件
            gameO.GetComponent<Text>().text ="有存档";
        }//end if
        else
        {
            gameO.GetComponent<Text>().text = "无存档";
        }//end else
    }//end function DisplayMessageFromSave


    public GameObject Text_A;
    public GameObject Text_B;
    public GameObject Text_C;

    public void DisplayMessageFromSave_A()
    {
        DisplayMessageFromSave(SaveName[0],Text_A);
    }//end function DisplayMessageFromSave_A
    public void DisplayMessageFromSave_B()
    {
        DisplayMessageFromSave(SaveName[1],Text_B);
    }//end function DisplayMessageFromSave_B
    public void DisplayMessageFromSave_C()
    {
        DisplayMessageFromSave(SaveName[2],Text_C);
    }//end function DisplayMessageFromSave_C

    //需要显示的对象
    public GameObject gameObject;
    

    //隐藏控件
    public void HideUI()
    {
        gameObject.SetActive(false);
        
    }//end function HideUI
    //显示控件
    public void DisplayUI()
    {
        gameObject.SetActive(true);
    }//end function DisplayUI
   
    public void Awake()
    {
        Screen.SetResolution(WindowWidth, WindowHeight, false);
        GameSystem.initialize();
    }//end function Awake

    void Start()
    {
        HideUI();
        DisplayMessageFromSave_B();
        DisplayMessageFromSave_A();
        DisplayMessageFromSave_C();
    }//end function Start

    public void Test()
    {
        HideUI();
        if (Input.GetMouseButtonUp(0))
        {

            float x = Input.mousePosition.x;
            float y = Input.mousePosition.y;
            float z = Input.mousePosition.z;
            //float gx = 200;
            //float gy = 300;

            gameObject.transform.position = new Vector3(x, y, z);
            DisplayUI();
        }//end if
    }//
    void Update()
    {
        DisplayMessageFromSave_A();
        DisplayMessageFromSave_B();
        DisplayMessageFromSave_C();

        
    }//end function Update
}//end class
