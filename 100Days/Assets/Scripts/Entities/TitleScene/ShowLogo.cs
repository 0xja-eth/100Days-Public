using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShowLogo : MonoBehaviour
{
    public Image Logo;
    public Image Background;
    float Transparency = 0f;
    bool isRising = true;
    bool hasShown = false;
    // Start is called before the first frame update
    void Start()
    {/*
        Logo = gameObject.GetComponent<Image>();
        Logo.color = new Color(255, 255, 255, Transparency);*/
    }

    // Update is called once per frame
    void Update()
    {
        //显示图片
        if (Transparency <= 1&&isRising)
        {
            Transparency += Time.deltaTime / 2;
            Logo.color = new Color(255, 255, 255, Transparency);
            Background.color = new Color(255, 255, 255, Transparency*0.75f);
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
                Background.color = new Color(255, 255, 255, Transparency * 0.75f);
            }
            else
            {
                hasShown = true;
                Destroy(this);
            }

        }

    }
    public void skip() {
        hasShown = true;
        gameObject.SetActive(false);
        Destroy(this);
    }
    public bool getShown() {
        return hasShown;
    }
}
