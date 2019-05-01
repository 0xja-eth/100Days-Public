using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Panel_controller : MonoBehaviour
{
    public GameObject enterInterface;
    public GameObject inforInterface;
    // Start is called before the first frame update
    void Start()
    {
        inforInterface.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Show_logo.getShown())
        {
            
            enterInterface.SetActive(false);
            inforInterface.SetActive(true);
        }
    }
}
