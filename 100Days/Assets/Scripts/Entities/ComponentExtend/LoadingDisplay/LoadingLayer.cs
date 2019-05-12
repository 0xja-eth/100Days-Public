using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Events;

public class LoadingLayer : AnimatableLayer {


    public Transform bar;
    public Text progress, tipText;
    public GameObject textArea;
    
    /*
    public void setupRequestLoader(UnityWebRequest webRequest, string tips = "",
        RequestObject.SuccessAction successAction = null, 
        RequestObject.ErrorAction errorAction = null) {
        tipText.text = tips;
        gameObject.SetActive(true);
        StartCoroutine(webRequestLoading(
            webRequest, successAction, errorAction));
    }
    /*
    IEnumerator webRequestLoading(UnityWebRequest webRequest,
        RequestObject.SuccessAction successAction = null, 
        RequestObject.ErrorAction errorAction = null) {
        *
        WWWForm form = new WWWForm();

        string route = "/question/query/all";

        Debug.Log("test");
        Debug.Log(form);

        UnityWebRequest webRequest = UnityWebRequest.Post(DataSystem.ServerURL + route, form);
        *
        successAction = successAction ?? ((data) => { Debug.Log(data); });
        errorAction = errorAction ?? ((error) => { Debug.Log(error); });

        webRequest.SendWebRequest();

        while (!webRequest.isDone) {
            setProgress(webRequest.downloadProgress);
            Debug.Log(webRequest.downloadProgress);
            yield return 1;
        }
        if (webRequest.isDone) setProgress(1);
        if (webRequest.isHttpError || webRequest.isNetworkError)
            errorAction.Invoke(webRequest.error);
        else
            successAction.Invoke(webRequest.downloadHandler.text);
        gameObject.SetActive(false);
    }
    */
    public void setup(string tips = "") {
        gameObject.SetActive(true);
        textArea.SetActive(tips.Length > 0);
        tipText.text = tips;
    }

    public void end() {
        gameObject.SetActive(false);
    }

    public void setProgress(float rate) {
        bar.localScale = new Vector3(rate, 0.9f, 1);
        progress.text = Mathf.RoundToInt(100 * rate).ToString()+'%';
        Debug.Log(progress.text);
    }

}
