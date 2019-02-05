using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerTutorialManager : GameManager {

    [Header("Tutorial Specific Variables")]
    public bool skipStart;
    
    public TutorialPlayer p;

	// Use this for initialization
	void Start () {
        if (skipStart)
        {
            gameStart = true;
            begin = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (skipStart)
        {
            twoPlayerTimer();
        }
	}
}
