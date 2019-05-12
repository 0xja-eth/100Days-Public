using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerJsonData {
    public string name;          
    public string school;
    public int maxEnergy, energy;
    public int maxPressure, pressure;
    public IntArray subjectParams;
    public int subjectSel;
    public double reduceRate;
    public double pressureReduceRate;
}

public class Player {
	string 		name;				    // 玩家名字
    string      school;                 // 玩家学校
    int         maxEnergy, energy;      // 最大精力，当前精力
    int         maxPressure, pressure;	// 最大压力，当前压力
    Subject[] 	subjectParams;		    // 选择的科目点数属性
	int 		subjectSel;             // 分科选择记录
    double      reduceRate;             // 科目点数遗忘率
    double      pressureReduceRate;     // 压力衰减率

    const int DefaultMaxEnergy = 150;
    const int DefaultMaxPressure = 100;
    const double DefaultReduceRate = 0.02;
    const double DefaultPressureReduceRate = 0.1;

    const double PressureRateBase = 0.25;
    const double PressureLinearBase = 0.75;
    const double PressureLinearK = 1;

    public string getName() {return name;}
    public string getSchool() {return school; }
    public int getEnergy() { return energy; }
    public int getMaxEnergy() { return maxEnergy; }
    public int getPressure() { return pressure; }
    public int getMaxPressure() { return maxPressure; }

    public string toJson() {
        return JsonUtility.ToJson(toJsonData());
    }
    public PlayerJsonData toJsonData() {
        PlayerJsonData data = new PlayerJsonData();
        int cnt = subjectParams.Length;
        data.name = name;
        data.school = school;
        data.maxEnergy = maxEnergy;
        data.energy = energy;
        data.maxPressure = maxPressure;
        data.pressure = pressure;
        data.subjectSel = subjectSel;
        data.reduceRate = reduceRate;
        data.pressureReduceRate = pressureReduceRate;
        data.subjectParams = new IntArray();
        for (int i = 0; i < cnt; i++)
            data.subjectParams.Add(subjectParams[i].getValue());
        return data;
    }
    public bool fromJsonData(PlayerJsonData data) {
        name = data.name;
        school = data.school;
        maxEnergy = data.maxEnergy;
        energy = data.energy;
        maxPressure = data.maxPressure;
        pressure = data.pressure;
        pressureReduceRate = data.pressureReduceRate;
        selectSubject(data.subjectSel);
        int cnt = subjectParams.Length;
        for (int i = 0; i < cnt; i++)
            subjectParams[i].setPoint(data.subjectParams[i]);
        return true;
    }

    public Player(string name, string school, int subjectsType) {
        energy = maxEnergy = DefaultMaxEnergy;
        maxPressure = DefaultMaxPressure;
        pressure = 0;
        reduceRate = DefaultReduceRate;
        pressureReduceRate = DefaultPressureReduceRate;
        selectSubject(subjectsType);
        this.school = school;
        this.name = name;
    }
    public Player(PlayerJsonData data) {
        fromJsonData(data);
    }

    public Subject getSubjectParam(int sid) {
        foreach (Subject s in subjectParams)
            if (s.getId() == sid) return s;
        return null;
    }
    public int getSubjectParamValue(int sid) {
        return getSubjectParam(sid).getValue();
    }
    public Subject getSubjectParamById(int id) {
        return subjectParams[id];
    }
    public int getSubjectParamValueById(int id) {
        return getSubjectParamById(id).getValue();
    }
    public void increaseSubjectParam(Subject inc){
		Subject s = getSubjectParam(inc.getId());
		if(s == null) return; s.addPoint(inc);
	}
    public int getSubjectCount() {
        return subjectParams.Length;
    }
	public int[] getSubjectIds(){
		return Subject.DefaultSubjectsSet[subjectSel];
	}
    /*
	public int[] getExamSubjectIds(){
		return Subject.DefaultExamSubjectsSet[subjectSel];
    }
    */
    public void selectSubject(int sel) {
        subjectSel = sel;
        subjectParams = Subject.getStandardSubjects(
            Subject.DefaultSubjectsSet[sel]);
    }
    public List<Subject> getMaxSubjects() {
        int mid = 0, cnt = subjectParams.Length;
        List<Subject> sbjs = new List<Subject>();
        for (int i = 1; i < cnt; i++)
            if (getSubjectParamValueById(i) >
                getSubjectParamValueById(mid)) mid = i;
        for (int i = 0; i < cnt; i++)
            if (getSubjectParamValueById(i) ==
                getSubjectParamValueById(mid))
                sbjs.Add(getSubjectParamById(i));
        return sbjs;
    }
    public List<Subject> getMinSubjects() {
        int mid = 0, cnt = subjectParams.Length;
        List<Subject> sbjs = new List<Subject>();
        for (int i = 1; i < cnt; i++)
            if (getSubjectParamValueById(i) <
                getSubjectParamValueById(mid)) mid = i;
        for (int i = 0; i < cnt; i++)
            if (getSubjectParamValueById(i) ==
                getSubjectParamValueById(mid))
                sbjs.Add(getSubjectParamById(i));
        return sbjs;
    }

    public void changeEnergy(int value) {
        energy = Mathf.Clamp(energy + value, 0, maxEnergy);
    }
    public void changePressure(int value) {
        pressure = Mathf.Clamp(pressure + value, 0, maxPressure);
    }
    public void onNextDay() {
        recoveryEnergy();
        reducePressure();
        reduceSubjectParams();
    }
    public void recoveryEnergy() {
        energy = maxEnergy;
    }
    public void recoveryPressure() {
        pressure = 0;
    }
    public void reducePressure() {
        changePressure((int)(-pressure * pressureReduceRate));
    }

    public double calcPressureEffect() {
        double pRate = pressure * 1.0 / maxPressure;
        double rate1 = (pRate > PressureLinearBase ?
            Mathf.Log((float)((1 - PressureLinearBase) / (1 - PressureRateBase)))
                - (pRate - PressureLinearBase) * PressureLinearK :
            Mathf.Log((float)((1 - pRate) / (1 - PressureRateBase))));
        return sigmoid(rate1) * 2;
    }
    double sigmoid(double x) {
        return 1 / (1 + Mathf.Exp(-(float)x));
    }

    public void reduceSubjectParams(){
		foreach(Subject s in subjectParams)
			s.reducePoint(reduceRate);
	}

    public void showSubjectParams() {
        foreach (Subject s in subjectParams)
            Debug.Log(s.getName() + ": " + s.getValue());
    }
}
