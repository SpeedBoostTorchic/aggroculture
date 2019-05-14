using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeText : MonoBehaviour {

    public PlayerClass parent;

    //Component Objects are held here:
    public GameObject self;
    public Text text;
    public Text text2;
    public Image worker;
    public Sprite empty;
    public Sprite assigned;

    //Text displays
    public Text wLevel;
    public Text fLevel;
    

    public GameObject timerObj;
    public Image timerIm;
    public Text timerText;

    // Use this for initialization
    void Start () {
        timerObj.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        text2.text = text.text;

        //If the player has selected a tile, and it is a farm...
        if (parent.selectedTile != null && parent.selectedTile.tileNum == 2)
        {
            wLevel.text = "Water x" + (parent.selectedTile.nearWater + parent.selectedTile.nearFarm/2);
            fLevel.text = "LvL " + parent.selectedTile.farmLevel;
        }
        else
        {
            wLevel.text = "";
            fLevel.text = "";
        }

        //if the player has selected a tile, it has a worker, and it is not a farm...
        if((parent.selectedTile != null  && parent.selectedTile.workerAssigned != 0)
            || parent.selectedTile != null && parent.selectedTile.tileNum == 2 && parent.selectedTile.workerAssigned == 0)
        {
            //Activates the radial image, and sets fill amount to match
            MapTile t = parent.selectedTile;

            //If the workers on the tile have not reached their final tile...
            //Or if the tile is a farm tile not controlled by the opposite player
            if(!(t.workerAssigned == 2 && t.tileNum == 1) && 
                !(t.workerAssigned == 1 && (t.tileNum == 0 || t.tileNum == 5 || t.tileNum == 6 || t.tileNum == 4))
                ||(t.tileNum == 2 && (t.controllingPlayer == parent.pNum || t.workerAssigned == 0)))
            {
                timerObj.SetActive(true);
                

                //The timer text display is different if the tile is a growing farm
                if (t.tileNum == 2 && t.cropLevel < 3 && t.workerAssigned != 2)
                {
                    float timerTemp = 0;
                    if((t.nearWater > 0 || t.nearFarm > 0) && (t.cropLevel < 3 || (t.cropLevel == 3 && t.workerAssigned > 0)))
                    {
                        //What maxTime looks like with the current setup
                        timerTemp = t.timerMax/t.timerRate;

                        //If the farm is still growing, the following formula determines correct amount of time remaining past all crop levels
                        if(t.cropLevel < 3)
                        {
                            timerIm.fillAmount = (1f - t.timer / t.timerMax) * 0.334f + 0.333f * (t.cropLevel);
                        }

                        //Otherwise, defaults back to normal timer formula
                        else
                        {
                            timerIm.fillAmount = 1f - (t.timer / t.timerMax);
                        }


                        timerText.text = "" + (int)(Mathf.Abs(t.timer / t.timerRate + timerTemp * (2 - t.cropLevel)) * Time.deltaTime + 1);
                    }
                    //Displays blank if the farm isn't growing
                    else
                    {
                        timerText.text = "-";
                        timerIm.fillAmount = 0f;
                    }
                    
                }
                else
                {
                    //Standard for non-farm tiles
                    if(t.tileNum != 2)
                    {
                        //timerText.text = "" + (int)(t.timer + 1);
                        timerText.text = "" + (int)(t.timer * (Time.deltaTime/ t.timerRate) + 1);
                        timerIm.fillAmount = 1f - (t.timer / t.timerMax);
                    }
                    else
                    {
                        timerText.text = "-";
                        timerIm.fillAmount = 0f;
                    }

                }
                
            }
            else
            {
                timerObj.SetActive(false);
                timerText.text = "";
            }
        }
        else
        {
            timerObj.SetActive(false);
        }


	}
}
