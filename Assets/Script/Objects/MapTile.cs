using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapTile : MonoBehaviour {

    public int orderNum;

    //Sprites go here:
    [Header("Sprites/Visuals")]
    public Sprite grass1;
    public Sprite rock1;
    public Sprite water1;
    public Sprite seed;
    public Sprite pit;
    public Sprite selectBase;

    public Sprite ash;
    public Sprite fire;

    private int cropNum;
    public Sprite fighting;
    public Sprite spring;
    public GameObject waterTop;
    public GameObject springTop;
    public ParticleSystem springPart;

    [Header("Multi-Sprites")]
    public Sprite[] digger;
    public Sprite[] planter;
    public Sprite[] cropSprites;
    public Sprite[] waterDrain;     //Sprites in order for draining water animation
    public Sprite[] selectionGlow;  // 0 - P1 Active, 1 - P1 Inactive, 2 - P2 Active, 3 - P2 Inactive
    private int drainIndex;
    private float drainTimer;
    public float drainTimerMax;

    [Header("Animators")]
    public GameObject p1Worker;
    private Animator p1w;
    public int lastSet;

    [Header("Water Mats")]
    public SpriteRenderer waterTopSR;
    public Material waterShader;
    public Vector2 waterVector1;
    public Vector2 waterVector2;
    public MapTile[] neighbors;             //Clockwise from top: 0 - NW (Y-1), 1 - NE (X+1), 2 - SE, 3 - SW
    public WaterFall[] waterFalls;          // 0 - NW, 1 - NE, 2 - SE, 3 - SW
    public bool[] assignedFalls = new bool[4];

    //Self Componenets:
    [Header("Self")]

    //Base info (position, time, etc.)
    public Vector2 gridPos;
    public SpriteRenderer sr;
    public GameObject crop;
    public int selected;
    public bool ahead;
    public Collider2D col;
    public float timer;
    public float timerMax;
    public float subAmount;         //The current rate at which the timer is actually decrementing

    //Used to determine how much money the player should be making
    public float nearWater;
    public float nearFarm;
    public float cropTimer;
    public bool edge;
    public int farmLevel;

    //These variables tie into the Breadth-First Search
    public MapTile waterParent;
    public MapTile lastParent;
    public bool keyTile;
    public bool searched;
    public bool idle;

    //Color Variables
    [Header("Colors")]
    public Color grassColor;
    public Color rockColor;
    public Color waterColor;
    public Color emptyColor;
    public Color cropColor;
    public Color enemyColor;
    public Color springColor;

    public GameObject p1Select;
    public GameObject p2Select;

    public GameObject p1EnemySelect;
    public GameObject p2EnemySelect;

    //public GameObject worker;
    public GameObject sGlow;

    [Header("Fire")]
    public float fireSpreadChance;  //Chance per-tick of spreading fire to neighbors (goes down each spread)
    public int fireTicks;           //Remaining number of ticks
    public bool ashBonus;          //A monetary bonus from building a farm on an ash tile
    private float workerTimer;

    //Other variables:
    [Header("Other")]
    public int tileNum;             //0 - Hole, 1 - Grass, 2 - Crop, 3 - Rock, 4 - Spring, 5 - Water, 6 - Draining Water, 7 - Fire, 8 - Ash
    public int controllingPlayer;
    public int workerAssigned;
    public int opposingWorker;


    // Use this for initialization
    void Start() {
        sr = this.gameObject.GetComponent<SpriteRenderer>();
        col = this.gameObject.GetComponent<EdgeCollider2D>();
        waterShader = waterTopSR.material;
        timer = timerMax;
        p1EnemySelect.GetComponent<SpriteRenderer>().color = enemyColor;
        p2EnemySelect.GetComponent<SpriteRenderer>().color = enemyColor;
        farmLevel = 0;
        waterTop.SetActive(false);

        //Grabs the animators off of the sprite objects
        p1w = p1Worker.GetComponent<Animator>();
       // p1p = p1Planter.GetComponent<Animator>();

        //If not a spring, deactivate all spring stuff
        if (tileNum != 4)
        {
            springTop.SetActive(false);
            springPart.Pause();
        }
        else
        {
            springTop.SetActive(true);
            springPart.Play();
        }

        //Grabs neighbords from GameManager
        neighbors = GameManager.gm.getNeighbors((int)gridPos.x, (int)gridPos.y);

        //If a water tile, find speed & dir
        if(tileNum == 5)
        {
            findWaterSpeed();
            setWaterfalls();
        }
            
    }

    // Update is called once per frame
    void Update() {

        if (searched)
        {
            findWaterSpeed();
        }

        //Changes the max timer when each round concludes
        if (GameManager.gm.roundFinish)
        {
            timerMax = 2f + 0.5f * GameManager.gm.numGames;
        }

        //---------------------------------------------------------------------------------------------------------------------------------
        //  UPDATES VALES ON TILE FOR SPECIFIC TILE TYPES
        //---------------------------------------------------------------------------------------------------------------------------------
        
        //Causes crops to grow
        if(tileNum == 2)
        {
            crop.SetActive(true);
            grow();
        }
        else
        {
            crop.SetActive(false);
            farmLevel = 0;
        }


        //Turns water shader on if water tile
        if (tileNum == 5)
        {
            waterTop.SetActive(true);
            setWaterfalls();
        }
        else
        {
            waterTop.SetActive(false);
            for(int i = 0; i < assignedFalls.Length; i++)
            {
                assignedFalls[i] = false;
            }
        }

        //Counts down and checks if it can spread if this is a fire tile
        if(tileNum == 7 && !GameManager.gm.roundFinish)
        {
            //If a worker is assigned to this burning tile
            if(workerAssigned != 0)
            {
                //Points a warning at this tile
                //GameManager.gm.spawnWarning(controllingPlayer, this.transform);

                //Time before worker dies
                workerTimer -= Time.deltaTime;

                //Times up - TODO, place a dying animation here
                if(workerTimer <= 0)
                {
                    workerAssigned = 0;
                    GameManager.gm.returnPlayer(controllingPlayer).killedWorkers += 1;
                }
            }

            //Ticks down the fire timer
            if(fireTicks > 0)
            {
                timer -= Time.deltaTime;

                //When a tick is reached...
                if(timer <= 0)
                {
                    float seed = Random.Range(0f, 1f);

                    //Checks to see if it can ignite any of the adjacent tiles...
                    if(seed < fireSpreadChance)
                    {
                        //...then Checks if any neighbors are grass or farm...
                        foreach(MapTile m in neighbors)
                        {
                            if(m.tileNum == 1 || m.tileNum == 2)
                            {
                                m.ignite(fireSpreadChance / 2);
                                break;
                            }
                        }

                    }

                    //Resets timer
                    timer = timerMax/2;
                    fireTicks--;
                }
            }

            //When fire burns out, replaces it with ashes
            else
            {
                tileNum = 8;
            }
        }

        //---------------------------------------------------------------------------------------------------------------------------------
        //  CONTROLS THE ANIMATIONS OF THE WORKER PLACED ON THIS TILE
        //---------------------------------------------------------------------------------------------------------------------------------

        //Resets farm level if it is not a farm
        if (controllingPlayer == 1 && workerAssigned != 0 && opposingWorker == 0)
        {
            p1Worker.SetActive(true);

            //Digger animations
            if(workerAssigned == 1)
            {
                //Digger is currently idle
                if((tileNum == 0 || tileNum == 5 || tileNum == 6 || tileNum == 7)) {
                    if(lastSet != 1)
                    {
                        p1w.Play("Digger - Idle");
                        lastSet = 1;
                    }
                }

                //Digger is currently active
                else
                {
                    if(lastSet != 2)
                    {
                        p1w.Play("Digger - Dig");
                        lastSet = 2;
                    }
                }       
            }

            //Planter animations
            else if (workerAssigned == 2)
            {
                //Planter is currently idle
                if((tileNum == 1))
                {
                    if(lastSet != 3)
                    {
                        p1w.Play("Planter - Idle");
                        lastSet = 3;
                    }
                }
                //Planter is working a farm tile
                else if (tileNum == 2)
                {
                    if (lastSet != 4)
                    {
                        p1w.Play("Planter - Pump");
                        lastSet = 4;
                    }
                }
                //Planter is on another type of tile
                else
                {
                    if (lastSet != 5)
                    {
                        p1w.Play("Planter - Fill");
                        lastSet = 5;
                    }
                }
            }
        }else if(controllingPlayer == 2 && workerAssigned != 0 && opposingWorker == 0)
        {
            p1Worker.SetActive(true);

            //Digger animations
            if (workerAssigned == 1)
            {
                //Digger is currently idle
                if ((tileNum == 0 || tileNum == 5 || tileNum == 6 || tileNum == 7))
                {
                    if (lastSet != 1)
                    {
                        p1w.Play("Digger2 Idle");
                        lastSet = 1;
                    }
                }

                //Digger is currently active
                else
                {
                    if (lastSet != 2)
                    {
                        p1w.Play("Digger2 Dig");
                        lastSet = 2;
                    }
                }
            }

            //Planter animations
            else if (workerAssigned == 2)
            {
                //Planter is currently idle
                if ((tileNum == 1))
                {
                    if (lastSet != 3)
                    {
                        p1w.Play("Planter2 Idle");
                        lastSet = 3;
                    }
                }
                //Planter is working a farm tile
                else if (tileNum == 2)
                {
                    if (lastSet != 4)
                    {
                        p1w.Play("Planter2 Pump");
                        lastSet = 4;
                    }
                }
                //Planter is on another type of tile
                else
                {
                    if (lastSet != 5)
                    {
                        p1w.Play("Planter2 Fill");
                        lastSet = 5;
                    }
                }
            }
        }else if (opposingWorker  > 0)
        {
            p1Worker.SetActive(true);
            if (lastSet != 6)
            {
                p1w.Play("Fight");
                lastSet = 6;
            }
        }
        else
        {
            p1Worker.SetActive(false);
            lastSet = 0;
        }

        //Detects when the worker on the title is idle
        if((workerAssigned == 1 && (tileNum == 0 || tileNum == 4 || tileNum == 5 || tileNum == 6 || tileNum == 7)) ||   //Digger on a hole, water, or spring tile
            workerAssigned == 2 && (tileNum == 1 || tileNum == 7))                                                      //Planter on a grass tile
        {
            idle = true;
        }
        else
        {
            idle = false;
        }

        //Activates a glow if a player is controlling the tile
        if(controllingPlayer != 0 && workerAssigned != 0)
        {
            sGlow.SetActive(true);

            //Determines which player is controlling the tile
            if(controllingPlayer == 1)
            {
                //Then determines if the tile is idle or not
                if (idle)
                {
                    sGlow.GetComponent<SpriteRenderer>().sprite = selectionGlow[1];
                }
                else
                {
                    sGlow.GetComponent<SpriteRenderer>().sprite = selectionGlow[0];
                }
            }
            else
            {
                if (idle)
                {
                    sGlow.GetComponent<SpriteRenderer>().sprite = selectionGlow[3];
                }
                else
                {
                    sGlow.GetComponent<SpriteRenderer>().sprite = selectionGlow[2];
                }
            }
        }
        //...and decativates if when all workers are removed
        else
        {
            sGlow.SetActive(false);
        }

        //If two workers are in conflict, and the original player pulls out,
        //swaps the player in control of the tile.
        if(opposingWorker > 0 && workerAssigned == 0)
        {
            workerAssigned = opposingWorker;
            opposingWorker = 0;

            //Changes which player is in control of the tile
            if(controllingPlayer == 1)
            {
                controllingPlayer = 2;
            }
            else
            {
                controllingPlayer = 1;
            }
            GameManager.gm.searchShell();
        }
        //Checks the location of the mouse
        selected = checkMouse();

        //If the player's cursor is over this tile...
        if(selected == 0)
        {
            ahead = false;
        }
        
        //...and this tile is the first on the grid...
        if (ahead) {

            //... then it changes the color of the tile to indicate selection
            colorChange(selected);

        } else
        {
            //Disables selection indicators
            p1Select.SetActive(false);
            p1EnemySelect.SetActive(false);
            p2Select.SetActive(false);
            p2EnemySelect.SetActive(false);

            //selected = 0;
        }

        updateSprites();


        //Activates the worker sprite if it's there
        if (workerAssigned == 0)
        {
            //crop.SetActive(false);
            
            //worker.SetActive(false);
            //p1Digger.SetActive(false);
            //p1Planter.SetActive(false);
        }
        //Digger
        else if ((workerAssigned == 1) && (opposingWorker == 0))
        {
            //p1Planter.SetActive(false);
            //Activates the digger animation

            if(controllingPlayer == 2)
            {
                //worker.GetComponent<SpriteRenderer>().sprite = digger[controllingPlayer - 1];
                //worker.SetActive(true);
            }
            else
            {
                //p1Digger.SetActive(true);
            }

            dig();
        }
        //Planter
        else if ((workerAssigned == 2) && (opposingWorker == 0))
        {
           // p1Digger.SetActive(false);
            if(controllingPlayer == 2)
            {
                //worker.GetComponent<SpriteRenderer>().sprite = planter[controllingPlayer - 1];
                //worker.SetActive(true);
            }
            else
            {
               // p1Planter.SetActive(true);
            }

            if (tileNum != 2)
                plant();



            //Activtes the crops
 
        }
        //Two opposing workers assigned to the same tile
        else if(opposingWorker != 0)
        {
           // p1Digger.SetActive(false);
           // p1Planter.SetActive(false);
            //worker.GetComponent<SpriteRenderer>().sprite = fighting;
            //worker.SetActive(true);
        }

        //Checks if this tile should exist
        if(tileNum == 5)
        {
            //checkWaterParent();
        }

    }//End Update()

    //checks if the mouse is over this particular tile
    public int checkMouse()
    {
        if(col.bounds.Contains(new Vector3(GameManager.gm.p1.posX, GameManager.gm.p1.posY, gameObject.transform.position.z)) && selected != 2)
        {
            return 1;
        }
        else if (col.bounds.Contains(new Vector3(GameManager.gm.p2.posX, GameManager.gm.p2.posY, gameObject.transform.position.z)) && selected != 1)
        {
            return 2;
        }
        return 0;
    }

    //The single player version
    public int checkMouseSP()
    {
        if (col.bounds.Contains(new Vector3(GameManager.gm.p1.posX, GameManager.gm.p1.posY, gameObject.transform.position.z)) && selected != 2)
        {
            return 1;
        }
        return 0;
    }

    //Does this if there is a digger assigned to the tile
    protected void dig()
    {
        //Will only dig if the game is going
        if (!GameManager.gm.bothReady || GameManager.gm.gameStart)
        {
            timer -= Time.deltaTime;
        }

        //When the timer reaches zero, changes the tile
        if(timer <= 0)
        {
            //Turns a stone tile into a pit
            if(tileNum == 3)
            {
                tileNum = 0;
                GameManager.gm.searchShell();
            }
            //Turns grass/seed tiles into stone
            else if((tileNum == 1) || (tileNum == 2))
            {
                tileNum = 3;
            }
            
            //Resets the timer
            timer = timerMax;
        }
    }

    //Does this if there is a planter assigned to this tile
    protected void plant()
    {
        //Will only plant if the game is going
        if ((!GameManager.gm.bothReady || GameManager.gm.gameStart) && tileNum != 2)
        {

            timer -= Time.deltaTime;


            //When timer reaches zero, performs an action based on the tile
            if (timer <= 0)
            {
                //If crop tile, increments the crop sprite
                if ((tileNum == 2) && (workerAssigned == 2) && (nearWater > 0 || nearFarm > 0))
                {


                }


                //If stone tile, changes to a grass tile
                if (tileNum == 3)
                {
                    tileNum = 1;
                }

                //If pit or water, changes to a rock tile
                else if ((tileNum == 5) || (tileNum == 0) || tileNum == 6)
                {
                    //Decrements "nearWater" var in nearby tiles if originally a water tile
                    if (tileNum == 5 || tileNum == 6)
                    {
                        //GameManager.gm.decWater((int)gridPos.x, (int)gridPos.y);

                    }
                    tileNum = 3;
                    GameManager.gm.searchShell();
                }

                //Resets the timer
                timer = timerMax;
            }
        }
    }

    //Crops will do this regardless of who works on them
    public void grow()
    {
        //Crops grow faster the more water tiles there are nearby
        if (farmLevel < 3)
        {
            //Crops grow faster the more water is nearby
            timer -= Time.deltaTime / 3 * (nearWater + (nearFarm / 2));
        }
        else if(workerAssigned == 2)
        {
            
            timer -= Time.deltaTime;
        }

        if(timer <= 0)
        {
            //Determines which crop sprite to use and how far along the crop is
            farmLevel += 1;
            if (farmLevel >= cropSprites.Length)
            {
                farmLevel = cropSprites.Length - 1;
            }

            //Resets the timer
            timer = timerMax;

            //Changes the sprite
            crop.GetComponent<SpriteRenderer>().sprite = cropSprites[farmLevel];

            //If the farm is fully grown, harvests it
            if (tileNum == 2 && farmLevel >= 3 && workerAssigned == 2)
            {
                PlayerClass p = GameManager.gm.returnPlayer(controllingPlayer);
                p.money += 1000;
                if (ashBonus) { p.money += 1000; }
                ashBonus = false;
                tileNum = 1;
                
            }
        }
    }

    //Changes the appearance of the tile
    public void updateSprites()
    {
        //Sets tile to hole
        if (tileNum == 0)
        {
            sr.sprite = pit;

            /*//Fills with water when near it
            if (nearWater > 0 || edge)
            {
                getWaterParent();
            }*/
        }

        //Sets tile to grass
        if (tileNum == 1)
        {
            sr.sprite = grass1;
        }

        //sets tile to crop
        if (tileNum == 2)
        {
            sr.sprite = seed;
        }


        //sets tile to rock
        if (tileNum == 3)
        {
            sr.sprite = rock1;
        }

        //Sets tile to spring
        if(tileNum == 4)
        {
            sr.sprite = spring;
        }

        //Water draining
        if(tileNum == 6)
        {

            //Drains water away by incrementing timer
            if(drainIndex < waterDrain.Length)
            {
                sr.sprite = waterDrain[drainIndex];
                if(drainTimer > 0)
                {
                    drainTimer -= Time.deltaTime;
                }
                else
                {
                    drainTimer = drainTimerMax;
                    drainIndex++;
                }
            }
            else //Changes tile to empty when drain is complete
            {
                tileNum = 0;
                //GameManager.gm.decWater((int)gridPos.x, (int)gridPos.y);
                GameManager.gm.searchShell();
            }
        }
        else //resets water drain animation for next time
        {
            drainIndex = 0;
            drainTimer = drainTimerMax;
        }

        //Sets tile to water
        if (tileNum == 5)
        {
            sr.sprite = water1;

            //If the search passes over this tile, 
            //it will begin to drain
            if (!searched)
            {
                tileNum = 6;
            }
        }

        //Sets the tile to fire
        if(tileNum == 7)
        {
            sr.sprite = fire;
        }

        //Sets tile to ash
        if(tileNum == 8)
        {
            sr.sprite = ash;
        }
    }

    //Changes the color of the tile the player is selecting
    private void colorChange(int x)
    {
        //Determines which player's selection tile to affect
        GameObject s = p1Select;
        if(x == 1)
        {
            s = p1Select;
            p1Select.SetActive(true);
            p1EnemySelect.SetActive(true);
        }
        else if (x == 2)
        {
            s = p2Select;
            p2Select.SetActive(true);
            p2EnemySelect.SetActive(true);
        }

        //If tiles is neutral or controlled by this player, display proper colors
        if(controllingPlayer == 0 || controllingPlayer == x)
        {
            //changes the color of the selection tile based on tile type
            if (tileNum == 0)
            {
                s.GetComponent<SpriteRenderer>().color = emptyColor;
            }
            else if (tileNum == 1)
            {
                s.GetComponent<SpriteRenderer>().color = grassColor;
            }
            else if (tileNum == 2)
            {
                s.GetComponent<SpriteRenderer>().color = cropColor;
            }
            else if (tileNum == 3)
            {
                s.GetComponent<SpriteRenderer>().color = rockColor;
            }
            else if (tileNum == 4)
            {
                s.GetComponent<SpriteRenderer>().color = springColor;
            }
            else if (tileNum == 5 || tileNum == 6)
            {
                s.GetComponent<SpriteRenderer>().color = waterColor;
            }
        }
        //Otherwise, display dedicated enemy color
        else
        {
            s.GetComponent<SpriteRenderer>().color = enemyColor;
        }
    }

    //If tile is a hole near water, checks for a water parent
    public void getWaterParent()
    {
        waterParent = GameManager.gm.findWaterParent(this);

        //When a parent is found, change to water tile and increment the water
        if(waterParent != null || edge)
        {
            tileNum = 5;
           // GameManager.gm.incWater((int)gridPos.x, (int)gridPos.y);
            GameManager.gm.searchShell();
            findWaterSpeed();
        }
    }

    //If this is a water tile, checks if its parent is still connected
    public void checkWaterParent()
    {
        //If the tile has a parent...
        if(waterParent != null)
        {
            //Breaks instantly if the parent is a water/spring tile
            if (waterParent.tileNum == 4 || waterParent.tileNum == 5)
            {
                tileNum = 5;
                lastParent = null;
                return;
            }
            else
            {
                //tries to find a new water parent
                lastParent = waterParent;
                waterParent = GameManager.gm.findWaterParent(this);
                if(waterParent != null)
                {
                    tileNum = 5;
                    return;
                }
            }
        }
        //If the water tile does not have a parent...
        else
        {
            //Breaks instantly if this tile is on the edge of the map
            if (edge)
            {
                lastParent = null;
                return;
            }
            else //Sets water to begin draining
            {
                waterParent = GameManager.gm.findWaterParent(this);
                if (waterParent != null)
                {
                    tileNum = 5;
                    return;
                }
                tileNum = 6;
            }

        }
    }

    //Checks the direction & velocity of the water
    public void findWaterSpeed()
    {
        //Top normal corosponds to the NW & SW tiles
        waterVector1 = new Vector2(0f, 0f);
        waterVector2 = new Vector2(0f, 0f);

        //If NW tile is water...
        if (neighbors[0] == null || (neighbors[0].tileNum == 5 || neighbors[0].tileNum == 4))
        {
            //AND SW tile is also water...
            if(neighbors[3] == null || (neighbors[3].tileNum == 5 || neighbors[3].tileNum == 4))
            {
                waterVector1 = new Vector2(-1, 0);
            }
            //BUT SW tile is NOT water...
            else
            {
                waterVector1 = new Vector2(-1, 1);
            }
        }
        //If NW tile is NOT water...
        else
        {
            //...and SW IS water...
            if (neighbors[3] == null || (neighbors[3].tileNum == 5 || neighbors[3].tileNum == 4))
            {
                waterVector1 = new Vector2(-1, -1);
            }
        }

        //If NE tile is water...
        if(neighbors[1] == null || (neighbors[1].tileNum == 5 || neighbors[1].tileNum == 4))
        {
            //AND SE tile is ALSO water...
            if(neighbors[2] == null || (neighbors[2].tileNum == 5 || neighbors[2].tileNum == 4))
            {
                waterVector2 = new Vector2(1, 0);
            }
            //But SE tile is NOT water...
            else
            {
                waterVector2 = new Vector2(1, 1);
            }
        }
        //If SE tile is NOT water...
        else
        {
            //...and SE IS water...
            if (neighbors[2] == null || (neighbors[2].tileNum == 5 || neighbors[2].tileNum == 4))
            {
                waterVector2 = new Vector2(1, -1);
            }
        }

        //If the first water vector has no speed, go the opposite direction...
        if(waterVector1 == new Vector2(0f, 0f))
        {
            waterVector1 = waterVector2;
        }
        //Same to the other water vector...
        if(waterVector2 == new Vector2(0f, 0f))
        {
            waterVector2 = waterVector1;
        }
        
        multiplySpeed();
        //checkNeighborSpring();

        waterShader.SetVector("_UV1PanSpeed", waterVector1);
        waterShader.SetVector("_UV2PanSpeed", waterVector2);
    }

    //Once direction of water is determined, calculates the multiplication
    //Base directions assume adjacent water tiles are parents, so need adjustment if they are children
    private void multiplySpeed()
    {
        //If the NW or SW tile is a CHILD...
        if ((neighbors[0] == null || neighbors[0].orderNum >= this.orderNum)
            || (neighbors[3] == null || neighbors[3].orderNum >= this.orderNum))
        {
            waterVector1 *= new Vector2(-1f, -1f);
        }

        //If the NE or SE tile is a CHILD...
        if ((neighbors[1] == null || neighbors[1].orderNum >= this.orderNum)
        || (neighbors[2] == null || neighbors[2].orderNum >= this.orderNum))
        {
            waterVector2 *= new Vector2(-1f, -1f);
        }

        checkNeighborSpring();

        //Normal slowdown applies if tile is not on the edge of the map...
        if(neighbors[0] != null && neighbors[1] != null && neighbors[2] != null && neighbors[3] != null)
        {
            waterVector1 *= new Vector2(Mathf.Clamp(0.9f - 0.1f * orderNum, 0.005f, 1f), Mathf.Clamp(0.9f - 0.1f * orderNum, 0.005f, 1f));
            waterVector2 *= new Vector2(Mathf.Clamp(0.45f - 0.05f * orderNum, 0.005f, 1f), Mathf.Clamp(0.45f - 0.05f * orderNum, 0.005f, 1f));
        }
        //Otherwise, the speed is close to max
        else
        {
            waterVector1 *= new Vector2(0.84f, 0.84f);
            waterVector2 *= new Vector2(0.55f, 0.55f);
        }

    }

    //Checks if any neighboring water tiles are springs, and if so, overwrite these rules!
    private void checkNeighborSpring()
    {
        //NW tile is srping
        if(neighbors[0] != null && neighbors[0].tileNum == 4)
        {
            waterVector1 = new Vector2(-1f, 1f);
            waterVector2 = waterVector1;
        }
        //NE tile is spring
        else if (neighbors[1] != null && neighbors[1].tileNum == 4)
        {
            waterVector1 = new Vector2(1f, 1f);
            waterVector2 = waterVector1;
        }
        //SW tile is spring
        else if (neighbors[3] != null && neighbors[3].tileNum == 4)
        {
            waterVector1 = new Vector2(-1f, -1f);
            waterVector2 = waterVector1;
        }
        //SE tile is spring
        else if (neighbors[2] != null && neighbors[2].tileNum == 4)
        {
            waterVector1 = new Vector2(1f, -1f);
            waterVector2 = waterVector1;
        }
    }

    //Spawns waterfalls if the relevant neighbors are null
    public void setWaterfalls()
    {
        //If NW neighbor is null, spawns a waterfall
        if (neighbors[0] == null && assignedFalls[0] == false)
        {
            WaterFall temp = Instantiate(waterFalls[0], transform.position, Quaternion.identity);
            temp.m = this;
            assignedFalls[0] = true;
        }

        //If NE neighbor is null, spawns a waterfall
        if (neighbors[1] == null && assignedFalls[1] == false)
        {
            WaterFall temp = Instantiate(waterFalls[1], transform.position, Quaternion.identity);
            temp.m = this;
            assignedFalls[1] = true;
        }

        //If SE neighbor is null, spawns a waterfall
        if (neighbors[2] == null && assignedFalls[2] == false)
        {
            WaterFall temp = Instantiate(waterFalls[2], transform.position, Quaternion.identity);
            temp.m = this;
            assignedFalls[2] = true;
        }

        //If SW neighbor is null, spawns a waterfall
        if (neighbors[3] == null && assignedFalls[3] == false)
        {
            WaterFall temp = Instantiate(waterFalls[3], transform.position, Quaternion.identity);
            temp.m = this;
            assignedFalls[3] = true;
        }
    }

    //Sets this tile on fire
    public void ignite(float percen)
    {
        tileNum = 7;
        fireTicks = 5;
        fireSpreadChance = percen;
        ashBonus = true;
        timer = timerMax/2;
    }
}
