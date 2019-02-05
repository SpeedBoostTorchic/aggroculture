using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlayer : PlayerClass {

    [Header("Tutorial Player Specific")]
    public bool active;


	// Use this for initialization
	void Start () {
        
	}

    // Update is called once per frame
    void Update () {

        //Allows the player to control, but only if the player is listed as active
        if (active)
        {
            if (base.pad)
            {
                base.padControl();
            }
            else
            {
                base.mouseControl();
            }
            
        }
	}
}
