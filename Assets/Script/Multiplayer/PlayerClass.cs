using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.UI;

public partial class PlayerClass : MonoBehaviour {

    public Player player;      //Rewired player input handler
    //[HideInInspector]
    public bool ready;


    [Header("Player Input Positions")]
    public Vector3 mousePos;
    public float posX;
    public float posY;
    //public Camera pCam;
    public CameraControl pCam;
    public bool pad;

    //Maximum & minimum values for each position
    public float mmAdjust;
    public float maxCamDist;
    private float maxX;
    private float minX;
    private float maxY;
    private float minY;

    [HideInInspector]
    public bool powerBuffered;

    //Key Player Stats
    [Header("Player Stats")]
    public int pNum;
    public int charNum;
    public float money;
    public float income;
    public float baseIncome;
    public float effectiveIncome;       //income - workers
    public float expectedIncome;        //Used to determine size of income display
    public float powerCooldown;
    public int workers;
    public int maxWorkers;
    public int killedWorkers;           //Permanently lowers maximum worker count
    public int assignedWorkers; 
    public MapTile selectedTile;
    public bool tileSelected;

    [Header("UI")]
    public Text moneyDisplay;
    public Text centDisplay;
    public Text incomeDisplay;
    public Text numWorkers;
    public bool menuOpen;
    public bool holdingMenu;
    public TypeText tileIndicator;

    //UI display bars for the player to alter
    public MoneyBar mb;
    public WorkersBar wb;

    //Flashes at end/when player tries to do something they can't
    public ScreenFlash flash;
    public Text warningMes;
    [HideInInspector]
    public float warningAlpha; public Color warningTarColor;
    private Color warningColor;
    protected float warningSpeed;

    //Determines the appearance of money
    protected Color curColor;
    protected Color moneyPrimaryColor;
    public Color moneyNegativeColor;
    public float moneyLerpNumber;

    [Header("Controls")]
    public GameObject pointer;
    public Vector3 ls;
    public Vector3 wls;
    public ParticleSystem cursor;
    public ParticleSystem cursorClick;
    public GameObject wheelObject;
    public SelectionWheel wheelCode;
    public float targetOffset;
    public float curOffset;
    public float xSpeed;
    public float ySpeed;
    public float camSpeed;
    protected bool buttonPressed;

    //Delay between subsequent button presses for movement
    public float pressTimeDelayMax;
    protected float pressTimeDelay;

    //These lists keep track of tiles on which the player has left farms or workers
    public List<MapTile> workQueue = new List<MapTile>();
    public List<MapTile> farmQueue = new List<MapTile>();
    private int wqIndex;
    private int fqIndex;

    //used for grid snapping w/ pad controls
    protected bool stickMoved;
    protected int stickDir;
    protected MapTile tempMap;
    protected float stickHoldTimer;
    public float stickHoldMax;
    public float stickHoldMin;
    protected bool snapped;
    public bool end;

    [Header("Audio SFX")]
    public AudioClip cymbal;
    public AudioClip denied;
    public AudioClip cashRegister;
    public AudioClip cdReady;
    public AudioClip placeWorker;
    public AudioClip takeWorker;


    // Use this for initialization
    void Start()
    {
        //Sets the standard positions for the set variables
        menuOpen = false;
        income = baseIncome;
        ls = pointer.transform.localScale;
        wls = wheelObject.transform.localScale;
        workers = maxWorkers - killedWorkers;
        destination = selectedTile;
        curPos = selectedTile.gridPos;

        //Checks if the player is meant to be using pad controls
        if (!pad)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        //Initializes Rewired
        player = ReInput.players.GetPlayer(pNum-1);

        //Finds the maximum & minimum positions
        int ms = GameManager.gm.mapSize - 1;
        minX = GameManager.gm.curMap[0, 0].gameObject.transform.position.x - mmAdjust;
        maxX = GameManager.gm.curMap[ms, ms].gameObject.transform.position.x - mmAdjust;
        minY = GameManager.gm.curMap[0, ms].gameObject.transform.position.y - mmAdjust;
        maxY = GameManager.gm.curMap[ms, 0].gameObject.transform.position.y + mmAdjust;

        //Pulls data carried over from previous
        if(DataHolder.dh != null)
        {
            Initialize();
        }
    }

    //GUI and Variable functions kept here:
    private void OnGUI()
    {
        //Alters the Money Bar key values
        mb.money = effectiveIncome;
        mb.maxMoney = expectedIncome;

        //Alters the Worker Bar's key values
        wb.numWorkers = maxWorkers;
        wb.usedWorkers = assignedWorkers;

        if (GameManager.gm.gameStart)
        {
            //Increments money based on income
            money += effectiveIncome * Time.deltaTime * 10;

            //Adjusts the player's income, as well as the color of the display
            //effectiveIncome = income - assignedWorkers;

            //Adjusts the color of the actual money display
            curColor = Color.Lerp(Color.red, moneyPrimaryColor, Mathf.Clamp01(moneyLerpNumber));

            //Causes the money color to lerp
            if(moneyLerpNumber <= 1)
            {
                moneyLerpNumber += Time.deltaTime;
            }

            //Determines the normal, base color of money display
            if(money < 0)
            {
                moneyPrimaryColor = moneyNegativeColor;
            }
            else
            {
                moneyPrimaryColor = Color.white;
            }

            //Sets the money displays to match color
            moneyDisplay.color = curColor;
            centDisplay.color = curColor;
        }

        //Changes all of the UI indicators here:
        numWorkers.text = "" + (workers-assignedWorkers);
        moneyDisplay.text = "$" + (int)money;
        centDisplay.text = "." + (Mathf.Abs((int)((money - (int)money)*100))).ToString("D2");

        //Changes the income display based on effective income value
        if(effectiveIncome > 0)
        {
            incomeDisplay.text = "$" + (int)effectiveIncome;
            incomeDisplay.color = Color.white;
        }
        else if (effectiveIncome == 0)
        {
            incomeDisplay.text = "$" + (int)effectiveIncome;
        }
        else
        {
            incomeDisplay.text = "-$" + (int)Mathf.Abs(effectiveIncome);
            incomeDisplay.color = Color.grey;
        }

        //Alters the warning message
        if(warningAlpha > 0f)
        {
            //Warning Message changes colors
            warningColor = Color.Lerp(Color.white, Color.magenta, Mathf.PingPong(Time.time* warningSpeed, 1));

            warningMes.color = new Color(warningColor.r, warningColor.g, warningColor.b, warningAlpha);

            //Decrements the warning alpha
            warningAlpha -= Time.deltaTime;
        }
    }
	
	// Update is called once per frame
	void Update () {
        workers = maxWorkers - killedWorkers;
        if (!GameManager.gm.moveLocked)
        {
            if (aiControlled)
            {
                aiControl();
            }
            else
            {
                //Determines which control scheme in order to use
                if (pad)
                {
                    padControl();
                    wheelCode.threshold = 0.2f;
                }
                else
                {
                    mouseControl();

                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        Cursor.lockState = CursorLockMode.None;
                    }
                }
            }
        }
        else
        {
            //Goes back to character select after the game is over
            if (GameManager.gm.gameFinish)
            {
                if (player.GetButtonDown("Start") || player.GetButtonDown("A"))
                {
                    GameManager.gm.transBack = true;
                }
            }
        }

        checkMap();

        //If a tile is selected, adjust the text
        if(tileSelected)
        {
            changeTypeText();
        }

        //...When the round is over entirely...
        if (GameManager.gm.gameFinish && !end)
        {
            flash.fade(Color.black, 2);
            print("Player " + pNum + " called fade!");
            end = true;
        }
	}

    //--------------------------------------------------------------------------------------------------------------------------------------------------
    //PLAYER CONTROL AND VARIABLE FUNCTIONS LISTED BELOW
    //--------------------------------------------------------------------------------------------------------------------------------------------------


    //iterates over the map, and counts the player's income
   /* public void checkIncome()
    {
        float x = baseIncome; //Starts with player's base-level income
        int ms = GameManager.gm.mapSize;
        MapTile[,] map = GameManager.gm.curMap;

        //Iterates over the map
        for(int i = 0; i < ms; i++)
        {
            for(int j = 0; j < ms; j++)
            {
                //If the tile is growing and the player controls it
                if ((map[i, j].growing) && map[i, j].controllingPlayer == pNum)
                {
                    //x += map[i, j].nearWater;
                    if(map[i,j].growing)
                    {
                        //If an opposing worker is on the tile, income is reduced
                        if(map[i,j].opposingWorker == 0)
                        {
                            x += (1f + (float)(map[i, j].nearWater * map[i, j].farmLevel + map[i, j].nearFarm));
                        }
                        else
                        {
                            x += ((float)(map[i, j].nearWater * map[i, j].farmLevel + map[i, j].nearFarm))/2f;
                        }

                        //Applies a 25% bonus if planted on Ashland
                        if(map[i,j].ashBonus && x > 0)
                        {
                            x *= 1.25f;
                        }
                        
                    }
                }
            }
        }
        income = x;
    }*/

    //Allows the player to input control with the mouse
    public void mouseControl()
    {

        //tracks mouse position, and gives it an ajusted value
        //mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos = new Vector2(mousePos.x + Input.GetAxis("Mouse X"), mousePos.y + Input.GetAxis("Mouse Y"));

        //Takes the player's mouse position, but only if mouse is not clicked
        if (!(Input.GetMouseButton(1)) && !(Input.GetMouseButton(0)))
        {
            posX = mousePos.x;
            posY = mousePos.y;
        }
        else
        {
            holdingMenu = true;
            menuOpen = true;
        }

        //Uses the WASD keys to move the camera:
        //This code  controls the Horizontal Axis:
        if ((pCam.gameObject.transform.position.x + (player.GetAxis("Horizontal") * Time.deltaTime * camSpeed) > -2)
            && (pCam.gameObject.transform.position.x + (player.GetAxis("Horizontal") * Time.deltaTime * camSpeed) < 48))
        {
            
            pCam.gameObject.transform.position = new Vector3(pCam.gameObject.transform.position.x + (player.GetAxis("Horizontal") * Time.deltaTime * camSpeed), pCam.gameObject.transform.position.y, pCam.gameObject.transform.position.z);
        }
        else
        {
            //Resets position if too far to the left...
            if(pCam.gameObject.transform.position.x < -2)
            {
                pCam.gameObject.transform.position = new Vector3(-2f, pCam.transform.position.y, pCam.transform.position.z);
            }

            //Resets if too far to the right...
            if (pCam.gameObject.transform.position.x > 48)
            {
                pCam.gameObject.transform.position = new Vector3(48f, pCam.transform.position.y, pCam.transform.position.z);
            }
        }

        //This code controls the Vertical Axis
        if ((pCam.gameObject.transform.position.y - (player.GetAxis("Vertical") * Time.deltaTime * camSpeed) > -10)
            && (pCam.gameObject.transform.position.y - (player.GetAxis("Vertical") * Time.deltaTime * camSpeed) < 10))
        {
            pCam.gameObject.transform.position = new Vector3(pCam.gameObject.transform.position.x, pCam.gameObject.transform.position.y - (player.GetAxis("Vertical") * Time.deltaTime * camSpeed), pCam.gameObject.transform.position.z);
        }

        if(!GameManager.gm.moveLocked)
        {
            if (selectedTile != null && player.GetAxis("Horizontal") == 0 && player.GetAxis("Vertical") == 0)
                pCam.lerpCam(selectedTile.transform.position);          
        }

        //Allows the user to zoom in/out using the mousewheel
        float scroll = player.GetAxis("Zoom");
        if (scroll != 0f)
        {
            float zt = pCam.zoomTar - pCam.zoomSpeed * scroll;
            zt = Mathf.Clamp(pCam.zoomTar, pCam.zoomMin, pCam.zoomMax); //Clamps amount of zoom
            pCam.zoomTar = zt;
        }


        //Takes the player's mouse position, but only if mouse is not clicked
        if ((!(Input.GetMouseButton(1)) && !(Input.GetMouseButton(0))) || (!GameManager.gm.gameStart))
        {
            holdingMenu = false;
            //cursor.Play();
            cursorClick.Stop();
            pointer.SetActive(true);

            //Does the selection wheel action
            if (menuOpen)
            {
                menuOpen = false;
                doSelect(wheelCode.selectNum);
                mousePos = new Vector2(posX, posY);
            }

            //Moves the mosue cursor to follow the world position of the mouse
            cursor.transform.position = new Vector3(posX, posY, -2);
            cursorClick.transform.position = new Vector3(posX, posY, -2);

            //Changes the scale offset
            if (curOffset < 10f)
            {
                curOffset += Time.deltaTime * 60f;
            }
            if (curOffset >= 10f)
            {
                wheelObject.SetActive(false);
                curOffset = 10f;
            }
        }
        else
        { //Changes the color of the particle while clicked
            cursor.Stop();
            cursorClick.Play();

            //Left Click --> Select Worker Actions
            if ((Input.GetMouseButton(0)) && (wheelObject.active == false))
            {
                wheelCode.lClick = true;
            }
            //Right Click --> Purchase Actions
            else if (Input.GetMouseButton(1) && (wheelObject.active == false))
            {
                wheelCode.lClick = false;
            }

            //Sets the selection wheel to active
            wheelObject.SetActive(true);
            pointer.SetActive(false);

            if (selectedTile.ahead == true)
            {
                wheelObject.transform.position = new Vector3(pointer.transform.position.x, pointer.transform.position.y, -12f);
                wheelCode.baseX = posX;
                wheelCode.baseY = posY;
            }


            //Changes the scale offset
            if (curOffset > targetOffset)
            {
                curOffset -= Time.deltaTime * 64;
            }
            if (curOffset < targetOffset)
            {
                curOffset = targetOffset;
            }

        }



       //Scales UI objects to match the current camera zoom
       pointer.transform.localScale = ls * (pCam.zoomTar / pCam.zoomMin / 1.6f);
        wheelObject.transform.localScale = wls * (pCam.zoomTar / pCam.zoomMin) * curOffset;

        //Plants the seed/Performs action if the player hits the A button
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (wheelObject.active == false)
            {
                doSelect(4);
            }

            if (wheelObject.active == true)
            {
               doSelect(wheelCode.selectNum);
            }
        }
    }//End mouseControl()

    //Controls with a GamePad instead of Keyboard/Mouse
    public void padControl()
    {
        //Won't move until snap timer is over
        if(stickHoldTimer > stickHoldMax-stickHoldMin || menuOpen)
        {
            //tracks mouse position, and gives it an ajusted value
            mousePos = new Vector2(posX + player.GetAxis("Horizontal") * Time.deltaTime * xSpeed, posY - player.GetAxis("Vertical") * Time.deltaTime * ySpeed);
        }




        //Moves the camera with the right stick
        if (!menuOpen)
        {
            //Camera follows the stick movement...
            if (!snapped)
            {
                if((Mathf.Abs(pCam.gameObject.transform.position.x) + Mathf.Abs(player.GetAxis("Horizontal 2") * Time.deltaTime * camSpeed) < Mathf.Abs(posX) + maxCamDist )
                && (Mathf.Abs(pCam.gameObject.transform.position.x) + Mathf.Abs(player.GetAxis("Horizontal 2") * Time.deltaTime * camSpeed) > Mathf.Abs(posX) - maxCamDist))
                    pCam.gameObject.transform.position = new Vector3(pCam.gameObject.transform.position.x + player.GetAxis("Horizontal 2") * Time.deltaTime * camSpeed, pCam.gameObject.transform.position.y, -10);

                if (Mathf.Abs(pCam.gameObject.transform.position.y) - Mathf.Abs(player.GetAxis("Vertical 2") * Time.deltaTime * camSpeed) < Mathf.Abs(posY) + maxCamDist)
                    pCam.gameObject.transform.position = new Vector3(pCam.gameObject.transform.position.x, pCam.gameObject.transform.position.y - player.GetAxis("Vertical 2") * Time.deltaTime * camSpeed, -10);

                //If the player is not using the right stick at all...
                if(player.GetAxisRaw("Horizontal 2") == 0 && player.GetAxisRaw("Vertical 2") == 0)
                {
                    snapped = true;
                }
            }
            //...Unless currently set to snap!
            else
            {
                //If movement is not currently disabled...
                if (!GameManager.gm.moveLocked)
                {
                    //If not touching left stick, move to selected tile
                    if (!stickMoved && selectedTile != null)
                    {
                        pCam.lerpCam(selectedTile.transform.position);
                    }
                    //If touching left stick, move to cursor
                    else
                    {
                        pCam.lerpCam(cursor.transform.position);
                    }
                }

                //If the player is using the second stick, make "snapped" false
                if (player.GetAxisRaw("Horizontal 2") != 0 || player.GetAxisRaw("Vertical 2") != 0)
                {
                    snapped = false;
                }
            }
        }

        //Camera influences cursor position as well
        // mousePos = new Vector2(mousePos.x + Input.GetAxis("Horizontal2" + pNum) * Time.deltaTime * xSpeed, mousePos.y - Input.GetAxis("Vertical2" + pNum) * Time.deltaTime * ySpeed);
        //lockCursor();

        //Takes the player's mouse position, but only if triggers are not held
        if (!player.GetButton("Left Trigger") && !player.GetButton("Right Trigger"))
        {
            //If the player has selected a tile, attempts to snap
            if (selectedTile != null)
            {
                gridSnap();
            }

            posX = mousePos.x;
            posY = mousePos.y;

            //Clamps values to maximum & minimum
            if(posX > maxX)
            {
                posX = maxX;
            }else if (posX < minX)
            {
                posX = minX;
            }
            if (posY > maxY)
            {
                posY = maxY;
            }
            else if(posY < minY)
            {
                posY = minY;
            }
        }
        //If the triggers ARE held...
        else
        {
            //The Radial Menus are activated
            holdingMenu = true;
            menuOpen = true;
            snapped = false;

            //The tile selection overlay is activated
            if (selectedTile != null && selectedTile.ahead == true)
            {
                wheelObject.transform.position = new Vector3(pointer.transform.position.x, pointer.transform.position.y, -12f);
                wheelCode.baseX = posX;
                wheelCode.baseY = posY;
            }
        }

        //If the triggers are not held...
        if (!player.GetButton("Right Trigger") && !player.GetButton("Left Trigger"))
        {
            //The Radial menus and other UI elements are disabled...
            holdingMenu = false;

            //Cursor only active if player is moving stick
            if (stickHoldTimer > stickHoldMin)
            {
                cursor.Play();
            }
            else
            {
                cursor.Stop();
            }

            cursorClick.Stop();
            pointer.SetActive(true);

            //TIf the radial menus had previous been open...
            if (menuOpen)
            {
                menuOpen = false;
                mousePos = new Vector2(posX, posY);
            }

            //Moves the mosue cursor to follow the world position of the mouse
            cursor.transform.position = new Vector3(posX, posY, -2);
            cursorClick.transform.position = new Vector3(posX, posY, -2);

            //Changes the scale offset
            if (curOffset < 10f)
            {
                curOffset += Time.deltaTime * 60f;
            }
            if (curOffset >= 10f)
            {
                wheelObject.SetActive(false);
                curOffset = 10f;
            }
        }
        else
        { 
            //Activates the player's power
            if(player.GetButton("Left Trigger") && player.GetButton("Right Trigger"))
            {
                //If your power is off cooldown....
                if(powerCooldown <= 0f)
                {
                    //Activation sequence only plays if not currently active
                    if (GameManager.gm.cutInActive)
                    {
                        powerBuffered = true;
                    }
                    else
                    {
                        activatePower(charNum);
                        
                    }
                }
                else
                {
                    AudioManager.am.play(denied);
                    warningMes.text = "COOLODOWN:  " + (int)(powerCooldown+ GameManager.gm.returnPlayerPower(pNum) + 1)+"s";
                    warningAlpha = 3.0f;
                }   
            }


            //Sanps camera to the selection wheel
            pCam.lerpCam(wheelObject.transform.position);
            posX = wheelObject.transform.position.x;
            posY = wheelObject.transform.position.y;

            //Changes the scale offset
            if (curOffset > targetOffset)
            {
                curOffset -= Time.deltaTime * 64;
            }
            if (curOffset < targetOffset)
            {
                curOffset = targetOffset;
            }

        }

        //Checks if power can be activated now...
        if (powerBuffered)
        {
            if(!GameManager.gm.cutInActive)
            {
                activatePower(charNum);
                powerBuffered = false;
            }
        }
        //If the player's power is on cooldown...
        if (powerCooldown > 0f)
        {
            //Checks for the proper variable based on player number
            if (GameManager.gm.returnPlayerPower(pNum) <= 0f)
            {
                powerCooldown -= Time.deltaTime;
            }
        }

        pointer.transform.localScale = ls * (pCam.zoomTar / pCam.zoomMin / 1.6f);
        wheelObject.transform.localScale = wls * (pCam.zoomTar / pCam.zoomMin) * curOffset;

        //Plants the seed/Performs action if the player hits the A button - Input.GetAxisRaw("Seed" + pNum) != 0
        if ( player.GetButtonDown("A"))
        {
            if (wheelObject.active == false) {
                doSelect(4);
            }
            
            if(wheelObject.active == true)
            {
                doSelect(wheelCode.selectNum);
            }
        }
        //Remove Worker/Fire Worker - Input.GetAxisRaw("Y"+pNum) != 0
        else if (player.GetButtonDown("Y") && wheelObject.active == false)
        {
            /*/If the game hasn't started, this button readies/unreadies
            if (!GameManager.gm.gameStart)
            {
                doSelect(0);
            }*/

            //If the tile is yours, and has a worker, removes them
            /*else*/ if ((selectedTile.workerAssigned != 0 && selectedTile.controllingPlayer == pNum)
                ||(selectedTile.opposingWorker > 0) && (pNum != selectedTile.controllingPlayer))
            {
                doSelect(1);
            }

            //Otherwise, moves to the next tile in the Queue
            else if(workQueue.Count > 0) 
            {
                bool tileFound = false;

                //First, searches for a tile w/ an idle worker
                foreach(MapTile m in workQueue)
                {
                    if((m.selected == 0 || m.selected == pNum) && m.idle)
                    {
                        selectedTile.ahead = false;
                        selectedTile.selected = 0;
                        selectedTile = m;
                        selectedTile.ahead = true;
                        selectedTile.selected = pNum;
                        snapped = true;
                        tileFound = true;
                        break;
                    }
                }

                //If no idle tile is found, goes for the next one
                if (!tileFound)
                {
                    foreach (MapTile m in workQueue)
                    {
                        if (m.selected == 0 || m.selected == pNum)
                        {
                            selectedTile.ahead = false;
                            selectedTile.selected = 0;
                            selectedTile = m;
                            selectedTile.ahead = true;
                            selectedTile.selected = pNum;
                            snapped = true;
                        }
                    }
                }
            }

            //If tile has no worker, and there is nothing in the queue...
            else
            {
                AudioManager.am.play(denied);
                warningMes.text = "NO WORKERS ASSIGNED!";
                warningAlpha = 3.0f;
                flash.flash(Color.red);
            }
        }
        //Hires a digger, and assigns them to the field - Input.GetAxisRaw("X" + pNum)
        else if (player.GetButtonDown("X") && wheelObject.active == false && (assignedWorkers <= workers))
        {
            //Then attempts to assign them to the field
            doSelect(2); 
        }
        //Hires a planter, and assigns them to the field - Input.GetAxisRaw("B" + pNum) != 0
        else if ( player.GetButtonDown("B") && wheelObject.active == false /*&& (assignedWorkers < workers)*/)
        {          
            //Then attempts to assign them to the field
            doSelect(3);          
        }

        //How the player readies up before the game starts
        if(player.GetButtonDown("Start") && !GameManager.gm.gameStart)
        {
            doSelect(0);
        }

        //Dpad controls cause the cam to move one tile at a time
        if (player.GetButtonDown("Up-Dpad") && pressTimeDelay <= 0)
        {
            findNextTile(4);
            snapped = true;
            pressTimeDelay = pressTimeDelayMax;
        }
        else if (player.GetButtonDown("Down-Dpad") && pressTimeDelay <= 0)
        {
            findNextTile(3);
            snapped = true;
            pressTimeDelay = pressTimeDelayMax;
        }
        else if (player.GetButtonDown("Left-Dpad") && pressTimeDelay <= 0)
        {
            findNextTile(1);
            snapped = true;
            pressTimeDelay = pressTimeDelayMax;
        }
        else if (player.GetButtonDown("Right-Dpad") && pressTimeDelay <= 0)
        {
            findNextTile(2);
            snapped = true;
            pressTimeDelay = pressTimeDelayMax;
        }
        if(pressTimeDelay > 0f)
        {
            pressTimeDelay -= Time.deltaTime;
        }

        //Bumpers will go to the next tile contained in the relevant list
        //Left Bumper goes backward in worker list
        if(player.GetButtonDown("Left Bumper"))
        {
            MapTile tm = prevInList(ref wqIndex, workQueue);
            if(tm != null && tm.selected == 0)
            {
                selectedTile.ahead = false;
                selectedTile.selected = 0;
                selectedTile = tm;
                selectedTile.ahead = true;
                selectedTile.selected = pNum;
                snapped = true;
            }
            else
            {
                AudioManager.am.play(denied);
                warningMes.text = "NO WORKERS QUEUED!";
                warningAlpha = 3.0f;
            }
        }

        //Right bumper goes forward in worker list
        if(player.GetButtonDown("Right Bumper"))
        {
            MapTile tm = nextInList(ref wqIndex, workQueue);
            if (tm != null && tm.selected == 0)
            {
                selectedTile.ahead = false;
                selectedTile.selected = 0;
                selectedTile = tm;
                selectedTile.ahead = true;
                selectedTile.selected = pNum;
                snapped = true;
            }
            else
            {
                AudioManager.am.play(denied);
                warningMes.text = "NO WORKERS QUEUED!";
                warningAlpha = 3.0f;
            }
        }


    }//End PadControl()

    private void gridSnap()
    {
        //Functions listed below are used for Pad Grid-Snapping
        if (player.GetAxis("Horizontal") != 0 || player.GetAxis("Vertical") != 0)
        {
            //Takes the currently selected tile
            if (!stickMoved)
            {
                tempMap = selectedTile;
            }
            stickMoved = true;
            stickDir = findStickDir(player.GetAxis("Horizontal"), player.GetAxis("Vertical"));
            stickHoldTimer += Time.deltaTime;
        }
        else
        {
            //If the selected tile hasn't changed
            if (tempMap == selectedTile && stickMoved && stickHoldTimer < stickHoldMax && stickHoldTimer > stickHoldMin)
            {
                findNextTile(stickDir, tempMap);
                snapped = true;
            }
            stickMoved = false;
            stickHoldTimer = 0;
        }
        //Moves pointer to new tile
        pointer.transform.position = new Vector3(selectedTile.gameObject.transform.position.x, selectedTile.gameObject.transform.position.y, selectedTile.gameObject.transform.position.z - 12f);

        //Snaps to currently selected tile
        if (selectedTile != null && player.GetAxis("Horizontal") == 0 && player.GetAxis("Vertical") == 0
         && player.GetAxis("Horizontal 2") == 0 && player.GetAxis("Vertical 2") == 0 )
        {
            mousePos = new Vector2(selectedTile.transform.position.x, selectedTile.transform.position.y);
        }

    }

    //Adjusts the text on the map indicator
    public void changeTypeText()
    {
        //changes the indicator sprite if there is a worker present
        if (selectedTile.workerAssigned > 0)
        {
            tileIndicator.worker.sprite = tileIndicator.assigned;
        }
        else
        {
            tileIndicator.worker.sprite = tileIndicator.empty;
        }

        //Hole
        if (selectedTile.tileNum == 0)
        {
            tileIndicator.text.text = "Empty Tile";
        }
        //Grass Tile
        else if (selectedTile.tileNum == 1)
        {
            tileIndicator.text.text = "Grass Tile";
        }
        else if (selectedTile.tileNum == 2)
        {
            tileIndicator.text.text = "Crop Tile";
        }
        //Rock tile
        else if (selectedTile.tileNum == 3)
        {
            tileIndicator.text.text = "Rock Tile";
        }
        else if (selectedTile.tileNum == 4)
        {
            tileIndicator.text.text = "Spring Tile";
        }
        //Water tile
        else if (selectedTile.tileNum == 5)
        {
            tileIndicator.text.text = "Water Tile";
        }
        // Water Draining...
        else if (selectedTile.tileNum == 6)
        {
            tileIndicator.text.text = "Water Tile (Draining)";
        }
        //Fire
        else if (selectedTile.tileNum == 7)
        {
            tileIndicator.text.text = "Fire Tile";
        }
        //ash
        else if (selectedTile.tileNum == 8)
        {
            tileIndicator.text.text = "Ash Tile";
        }
    }

    //Checks to see if a tile is selected
    public void checkMap()
    {
        if (!tileSelected)
        {
            pointer.SetActive(false);
        }
        if (pointer.active == false)
        {
            tileSelected = false;
        }

        //iterates over the map, and checks if a tile is selected
        for (int i = 0; i < GameManager.gm.mapSize; i++)
        {
            for (int j = GameManager.gm.mapSize - 1; j >= 0; j--)
            {
                if ((GameManager.gm.curMap[i, j].selected == pNum) && (tileSelected == false))
                {
                    tileSelected = true;
                    selectedTile = GameManager.gm.curMap[i, j];
                    selectedTile.ahead = true;
                    pointer.SetActive(true);
                    pointer.transform.position = new Vector3(selectedTile.gameObject.transform.position.x, selectedTile.gameObject.transform.position.y, selectedTile.gameObject.transform.position.z - 12f);

                    //changes the indicator sprite if there is a worker present
                    if (selectedTile.workerAssigned > 0 && selectedTile.controllingPlayer == pNum)
                    {
                        tileIndicator.worker.sprite = tileIndicator.assigned;
                    }
                    else if (selectedTile.opposingWorker > 0 && selectedTile.controllingPlayer != pNum)
                    {
                        tileIndicator.worker.sprite = tileIndicator.assigned;
                    }
                    else
                    {
                        tileIndicator.worker.sprite = tileIndicator.empty;
                    }

                }
                else if ((GameManager.gm.curMap[i, j].selected == 0) && (GameManager.gm.curMap[i, j].ahead == true))
                {
                    //tileSelected = false;
                    GameManager.gm.curMap[i, j].ahead = false;
                    //pointer.SetActive(false);
                }
            }
        }
        //If no tile is currently selected
        if (selectedTile != null) { 
            if (selectedTile.selected != pNum)
            {
                tileSelected = false;
                selectedTile.ahead = false;
                pointer.SetActive(false);
            }
         }
    }

    //Locks the cursor within view distance of the camera
    private void lockCursor()
    {
        if (mousePos.x > pCam.gameObject.transform.position.x + 10.3f)
        {
            mousePos.x = pCam.gameObject.transform.position.x + 10.3f;
        }
        else if (mousePos.x < pCam.gameObject.transform.position.x - 10.3f)
        {
            mousePos.x = pCam.gameObject.transform.position.x - 10.3f;
        }
        if (mousePos.y > pCam.gameObject.transform.position.y + 11.6f)
        {
            mousePos.y = pCam.gameObject.transform.position.y + 11.6f;
        }
        else if (mousePos.y < pCam.gameObject.transform.position.y - 11.6f)
        {
            mousePos.y = pCam.gameObject.transform.position.y - 11.6f;
        }
    }

    //Plants a seed in the ground, or upgrades an existing seed
    private void plantSeed()
    {
        //If the player has a seed, and the tile isn't planted...
        if (((selectedTile.tileNum == 1)||(selectedTile.tileNum ==  8))&& (selectedTile.controllingPlayer == 0 || selectedTile.controllingPlayer == pNum))
        {
            bool temp = false;

            /*//Plants a seed if you have one in reserve...
            if((seeds > 0)) { 
                seeds -= 1;
                temp = true;
            }*/

            //...or charges you if you don't.
            if(!temp && money >= 100)
            {
                money -= 100;
                temp = true;
            }
            //Denies the player if they do not have enough money
            else if(money < 100)
            {
                AudioManager.am.play(denied);
                warningMes.text = "NOT ENOUGH MONEY!";
                warningAlpha = 3.0f;
                moneyLerpNumber = 0f;
                return;
            }

            //checks if tile has a worker already assigned.
            if (temp)
            {
                selectedTile.tileNum = 2;
                selectedTile.plantingPlayer = pNum;
                farmQueue.Add(selectedTile);

               /* if (selectedTile.workerAssigned == 2)
                {
                    selectedTile.growing = true;
                }*/
            }
        }
        //If the tile is already a farm, and the player is in control of it...
        else if(selectedTile.tileNum == 2 && (selectedTile.controllingPlayer == 0 || selectedTile.controllingPlayer == pNum))
        {
            //...and the farm isn't max level, and the player has enough money
            if(selectedTile.farmLevel < 3 && money > 200 * selectedTile.farmLevel)
            {
                money -= 200 * selectedTile.farmLevel;
                selectedTile.farmLevel += 1;

                //Upgrading the tile resets it
                selectedTile.cropLevel = 0;
                selectedTile.timer = 0;
                selectedTile.textPopped = false;
                //checkIncome();
                return;
            }
            else
            {
                AudioManager.am.play(denied);
                //flash.flash(Color.red);
                
                //Displays appropriate warning
                if(selectedTile.farmLevel >= 3)
                {
                    warningMes.text = "FARM IS MAX LEVEL!";
                    warningAlpha = 3.0f;
                    flash.flash(Color.red);
                }
                else
                {
                    warningMes.text = "NOT ENOUGH MONEY!";
                    warningAlpha = 3.0f;
                    moneyLerpNumber = 0f;
                }
                return;
            }
        }

        //If the action cannot complete for any reason...
        else
        {
            AudioManager.am.play(denied);
            //flash.flash(Color.red);
            warningMes.text = "CAN'T DO THAT HERE!";
            warningAlpha = 3.0f;
        }


       // checkIncome();
    }

    //Performs a given action
    public void doSelect(int selectNum)
    {
        //Only performs first action if the game hasn't started
        if (!GameManager.gm.gameStart && GameManager.gm.bothReady)
        {
            selectNum = 0;
        }
  
        //Do nothing, but flips ready switch
        if (selectNum == 0)
        {
            //Cannot unready after both players are ready
            if (!GameManager.gm.bothReady)
            {
                ready = !ready;
                if (ready)
                {
                    AudioManager.am.play(cymbal);
                }
            }         
            return;
        }
        //Remove Worker
        else if (selectNum == 1)
        {
            //If the player controlling the tile is the same, and the tile has a worker attatched
            if ((selectedTile.workerAssigned > 0) && (pNum == selectedTile.controllingPlayer))
            {
                //Farm tiles keep growing
                if(selectedTile.tileNum != 2)
                {
                    //Resets the work timer on the tile
                    selectedTile.timer = selectedTile.timerMax;
                }

                //Removes worker from tile, and removes tile from queue
                selectedTile.workerAssigned = 0;
                assignedWorkers -= 1;
                //selectedTile.growing = false;
                workQueue.Remove(selectedTile);

                //Sets tile to neutral if there is nobody to contest it
                if(selectedTile.opposingWorker == 0)
                {
                    selectedTile.controllingPlayer = 0;
                }

                //Plays removal sound
                AudioManager.am.playTwoPlayerSound(takeWorker, 0.5f, (Vector2) selectedTile.transform.position);
            }
            //If the enemy is controlling the tile, and the player has assigned an opposing worker
            else if ((selectedTile.opposingWorker > 0) && (pNum != selectedTile.controllingPlayer))
            {
                selectedTile.opposingWorker = 0;
                assignedWorkers -= 1;
                workQueue.Remove(selectedTile);

                //Plays removal sound
                AudioManager.am.playTwoPlayerSound(takeWorker, 0.5f, (Vector2)selectedTile.transform.position);
            }
            //If there is nothing whatsoever to remove, plays denial
            else
            {
                AudioManager.am.play(denied);
                flash.flash(Color.red);
                warningMes.text = "NO WORKER TO REMOVE!";
                warningAlpha = 3.0f;
            }
            //checkIncome();
        }
        //Assign Digger
        else if (selectNum == 2)
        {
            bool acted = false;

            //Cannot place on spring tiles
            if (selectedTile.tileNum != 4)
            {
                //Assigns a worker to the tile, and places it under this players control
                if ((selectedTile.workerAssigned == 0) && (assignedWorkers < workers))
                {
                    //Resets the work timer on the tile
                    selectedTile.timer = selectedTile.timerMax;

                    //Assigns a digger to the tile
                    selectedTile.workerAssigned = 1;
                    selectedTile.controllingPlayer = pNum;
                    assignedWorkers += 1;
                    GameManager.gm.checkAdjacent((int)selectedTile.gridPos.x, (int)selectedTile.gridPos.y, pNum);
                    acted = true;
                }
                //If the player already controls the tile, and it has a planter, switches planter to digger
                else if ((selectedTile.workerAssigned == 2) && (selectedTile.controllingPlayer == pNum))
                {
                    selectedTile.workerAssigned = 1;
                    if (selectedTile.tileNum == 2)
                    {
                        //selectedTile.growing = false;
                    }
                    acted = true;
                }
                //If the tile is controlled by an enemy, assign worker to mess with them
                else if ((selectedTile.opposingWorker != 1) && (pNum != selectedTile.controllingPlayer) && selectedTile.controllingPlayer != 0 && (assignedWorkers < workers))
                {
                    //If the tile was empty before, increases worker assignment
                    if (selectedTile.opposingWorker == 0)
                    {
                        assignedWorkers += 1;
                    }
                    selectedTile.opposingWorker = 1;
                    GameManager.gm.spawnWarning(selectedTile.controllingPlayer, selectedTile.gameObject.transform);
                    acted = true;
                }
            }
            //Adds the relevant tile to the worker queue if something can be done
            if (acted)
            {
                //Ensures that tile doesn't exist in Work Queue, then adds to it
                workQueue.Remove(selectedTile);
                workQueue.Add(selectedTile);

                //Plays add sound
                AudioManager.am.playTwoPlayerSound(placeWorker, 0.5f, (Vector2)selectedTile.transform.position);
            }

            //If the player couldn't do any of the above, plays denial song
            //and flashes the screen
            else
            {
                AudioManager.am.play(denied);
                flash.flash(Color.red);

                //Displays relevant warning message
                if (selectedTile.tileNum == 4)
                {
                    warningMes.text = "CAN'T ASSIGN HERE!";
                }
                else if (assignedWorkers >= workers)
                {
                    warningMes.text = "NO MORE WORKERS!";
                }
                warningAlpha = 3.0f;
            }
        }
        //Assign Planter
        else if (selectNum == 3)
        {
            bool acted = false;

            //Cannot place on spring tiles
            if (selectedTile.tileNum != 4)
            {
                //If there are no workers currently on the tile...
                if ((selectedTile.workerAssigned == 0) && ((assignedWorkers < workers)))
                {
                    acted = true;
                    //Spawns warning immediately if tile was formerly under opponent control
                    if (selectedTile.controllingPlayer != pNum && selectedTile.controllingPlayer != 0)
                    {
                        GameManager.gm.spawnWarning(selectedTile.controllingPlayer, selectedTile.gameObject.transform);
                    }
                    //Otherwise, checks the adjacent tiles for important stuff
                    else
                    {
                        GameManager.gm.checkAdjacent((int)selectedTile.gridPos.x, (int)selectedTile.gridPos.y, pNum);
                    }

                    //Resets the work timer on the tile, unless it is a growing farm
                    if (!(selectedTile.tileNum == 2 && selectedTile.cropLevel < 3))
                    {      
                        selectedTile.timer = selectedTile.timerMax;
                    }


                    //Assigns a planter to the tile and places it under your control
                    selectedTile.workerAssigned = 2;
                    selectedTile.controllingPlayer = pNum;
                    GameManager.gm.searchShell();
                    assignedWorkers += 1;
                    if (selectedTile.tileNum == 2)
                    {
                        //selectedTile.growing = true;
                    }

                }
                //If the player already controls the tile, and it has a digger, switches digger to planter
                else if ((selectedTile.workerAssigned > 0) && (selectedTile.controllingPlayer == pNum))
                {
                    acted = true;
                    selectedTile.workerAssigned = 2;
                    if (selectedTile.tileNum == 2)
                    {
                        //selectedTile.growing = true;
                    }
                }
                //If the tile is controlled by an enemy, assign worker to mess with them
                else if ((selectedTile.opposingWorker != 2) && (pNum != selectedTile.controllingPlayer) && selectedTile.controllingPlayer != 0 && (assignedWorkers < workers))
                {
                    //If tile didn't already have an opposing worker, increase assignment num
                    if (selectedTile.opposingWorker == 0)
                    {
                        assignedWorkers += 1;
                    }
                    acted = true;
                    selectedTile.opposingWorker = 2;
                    GameManager.gm.spawnWarning(selectedTile.controllingPlayer, selectedTile.gameObject.transform);
                }
               // checkIncome();
            }

            //Adds the relevant tile to the worker queue if something can be done
            if (acted)
            {
                //Ensures that tile doesn't exist in Work Queue, then adds to it
                workQueue.Remove(selectedTile);
                workQueue.Add(selectedTile);

                //Plays add sound
                AudioManager.am.playTwoPlayerSound(placeWorker, 0.75f, (Vector2)selectedTile.transform.position);
            }

            //If the player couldn't do any of the above, plays denial song
            //and flashes the screen
            else
            {
                AudioManager.am.play(denied);
                flash.flash(Color.red);

                //Displays relevant warning message
                if (selectedTile.tileNum == 4)
                {
                    warningMes.text = "CAN'T ASSIGN HERE!";
                }
                else if (assignedWorkers >= workers)
                {
                    warningMes.text = "NO MORE WORKERS!";
                }
                warningAlpha = 3.0f;
            }
        }
        //Buy Seeds, and automatically attempts to plant it
        else if (selectNum == 4)
        {
            plantSeed();
        }

   
        //Fires all diggers!
        else if (selectNum == 5)
        {
            MapTile[] temp = new MapTile[workQueue.Count];
            int index = 0;

            //iterates over the entire queue
            foreach (MapTile m in workQueue)
            {
                //Remove worker differs depending on which player controls
                if (m.controllingPlayer == pNum && m.workerAssigned == 1)
                {
                    m.workerAssigned = 0;

                    //Lowers assigned worker count, then removes tile
                    assignedWorkers -= 1;
                    temp[index] = m;
                    index++;
                }

                else if (m.controllingPlayer != pNum && m.opposingWorker == 1)
                {
                    m.opposingWorker = 0;

                    //Lowers assigned worker count, then removes tile
                    assignedWorkers -= 1;
                    temp[index] = m;
                    index++;
                }
            }

            //Removes all affected tiles
            for(int i = 0; i < index; i++)
            {
                if(temp[i] != null)
                {
                    workQueue.Remove(temp[i]);
                }
            }

            //Clears the whole list
            //workQueue = new List<MapTile>();
        }

        // THESE CURRENTLY OBSOLETE - REPLACE THEM WITH SOMETHING!!!!!!!
        //Fires all planters
        else if (selectNum == 6)
        {

            MapTile[] temp = new MapTile[workQueue.Count];
            int index = 0;

            //iterates over the entire queue
            foreach (MapTile m in workQueue)
            {
                //Remove worker differs depending on which player controls
                if (m.controllingPlayer == pNum && m.workerAssigned == 2)
                {
                    m.workerAssigned = 0;

                    //Lowers assigned worker count, then removes tile
                    assignedWorkers -= 1;
                    temp[index] = m;
                    index++;
                }

                else if (m.controllingPlayer != pNum && m.opposingWorker == 2)
                {
                    m.opposingWorker = 0;

                    //Lowers assigned worker count, then removes tile
                    assignedWorkers -= 1;
                    temp[index] = m;
                    index++;
                }
            }

            //Removes all affected tiles
            for (int i = 0; i < index; i++)
            {
                if (temp[i] != null)
                {
                    workQueue.Remove(temp[i]);
                }
            }
        }
        selectNum = 0;
    }



    //Method picks out the next available tile
    //1 - UP, 2 - DOWN, 3 - LEFT, 4 - RIGHT
    public void findNextTile(int d, int tempX, int tempY)
    {
        //finds and assigns the next tile
        switch (d)
        {
            //Find the next tile NORTH-WEST
            case 1:
                if(tempY > 0)
                {
                    //Finds next map tile, and assigns if it isn't selected
                    MapTile temp = GameManager.gm.curMap[tempX, tempY - 1];
                    if(temp.selected == 0 || temp.selected == pNum)
                    {
                        selectedTile.ahead = false;
                        selectedTile.selected = 0;
                        temp.ahead = true;
                        temp.selected = pNum;
                        selectedTile = temp;
                    }else if(temp.selected != pNum)
                    {
                        findNextTile(1, tempX, tempY - 1);
                    }
                    break;
                }
                else { break; }
            //Find the next tile SOUTH-EAST
            case 2:
                if (tempY < GameManager.gm.mapSize - 1)
                {
                    //Finds next map tile, and assigns if it isn't selected
                    MapTile temp = GameManager.gm.curMap[tempX, tempY + 1];
                    if (temp.selected == 0)
                    {
                        selectedTile.ahead = false;
                        selectedTile.selected = 0;
                        temp.ahead = true;
                        temp.selected = pNum;
                        selectedTile = temp;
                    }
                    //If the tile is selected by the enemy, attempts to jump over
                    else if (temp.selected != pNum)
                    {
                        findNextTile(1, tempX, tempY + 1);
                    }
                    break;
                }
                else { break; }

            //Find the next tile SOUTH-WEST
            case 3:
                if (tempX > 0)
                {
                    //Finds next map tile, and assigns if it isn't selected
                    MapTile temp = GameManager.gm.curMap[tempX -1, tempY];
                    if (temp.selected == 0)
                    {
                        selectedTile.ahead = false;
                        selectedTile.selected = 0;
                        temp.ahead = true;
                        temp.selected = pNum;
                        selectedTile = temp;
                    }
                    //If the tile is selected by the enemy, attempts to jump over
                    else if (temp.selected != pNum)
                    {
                        findNextTile(1, tempX-1, tempY);
                    }
                    break;
                }
                else { break; }

            //Find the next tile NORTH-EAST
            case 4:
                if (tempX < GameManager.gm.mapSize - 1)
                {
                    //Finds next map tile, and assigns if it isn't selected
                    MapTile temp = GameManager.gm.curMap[tempX+1, tempY];
                    if (temp.selected == 0)
                    {
                        selectedTile.ahead = false;
                        selectedTile.selected = 0;
                        temp.ahead = true;
                        temp.selected = pNum;
                        selectedTile = temp;
                    }                   
                    //If the tile is selected by the enemy, attempts to jump over
                    else if (temp.selected != pNum)
                    {
                        findNextTile(1, tempX + 1, tempY);
                    }
                    break;
                }
                else { break; }
        }

    }
    //Overload for findNextTile()
    //Defaults to currently selected tile location
    public void findNextTile(int d)
    {
        /// Debug.Log("attmepting to snap: " +stickDir);
        findNextTile(d, (int)selectedTile.gridPos.x, (int)selectedTile.gridPos.y);
    }
    public void findNextTile(int d, MapTile m)
    {
       // Debug.Log("attmepting to snap: " + stickDir);
        findNextTile(d, (int)m.gridPos.x, (int)m.gridPos.y);
    }

    //takes stick inputs, and returns a direction
    public int findStickDir(float x, float y)
    {
        //if the horizontal axis is greter than the vertical...
        if(Mathf.Abs(x) > Mathf.Abs(y))
        {
            //Right
            if(x >= 0) { return 2;}
            //Left
            else{return 1;}
        }
        else
        {
            //Down
            if (y >= 0) { return 3; }
            //Up
            else { return 4; }
        }

    }

    //Cycles through and finds next available MapTile, after currently selected one
    public MapTile nextInList (ref int index, List<MapTile> list)
    {
        MapTile temp = null;

        //if the queue isn't empty...
        if(list.Count > 0)
        {
            //...increments the index...
            index+= 1;
            if (index >= list.Count || index < 0)
            {
                index = 0;
            }

            temp = list[index];
        }

        //...and returns the relevant tile
        return temp;
    }
    public MapTile prevInList (ref int index, List<MapTile> list)
    {
        MapTile temp = null;

        //if the queue isn't empty...
        if (list.Count > 0)
        {
            //...increments the index...
            index -= 1;
            if (index >= list.Count || index < 0)
            {
                index = list.Count-1;
            }

            temp = list[index];
        }

        //...and returns the relevant tile
        return temp;
    }

    //Actives a power based on the character the player has selected
    public void activatePower(int c)
    {
        //Cannot activate a power while it is still currently active
        if((pNum == 1 && GameManager.gm.p1Timer > 0f) || (pNum == 2 && GameManager.gm.p2Timer > 0f))
        {
            return;
        }

        //Norm's power slows time down
        if(c == Glossary.gs.character["Norm"] || c == Glossary.gs.character["NormF"])
        {
            GameManager.gm.actPower(pNum, 5f);
            GameManager.gm.maxTimeScale = 0.5f;
            powerCooldown = Glossary.gs.cooldowns[charNum];
        }

        //Rivan's power increases working rate if player is losing
        if(c == Glossary.gs.character["Rivan"])
        {
            GameManager.gm.actPower(pNum, 15f);
            powerCooldown = Glossary.gs.cooldowns[charNum];
        }

        //Flint sacrifices a worker to set the selected tile on fire
        if(c == Glossary.gs.character["Flint"])
        {
            //Can only ignite tile if you control it
            if(selectedTile.workerAssigned > 0 && selectedTile.controllingPlayer == pNum
                && killedWorkers < maxWorkers-1 && (selectedTile.tileNum == 1 || selectedTile.tileNum == 2))
            {

                //Activates the cut-in from the GameManger
                GameManager.gm.actPower(pNum, 10f, selectedTile);
                powerCooldown = Glossary.gs.cooldowns[charNum];

                //Kills the your worker
                killedWorkers++;
                selectedTile.workerAssigned = 0;
                selectedTile.controllingPlayer = 0;
                //selectedTile.ignite(1f);
                workQueue.Remove(selectedTile);
            }
            //Displays warning messages if...
            else
            {
                warningMes.text = "";

                //...There are too few workers left
                if(killedWorkers >= maxWorkers - 1)
                {
                    warningMes.text = "TOO FEW WORKERS!";
                }
                //...There is no worker on this tile.
                else if (selectedTile.workerAssigned == 0)
                {
                    warningMes.text = "NO WORKER TO BURN!";
                }
                //...The player doesn't control this tile.
                else if (selectedTile.controllingPlayer != pNum)
                {
                    warningMes.text = "TILE MUST BE YOURS!";
                }
                //...If it's the wrong type of tile.
                else if(selectedTile.tileNum != 1 && selectedTile.tileNum != 2)
                {
                    warningMes.text = "TILE NOT FLAMMABLE!";
                }

                warningAlpha = 3.0f;
                AudioManager.am.play(denied);
            }
        }

        //Cash borrows money, and goes into debt after a dely
        if(c == Glossary.gs.character["Cash"])
        {
            if(money > -2000)
            {
                money += 2000;
                GameManager.gm.actPower(pNum, 10f);
                powerCooldown = Glossary.gs.cooldowns[charNum];
                warningMes.text = "BORROWED $2000!";
                warningAlpha = 3.0f;
                AudioManager.am.play(cashRegister);
            }

            //Too far into debt to continue borrowing
            else
            {
                warningMes.text = "TOO FAR IN DEBT!";
                warningAlpha = 3.0f;
                AudioManager.am.play(denied);
            }


        }

        //TODO - DECIDE POWER FOR REGINA
        if(c == Glossary.gs.character["Regina"])
        {
            //If the tile is controlled by the enemy player
            if(selectedTile.controllingPlayer != pNum && selectedTile.workerAssigned > 0)
            {
                //Removes your worker if it is a stalemate tile
                if(selectedTile.opposingWorker > 0)
                {
                    selectedTile.opposingWorker = 0;
                    assignedWorkers -= 1;
                }
                GameManager.gm.actPower(pNum, 10f, selectedTile);
                powerCooldown = Glossary.gs.cooldowns[charNum];
            }
            //Power fails if...
            else
            {
                //...there is no worker on the tile
                if(selectedTile.workerAssigned == 0)
                {
                    warningMes.text = "NO WORKER HERE!";

                }
                //...the tile is already under your control
                if(selectedTile.controllingPlayer == pNum)
                {
                    warningMes.text = "ALREADY YOURS!";
                }

                    warningAlpha = 3.0f;
                    AudioManager.am.play(denied);
            }
        }

        //Raine changes the weather to Rain or STOPS active rain
        if(c == Glossary.gs.character["Raine"])
        {
            GameManager.gm.actPower(pNum, 30f);
            powerCooldown = Glossary.gs.cooldowns[charNum];
        }

        //Sui changes the current tile to a spring
        if (c == Glossary.gs.character["Sui"])
        {
            //Can only activate on a hole tile that is neutral or controlled by this player
            if((selectedTile.tileNum == 0 ||  selectedTile.tileNum == 5 || selectedTile.tileNum == 6)
                && (selectedTile.controllingPlayer == 0 || selectedTile.controllingPlayer == pNum))
            {
                GameManager.gm.actPower(pNum, 30f, selectedTile);
                powerCooldown = Glossary.gs.cooldowns[charNum];

                /*/Converts tile to a spring, then adds it to the spring list
                selectedTile.tileNum = 4;
                GameManager.gm.springTiles.Add(selectedTile);
                GameManager.gm.searchShell();*/
            }
            //Power fails if...
            else
            {
                //...The tile is ALREADY a spring
                if(selectedTile.tileNum == 4)
                {
                    warningMes.text = "ALREADY A SPRING!";
                }
                //...The tile is not a pit or water
                else if (selectedTile.tileNum != 0 || selectedTile.tileNum == 5 || selectedTile.tileNum == 6)
                {
                    warningMes.text = "MUST BE A HOLE!";
                }
                //...The tile is under enemy control
                else if (selectedTile.controllingPlayer != 0 || selectedTile.controllingPlayer != pNum)
                {
                    warningMes.text = "ENEMY CONTROLLED!";
                }


                warningAlpha = 3.0f;
                AudioManager.am.play(denied);
            }
            
        }

        //Narcia will sacrifice a worker to destroy the tile 
        //underneath them if the tile is in stalemate
        if (c == Glossary.gs.character["Narcia"])
        {
            if(selectedTile.opposingWorker > 0 && selectedTile.workerAssigned > 0)
            {
                //Activates GM
                GameManager.gm.actPower(pNum, 0f);

                //Attacker loses a unit, defender keeps it
                if(selectedTile.controllingPlayer == pNum)
                {
                    GameManager.gm.returnOppositePlayer(this).killedWorkers++;
                }
                else
                {
                    killedWorkers++;
                }
                //Destroys tile and removes the workers
                selectedTile.tileNum = 0;
                selectedTile.opposingWorker = 0;
                selectedTile.workerAssigned = 0;
                selectedTile.controllingPlayer = 0;

                powerCooldown = Glossary.gs.cooldowns[charNum];
            }
            //...The power fails if...
            else
            {
                //...The tile isn't a stalemate
                if(selectedTile.opposingWorker == 0 || selectedTile.workerAssigned == 0)
                {
                    warningMes.text = "NOT A STALEMATE!";
                    warningAlpha = 3.0f;
                    AudioManager.am.play(denied);
                }
                //...The tile wasn't originally the players
                else if (selectedTile.controllingPlayer != pNum)
                {
                    warningMes.text = "NOT YOUR TILE!";
                    warningAlpha = 3.0f;
                    AudioManager.am.play(denied);
                }
            }
        }

        //Rose will enahnce a spring tile, then switch to Thorn
        if(c == Glossary.gs.character["Rose"])
        {
            if(selectedTile.tileNum == 4)
            {
                charNum = Glossary.gs.character["Thorn"];
                GameManager.gm.actPower(pNum, 20f, selectedTile);
                powerCooldown = Glossary.gs.cooldowns[charNum];
            }
            else
            {
                warningMes.text = "MUST TARGET SPRING!";
                warningAlpha = 3.0f;
                AudioManager.am.play(denied);
            }
        }
        //Thorn will poison a spring tile, then switch to Rose
        if(c == Glossary.gs.character["Thorn"])
        {
            if(selectedTile.tileNum == 4)
            {
                charNum = Glossary.gs.character["Rose"];
                GameManager.gm.actPower(pNum, 20f, selectedTile);
                powerCooldown = Glossary.gs.cooldowns[charNum];
            }
            else
            {
                warningMes.text = "MUST TARGET SPRING!";
                warningAlpha = 3.0f;
                AudioManager.am.play(denied);
            }
        }

    }

    //Resets the player's important values
    public void Reset()
    {
        assignedWorkers = 0;
        killedWorkers = 0;

    }

    //Pulls relevant info from the DataHolder
    public void Initialize()
    {
        //Pulls player 1 data
        if (pNum == 1)
        {
            player = DataHolder.dh.p1;
            charNum = DataHolder.dh.p1Num;
            pad = DataHolder.dh.p1Pad;
        }

        //Pulls player 2 data
        else if (pNum == 2)
        {
            player = DataHolder.dh.p2;
            charNum = DataHolder.dh.p2Num;
            pad = DataHolder.dh.p2Pad;
        }
    }
}
