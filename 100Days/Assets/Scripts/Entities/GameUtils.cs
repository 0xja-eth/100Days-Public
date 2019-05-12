using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class GameUtils {
	static Transform objectFinder;
    static Transform camera;
    static Transform uiLayer;
    static Transform alertWindow;
    static Transform loadingScene;

    static CameraControl cameraControl;
    static UIBaseLayer uiBaseLayer;
    static AlertLayer alertLayer;
    static LoadingLayer loadingLayer;

    static Texture2D[] texturePool = new Texture2D[0];

    public const string spaceIdentifier = "&S&";
    public const string spaceEncode = "\u00A0";

    public static void initialize(string uiLayerPath = null, 
        string alertWindowPath = null, string loadingScenePath = null) {
		objectFinder = GameObject.Find("ObjectFinder").transform;
        if (objectFinder) camera = find<Transform>("MainCamera");
        if (uiLayerPath != null && objectFinder)
            uiLayer = find<Transform>(uiLayerPath);
        if (alertWindowPath != null && objectFinder)
            alertWindow = find<Transform>(alertWindowPath);
        if (loadingScenePath != null && objectFinder)
            loadingScene = find<Transform>(loadingScenePath);
        if (camera) cameraControl = get<CameraControl>(camera);
        if (uiLayer) uiBaseLayer = get<UIBaseLayer>(uiLayer);
        if (alertWindow) alertLayer = get<AlertLayer>(alertWindow);
        if (loadingScene) loadingLayer = get<LoadingLayer>(loadingScene);
    }

    public static Transform getCamera() { return camera; }
    public static Transform getUILayer() { return uiLayer; }
    public static Transform getAlertWindow() { return alertWindow; }
    public static Transform getLoadingScene() { return loadingScene; }

    public static CameraControl getCameraControl(){ return cameraControl; }
    public static UIBaseLayer getUIBaseLayer() { return uiBaseLayer; }
    public static AlertLayer getAlertLayer() { return alertLayer; }
    public static LoadingLayer getLoadingLayer() { return loadingLayer; }

    public static void setTexturePool(Texture2D[] textures) { texturePool = textures; }
    public static Texture2D[] getTexturePool() { return texturePool; }

    public static void alert(string msg, string[] btns = null, UnityAction[] actions = null) {
        if(alertLayer) alertLayer.setup(msg, btns, actions);
    }

    public static void startLoadingScreen(string tips="") {
        if (loadingLayer) loadingLayer.setup(tips);
    }
    public static void endLoadingScreen() {
        if (loadingLayer) loadingLayer.end();
    }
    public static void setLoadingProgress(float rate) {
        if (loadingLayer) loadingLayer.setProgress(rate);
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

    public static Vector2 rectToSize(Rect r) {
        return new Vector2(r.width, r.height);
    }

    public static void setRectWidth(RectTransform rt, float w) {
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
    }
    public static void setRectHeight(RectTransform rt, float h) {
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
    }
    public static string time2Str(TimeSpan span) {
        return String.Format("{0:00}:{1:00}", Math.Floor(span.TotalMinutes), span.Seconds);
    }
    public static string time2StrWithHour(TimeSpan span) {
        return String.Format("{0:00}:{1:00}:{2:00}", Math.Floor(span.TotalHours), span.Minutes, span.Seconds);
    }
    public static string adjustText(string text) {
        text = Regex.Replace(text, @"(?<=<.*?) (?=.*?>)", spaceIdentifier);
        text = text.Replace(" ", spaceEncode);
        text = text.Replace(spaceIdentifier, " ");
        return text.ToString();
    }
}
