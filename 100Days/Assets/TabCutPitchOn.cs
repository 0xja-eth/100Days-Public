using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class TabCutPitchOn : MonoBehaviour
{
    //得到EventSystem组件
    private EventSystem system;
    //字典：key是游戏的物体编号，GameObject是游戏物体
    private Dictionary<int, GameObject> dicObj;
    //index用于存储得到的字典的索引
    int index;
    // Start is called before the first frame update
    void Start()
    {
        system = EventSystem.current;
        dicObj = new Dictionary<int, GameObject>();
        index = 0;

        for (int i = 0; i < transform.childCount; i++)
        {
            dicObj.Add(i, transform.GetChild(i).gameObject);
        }
        GameObject obj;
        dicObj.TryGetValue(index, out obj);
        // 设置第一个可交互的UI为高亮状态
         system.SetSelectedGameObject(obj, new BaseEventData(system));
    }

    // Update is called once per frame
    void Update()
    {
        if (system.currentSelectedGameObject != null && Input.GetKeyDown(KeyCode.Tab))
        {
            GameObject hightedObj = system.currentSelectedGameObject;
            foreach (KeyValuePair<int, GameObject> item in dicObj)
            {
                if (item.Value == hightedObj)
                {
                    index = item.Key + 1;
                    // 超出索引 将Index归零
                    if (index == dicObj.Count)
                    {
                        index = 0;
                    }
                    break;
                }
                
            }
            GameObject obj;
            dicObj.TryGetValue(index, out obj);
            system.SetSelectedGameObject(obj, new BaseEventData(system));
        }
    }
}
