using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {
	string 		name;				// 玩家名字
	int 		maxEnergy, energy;	// 最大精力，当前精力
	Subject[] 	subjectParams;		// 选择的科目点数属性
	int 		subjectSel;			// 分科选择记录
	double		reduceRate;			// 科目点数遗忘率

	const int DefaultMaxEnergy = 100;
	const double DefaultReduceRate = 0.02;
	
	public string getName() {return name;}
	public int getEnergy() {return energy;}
	public int getMaxEnergy() {return maxEnergy;}

	public Player(string name, int subjectsType){
		energy = maxEnergy = DefaultMaxEnergy;
		reduceRate = DefaultReduceRate;
		selectSubject(subjectsType);
		this.name = name;
	}

	public Subject getSubjectParam(int sid){
		foreach(Subject s in subjectParams)
			if(s.getId() == sid) return s;
		return null;
	}
	public int getSubjectParamValue(int sid){
		return getSubjectParam(sid).getValue();
	}
	public void increaseSubjectParam(Subject inc){
		Subject s = getSubjectParam(inc.getId());
		if(s == null) return; s.addPoint(inc);
	}
	public int[] getSubjectIds(){
		return Subject.DefaultSubjectsSet[subjectSel];
	}
	public int[] getExamSubjectIds(){
		return Subject.DefaultExamSubjectsSet[subjectSel];
	}
	public void selectSubject(int sel){
		subjectSel = sel;
		Debug.Log(sel);
		Debug.Log(Subject.DefaultSubjectsSet);
		Debug.Log(Subject.DefaultSubjectsSet[sel]);
		Debug.Log(Subject.DefaultSubjectsSet[sel].ToString());
		Debug.Log(Subject.DefaultSubjectsSet[sel][4]);
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
