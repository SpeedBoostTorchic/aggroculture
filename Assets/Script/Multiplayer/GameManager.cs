using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    //SIngleton
    public static GameManager gm;

    //Prefab tiles go here:
    [Header("Prefabs")]
    public GameObject baseTile;

    //Player mouse & controls
    [Header("Players")]
    public PlayerClass p1;
    public PlayerClass p2;
    public bool moveLocked;
    public bool gameStart;
    public bool roundFinish;
    public bool gameFinish;
    public int roundLength;

    //UI and Visuals
    [Header("UI/VIsuals")]
    public GameObject pointer;
    public Vector3 ls;
    public ParticleSystem cursor;
    public ParticleSystem cursorClick;
    public TypeText tileIndicator;
    public GameObject wheelObject;
    public SelectionWheel wheelCode;
    public Vector3 wls;
    public float targetOffset;
    public float curOffset;
    public Text timer;
    public Text winMes;
    public GameObject panel;
    public GameObject screenDivider;

    public GameObject winRatio;
    public Text p1Ratio;
    public Text p2Ratio;

    public GameObject tutorialImage;

    //Used for the countdown start
    [Header("Start/Timer Related Variables")]
    public GameObject countdown;
    protected Text cd;
    public float cdTargetSize;
    protected float cdCurSize;
    protected float cdTimer;
    protected int count;
    protected bool begin;
    public float seconds;
    public Text introMes;
    public float preCountDown;
    public Text p1Mes;
    public Text p2Mes;
    public GameObject p1Canvas;
    public GameObject p2Canvas;
    public bool bothReady;

    //For the flashy, countdown at start
    public GameObject countdownWhole;
    public Text countdownTimer;
    public Text roundMes;

    //Used for the water pathfinding algorithm
    [Header("BFS Variables")]
    public List<MapTile> searchThese = new List<MapTile>();     //A list of all the maptiles needed to be searched
    public List<MapTile> searchFarms= new List<MapTile>();
    public List<MapTile> springTiles = new List<MapTile>();     //Starting locations for the BFS


    //Other variables:
    [Header("Other")]
    public int mapSize = 0;
    public int numSpring = 0;
    public int numRock = 0;
    public int numHole = 0;
    public int[,] tileMap;
    public MapTile[,] curMap;
    public MapTile selectedTile;
    public GameObject warning;
    public int weather;         // 0-Normal, 1-Rain, 2-Sun

    [Header("Multi-Round Variables")]
    //Used to track a winner
    public int p1V = 0;
    public int p2V = 0;
    protected bool vAdded;
    public int numGames;
    public int maxNumGames;
    public float roundEndTimer;
    public Text roundText;

    //Instantiates the tile map, and sets primary gameplay variables
    void Awake () {

        roundEndTimer = 5f;
        //Sets the timer
        /*minutes = 2;
        seconds = 0;*/
        
        //Singleton
        gm = this;

        //Initializes the Map
        tileMap = new int[mapSize, mapSize];
        curMap = new MapTile[mapSize, mapSize];
        setMap();
        drawMap();
        findWaterOrigins();
        searchShell();

        //Hides the pre-round countdown assets
        countdownWhole.SetActive(false);
        winRatio.SetActive(false);
        roundMes.text = "";

        //These commands initialize necissary Game Objects & Variables
        pointer.SetActive(false);
        ls = pointer.transform.localScale;
        wls = wheelObject.transform.localScale;
        cursor.Play();
        cursorClick.Stop();
        Cursor.visible = false;
        wheelObject.SetActive(false);
        cd = countdown.GetComponent<Text>();
        count = 3;
        countdown.SetActive(false);

        p1Canvas.SetActive(false);
        p2Canvas.SetActive(false);
        begin = true;

        //Positions the second player;
        p2.posX = 5.6f * mapSize;
        p2.pCam.transform.position = new Vector3(p2.posX, 0f, 0f);
    }


    // Update is called once per frame
    void Update () {

        //Begins the countdown
        if (begin)
        {
            if(numGames == 0)
            {
                countdownStart();
            }
            else
            {
                countdownStart("ROUND " + (numGames));
            }
            
        }
        
        twoPlayerTimer();

        if (Input.GetKeyDown(KeyCode.O))
        {
            searchShell();
        }
    }

    /*------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        MAP SETUP RELATED FUNCTIONS HELD HERE:
            * setMap()
            * drawMap()
            * clearMap()
            * ranTile();
            * checkInitialWater();
    -------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    //Randomly generates the map
    public void setMap()
    {
        //Sets all map tiles to grass
        for(int i = 0; i < mapSize; i++)
        {
            for(int j = 0; j < mapSize; j++)
            {
                tileMap[i,j] = 1;
            }
        }

        //Sets the spring tiles
        springsAtCorners();
        //Randomly sets x tiles to hole
        ranTile(numHole, 0);
        //Randomly sets x tiles to Rock
        ranTile(numRock, 3);
    }

    //Instantiates the maps initial tiles
    public void drawMap()
    {
        for (int i = 0; i < mapSize; i++)
        {
            //Draws each tile in sequence, then sets it to the corrosponding array position
            for (int j = 0; j < mapSize; j++)
            {
                curMap[i,j] = Instantiate(baseTile, new Vector3((3.2f*i)+ (3.2f * j), (-1.6f*j)+(1.6f*i),(j*-0.2f)+(i*2f)), Quaternion.identity).gameObject.GetComponent<MapTile>();
                curMap[i,j].tileNum = tileMap[i,j];
                curMap[i,j].gridPos = new Vector2(i, j);

               /* //Sets which tiles are on the edge of the map
                if(i == 0 || i == mapSize-1 || j == 0 || j == mapSize - 1)
                {
                    curMap[i, j].edge = true;
                }*/
            }
        }
    }

    //Used by setMap() to replace the requried number of given tiles
    public void ranTile(int n, int t)
    {

        //Randomly sets n tiles to t
        for (int i = 0; i < n; i++)
        {
            while (true)
            {
                int x = Random.Range(0, mapSize);
                int y = Random.Range(0, mapSize);

                //if tile is grass, set it to t. Else, find new tile
                if (tileMap[x, y] == 1)
                {
                    tileMap[x, y] = t;
                    break;
                }
            }
        }
    }

    //Clears the map of pre-start changes
    public void clearMap()
    {
        //Iterates over the map
        for(int i = 0; i < mapSize; i++)
        {
            for(int j = 0; j < mapSize; j++)
            {
                //Resets each tile
                curMap[i,j].controllingPlayer = 0;
                curMap[i, j].workerAssigned = 0;
                curMap[i, j].opposingWorker = 0;
                curMap[i, j].tileNum = tileMap[i, j];
            }
        }

        //Resets the number of workers each player has
        p1.assignedWorkers = 0;
        p2.assignedWorkers = 0;
    }

    //Places spring tiles in the same, dedicated location each time
    public void springsAtCorners()
    {
        tileMap[1, 1] = 4;
        tileMap[1, mapSize - 2] = 4;
        tileMap[mapSize - 2, 1] = 4;
        tileMap[mapSize - 2, mapSize - 2] = 4;
    }

    //After map is drawn, goes over original map and checks for all spring tiles
    private void checkInitialWater()
    {
        for (int i = 0; i < mapSize; i++)
        {
            for(int j = 0; j < mapSize; j++)
            {
                //Also checks for standard water tiles & drain tiles, just in case...
                if(curMap[i,j].tileNum == 5 || curMap[i, j].tileNum == 4 || curMap[i, j].tileNum == 6)
                {
                    incWater(i, j);
                }
            }
        }
    }

    //Finds all edge and spring tiles on the map and adds it to a list
    public void findWaterOrigins()
    {
        for(int i = 0; i < mapSize; i++)
        {
            for(int j = 0; j < mapSize; j++)
            {
                if(curMap[i,j].tileNum == 4 || curMap[i, j].edge)
                {
                    springTiles.Add(curMap[i, j]);
                }
            }
        }
    }
    /*------------------------------------------------------------------------------------------------------------------------------------------------------------------------
     *  BREADTH-FIRST TILE SEARCH FUNCTIONS:
     *      * search();
     *      * searchReset();
     *      * searchShell();
     *      * getNeighbors();
     * -------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    
    //Searches the area around the MapTile
    public void search(MapTile t)
    {
        //If current tile is water or a spring
        if (t.tileNum == 4 || t.tileNum == 5 || t.tileNum == 2)
        {
            //Marks the tile as selected, and grabs its neighbors
            t.searched = true;
            MapTile[] n = getNeighbors((int)t.gridPos.x, (int)t.gridPos.y);

            //Goes through the neighbors, and...
            for (int i = 0; i < n.Length; i++)
            {
                //Skips the other steps if it is null
                if (n[i] == null)
                {
                    continue;
                }

                //Farm tiles...
                if(t.tileNum == 2)
                {
                    //Debug.Log("Searched Farm");
                    //Farm tiles spread water to nearby tiles if they're controlled by the same player
                    if (n[i].controllingPlayer == t.controllingPlayer || n[i].controllingPlayer == 0)
                    {
                        //If greater than its own water content, spreads it
                        n[i].nearFarm += t.nearWater / 2;
                        
                    }
                }
                //Water & Spring tiles...
                else if(t.tileNum == 4 || t.tileNum == 5)
                {
                    //Increments it's near water timer...
                    n[i].nearWater += 1;
                }

                //Change it to a water tile if it is a hole or draining...
                if ((n[i].tileNum == 0 || n[i].tileNum == 6) && t.tileNum != 2)
                {
                    n[i].tileNum = 5;
                }


                //Adds it to the search list if it is a water tile and unsearched...
                if ((n[i].tileNum == 5 || n[i].tileNum == 0 || n[i].tileNum == 6) && n[i].searched == false)
                {
                    n[i].orderNum = t.orderNum + 1;
                    searchThese.Add(n[i]);
                }
                //Adds farms to the search queue, but only if the current tile is not itself a farm
                if (n[i].tileNum == 2 && n[i].searched == false && (t.tileNum == 5 || t.tileNum == 4))
                {
                    searchFarms.Add(n[i]);
                }
            }
        }
        //Removes current tile from the top of the List
        if (t.tileNum == 2)
        {
            searchFarms.Remove(t);
        }
        else
        {
            searchThese.Remove(t);
        }
    }

    //Resets all necissary elements to run another search
    public void searchReset()
    {
        //Iterates over the map and...
        for(int i = 0; i < mapSize; i++)
        {
            //...resets every single tile to unsearched and no water.
            for(int j = 0; j < mapSize; j++)
            {
                curMap[i, j].searched = false;
                curMap[i, j].nearWater = 0;
                curMap[i, j].nearFarm = 0;
            }
        }

        //Remove all elements from the List
        searchThese.RemoveRange(0, searchThese.Count);
        searchFarms.RemoveRange(0, searchFarms.Count);

        //Adds the spring tiles to the search queue
        for(int i = 0; i < springTiles.Count; i++)
        {
            //Checks if current tile is water/spring, for maps with "edge" tiles
            if(springTiles[i].tileNum == 5 || springTiles[i].tileNum == 4)
            searchThese.Add(springTiles[i]);
        }
    }

    //Performs a search for all possible water tiles in map
    public void searchShell()
    {
      
        //Resets variables on current map first
        searchReset();

        while(searchThese.Count > 0)
        {
            search(searchThese[0]);
        }
        while(searchFarms.Count > 0)
        {
            search(searchFarms[0]);
        }
    }

    //Used by "search()" to determine what the tile's neighbors are
    public MapTile[] getNeighbors(int x, int y)
    {
        MapTile[] temp = new MapTile[4];    //MapTile array, to be returned
        int index = 0;

        //If the tile is not on the leftmost edge...
        if (x > 0) 
        {
            temp[3] = curMap[x - 1, y];
            index += 1;
        }
        //If the tile is not on the rightmost edge...
        if(x < mapSize - 1)
        {
            temp[1] = curMap[x + 1, y];
            index += 1;
        }
        //If the tile is not on the bottommost row...
        if(y > 0)
        {
            temp[0] = curMap[x, y - 1];
            index += 1;
        }
        //If the tile is not on the topmost row...
        if(y < mapSize - 1)
        {
            temp[2] = curMap[x, y + 1];
            index += 1;
        }
        return temp;
    }


    /*------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        TIMER RELATED FUNCTIONS HELD HERE:
            * countdownStart()
            * twoPlayerTimer()
            * bigTimerCD()
    -------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    //The basic version of countdownStart()
    public void countdownStart()
    {
        countdownStart("Round 1");
    }

    //initializes the countdown to starting the match
    public void countdownStart(string introText)
    {

        //Precountdown allows players to move, giving them a chance to see the map
        if ((!p1.ready || !p2.ready))
        {
            moveLocked = false;
            introMes.text = "LOOK AROUND!";

            //Shows that either player is ready
            if (p1.ready)
            {
                p1Mes.text = "P1 READY!";
                p1Mes.gameObject.GetComponent<TextChanger>().changeColor(Color.cyan);
            }
            else //Message becomes grey if ready recinded
            {
                p1Mes.text = "Press Start";
                p1Mes.gameObject.GetComponent<TextChanger>().changeColor(Color.grey);
            }
            if (p2.ready)
            {
                p2Mes.text = "P2 READY!";
                p2Mes.gameObject.GetComponent<TextChanger>().changeColor(Color.magenta);
            }
            else
            {
                p2Mes.text = "Press Start";
                p2Mes.gameObject.GetComponent<TextChanger>().changeColor(Color.grey);
            }
        }
        //Actual countdown begins after a set period of time, or both players ready
        else
        {
            moveLocked = true;
            countdownWhole.SetActive(true);
            roundMes.text = introText;
            if (!bothReady)
            {
                clearMap();
                roundReset();
                searchShell();
            }
            bothReady = true;
            tutorialImage.SetActive(false);

            //Starts up the main song
            if (!AudioManager.am.started)
            {
                AudioManager.am.startMain();
            }

            //Activates the normal player UI
            p1Mes.gameObject.SetActive(false);
            p2Mes.gameObject.SetActive(false);
            p1Canvas.SetActive(true);
            p2Canvas.SetActive(true);
            introMes.text = introText;
            introMes.GetComponent<TextChanger>().changeColor(Color.red);

            //Pauses right before action, long enough for music to drop
            if (preCountDown > 4)
            {
                preCountDown -= Time.deltaTime;
                countdownTimer.text = "" + (int)(preCountDown);
                cdCurSize = 25;

                //Moves the player cams to look over the map
                if (moveLocked)
                {
                    //...Takes 2 seconds to orient cameras to the top
                    if(preCountDown > 8)
                    {
                        //Calculates proper distance as linear interpolation of remaining time
                        float temp = 1 - ((preCountDown - 8f) / 2f);
                        Vector3 temp1 = Vector3.Lerp(p1.pCam.orig3, p1.pCam.neutralPos, temp);
                        Vector3 temp2 = Vector3.Lerp(p2.pCam.orig3, p2.pCam.neutralPos, temp);

                        //Adjusts the cameras
                        p1.pCam.lerpCam(temp1);
                        p2.pCam.lerpCam(temp2);
                        p1.pCam.zoomTar = 15f;
                        p2.pCam.zoomTar = 15f;

                        //minimizes the screen divider
                        screenDivider.transform.localScale = new Vector3(Mathf.Clamp01(0.5f-temp), 1f, 0f);
                    }
                    else
                    {
                        Vector3 temp1 = Vector3.Lerp(p1.pCam.neutralPos, p1.pCam.endPos, 1f-(float)(preCountDown - 4)/4);
                        Vector3 temp2 = Vector3.Lerp(p2.pCam.neutralPos, p2.pCam.endPos, 1f-(float)(preCountDown - 4)/4);
                        p1.pCam.lerpCam(temp1);
                        p2.pCam.lerpCam(temp2);
                    }
                }
            }
            //Last 3 sec-ish countdown begins after precountdown concludes
            else
            {
                //If currently moveLocked, reset
                if (moveLocked)
                {
                    moveLocked = false;
                    p1.pCam.zoomTar = 12f;
                    p2.pCam.zoomTar = 12f;
                }

                //Lowers the countdown, and uses it to lerp the divider
                preCountDown -= Time.deltaTime;
                float temp = preCountDown / 4f;
                screenDivider.transform.localScale = new Vector3(1f - temp, 1f, 1f - temp);

                //Hides the countdown visual
                countdownWhole.SetActive(false);

                if (countdown != null)
                {
                    countdown.SetActive(true);
                }

                //When text reaches minimum size...
                if (preCountDown <= (float)count)
                {
                    if (count > 0)
                    {
                        count -= 1;
                        //cdCurSize = 25;
                    }
                    else
                    {
                        roundMes.text = "";
                        moveLocked = false;
                        introMes.gameObject.SetActive(false);
                        gameStart = true;
                        countdown.SetActive(false);
                        begin = false;
                        cdCurSize = 25;
                    }
                }

                if(preCountDown > 1f)
                {
                    bigTimerCD("" + count, preCountDown-(float)count);

                }
                else
                {
                    bigTimerCD("GO", preCountDown-(float)count);
                }
                

                //Applies new scale to countdown object
                if (countdown != null)
                {
                    countdown.transform.localScale = new Vector3(cdCurSize, cdCurSize, cdCurSize);
                }
            }//End Countdown()       
        }
    }
    //Timer and Win-Condition for 2-Player mode
    public void twoPlayerTimer()
    {
        //Alters the timer if the game is going
        if (gameStart)
        {

            //Displays the time left - changes appearance if less than 10 seconds
            if (seconds >= 6)
            {
                timer.text = "" + (int)seconds;
            }
            else if (seconds < 6 && seconds >= 0)
            {
                /*timer.text = "" + (int)seconds;
                timer.color = Color.red;*/

                //Reactivates the Countdown gameobject
                if (countdown != null && countdown.active == false)
                {
                    countdown.SetActive(true);
                    timer.text = "";
                }

                //Determines the text of the Game object
                if (seconds >= 1f)
                {
                    bigTimerCD("" + Mathf.Floor(seconds), seconds);

                }
                else
                {
                    bigTimerCD("TIME", seconds);
                }

                //Adjusts size accordingly
                countdown.transform.localScale = new Vector3(cdCurSize, cdCurSize, cdCurSize);
            }
            else
            {             
                roundFinish = true;
                gameStart = false;
                vAdded = false;
                countdown.SetActive(false);
            }


            //Increments the timer
            seconds -= Time.deltaTime;
        }
        else
        {
            //The timer textbox appears empty when the game isn't in action
            timer.text = "";

            //The game is now afoot!
            if (!roundFinish)
            {
                /*if (begin)
                 {
                     countdownStart();
                 }
                 else
                 {

                     if (Input.GetKeyDown(KeyCode.Space))
                     {
                         countdown.SetActive(true);
                         begin = true;
                     }
                 }*/
                //begin = true;
            }
            //This happens when the game concludes
            else
            {        
                moveLocked = true;

                //Determines the winner based on who has the most savings
                panel.SetActive(true);

                //Says which round it is
                roundText.text = "ROUND " + numGames;

                //Determines winner of the round
                if (p1.money > p2.money)
                {
                    winMes.text = "P1 WINS!";

                    //Adds a victory to P1's victory count
                    if (!vAdded)
                    {
                        p1V++;
                        vAdded = true;
                    }      
                }
                else if (p2.money > p1.money)
                {
                    winMes.text = "P2 WINS!";

                    //Adds a victory to P2's victory count
                    if (!vAdded)
                    {
                        p2V++;
                        vAdded = true;
                    }
                }
                else
                {
                    winMes.text = "2-WAY TIE!";
                }

                roundEndTimer -= Time.deltaTime;

                //Ones the round End Timer ends
                if (roundEndTimer  <= 0f)
                {
                   winMes.text = "";
                   numGames++;
                    //If we've reached the final game...
                    if (numGames > maxNumGames || p1V > ((maxNumGames-numGames)+p2V+1) || p2V > (maxNumGames-numGames)+p1V+1)
                    {
                        roundText.text = "Game Finish!";
                        
                        //Determines the winnder
                        if(p1V > p2V)
                        {
                            winMes.text = "P1 WINS!";
                        }else if (p2V > p1V)
                        {
                            winMes.text = "P2 WINS!";
                        }
                        else
                        {
                            winMes.text = "2-WAY TIE!";
                        }
                        gameFinish = true;

                        //Plays the final musical sting
                        if (AudioManager.am.started)
                        {
                            AudioManager.am.playSting();
                        }
                        
                        //Displays the final score between two players
                        winRatio.SetActive(true);
                        p1Ratio.text = "" + p1V;
                        p2Ratio.text = "" + p2V;
                    }
                    //...otherwise, starts the next round.
                    else
                    {
                        panel.SetActive(false);
                        roundReset();
                    }
                }
            }
        }
    }//End two player timer

    //Resets everything necissary to begin the next round
    public void roundReset()
    {
        seconds = roundLength;
        begin = true;
        roundFinish = false;
        gameStart = false;
        roundEndTimer = 5f;
        count = 3;
        preCountDown = 10;
        countdown.SetActive(false);
        p1.ready = true;
        p2.ready = true;
        p1.money = 1000;
        p2.money = 1000;
        //moveLocked = false;

        //Changes the player's UI indicators
        p1.maxWorkers = (int)Mathf.Pow(2, numGames + 1);
        p2.maxWorkers = (int)Mathf.Pow(2, numGames + 1);
        p1.expectedIncome = (int)Mathf.Pow(2, numGames) * 20;
        p2.expectedIncome = (int)Mathf.Pow(2, numGames) * 20;
    }

    //Makes a big number countdown for the last couple of seconds on the timer
    public void bigTimerCD(string display, float size)
    {

        //Truncates size to its decimal value
        size = size % 1;
        if (size == 0)
            size = 1;

        //Sets display to proper message
        cd.text = display;
        
        //Determines size based on decimal value
        if(size > 0.75f)
        {
           cdCurSize = Mathf.Lerp(25f, 3.5f, 1-(size - 0.75f) * 4);
        }
        else if (size <= 0.75f && size > 0.25f)
        {
           cdCurSize = Mathf.Lerp(3.5f, 2f, 1-(size - 0.25f) * 2);
        }
        else
        {
           cdCurSize = Mathf.Lerp(2f, 0.0001f, 1-(size * 4));
        }
    }

    /*------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        PLAYER RELATED FUNCTIONS HELD HERE:
            * spawnWarning()
            * checkAdjacent()
            * returnPlayer()
    -------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
   
    //Spawns a warning marker
    public void spawnWarning(PlayerClass p, Transform t)
    {
        GameObject w = Instantiate(warning, p.transform);
        w.GetComponent<WarningMarker>().initialize(t, p);
    }
    public void spawnWarning(int p, Transform t)
    {
        Debug.Log("Attempted to Spawn");
        if(p == 1)
        {
            GameObject w = Instantiate(warning);
            w.GetComponent<WarningMarker>().initialize(t, p1);
        }
        else
        {
            GameObject w = Instantiate(warning);
            w.GetComponent<WarningMarker>().initialize(t, p2);
        }
    }

    //Checks if digger was placed on important tile
    public void checkAdjacent(int tileX, int tileY, int p)
    {
        //Debug.Log("Attempted to check Adjacent");
        bool temp = false;

        //Checks if any of the adjacent tiles are under the control of the enemy
        if(tileX > 0)
        {
            if(curMap[tileX-1,tileY].controllingPlayer != p && curMap[tileX - 1, tileY].controllingPlayer != 0)
            {
                temp = true;
            }
        }
        if(tileX < mapSize-1)
        {
            if (curMap[tileX + 1, tileY].controllingPlayer != p && curMap[tileX + 1, tileY].controllingPlayer != 0)
            {
                temp = true;
            }
        }
        if (tileY > 0)
        {
            if (curMap[tileX, tileY-1].controllingPlayer != p && curMap[tileX, tileY-1].controllingPlayer != 0)
            {
                temp = true;
            }
        }
        if (tileY < mapSize - 1)
        {
            if (curMap[tileX, tileY + 1].controllingPlayer != p && curMap[tileX, tileY + 1].controllingPlayer != 0)
            {
                temp = true;
            }
        }

        //If so, then spawns a warning for that player
        if (temp)
        {
            if(p == 1)
            {
                spawnWarning(2, curMap[tileX, tileY].gameObject.transform);
            }
            else
            {
                spawnWarning(1, curMap[tileX, tileY].gameObject.transform);
            }
        }
    }

    //Returns the relevant player when prompted
    public PlayerClass returnPlayer(int p)
    {
        if(p == 1)
        {
            return p1;
        }
        else if(p == 2)
        {
            return p2;
        }
        else
        {
            return null;
        }
    }

    /*------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    TILE RELATED FUNCTIONS HELD HERE:
        * incWater()
        * decWater()
        * findWaterParent()
        * tileCompare()
    -------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
   
    //Increments the number of water tiles
    public void incWater(int i, int j)
    {
        //Searches all adjascent tiles, and increases the "nearWater" variable
        if (i > 0)
        {
            curMap[i - 1, j].nearWater += 1;
        }
        if (i < mapSize - 1)
        {
            curMap[i + 1, j].nearWater += 1;
        }
        if (j > 0)
        {
            curMap[i, j - 1].nearWater += 1;
        }
        if (j < mapSize - 1)
        {
            curMap[i, j + 1].nearWater += 1;
        }
    }

    //Decrements the number of nearby water tiles when a water tile is filled
    public void decWater(int i, int j)
    {
        //Searches all adjascent tiles, and decreases the "nearWater" variable, if >0
        if (i > 0)
        {
            if (curMap[i - 1, j].nearWater > 0)
            {
                curMap[i - 1, j].nearWater -= 1;
            }

        }
        if (i < mapSize - 1)
        {
            if (curMap[i + 1, j].nearWater > 0)
            {
                curMap[i + 1, j].nearWater -= 1;
            }
        }
        if (j > 0)
        {
            if (curMap[i, j - 1].nearWater > 0)
            {
                curMap[i, j - 1].nearWater -= 1;
            }
        }
        if (j < mapSize - 1)
        {
            if (curMap[i, j + 1].nearWater > 0)
            {
                curMap[i, j + 1].nearWater -= 1;
            }
        }
    }

    //Given a tile, will find the tile's parent
    public MapTile findWaterParent(MapTile c)
    {
        //Checks all this tile's adjacent tiles, starting with the top
        if((int)c.gridPos.x > 0)
        {
            //Checks tile to the left
            if(tileCompare(c, curMap[(int)c.gridPos.x-1, (int)c.gridPos.y])){
                return curMap[(int)c.gridPos.x-1, (int)c.gridPos.y];
            }
        }
        if ((int)c.gridPos.x < mapSize-1)
        {
            //Checks tile to the right
            if (tileCompare(c, curMap[(int)c.gridPos.x + 1, (int)c.gridPos.y]))
            {
                return curMap[(int)c.gridPos.x+1, (int)c.gridPos.y];
            }
        }
        if ((int)c.gridPos.y > 0)
        {
            //Checks tile above cureent
            if (tileCompare(c, curMap[(int)c.gridPos.x, (int)c.gridPos.y-1]))
            {
                return curMap[(int)c.gridPos.x, (int)c.gridPos.y-1];
            }
        }
        if ((int)c.gridPos.y < mapSize-1)
        {
            //Checks tile below current
            if (tileCompare(c, curMap[(int)c.gridPos.x, (int)c.gridPos.y+1]))
            {
                return curMap[(int)c.gridPos.x, (int)c.gridPos.y+1];
            }
        }

        //Returns null if no compatible parent is found
        return null;
    }

    //What "findWaterParent()" uses to compare two tiles
    public bool tileCompare(MapTile c, MapTile p)
    {
        //If C is NOT the parent of P and not the previous parent...
        if(p.waterParent != c && p.lastParent != c)
        {
            //...and it is either a Water or Spring tile...
            if(p.tileNum == 5 || p.tileNum == 4)
            {
                return true;
            }
        }
        //Otherwise...
        return false;
    }
}
