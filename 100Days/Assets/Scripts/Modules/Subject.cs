using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subject {
	int id;			// 科目代号
	string name;	// 科目名
	int value; 		// 科目点数/增减

	public const int SubjectCount = 9;
	public static readonly string[] SubjectName = {"语文","数学","英语",
		"物理","化学","生物","政治","历史","地理","理综","文综"};
	public static readonly int[][] DefaultSubjectsSet = {
		new int[] {0,1,2,6,7,8}, new int[] {0,1,2,3,4,5}
	};
	public static readonly int[][] DefaultExamSubjectsSet = {
		new int[] {0,1,2,10}, new int[] {0,1,2,9}
	};
	public static readonly int[][] DefaultMultSubjectSet = {
		new int[] {0}, new int[] {1}, new int[] {2}, new int[] {3},
		new int[] {4}, new int[] {5}, new int[] {6}, new int[] {7},
		new int[] {8}, new int[] {3,4,5}, new int[] {6,7,8}
	};	
	public static readonly int[] MaxScores = {
		150, 150, 150, 110, 100, 90, 100, 100, 100, 300, 300
	}; // 每科目最高成绩


	public int getId() {return id;}
	public string getName() {return name;}
	public int getValue() {return value;}

	public Subject(int id, string name) {
		this.id = id; this.name = name;
		resetPoint();
	}

	public void resetPoint(){value = 0;}

	public void addPoint(int value){
		this.value += value;
	}
	public void addPoint(Subject s){
		if(this.id != s.id) return;
		addPoint(s.value);
	}
	public void reducePoint(double rate){
		this.value = Mathf.RoundToInt((float)(this.value*(1-rate)));
	}

	public static Subject getStandardSubject(int id) {
		return new Subject(id, SubjectName[id]);
	}

	public static Subject[] getStandardSubjects(int[] ids) {
		Subject[] res = new Subject[ids.Length];
		Debug.Log("getStandardSubjects");
		Debug.Log(ids);
		Debug.Log(res);
		for(int i=0;i<ids.Length;i++)
			res[i] = getStandardSubject(ids[i]);
		Debug.Log(res);
		return res;
	}
}
