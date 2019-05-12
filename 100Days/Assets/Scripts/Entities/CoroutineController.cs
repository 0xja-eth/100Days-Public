using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineController : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        // 检查RequestList
        updateNetworkRequests();
    }

    void updateNetworkRequests() {
        if (NetworkSystem.hasRequestObject()) {
            RequestObject req = NetworkSystem.popRequestObject();
            createCoroutine(NetworkSystem.requestProcessThread(req));
        }
    }

    public void createCoroutine(IEnumerator func) {
        StartCoroutine(func);
    }
}
