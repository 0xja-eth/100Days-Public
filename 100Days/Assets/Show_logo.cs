using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Show_logo : MonoBehaviour
{
    public Image Logo;
    float Transparency = 0f;
    bool isRising = true;
    static bool hasShown = false;
    // Start is called before the first frame update
    void Start()
    {
        Logo = gameObject.GetComponent<Image>();
        Logo.color = new Color(255, 255, 255, Transparency);
    }

    // Update is called once per frame
    void Update()
    {
        //显示图片
        if (Transparency <= 1&&isRising)
        {
            Transparency += Time.deltaTime / 2;
            Logo.color = new Color(255, 255, 255, Transparency);
        }
        else
        {
            isRising = false;
        }
        //隐藏图片
        if ((!isRising))
        {
            if (Transparency > 0)
            {
                Transparency -= Time.deltaTime / 2;
                Logo.color = new Color(255, 255, 255, Transparency);
            }
            else
            {
                hasShown = true;
                Destroy(this);
            }

        }

    }
       
    static public bool getShown()
    {
        return hasShown;
    }
}
