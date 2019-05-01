using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class show_details : MonoBehaviour
{
    public GameObject arts;
    // Start is called before the first frame update
    void Start()
    {
        arts = GameObject.Find("Arts_image");
    }

    // Update is called once per frame
    void Update()
    {
        //本来要做一个指针放到上面就显示详细介绍的功能
    }

    public void show_detail()
    {
        arts.SetActive(false);
    }

}
