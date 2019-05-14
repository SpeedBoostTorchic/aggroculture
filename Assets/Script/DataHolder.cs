using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class DataHolder : MonoBehaviour {

    public static DataHolder dh;

    //These are the rewired controllers that the player handles
    public Player p1;
    public Player p2;

    //This represents the control format the player has selected
    public bool p1Pad;
    public bool p2Pad;

    //These represent the characters the player have selected
    public int p1Num;
    public int p2Num;



    //Data holder persists between scenes
	void Awake () {

        //Singleton pattern for DataHolder
        if(dh == null)
        {
            dh = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
       
	}
}
