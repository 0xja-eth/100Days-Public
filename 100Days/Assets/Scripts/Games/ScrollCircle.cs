using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// AchoredPostion是锚点到重心点的向量 既有方向又有大小
/// </summary>

public class ScrollCircle : ScrollRect {

    public float speed = 10;

    private float r;//滑动半径
    private float mr;//小圆正在滑动的半径
    private bool isOnDrag;
    public SnakeHead p;

    void Start() {
        //RectTransform继承了transform,脚本只能调用transform,这里强制转换
        r = (transform as RectTransform).sizeDelta.x / 3;//设置滑动半径  这里用大圆半径的1/3
        horizontal = true;
        vertical = true;

        //p = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update() {
        Vector2 V = content.anchoredPosition; //获取滑动向量
        controllPlayer(V);//控制移动
    }

    //小球滑动功能
    public override void OnDrag(PointerEventData eventData) {
        base.OnDrag(eventData);
        isOnDrag = true;
        mr = content.anchoredPosition.magnitude;//小圆滑动向量的长度

        Debug.Log("允许的半径:" + r + "   已滑动的半径：" + mr);
        if (mr > r) {
            Debug.Log("超出范围");
            //接下来控制小球在边缘不出去 也就是说anchoredpostion的长度要=r 但是人物移动方向还是要存在 所以利用滑动向量单位化 仅保留方向 长度置为1 
            content.anchoredPosition = content.anchoredPosition.normalized * r;//向量单位化 保留方向变化 向量长度为1*R
                                                                               //Wrong  content.anchoredPosition = new Vector2(r, r);//错误的做法 因为丢失了方向
        }


    }

    //小球复位功能 当玩家松手之后 小球才能复位
    public override void OnEndDrag(PointerEventData eventData) {
        base.OnEndDrag(eventData);
        Debug.Log("-----------------------------------------Stopped dragging！");
        content.anchoredPosition = new Vector2(0, 0);//复位
        isOnDrag = false;
    }
    //控制移动
    private void controllPlayer(Vector2 v)//传入摇杆的滑动向量
    {
        //float x = v.x;//获取x轴的向量 有长度+方向
       // float y = v.y;//y轴的
        if (isOnDrag == true) p.onScrollMove(v);
        //speed = content.anchoredPosition.magnitude / 5;//设置速度=向量长度的五分之一
        //p.transform.Translate(new Vector3(x, y, 0).normalized * Time.deltaTime * speed);//仅保留方向的单位向量 乘以 速度
        
    }
}
