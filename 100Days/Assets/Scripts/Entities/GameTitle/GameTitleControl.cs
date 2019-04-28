using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTitleControl : MonoBehaviour {
    
    const int WindowWidth = 1248;
    const int WindowHeight = 702;

    // Use this for initialization
    void Awake () {
        Screen.SetResolution(WindowWidth, WindowHeight, false);
        GameSystem.initialize();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void enterGame() {
        if (StorageSystem.hasSaveFile(0))
            GameSystem.continueGame(0);
        else GameSystem.newGame(0);
    }

    public void exitGame() {
        Application.Quit();
    }
}
