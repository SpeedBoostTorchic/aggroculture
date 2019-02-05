using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionWheel : MonoBehaviour {

    private bool lTrigUse;
    private bool rTrigUse;

    //Used to determine selection
    [Header("Position Information")]
    public float baseX;
    public float baseY;
    public float threshold;
    public int selectNum;
    public bool lClick;
    public MapTile curTile;
    public PlayerClass p;    //Which player this wheel is assigned to


    //Quarter circles
    [Header("UI Images")]
    public Image upper;
    public Image left;
    public Image right;
    public GameObject upper2;
    public GameObject left2;
    public GameObject right2;

    //Worker icons
    public Image upIm;
    public Image leftIm;
    public Image rightIm;

    [Header("Left-Click Sprites")]
    public Sprite removebw;
    public Sprite digbw;
    public Sprite waterbw;
    public Sprite removeColor;
    public Sprite digColor;
    public Sprite waterColor;

    [Header("Right-Click Sprites")]
    public Sprite seedbw;
    public Sprite firebw;
    public Sprite hirebw;
    public Sprite seedColor;
    public Sprite fireColor;
    public Sprite hireColor;


    //Other
    [Header("Visuals")]
    public Text text;
    public Text text2;
    //public Color upperColor;
    public Color leftColor;
    public Color rightColor;
    public Color baseColor;
    public Color disableColor;
    public bool upCan;
    public bool leftCan;
    public bool rightCan;

	// Initialize baseline colors/sprites
	void Start () {
        upper.color = baseColor;
        left.color = baseColor;
        right.color = baseColor;
        text.text = "Worker Wheel";
	}
	
	// Update is called once per frame
	void Update () {



        //Allows same action to be performed multiple times in succession
       /* if (lClick)
        {

            //Right-Clicking while holding left excutes selected action.
            if (Input.GetMouseButtonDown(1) && !p.pad)
            {
                doSelect();
            }
            if((Input.GetAxisRaw("Fire2" + p.pNum) != 0) && p.pad && !rTrigUse)
            {
                rTrigUse = true;
                doSelect();
            }
            else if(Input.GetAxis("Fire2" + p.pNum) == 0)
            {
                rTrigUse = false;
            }
        }
        else
        {
            //Vice-Versa for left-clicking while right button is held down
            if (Input.GetMouseButtonDown(0) && !p.pad)
            {
                doSelect();
            }
            if (Input.GetAxis("Fire1" + p.pNum) > 0.5f && p.pad && !lTrigUse)
            {
                lTrigUse = true;
                doSelect();
            }
            else if (Input.GetAxis("Fire1" + p.pNum) == 0) 
            {
                lTrigUse = false;
            }
        }*/

        text2.text = text.text;
        if (p.holdingMenu)
        {
            //Player selecting "Remove Worker"/"Buy Seed"
            if ((p.player.GetAxis("Vertical") < 0) && (upCan))
            {
                if (lClick) //"Remove Worker"
                {
                    selectNum = 1;
                    upIm.sprite = removeColor;
                    text.text = "Remove Worker";
                    upper.color = leftColor;
                }
                else //"Buy Seed"
                {
                    selectNum = 4;
                    upIm.sprite = seedColor;
                    text.text = "Buy Seed";
                    upper.color = rightColor;
                }
                
                resetLeft();
                resetRight();
                upper2.SetActive(true);

            }
            //Player moves the mouse down....
            else if ((p.player.GetAxis("Vertical") > 0))
            {
                //...Player has selected "Set Dig"
                if ((p.player.GetAxis("Horizontal") < 0) && (leftCan))
                {
                    if (lClick) //"Set Dig"
                    {
                        selectNum = 2;
                        leftIm.sprite = digColor;
                        text.text = "Assign Digger";
                        left.color = leftColor;
                    }
                    else //"Fire Worker"
                    {
                        selectNum = 5;
                        leftIm.sprite = fireColor;
                        text.text = "Remove All Diggers";
                        left.color = rightColor;
                    }

                   
                    left2.SetActive(true);
                    resetUpper();
                    resetRight();

                }
                //...Player has selected "Set Water"
                else if ((p.mousePos.x > baseX + (threshold)) && rightCan)
                {

                    if (lClick) //"Set Water"
                    {
                        selectNum = 3;
                        rightIm.sprite = waterColor;
                        text.text = "Assign Planter";
                        right.color = leftColor;
                    }
                    else //"Hire Worker"
                    {
                        selectNum = 6;
                        rightIm.sprite = hireColor;
                        text.text = "Remove All Planters";
                        right.color = rightColor;
                    }

                    
                    resetUpper();
                    resetLeft();
                    right2.SetActive(true);

                }
            }
            else
            {
                selectNum = 0;
                resetUpper();
                resetLeft();
                resetRight();
                if (lClick)
                {
                    setColorAll(false);
                    text.text = "Worker Wheel";
                }
                else
                {
                    setColorAll(true);
                    text.text = "Purchase Wheel";
                }

            }
        }
	}
    //------------------------------------------------
    //Below methods reset each of the selection icons
    //------------------------------------------------
    public void resetUpper()
    {
        if (lClick)
        {
            upIm.sprite = removebw;
            setColorUp(false);
        }
        else
        {
            upIm.sprite = seedbw;
            setColorUp(true);
        }
        //upper.color = baseColor;
        upper2.SetActive(false);
    }
    public void resetLeft()
    {
        if (lClick)
        {
            leftIm.sprite = digbw;
            setColorLeft(false);
        }
        else
        {
            leftIm.sprite = firebw;
            setColorLeft(true);
        }
        //left.color = baseColor;
        left2.SetActive(false);
    }
    public void resetRight()
    {
        if (lClick)
        {
            rightIm.sprite = waterbw;
            setColorRight(false);
        }
        else
        {
            rightIm.sprite = hirebw;
            setColorRight(true);
        }
        
        right2.SetActive(false);
    }
    //------------------------------------------------
    //Does the thing that was selected
    /*public void doSelect()
    {
       //Debug.Log("Do Called: " + selectNum);
       //Do nothing
       if(selectNum == 0)
        {
            return;
        }
       //Remove Worker
       else if(selectNum == 1)
        {
            //If the player controlling the tile is the same, and the tile has a worker attatched
            if((p.selectedTile.workerAssigned > 0) && (p.pNum == p.selectedTile.controllingPlayer))
            {
                p.selectedTile.workerAssigned = 0;
                p.assignedWorkers -= 1;
                p.selectedTile.growing = false;
            }else if((p.selectedTile.opposingWorker > 0) && (p.pNum != p.selectedTile.controllingPlayer))
            {
                p.selectedTile.opposingWorker = 0;
                p.assignedWorkers -= 1;
            }
            p.checkIncome();
        }
       //Assign Digger
        else if(selectNum == 2)
        {
            //Assigns a worker to the tile, and places it under this players control
            if ((p.selectedTile.workerAssigned == 0) && ((p.workers-p.assignedWorkers )> 0))
            {
                p.selectedTile.workerAssigned = 1;
                p.selectedTile.controllingPlayer = p.pNum;
                p.assignedWorkers += 1;
                GameManager.gm.checkAdjacent((int)p.selectedTile.gridPos.x, (int)p.selectedTile.gridPos.y, p.pNum);
            }           
            //If the player already controls the tile, and it has a planter, switches planter to digger
            else if ((p.selectedTile.workerAssigned == 2) && (p.selectedTile.controllingPlayer == p.pNum))
            {
                p.selectedTile.workerAssigned = 1;
                if (p.selectedTile.tileNum == 2)
                {
                    p.selectedTile.growing = false;
                }
            }
            //If the tile is controlled by an enemy, assign worker to mess with them
            else if((p.selectedTile.opposingWorker == 0) && (p.pNum != p.selectedTile.controllingPlayer))
            {
                p.selectedTile.opposingWorker = 1;
                p.assignedWorkers += 1;

                GameManager.gm.spawnWarning(p.selectedTile.controllingPlayer, p.selectedTile.gameObject.transform);
            }
        }
       //Assign Planter
       else if(selectNum == 3)
        {
            if ((p.selectedTile.workerAssigned == 0) && ((p.workers - p.assignedWorkers) > 0))
            {
                p.selectedTile.workerAssigned = 2;
                p.selectedTile.controllingPlayer = p.pNum;
                p.assignedWorkers += 1;
                if(p.selectedTile.tileNum == 2)
                {
                    p.selectedTile.growing = true;
                }
            }
            //If the player already controls the tile, and it has a digger, switches digger to planter
            else if((p.selectedTile.workerAssigned == 1) && (p.selectedTile.controllingPlayer == p.pNum))
            {
                p.selectedTile.workerAssigned = 2;
                if (p.selectedTile.tileNum == 2)
                {
                    p.selectedTile.growing = true;
                }
            }
            //If the tile is controlled by an enemy, assign worker to mess with them
            else if ((p.selectedTile.opposingWorker == 0) && (p.pNum != p.selectedTile.controllingPlayer))
            {
                p.selectedTile.opposingWorker = 2;
                p.assignedWorkers += 1;

                GameManager.gm.spawnWarning(p.selectedTile.controllingPlayer, p.selectedTile.gameObject.transform);
            }
            p.checkIncome();
        }
       //Buy Seeds
        else if(selectNum == 4)
        {
            if ((p.money > 100))
            {
                p.money -= 100;
            }
        }
       //Fire Worker / Plant Seed(?)
        else if(selectNum == 5)
        {
            if(p.workers-p.assignedWorkers > 0)
            {
                p.workers -= 1;
            }
        }
       //Hire Worker
       else if(selectNum == 6)
        {
            p.workers += 1;
        }
        selectNum = 0;
    }*/

    //Checks if the player can perform the actions on the left wheel
    private void checkLeftCan()
    {
        //Checks if the player has workers to fire
        if(p.assignedWorkers > 0)
        {
            upCan = true;
        }
        else
        {
            upCan = false;
        }

        //Checks if the player can assign a worker to the tile
        if(p.workers - p.assignedWorkers > 0 && GameManager.gm.gameStart)
        {
            leftCan = true;
            rightCan = true;
        }
        else
        {
            leftCan = false;
            rightCan = false;
        }
    }

    //CHecks if the player can perform the actions on the right wheel
    private void checkRightCan()
    {
        //Checks if the player has enough money to buy seeds
        if(p.money > 100 && GameManager.gm.gameStart)
        {
            upCan = true;
        }
        else
        {
            upCan = false;
        }

        //Checks if players has workers to fire
        if (p.workQueue.Count > 0)
        {
            leftCan = true;
        }
        else 
        {
            leftCan = false;
        }

        //Always possible to hire more workers unless game not started
        if(GameManager.gm.gameStart)
        {
            rightCan = true;
        }
        else
        {
            rightCan = false;
        }
        
    }

    //Sets colors of the wheel
    private void setColorAll(bool r)
    {
        setColorUp(r);
        setColorLeft(r);
        setColorRight(r);
    }

    private void setColorUp(bool r)
    {
        //First determines side
        if (r)
        {
            checkRightCan();
        }
        else
        {
            checkLeftCan();
        }
        if (upCan)
        {
            upper.color = baseColor;
        }
        else
        {
            upper.color = disableColor;
        }
    }

    private void setColorLeft(bool r)
    {
        //First determines side
        if (r)
        {
            checkRightCan();
        }
        else
        {
            checkLeftCan();
        }
        if (leftCan)
        {
            left.color = baseColor;
        }
        else
        {
            left.color = disableColor;
        }
    }

    private void setColorRight(bool r)
    {
        //First determines side
        if (r)
        {
            checkRightCan();
        }
        else
        {
            checkLeftCan();
        }
        if (rightCan)
        {
            right.color = baseColor;
        }
        else
        {
            right.color = disableColor;
        }
    }
}
