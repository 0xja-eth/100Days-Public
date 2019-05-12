using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class RespondJsonData {
    protected string json;
    public int status;
    public string errmsg;
    /*
    public RespondJsonData(RespondJsonData data) {
        this.json = data.json;
        this.status = data.status;
        this.errmsg = data.errmsg;
    }*/
    public void setJson(string json) {
        this.json = json;
    }
    public string getJson() {
        return json;
    }
}

public class RequestObject {
    public delegate void SuccessAction(RespondJsonData data);
    public delegate void ErrorAction(RespondStatus status, string errmsg);

    public UnityWebRequest webRequest;
    public SuccessAction onSuccess;
    public ErrorAction onError;

    public bool showLoading;
    public string tipsText;

    public RequestObject(UnityWebRequest request, 
        SuccessAction success = null, ErrorAction error = null, 
        bool show = true, string tips = "") {
        webRequest = request; onSuccess = success; onError = error;
        showLoading = show; tipsText = tips;
    }
}

public enum RespondStatus {
    // Http
    HttpError = -1,

    // Common
    Success = 0,  // 成功，无错误
	InvalidRequest = 1,  // 非法的请求方法
	ParameterError = 2,  // 参数错误

	// Savefile
	FileNotFound = 10, // 文件未找到
	PlayerDisMatch = 11, // 玩家不匹配
	SavefileError = 12, // 存档错误
	NoSavefileInfo = 13, // 无存档信息

	// Player
	PlayerNotExist = 20, // 玩家不存在
	PlayerExist = 21, // 玩家已存在
	SchoolNotExist = 22 // 学校不存在
}

public static class NetworkSystem {

    public const string ServerURL = "http://152.136.134.132:8001";
    public const string QueryIdsRoute = "/question/query/ids";
    public const string QuertCountRoute = "/question/query/count";
    public const string ExamRoute = "/question/generate/exam";
    public const string ExerciseRoute = "/question/generate/all";
    public const string NewGameRoute = "/player/player/create";
    public const string SavefileRoute = "/player/player/save";
    public const string DeleteRoute = "/player/player/delete";
    public const string GetSchoolRoute = "/player/school/get";

    static readonly RequestObject.SuccessAction DefaultSuccessHandler =
        (data) => { Debug.Log(data.getJson()); };
    static readonly RequestObject.ErrorAction DefaultErrorHandler =
        (status, errmsg) => { Debug.LogError(status + ": " + errmsg); };

    static RequestObject.SuccessAction successHandler;
    static RequestObject.ErrorAction errorHandler;
    static bool showLoading = true;
    static string tipsText = "";

    static Queue<RequestObject> requestList;

    static bool initialized = false;

    public static void setSuccessHandler(RequestObject.SuccessAction h) {
        successHandler = h;
    }
    public static void setErrorHandler(RequestObject.ErrorAction h) {
        errorHandler = h;
    }
    public static void setShowLoading(bool value) {
        showLoading = value;
    }
    public static void setTipsText(string text) {
        tipsText = text;
    }

    public static bool hasRequestObject() {
        return requestList.Count > 0;
    }
    public static RequestObject getRequestObject() {
        return requestList.Peek();
    }
    public static RequestObject popRequestObject() {
        return requestList.Dequeue();
    }
    public static void pushRequestObject(RequestObject req) {
        requestList.Enqueue(req);
    }
    public static void pushRequestObject(UnityWebRequest request,
        RequestObject.SuccessAction success = null,
        RequestObject.ErrorAction error = null, 
        bool show = true, string tips = "") {
        pushRequestObject(new RequestObject(request, success, error, show, tips));
    }

    public static void initialize() {
        if (initialized) return;
        initialized = true;
        requestList = new Queue<RequestObject>();
        clear();
    }

    public static void setup(
        RequestObject.SuccessAction success = null,
        RequestObject.ErrorAction error = null,
        bool show = true, string tips = "") {
        setSuccessHandler(success);
        setErrorHandler(error);
        setShowLoading(show);
        setTipsText(tips);
    }
    public static void clear(
        RequestObject.SuccessAction success = null,
        RequestObject.ErrorAction error = null,
        bool show = true, string tips = "") {
        setSuccessHandler(null);
        setErrorHandler(null);
        setShowLoading(true);
        setTipsText("");
    }

    public static void setupRequest(string route, WWWForm form=null,
        RequestObject.SuccessAction success = null,
        RequestObject.ErrorAction error = null,
        bool show = true, string tips = "") {
        setup(success, error, show, tips);
        postRequest(route, form);
    }
    public static void postRequest(string route, WWWForm form=null) {
        Debug.Log("PostRequest:" + route);
        form = form ?? new WWWForm();
        UnityWebRequest webRequest = UnityWebRequest.Post(ServerURL + route, form);

        pushRequestObject(webRequest, successHandler, errorHandler, showLoading, tipsText);
    }

    public static IEnumerator requestProcessThread(RequestObject req) {
        RequestObject.SuccessAction successAction = req.onSuccess ?? DefaultSuccessHandler;
        RequestObject.ErrorAction errorAction = req.onError ?? DefaultErrorHandler;

        UnityWebRequest webRequest = req.webRequest;

        webRequest.SendWebRequest();

        if (req.showLoading) GameUtils.startLoadingScreen(req.tipsText);

        while (!webRequest.isDone) {
            if (req.showLoading) GameUtils.setLoadingProgress(webRequest.downloadProgress);
            Debug.Log(webRequest.downloadProgress);
            yield return 1;
        }
        if (webRequest.isDone && req.showLoading) GameUtils.setLoadingProgress(1);
        if (webRequest.isHttpError || webRequest.isNetworkError)
            errorAction.Invoke(RespondStatus.HttpError, webRequest.error);
        else {
            string text = webRequest.downloadHandler.text;
            RespondJsonData data = JsonUtility.FromJson<RespondJsonData>(text);
            data.setJson(text);
            if(data.status == 0) successAction.Invoke(data);
            else errorAction.Invoke((RespondStatus)data.status, data.errmsg);
        }

        if (req.showLoading) GameUtils.endLoadingScreen();
    }
}
