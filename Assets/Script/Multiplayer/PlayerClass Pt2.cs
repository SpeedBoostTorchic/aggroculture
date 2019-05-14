using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The second part of player class controls the AI Behavior
public partial class PlayerClass : MonoBehaviour
{
    //If true, skips over all the player stuffs
    [Header("AI High-Level Stuff")]
    public bool aiControlled;
    
    //Determines AI behavior types
    [Range(1,5)]
    public int difficulty;
    public int searchRange;
    public int state;           //0 - Search, 1 - Move TO DEST, 

    public float stateCycleRate;        //How often do I check if I should switch?
    public float stateSwitchRate;       //What are the chances I will switch?
    private float stateTimer;
    private int nextState;              //Which one would I switch to?
    private bool breakState;

    [Header("Movement & Pathfinding")]
    public float moveDelay;
    private float moveDelayTimer;
    public MapTile destination;
    public Vector2 curPos;
    

	
	// Update is called once per frame
	public void aiControl () {
		
        if(state == 1)
        {
            state1();
        }
	}

    //State 1: Movement
    public void state1()
    {
        moveDelayTimer -= Time.deltaTime;
        

        //Determines relative distance to destination
        int xDiff = (int)destination.gridPos.x - (int)curPos.x;
        int yDiff = (int)destination.gridPos.y - (int)curPos.y;

        //When the itmer hits zero, picks a direction to move
        if (moveDelayTimer <= 0 && destination != selectedTile)
        {
            print(""+xDiff + "," + yDiff);

            //If Horizontal distance is greater...
            if (Mathf.Abs(xDiff) > Mathf.Abs(yDiff))
            {
                //Move Left
                if(xDiff > 0)
                {
                    print("attempting to move Left");


                        findNextTile(4);
                        snapped = true;

                }
                //Move Right
                else if(xDiff < 0)
                {
                    print("attempting to move Right");

                    /*/If the tile is free and clear
                    if ((GameManager.gm.curMap[(int)curPos.x + 1, (int)curPos.y] != null) &&
                            (GameManager.gm.curMap[(int)curPos.x + 1, (int)curPos.y].selected == 0))
                    {*/

                    findNextTile(3);

                    /*}
                    else
                    {
                        could = false;
                    }*/
                }
            }
            //If Vertical distance is greater...
            else
            {
                //Move Down
                
                if (yDiff > 0)
                {

                    findNextTile(2);
                    snapped = true;
                }
                //Move Down?
                

                else if (yDiff < 0)
                {

                    findNextTile(1);
                    snapped = true;
                }
            }

            //Sets new position
            curPos = selectedTile.gridPos;

            //Resets the move delay timer
            moveDelayTimer = moveDelay;
        }
    }


    private void selectTile(MapTile m)
    {
        selectedTile.ahead = false;
        selectedTile.selected = 0;
        selectedTile = m;
        selectedTile.ahead = true;
        selectedTile.selected = pNum;
        snapped = true;
    }
}
