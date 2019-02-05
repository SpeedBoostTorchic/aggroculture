using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class winTrackerUI : MonoBehaviour {

    //The player this class is tracking
    public PlayerClass p;
    private int curV;

    //The "Win Diamonds"
    public Image d1;
    public Image d2;
    public Image d3;

    //The sprites to use
    public Sprite neutral;
    public Sprite win;


	// Update is called once per frame
	void Update () {

        //Doesn't run until the round is over
        if (GameManager.gm.roundFinish)
        {
            //Tracks the number of victories the player has accrued; 
            if (p.pNum == 1)
            {
                curV = GameManager.gm.p1V;
            }
            else
            {
                curV = GameManager.gm.p2V;
            }
        }
        
        //Changes the relevant sprites at a certain number of victories
        switch (curV)
        {
            case 3:
                d3.sprite = win;
                break;
            case 2:
                d2.sprite = win;
                break;
            case 1:
                d1.sprite = win;
                break;

            default:
                break;
        }
        
	}
}
