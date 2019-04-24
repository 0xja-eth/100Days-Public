using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public static class GameUtils {
	static Transform objectFinder;
	static Transform camera;
	static Transform alertWindow;

	static CameraControl cameraControl;
	static AlertLayer alertLayer;

	public static void initialize(){
		objectFinder = GameObject.Find("ObjectFinder").transform;
		camera = find<Transform>("MainCamera");
		alertWindow = find<Transform>("Canvas2D/AlertWindow");
		cameraControl = get<CameraControl>(camera);
		alertLayer = get<AlertLayer>(alertWindow);
	}

	public static Transform getCamera(){return camera;}
	public static Transform getAlertWindow(){return alertWindow;}

	public static CameraControl getCameraControl(){return cameraControl;}
	public static AlertLayer getAlertLayer(){return alertLayer;}

    public static void alert(string msg, string[] btns = null, UnityAction[] actions = null) {
        alertLayer.setup(msg, btns, actions);
    }

    public static T get<T> (Transform t){
		return t.GetComponent<T>();
	}
	public static T get<T> (GameObject obj){
		return obj.GetComponent<T>();
	}
	public static T find<T> (string obj){
		return get<T> (objectFinder.Find(obj));
    }
    public static T find<T>(Transform parent, string obj) {
        return get<T>(parent.Find(obj));
    }
    public static T find<T>(GameObject parent, string obj) {
        return get<T>(parent.transform.Find(obj));
    }
    public static GameObject find(Transform parent, string obj) {
        return parent.Find(obj).gameObject;
    }
    public static GameObject find(GameObject parent, string obj) {
        return parent.transform.Find(obj).gameObject;
    }
    public static GameObject find (string obj){
		return objectFinder.Find(obj).gameObject;
	}

    public static Text text(Transform t) {
        return get<Text>(t);
    }
    public static Text text(GameObject obj) {
        return get<Text>(obj);
    }
    public static Button button (Transform t){
		return get<Button>(t);
	}
    public static Button button(GameObject obj) {
        return get<Button>(obj);
    }

    public static string time2Str(TimeSpan span) {
        return String.Format("{0:00}:{1:00}", Math.Floor(span.TotalMinutes), span.Seconds);
        /*
        string str = Regex.Replace(span.ToString(), @"\.\d+$", string.Empty);
        return Regex.Replace(str, @"^\d+:", string.Empty);*/
    }
}
