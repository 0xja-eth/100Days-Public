using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameEndScene : MonoBehaviour {
    float moveSpeed = 0.6f;
    float fadeSpeed = 0.008f;
    float stoppingY = 192f;

    public RectTransform textRect;
    public GameObject skipBtn;
    public GameObject background;

    public AnimatableLayer gard, content, result, night;

    public Text title, name, school;
    public RectTransform[] subjects;

    Text text;

    FinalExam examSet;
    Player player;

    List<int> scores = new List<int>();
    List<int> maxs = new List<int>();
    List<float> values = new List<float>();
    List<string> names = new List<string>();

    float aniInterval;
    float aniIntervalRate = 1.05f;
    float maxAniInterval;
    float timer = 0;

    string[] schools = { "北京大学",
        "清华大学",
        "浙江大学",
        "复旦大学",
        "中国人民大学",
        "上海交通大学",
        "武汉大学",
        "南京大学",
        "中山大学",
        "吉林大学",
        "华中科技大学",
        "四川大学",
        "天津大学",
        "南开大学",
        "西安交通大学",
        "中国科学技术大学",
        "中南大学",
        "哈尔滨工业大学",
        "北京师范大学",
        "山东大学",
        "厦门大学",
        "东南大学",
        "同济大学",
        "北京航空航天大学",
        "大连理工大学",
        "东北大学",
        "华南理工大学",
        "华东师范大学",
        "北京理工大学",
        "西北工业大学",
        "重庆大学",
        "兰州大学",
        "中国农业大学",
        "电子科技大学",
        "湖南大学",
        "东北师范大学",
        "西南大学",
        "武汉理工大学",
        "西南交通大学",
        "北京交通大学",
        "华中师范大学",
        "河海大学",
        "南京农业大学",
        "南京理工大学",
        "南京师范大学",
        "西安电子科技大学",
        "北京科技大学",
        "华中农业大学",
        "郑州大学",
        "中国海洋大学",
        "西北大学",
        "华东理工大学",
        "中国矿业大学（徐州）",
        "南京航空航天大学",
        "上海大学",
        "北京协和医学院",
        "西北农林科技大学",
        "苏州大学",
        "北京邮电大学",
        "中国地质大学（武汉）",
        "北京化工大学",
        "上海财经大学",
        "长安大学",
        "云南大学",
        "中国政法大学",
        "哈尔滨工程大学",
        "合肥工业大学",
        "湖南师范大学",
        "东华大学",
        "南昌大学",
        "中南财经政法大学",
        "昆明理工大学",
        "深圳大学",
        "暨南大学",
        "首都师范大学",
        "江南大学",
        "华南师范大学",
        "陕西师范大学",
        "福建师范大学",
        "中央民族大学",
        "福州大学",
        "北京工业大学",
        "广西大学",
        "燕山大学",
        "河南大学",
        "中国石油大学（华东）",
        "华南农业大学",
        "宁波大学",
        "浙江工业大学",
        "山西大学",
        "对外经济贸易大学",
        "浙江师范大学",
        "北京林业大学",
        "西南财经大学",
        "上海理工大学",
        "杭州电子科技大学",
        "天津师范大学",
        "扬州大学",
        "中央财经大学",
        "首都医科大学",
        "东北林业大学",
        "河北大学",
        "安徽大学",
        "太原理工大学",
        "辽宁大学",
        "南京工业大学",
        "新疆大学",
        "华北电力大学（北京）",
        "湘潭大学",
        "东北财经大学",
        "上海师范大学",
        "黑龙江大学",
        "江苏大学",
        "南方医科大学",
        "北京中医药大学",
        "中国石油大学（北京）",
        "大连海事大学",
        "南京邮电大学",
        "福建农林大学",
        "安徽师范大学",
        "长沙理工大学",
        "贵州大学",
        "内蒙古大学",
        "天津医科大学",
        "哈尔滨医科大学",
        "西南政法大学",
        "中国药科大学",
        "山东师范大学",
        "东北农业大学",
        "西北师范大学",
        "北京语言大学",
        "江西师范大学",
        "上海中医药大学",
        "山东农业大学",
        "广东外语外贸大学",
        "青岛大学",
        "河南科技大学",
        "南京医科大学",
        "四川农业大学",
        "西安建筑科技大学",
        "江西财经大学",
        "中国地质大学（北京）",
        "浙江工商大学",
        "华侨大学",
        "山东科技大学",
        "广州大学",
        "湖南农业大学",
        "河北工业大学",
        "武汉科技大学",
        "海南大学",
        "广东工业大学",
        "天津工业大学",
        "河北师范大学",
        "河南农业大学",
        "湖北大学",
        "中国医科大学",
        "广西师范大学",
        "南京信息工程大学",
        "汕头大学",
        "云南师范大学",
        "南京林业大学",
        "成都理工大学",
        "石河子大学",
        "浙江理工大学",
        "重庆医科大学",
        "哈尔滨师范大学",
        "重庆邮电大学",
        "西安理工大学",
        "首都经济贸易大学",
        "青岛科技大学",
        "杭州师范大学",
        "云南民族大学",
        "长春理工大学",
        "哈尔滨理工大学",
        "中南民族大学",
        "河北农业大学",
        "华东政法大学",
        "四川师范大学",
        "河南师范大学",
        "广州中医药大学",
        "延边大学",
        "南京中医药大学",
        "天津中医药大学",
        "东北电力大学",
        "中国计量大学",
        "济南大学",
        "大连大学",
        "中北大学",
        "兰州交通大学",
        "江苏师范大学",
        "温州医科大学",
        "云南农业大学",
        "中国矿业大学（北京）",
        "天津科技大学",
        "天津理工大学",
        "沈阳农业大学",
        "河南理工大学",
        "西南民族大学",
        "辽宁师范大学",
        "重庆交通大学",
        "温州大学",
        "上海海事大学",
        "北京工商大学",
        "宁夏大学",
        "内蒙古农业大学",
        "上海海洋大学",
        "西南石油大学",
        "辽宁工程技术大学",
        "长江大学",
        "湖南科技大学",
        "兰州理工大学",
        "河南工业大学",
        "山东理工大学",
        "天津财经大学",
        "桂林电子科技大学",
        "沈阳工业大学",
        "东北石油大学",
        "成都中医药大学",
        "三峡大学",
        "曲阜师范大学",
        "大连医科大学",
        "湖北工业大学",
        "吉林农业大学",
        "重庆师范大学",
        "安徽农业大学",
        "西安科技大学",
        "陕西科技大学",
        "河北医科大学",
        "安徽工业大学",
        "安徽医科大学",
        "广西民族大学",
        "东莞理工学院",
        "黑龙江中医药大学",
        "安徽理工大学",
        "烟台大学",
        "上海对外经贸大学",
        "江西理工大学",
        "江西农业大学",
        "中南林业科技大学",
        "西北民族大学",
        "贵州师范大学",
        "内蒙古师范大学",
        "南通大学",
        "集美大学",
        "华东交通大学",
        "山东财经大学",
        "武汉纺织大学",
        "西藏大学",
        "广州医科大学",
        "南昌航空大学",
        "青岛理工大学",
        "广西医科大学",
        "河北科技大学",
        "大连工业大学",
        "北华大学",
        "安徽财经大学",
        "西南科技大学",
        "新疆医科大学",
        "新疆师范大学",
        "新疆农业大学",
        "沈阳建筑大学",
        "沈阳药科大学",
        "长春工业大学",
        "石家庄铁道大学",
        "浙江农林大学",
        "重庆工商大学",
        "哈尔滨商业大学",
        "景德镇陶瓷大学",
        "浙江海洋大学",
        "桂林理工大学",
        "上海工程技术大学",
        "浙江财经大学",
        "内蒙古工业大学",
        "南华大学",
        "海南师范大学",
        "鲁东大学",
        "沈阳师范大学",
        "北京建筑大学",
        "甘肃农业大学",
        "中国民航大学",
        "宁夏医科大学",
        "山西师范大学",
        "内蒙古科技大学",
        "吉林师范大学",
        "东华理工大学",
        "南京财经大学",
        "云南财经大学",
        "福建医科大学",
        "沈阳航空航天大学",
        "江苏科技大学",
        "西北政法大学",
        "青海大学",
        "安徽建筑大学",
        "华北理工大学",
        "河南中医药大学",
        "山西财经大学",
        "浙江中医药大学",
        "湖南中医药大学",
        "西安石油大学",
        "北京信息科技大学",
        "武汉轻工大学",
        "聊城大学",
        "大连交通大学",
        "齐齐哈尔大学",
        "山东建筑大学",
        "山东中医药大学",
        "北京联合大学",
        "武汉工程大学",
        "浙江科技学院",
        "北方工业大学",
        "河北经贸大学",
        "华北水利水电大学",
        "江西中医药大学",
        "山西农业大学",
        "渤海大学",
        "常州大学",
        "西华大学",
        "重庆理工大学",
        "天津商业大学",
        "太原科技大学",
        "辽宁科技大学",
        "成都信息工程大学",
        "西华师范大学",
        "辽宁中医药大学",
        "吉林财经大学",
        "河南财经政法大学",
        "天津职业技术师范大学",
        "长春中医药大学",
        "西安工程大学",
        "广东财经大学",
        "山西医科大学",
        "西安邮电大学",
        "北京服装学院",
        "辽宁石油化工大学",
        "上海电力学院",
        "西安工业大学",
        "大连海洋大学",
        "福建中医药大学",
        "黑龙江科技大学",
        "湖北中医药大学",
        "湖南工业大学",
        "延安大学",
        "昆明医科大学",
        "内蒙古民族大学",
        "佳木斯大学",
        "安徽工程大学",
        "河北地质大学",
        "福建工程学院",
        "贵州医科大学",
        "南京审计大学",
        "沈阳理工大学",
        "沈阳化工大学",
        "大连民族大学",
        "苏州科技大学",
        "河北工程大学",
        "西南林业大学",
        "北京农学院",
        "徐州医科大学",
        "上海立信会计金融学院",
        "湖州师范学院",
        "江西科技师范大学",
        "绍兴文理学院",
        "中国青年政治学院",
        "安徽中医药大学",
        "北京印刷学院",
        "锦州医科大学",
        "新疆财经大学",
        "厦门理工学院",
        "黑龙江工程学院",
        "上海政法学院",
        "淮北师范大学",
        "新乡医学院",
        "内蒙古财经大学",
        "天津城建大学",
        "嘉兴学院",
        "浙江万里学院",
        "长春师范大学",
        "上海电机学院",
        "井冈山大学",
        "大理大学",
        "南昌工程学院",
        "海南医学院",
        "兰州财经大学",
        "上海金融学院",
        "西南医科大学",
        "闽南师范大学",
        "北京石油化工学院",
        "北京物资学院",
        "湖北经济学院",
        "内蒙古医科大学",
        "中国民用航空飞行学院",
        "闽江学院",
        "河北科技师范学院",
        "潍坊医学院",
        "川北医学院",
        "山西中医药大学",
        "沈阳工程学院",
        "云南中医学院",
        "上海海关学院",
        "中华女子学院",
        "牡丹江医学院",
        "吉林工程技术师范学院",
        "白城师范学院",
        "太原师范学院",
        "中国劳动关系学院",
        "伊犁师范学院",
        "齐齐哈尔医学院",
        "通化师范学院",
        };

    bool speed = false, textAnimating = true;
    bool pause = false, pausable = false;

    bool schoolAni = false;
    bool closeable = false;

    bool resultAniType = false;
    // Use this for initialization
    void Awake() {
        /*
        GameUtils.initialize(null,
            "Canvas/PromptLayer/AlertWindow",
            "Canvas/PromptLayer/LoadingScene");*/
        aniInterval = 10 * Time.deltaTime;
        maxAniInterval = 500 * Time.deltaTime;
    }
    void Start () {
        pause = true;
        setupScoreLayer();
    }

    // Update is called once per frame
    void startAnimation() {
        fadeIn(() => {
            gard.showWindow(() => {
                StartCoroutine(laterShow(1.5f));//laterShow()方法  
            }, new Vector3(1, 1, 1));
        });
    }

    //定义 laterShow（）方法  
    IEnumerator laterShow(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        //等待之后执行的动作  
        content.showWindow(startTextAnimation, new Vector3(1, 1, 1));
    }  
    void startTextAnimation() {
        Debug.Log("startTextAnimation");
        pause = false;
    }

    void Update () {
        updatePause();
        updateSchoolAni();
        updateResultAni();
        updateTextAnimation();
        updateButtonAnimation();
    }
    void updatePause() {
        if(pausable) pause = background.activeInHierarchy;
    }
    void updateSchoolAni() {
        if (!schoolAni) return;
        timer += Time.deltaTime;
        if (timer >= aniInterval) {
            aniInterval *= aniIntervalRate;
            int index = UnityEngine.Random.Range(0, schools.Length);
            string school = schools[index];
            this.school.text = school;
        }
        if (aniInterval >= maxAniInterval) onSchoolAniEnd();
    }
    void updateResultAni() {
        if (!closeable) return;
        if((timer += Time.deltaTime) < aniInterval) return;
        Image img = GameUtils.get<Image>(result.gameObject);
        Color c = img.color;
        if (resultAniType) resultAniType = (c.a += fadeSpeed) < 1;
        else resultAniType = (c.a -= fadeSpeed) < 0.85;
        img.color = c;
    }
    void onSchoolAniEnd() {
        timer = 0;
        schoolAni = false;
        closeable = true;
        skipBtn.SetActive(true);
    }

    public void closeResult() {
        if (!closeable) return;
        result.hideWindow(new Vector3(1, 0, 0));
        startAnimation();
    }

    void updateTextAnimation() {
        if (!pause) textRect.position = textRect.position + Vector3.up * getMoveSpeed();
        if (isTextAnimationStopping() && textAnimating) onTextAnimationEnd();
    }
    void updateButtonAnimation() {
        if (!textAnimating) fadeOutText(); 
    }
    void fadeOutText() {
        Color c = text.color; c.a -= fadeSpeed; text.color = c;
    }
    bool isTextAnimationStopping() {
        return textRect.position.y > textRect.rect.height + stoppingY;
    }
    void onTextAnimationEnd() {
        pausable = pause = false;
        textAnimating = false;
        skipBtn.SetActive(false);
        fadeOut(endGame);
    }

    public void fadeIn(UnityAction act = null) {
        night.colorTo(new Color(0, 0, 0, 0), "fadeIn", act);
    }
    public void fadeOut(UnityAction act = null) {
        night.showWindow();
        night.colorTo(new Color(0, 0, 0, 1), "fadeOut", act);
    }

    public void toggleAnimation() {
        if (pausable) pause = !pause;
    }
    public void toggleSpeedAnimation() {
        speed = !speed;
    }
    float getMoveSpeed() {
        return speed ? moveSpeed * 5 : moveSpeed;
    }

    public void skipAnimation() {
        moveSpeed *= 50;
        onTextAnimationEnd();
    }

    public void endGame() {
        NetworkSystem.clear();
        NetworkSystem.setSuccessHandler((data)=> {
            StorageSystem.deleteSave(GameSystem.getSaveIndex());
            SceneManager.LoadScene("GameTitleScene");
        });
        StorageSystem.saveGame();
    }

    public void setupScoreLayer() {
        result.showWindow();
        examSet = GameSystem.getFinalExam();
        player = GameSystem.getPlayer();
        title.text = examSet.getName();
        name.text = "姓名：<size=32>" + player.getName() + "</size>";
        perpareFinalExamData();
        drawSubjects(0);
    }
    void perpareFinalExamData() {
        int sum = 0, sumMax = 0;
        int cnt = examSet.getExamCount();
        for (int i = 0; i < cnt; i++) {
            Exam e = examSet.getExamById(i);
            int score = e.getFinalScore();
            int sid = e.getSubjectId();
            string sname = Subject.SubjectName[sid];
            int max = Subject.MaxScores[sid];
            sum += score; sumMax += max;
            scores.Add(score); maxs.Add(max);
            values.Add(score * 1.0f / max);
            names.Add(sname);
        }
        scores.Add(sum); maxs.Add(sumMax);
        values.Add(sum * 1.0f / sumMax);
        names.Add("总分");
    }
    void drawSubjects(int i) {
        if (i >= subjects.Length) playSchoolAnimation();
        else {
            subjects[i].gameObject.SetActive(true);
            Text name = GameUtils.find<Text>(subjects[i], "Name");
            Text value = GameUtils.find<Text>(subjects[i], "Value");
            AnimatableLayer bar = GameUtils.find<AnimatableLayer>(subjects[i], "Bar/Bar");
            Image img = GameUtils.find<Image>(subjects[i], "Bar/Bar");
            bar.image = img;
            name.text = names[i];
            value.text = scores[i] + "/" + maxs[i];
            bar.colorTo(new Color(1 - values[i], values[i], 0));
            bar.scaleTo(new Vector3(values[i], 1, 1), "scale",
                () => { drawSubjects(i + 1); });
        }
    }
    void playSchoolAnimation() {
        schoolAni = true;
    }
}
