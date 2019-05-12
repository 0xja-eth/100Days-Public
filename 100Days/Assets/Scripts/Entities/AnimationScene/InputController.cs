using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class InputController : AnimatableLayer {
    public int maxNameLength = 8;
    public int maxSchoolLength = 16;

    public Text username;
    public Text school;
    public InputField schoolInput;
    public Dropdown schoolSel;

    public Text name_explainer;
    public Text school_explainer;

    public GameObject[] images = new GameObject[4];

    Text schoolSelName;

    string[] schools;
    // Start is called before the first frame update
    void Awake() {
        base.Awake();
        schools = schools ?? new string[0];
        schoolSelName = GameUtils.find<Text>(schoolSel.transform, "Label");
    }

    void Start(){
        base.Start();
    }

    // Update is called once per frame
    void Update() {
        base.Update();
    }
    public void setSchools(string[] schools) {
        this.schools = schools;
        schoolSel.options.Clear();
        schoolSel.options.Add(new Dropdown.OptionData("选择加入一间学校"));
        foreach (string s in schools)
            schoolSel.options.Add(new Dropdown.OptionData(s));
    }
    public void onNameInputChange() {
        string text = username.text;
        if (checkName(text)) {
            images[1].SetActive(true);
            images[0].SetActive(false);
        } else {
            images[1].SetActive(false);
            images[0].SetActive(true);
        }
    }
    public void onSchoolInputChange() {
        string text = school.text;
        if (text.Length > 0)
            schoolSel.value = 0;
        if (checkSchool(text)) {
            images[3].SetActive(true);
            images[2].SetActive(false);
        } else {
            images[3].SetActive(false);
            images[2].SetActive(true);
        }
    }
    public void refresh() {
        onNameInputChange();
        onSchoolInputChange();
    }
    public bool check() {
        onNameInputChange();
        onSchoolInputChange();
        bool res = checkName();
        res = checkSchool() && res;
        return res;
    }
    public void onSchoolSelectChange() {
        Debug.Log(schoolSel.value);
        Debug.Log(schoolSelName.text);
        if (schoolSel.value > 0)
            schoolInput.text = schoolSelName.text;
    }
    public string getName() { return username.text; }
    public string getSchool() {
        return school.text.Length > 0 ? school.text : schoolSelName.text;
    }
    public bool checkName() {
        return checkName(username.text);
    }
    public bool checkSchool() {
        return checkSchool(school.text);
    }
    bool checkName(string name) {
        Debug.Log(name + ":" + name.Length);
        string text = "";
        if (name.Length <= 0)
            text = "名字不能为空";
        else if(name.Length > maxNameLength)
            text = "名字不能超过 " + maxNameLength + " 个字";
        /*else if(hasDigit(name))
            text = "名字不能有数字";*/
        setNameExplainText(text);
        Debug.Log(text+":"+ text.Length);
        return text.Length == 0;
    }
    bool checkSchool(string school) {
        Debug.Log(school);
        Debug.Log(schoolSelName.text + " : " + schoolSel.value);
        Debug.Log(string.Join(",", schools));
        string text = "";
        if (school.Length <= 0 && schoolSel.value==0)
            text = "请选择加入或输入创建一所学校";
        else if (school.Length > maxSchoolLength)
            text = "学校名不能超过 " + maxSchoolLength + " 个字";
        /*else if (isSchoolExist(school) && school != schoolSelName.text) {
            text = "该学校已存在，继续创建你将加入该学校";
            for (int i = 0; i < schools.Length; i++)
                if (school == schools[i]) schoolSel.value = i + 1;
        } else if(hasDigit(school))
            text = "学校名不能有数字";*/
        else if (!regularSchoolCheck(school))
            text = "学校名必须以“中学”“高中”“学校”等词结尾";
        setSchoolExplainText(text);
        Debug.Log(text + ":" + text.Length);
        return text.Length == 0;

    }
    bool isSchoolExist(string school) {
        foreach (string s in schools)
            if (s == school) return true;
        return false;
    }

    bool hasDigit(string content){
        foreach(char c in content)
            if (char.IsDigit(c))
                return true;
        return false;
    }
    bool regularSchoolCheck(string school) {
        string schoolReg = @".+(中学|高中|学校|School)$";
        Regex reg = new Regex(schoolReg, RegexOptions.IgnoreCase);
        return reg.IsMatch(school);
    }

    public void setNameExplainText(string text) {
        name_explainer.text = text;
    }
    public void setSchoolExplainText(string text) {
        school_explainer.text = text;
    }

}
