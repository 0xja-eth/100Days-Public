using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerJsonData {
    public string name;          
    public string school;  
    public int maxEnergy, energy;  
    public int[] subjectParams;
    public int subjectSel;         
    public double reduceRate;	   
}

public class Player {
	string 		name;				// 玩家名字
    string      school;             // 玩家学校
	int 		maxEnergy, energy;	// 最大精力，当前精力
	Subject[] 	subjectParams;		// 选择的科目点数属性
	int 		subjectSel;			// 分科选择记录
	double		reduceRate;			// 科目点数遗忘率

	const int DefaultMaxEnergy = 100;
	const double DefaultReduceRate = 0.02;
	
    public string getName() {return name;}
    public string getSchool() {return school;}
	public int getEnergy() {return energy;}
	public int getMaxEnergy() {return maxEnergy;}

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
        data.subjectSel = subjectSel;
        data.reduceRate = reduceRate;
        data.subjectParams = new int[cnt];
        for (int i = 0; i < cnt; i++)
            data.subjectParams[i] = subjectParams[i].getValue();
        return data;
    }
    public bool fromJsonData(PlayerJsonData data) {
        name = data.name;
        school = data.school;
        maxEnergy = data.maxEnergy;
        energy = data.energy;
        selectSubject(data.subjectSel);
        int cnt = subjectParams.Length;
        for (int i = 0; i < cnt; i++)
            subjectParams[i].setPoint(data.subjectParams[i]);
        return true;
    }

    public Player(string name, string school, int subjectsType) {
        energy = maxEnergy = DefaultMaxEnergy;
        reduceRate = DefaultReduceRate;
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
	public int[] getExamSubjectIds(){
		return Subject.DefaultExamSubjectsSet[subjectSel];
	}
	public void selectSubject(int sel){
		subjectSel = sel;
		subjectParams = Subject.getStandardSubjects(
			Subject.DefaultSubjectsSet[sel]);
	}

	public void changeEnergy(int value){
		energy = Mathf.Clamp(energy+value, 0, maxEnergy);
	}
	public void recoveryEnergy(){
		energy = maxEnergy;
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
