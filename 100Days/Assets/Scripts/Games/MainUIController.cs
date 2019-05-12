using UnityEngine;
using UnityEngine.UI;

/**
 * 单例模式
 * 游戏main场景UI控制类，游戏逻辑主要在蛇头SnakeHead上
 * */
public class MainUIController : MonoBehaviour
{
    private static MainUIController _instance;
    public static MainUIController Instance
    {
        get
        {
            return _instance;
        }
    }

    public bool hasBorder = true;	//是否有边界
    public bool isPause = false;	//是否暂停
    public int score = 0;			//得分
    public int length = 0;			//长度

    public Text msgText;		//阶段多少text
    public Text scoreText;		//得分text
    public Text lengthText;		//length text

    public Image pauseImage;
    public Sprite[] pauseSprites;

    public Image bgImage;
    private Color tempColor;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
		//获取border数据，根据数据来设置border
        if (PlayerPrefs.GetInt("border", 1) == 0)
        {
			//隐藏bg里的border image
            hasBorder = false;
            foreach (Transform t in bgImage.gameObject.transform)
            {
                t.gameObject.GetComponent<Image>().enabled = false;
            }
        }
    }

    void Update()
    {
		//根据得分获取阶段，换背景color,
        switch (score / 100)
        {
            case 0:
            case 1:
            case 2:
                break;
            case 3:
            case 4:
                ColorUtility.TryParseHtmlString("#CCEEFFFF", out tempColor);
                bgImage.color = tempColor;
                msgText.text = "阶段" + 2;
                break;
            case 5:
            case 6:
                ColorUtility.TryParseHtmlString("#CCFFDBFF", out tempColor);
                bgImage.color = tempColor;
                msgText.text = "阶段" + 3;
                break;
            case 7:
            case 8:
                ColorUtility.TryParseHtmlString("#EBFFCCFF", out tempColor);
                bgImage.color = tempColor;
                msgText.text = "阶段" + 4;
                break;
            case 9:
            case 10:
                ColorUtility.TryParseHtmlString("#FFF3CCFF", out tempColor);
                bgImage.color = tempColor;
                msgText.text = "阶段" + 5;
                break;
            default:
                ColorUtility.TryParseHtmlString("#FFDACCFF", out tempColor);
                bgImage.color = tempColor;
                msgText.text = "无尽阶段";
                break;
        }
    }

    public void UpdateUI(int s = 5, int l = 1)
    {
        score += s;
        length += l;
        scoreText.text = "得分:\n" + score;
        lengthText.text = "长度:\n" + length;
    }

	/* 通过拖拽的方式添加button事件 */
	//暂停开始按钮
    public void Pause()
    {
        isPause = !isPause;
        if (isPause)
        {
            Time.timeScale = 0;
            pauseImage.sprite = pauseSprites[1];
        }
        else
        {
            Time.timeScale = 1;
            pauseImage.sprite = pauseSprites[0];
        }
    }

	//home按钮
    public void Home()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SnakeMain");
    }
}
