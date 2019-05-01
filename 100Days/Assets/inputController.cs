using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class inputController : MonoBehaviour
{
    public GameObject username;
    public GameObject school;
    string name_input;
    string school_input;

    public GameObject explainer1;
    public GameObject explainer2;

    public GameObject[] images = new GameObject[4];
    // Start is called before the first frame update
    void Start()
    {
        //无法获取，只能手动选择

        /*username = GameObject.Find("inforInterface/name_enter/Text");
        school = GameObject.Find("inforInterface/school_enter/Text");
        explainer1 = GameObject.Find("inforInterface/name_enter/explainer");
        explainer2 = GameObject.Find("inforInterface/school_enter/explainer");
        */
        explainer1.SetActive(true);
        explainer2.SetActive(true);
        for(int i=0;i<4;i++)
        {
            images[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

        name_input = username.GetComponent<Text>().text;
        school_input = school.GetComponent<Text>().text;
        if(name_input==""||HasDigit(name_input))
        {
            explainer1.SetActive(true);
            images[0].SetActive(true);
            images[1].SetActive(false);
        }
        else
        {
            explainer1.SetActive(false);
            images[0].SetActive(false);
            images[1].SetActive(true);
        }
        if (school_input == ""||HasDigit(school_input))
        {
            explainer2.SetActive(true);
            images[2].SetActive(true);
            images[3].SetActive(false);
        }
        else
        {
            explainer2.SetActive(false);
            images[2].SetActive(false);
            images[3].SetActive(true);
        }
    }
    bool HasDigit(string content)
    {
        int a = content.Length ;
        for(int i=0;i<a;i++)
        {
            if (char.IsDigit(content[i]))
                return true;
        }
        return false;
    }

}
