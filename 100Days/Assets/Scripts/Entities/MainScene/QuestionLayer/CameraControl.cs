using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CameraControlCallback();

public class CameraControl : MonoBehaviour {
    //public int 

    const float moveSpeed = 0.05f;
    const float rotateSpeed = 0.05f;

    const float stopMoveDist = 1f;
    const float stopRotaDist = 1f;

	Vector3 targetPos, targetRot;
	Vector3 lookVector;
	Transform lookTarget;
	string moveType;

	CameraControlCallback callback;

	// Use this for initialization
	void Awake () {
		targetPos = transform.position;
		targetRot = transform.eulerAngles;
	}
	
	// Update is called once per frame
	void Update () {
		if(isMoving()) updateMove();
        //Debug.Log(moveType + ": " + targetPos + ", " + targetRot);
    }

    void updateMove(){
        switch (moveType){
			case "normal":
				transform.position += (targetPos-transform.position)*moveSpeed;
				transform.rotation = Quaternion.Slerp(transform.rotation, 
					Quaternion.Euler(targetRot), rotateSpeed);
				if(isStopping()) stopMove();
				break;
			case "toward":
				transform.position += (targetPos-transform.position)*moveSpeed;
				transform.rotation = Quaternion.Slerp(
					transform.rotation, Quaternion.LookRotation(
					lookVector - transform.position), rotateSpeed*100); 
				break;
			case "toward2":
				transform.position += (targetPos-transform.position)*moveSpeed;
				transform.rotation = Quaternion.Slerp(
					transform.rotation, Quaternion.LookRotation(
					lookTarget.position - transform.position), rotateSpeed*100); 
				break;

		}
	}

	public bool isMoving(){
		return transform.position != targetPos || transform.eulerAngles != targetRot;
	}

	public bool isStopping(){
		return Vector3.Distance(transform.position,targetPos)<stopMoveDist
			&& Vector3.Distance(transform.eulerAngles,targetRot)<stopRotaDist;
	}

	public void stopMove(bool target=true) {
        if (target){
			transform.position = targetPos;
			transform.eulerAngles = targetRot;
		}else {
			targetPos = transform.position;
			targetRot = transform.eulerAngles;
		}
		if(this.callback!=null) this.callback();
		this.callback = null;
	}

	public void moveTo(Vector3 position, Vector3 rotation){
		moveType = "normal";
		targetPos = position; targetRot = rotation;
    }
	public void moveToward(Vector3 position, Vector3 faceTo){
		moveType = "toward";
		targetPos = position; lookVector = faceTo;
	}
	public void moveToward(Vector3 position, Transform faceTo){
		moveType = "toward2";
		targetPos = position; lookTarget = faceTo;
	}
	public void setCallback(CameraControlCallback callback){
		if(!isMoving()) callback();
		else this.callback = callback;
	}
}
