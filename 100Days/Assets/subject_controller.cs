using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class subject_controller : MonoBehaviour
{
    [SerializeField] string scene_name;
    Toggle arts;
    Toggle science;
    bool isArts=false;
    bool isScience=false;
    string subject;
    // Start is called before the first frame update
    void Start()
    {
        arts = GameObject.Find("Arts_Toggle").GetComponent<Toggle>();
        science = GameObject.Find("Science_Toggle").GetComponent<Toggle>();
    }

    // Update is called once per frame
    void Update()
    {
        isArts = arts.isOn;
        isScience = science.isOn;
    }

    public void on_click()
    {
        if(isArts)
        {
            subject = "arts";
        }
        else if(isScience)
        {
            subject="science";
        }
        SceneManager.LoadScene(scene_name);
    }

}
