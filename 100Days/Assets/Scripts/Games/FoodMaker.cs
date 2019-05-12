using UnityEngine;
using UnityEngine.UI;

/**
 * 单例类
 * 随机生成food
 * 挂在ScriptsHolder
 * */
public class FoodMaker : MonoBehaviour
{
    private static FoodMaker _instance;
    public static FoodMaker Instance
    {
        get
        {
            return _instance;
        }
    }

    public int xlimit = 21;
    public int ylimit = 11;
    public int xoffset = 7;
    public GameObject foodPrefab;
    public GameObject rewardPrefab;
    public Sprite[] foodSprites;
    private Transform foodHolder;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        foodHolder = GameObject.Find("FoodHolder").transform;
        MakeFood(false);
    }

    public void MakeFood(bool isReward)
    {
        int index = Random.Range(0, foodSprites.Length);			//生成的食物图片的index

        GameObject food = Instantiate(foodPrefab);
        food.GetComponent<Image>().sprite = foodSprites[index];		//修改食物的image
        food.transform.SetParent(foodHolder, false);				//添加到容器

        int x = Random.Range(-xlimit + xoffset, xlimit);			//根据范围计算生成坐标x,y
        int y = Random.Range(-ylimit, ylimit);
        food.transform.localPosition = new Vector3(x * 30, y * 30, 0);

        if (isReward)
		{	//生成奖励(奖励也是一种食物，只不过是另一个预制体)
            GameObject reward = Instantiate(rewardPrefab);
            reward.transform.SetParent(foodHolder, false);
            x = Random.Range(-xlimit + xoffset, xlimit);
            y = Random.Range(-ylimit, ylimit);
            reward.transform.localPosition = new Vector3(x * 30, y * 30, 0);
        }
    }
}
