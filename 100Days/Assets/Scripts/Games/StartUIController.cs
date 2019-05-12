using UnityEngine;
using UnityEngine.UI;

/**
 * 开始界面ui控制类
 * */
public class StartUIController : MonoBehaviour
{
    public Text lastText;
    public Text bestText;
    public Toggle blue;
    public Toggle yellow;
    public Toggle border;
    public Toggle noBorder;

    public GameObject tips;
    public Button start;

    void Awake()
    {
        Player player = GameSystem.getPlayer();
		//显示上次的成绩和最好的成绩
        lastText.text = "上次：长度" + PlayerPrefs.GetInt("lastl", 0) + "，分数" + PlayerPrefs.GetInt("lasts", 0);
        bestText.text = "最好：长度" + PlayerPrefs.GetInt("bestl", 0) + "，分数" + PlayerPrefs.GetInt("bests", 0);
        if(player.getEnergy() < ScheduleLayer.MinGameEnergy) {
            tips.SetActive(true);
            start.interactable = false;
        }
    }

    void Start()
    {
		//获取之前保存的蛇的数据
        if (PlayerPrefs.GetString("sh", "Snake/sh01") == "Snake/sh01")
        {
            blue.isOn = true;
            PlayerPrefs.SetString("sh", "Snake/sh01");
            PlayerPrefs.SetString("sb01", "Snake/sb0101");
            PlayerPrefs.SetString("sb02", "Snake/sb0102");
        }
        else
        {
            yellow.isOn = true;
            PlayerPrefs.SetString("sh", "Snake/sh02");
            PlayerPrefs.SetString("sb01", "Snake/sb0201");
            PlayerPrefs.SetString("sb02", "Snake/sb0202");
        }
		//获取之前保存的boder数据
        if (PlayerPrefs.GetInt("border", 1) == 1)
        {
            border.isOn = true;
            PlayerPrefs.SetInt("border", 1);
        }
        else
        {
            noBorder.isOn = true;
            PlayerPrefs.SetInt("border", 0);
        }
    }

	/* 下面的boder模式，皮肤style的选择只是修改了数据，在Main界面初始化时会根据保存的数据显示 */
	//绑定在OnValueChanged事件上
    public void BlueSelected(bool isOn)
    {
        if (isOn)
        {
            PlayerPrefs.SetString("sh", "Snake/sh01");
            PlayerPrefs.SetString("sb01", "Snake/sb0101");
            PlayerPrefs.SetString("sb02", "Snake/sb0102");
        }
    }

    public void YellowSelected(bool isOn)
    {
        if (isOn)
        {
            PlayerPrefs.SetString("sh", "Snake/sh02");
            PlayerPrefs.SetString("sb01", "Snake/sb0201");
            PlayerPrefs.SetString("sb02", "Snake/sb0202");
        }
    }

    public void BorderSelected(bool isOn)
    {
        if (isOn)
        {
            PlayerPrefs.SetInt("border", 1);
        }
    }

    public void NoBorderSelected(bool isOn)
    {
        if (isOn)
        {
            PlayerPrefs.SetInt("border", 0);
        }
    }

    //游戏开始，跳到Main 界面
    public void StartGame() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SnakeMain");
    }
    public void BackGame() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameMainScene");
    }
}
