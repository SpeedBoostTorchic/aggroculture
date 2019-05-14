using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {

    [Header("Rewired & Control")]
    //Checks relevant player input methods
    public GameObject menuInput;
    public Player t1;
    public Player t2;
    public Player t3;   //Keyboard and Mouse
    public float inputCooldown;
    public float maxCooldown;
    public float speed;

    [Header("Visuals & States")]
    public GameObject logo;
    public float logoStateFlow;

    //Determines how the logo moves
    public Vector3 logoDefaultPos;
    public Vector3 logoTarPos;
    public Vector3 logoDefaultScale;
    public Vector3 logoTarScale;

    //Represent which part of the menu the player is currently in
    // 0 = "Press Start", 1 = "Single Player / Mulitplyer / Options", 2 = "1P vs 2P/1P vs Comp"
    public int state;
    public GameObject state0;
    public GameObject state1;
    public GameObject state2;
    public float stateAdjustmentSize;

    [Header("Buttons & Selection")]
    public int selected;
    public Button[] state1Buttons;
    public Button[] state2Buttons;

    public Text startGlow;
    private float glowAlpha;

    public ScreenFader sf;


    // Use this for initialization
    void Start () {
        t1 = ReInput.players.GetPlayer(0);
        t2 = ReInput.players.GetPlayer(1);
        t3 = ReInput.players.GetPlayer(2);
        sf.gameObject.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {

        //Takes input from all player methods
        takeInput(t1);
        takeInput(t2);
        takeInput(t3);

        //Lowers cooldown for input
        if (inputCooldown > 0)
        {
            inputCooldown -= Time.deltaTime;
        }


        //----------------------------------------------------------------------------------------------------------
        //STATE INFORMATION BELOW
        //----------------------------------------------------------------------------------------------------------

        //Visually process the different states here
        switch (state)
        {
            //"Press Start"
            case 0:

                if(logoStateFlow > 0f)
                {
                    logoStateFlow -= Time.deltaTime * (logoStateFlow + 0.01f) * speed;

                    //Resets flow up if it overflows
                    if(logoStateFlow < 0f)
                    {
                        logoStateFlow = 0f;
                    }
                }

                //Makes "Press Start" Glow
                glowAlpha = Mathf.PingPong(Time.time, 0.5f);
                startGlow.color = new Color(startGlow.color.r, startGlow.color.g, startGlow.color.b, glowAlpha + 0.3f);

                break;

            //Select Mode
            case 1:
                
                //If it is less than the target value....
                if(logoStateFlow < 0.5f)
                {
                    logoStateFlow += Time.deltaTime * ((0.5f - logoStateFlow) * 2 + 0.01f) * speed;

                    //Snaps when close enough
                    if(logoStateFlow >= 0.499)
                    {
                        logoStateFlow = 0.5f;
                    }
                }
                //If Greater than target value...
                else if (logoStateFlow > 0.5f)
                {
                    logoStateFlow -= Time.deltaTime * ((logoStateFlow-0.5f) * 2 +0.01f) * speed;
                    //Snaps when close enough
                    if (logoStateFlow <= 0.501)
                    {
                        logoStateFlow = 0.5f;
                    }
                }

                break;

            //Multiplayer mode
            case 2:

                if(logoStateFlow < 1f)
                {
                    logoStateFlow += Time.deltaTime * ((1f - logoStateFlow)*2f + 0.01f) * speed;
                    if(logoStateFlow > 1f)
                    {
                        logoStateFlow = 1f;
                    }
                }

                break;
        }

        //Changes the size and position of the logo
        logo.GetComponent<RectTransform>().localPosition = Vector3.Lerp(logoDefaultPos, logoTarPos, logoStateFlow);
        logo.GetComponent<RectTransform>().localScale = Vector3.Lerp(logoDefaultScale, logoTarScale, logoStateFlow);

        //Change positions for the states as well
        state0.GetComponent<RectTransform>().localPosition = Vector3.Lerp(new Vector3(0f, 0f, 0f), new Vector3(-stateAdjustmentSize *2, 0f, 0f), logoStateFlow);
        state1.GetComponent<RectTransform>().localPosition = Vector3.Lerp(new Vector3(stateAdjustmentSize, 0f, 0f), new Vector3(-stateAdjustmentSize, 0f, 0f), logoStateFlow);
        state2.GetComponent<RectTransform>().localPosition = Vector3.Lerp(new Vector3(stateAdjustmentSize * 2, 0f, 0f), new Vector3(0f, 0f, 0f), logoStateFlow);
    }


    //Takes input - this method is called for multiple sources
    public void takeInput(Player p)
    {
        //If input isn't on cooldown...
        if (inputCooldown <= 0)
        {
            //When the player presses A or Start, they advance through the menu
            if (p.GetButtonDown("Start") || p.GetButtonDown("A"))
            {
                if(state == 0)
                {
                    advState();
                }
                   

                //Sets the cooldown
                inputCooldown = maxCooldown;
            }
            //When the player presses B, the head backwards
            if (p.GetButtonDown("B"))
            {
                revState();

                //Sets the cooldown
                inputCooldown = maxCooldown;
            }

            if (p.GetButtonDown("Up-Dpad") || p.GetButtonDown("Right-Dpad"))
            {
                //Determines the next button the player can select
                if (state == 1)
                {
                    prevSelect(ref state1Buttons);
                }
                if (state == 2)
                {
                    prevSelect(ref state2Buttons);
                }
            }


            if (p.GetButtonDown("Down-Dpad") || p.GetButtonDown("Left-Dpad"))
            {
                //Determines the next button the player can select
                if (state == 1)
                {
                    nextSelect(ref state1Buttons);
                }
                if (state == 2)
                {
                    nextSelect(ref state2Buttons);
                }
            }
        }

        
    }

    //advancesState
    public void advState()
    {
        if (state < 2)
        {
            state++;
        }                
        
        //Sets transition to the next level when selected
        else if (state == 2)
        {
            sf.fadeIn = false;
            Destroy(menuInput);
            sf.targetScene = 1;
        }
        selected = 0;


    }

    //Reverses state
    public void revState()
    {
        if (state > 0)
        {
            state--;
        }
        if (state < 0)
        {
            state = 0;
        }
        selected = 0;
    }

    public void nextSelect(ref Button[] btns)
    {
        print("attempts");
        selected++; 

        //Wheels selected around...
        if(selected > btns.Length - 1)
        {
            selected = 0;
        }

        //Selects button if it can
        if (btns[selected].interactable)
        {
            btns[selected].Select();print("selected");
        }
        //Otherwise, calls function recursively
        else
        {
            nextSelect(ref btns);
        }
    }

    public void prevSelect(ref Button[] btns)
    {
        print("attempts");
        selected--;

        //Wheels selected around...
        if (selected < 0 )
        {
            selected = btns.Length-1;
        }

        //Selects button if it can
        if (btns[selected].interactable)
        {
            btns[selected].Select(); print("selected");
        }
        //Otherwise, calls function recursively
        else
        {
            prevSelect(ref btns);
        }
    }
}
