using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Rewired;

public class CharacterSelectManager : MonoBehaviour {

    //The player objects from which input is recieved
    [Header("Player Tabs")]
    public Player p1;
    public Player p2;
    public bool p1Ready;
    public bool p2Ready;
    public int p1Selection;         //Has the player entered their control style?
    public int p2Selection;
    public int maxCharacterNum;
    public bool p1CharacterChosen;
    public bool p2CharacterChosen;  //Have Both players selected their character?

    [Header("Info Boxes")]
    //Information overlays on the character portraits
    public GameObject p1Info;
    public GameObject p2Info;
    public bool p1InfoCalled;
    public bool p2InfoCalled;
    public Vector2 p1InfoTar;
    public Vector2 p1InfoOrig;
    public Vector2 p2InfoTar;
    public Vector2 p2InfoOrig;
    private float p1InfoPos;
    private float p2InfoPos;

    //Control styles
    public Player t1;
    public Player t2;
    public Player t3;   //Keyboard and Mouse

    //Used for the visuals on the player tabs
    public GameObject p1Overlay;
    public GameObject p2Overlay;
    public Text p2Text;
    public GameObject p1Name;
    public GameObject p2Name;

    [Header("Sprites and Images")]
    public int[] lockedCharacters;
    public Sprite[] characterPortraits;
    public Sprite[] characterIcons;
    public Sprite defaultPortrait;
    public Sprite defaultIcon;
    public string[] characterNames;
    public Sprite unready;
    public Sprite p1ReadyOverlay;
    public Sprite p2ReadyOverlay;

    //Colors to use on the background of each character
    public Color[] bgColor;
    public Color[] gradientColors;

    [Header("P1 Visual Components")]
    //The components of the p1 selections screen
    public Image p1Portrait;
    public Image p1BG;
    public Image p1BGOverlay;
    public Image p1Gradient;
    public Image p1Icon;
    public Text  p1CharName;
    public GameObject l;
    public GameObject r;
    public GameObject readyOverlay1;
    public CharacterInfo ciP1;

    [Header("P2 Visual Components")]
    //The components of the p2 selection screen;
    public Image p2Portrait;
    public Image p2BG;
    public Image p2BGOverlay;
    public Image p2Gradient;
    public Image p2Icon;
    public Text p2CharName;
    public GameObject l2;
    public GameObject r2;
    public GameObject readyOverlay2;
    public CharacterInfo ciP2;


    [Header("Map Selection")]
    public bool bothSelected;
    public GameObject characterSelectObj;
    public GameObject MapSelectionObj;
    public GameObject preview;
    public float speed;
    public float anchorPos;
    public float outPos;
    public float measure;
    public ScreenFader sf;




    // Use this for initialization
    void Awake () {
        t1 = ReInput.players.GetPlayer(0);
        t2 = ReInput.players.GetPlayer(1);
        t3 = ReInput.players.GetPlayer(2);
    }

    void Start()
    {
        //Ensures players do not start on a locked character
        skipLocked(ref p1Selection, true);
        skipLocked(ref p2Selection, true);

        sf.gameObject.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {


        //Switches to map selection when both characters are ready
        if(p1CharacterChosen && p2CharacterChosen)
        {
            writeData();
            bothSelected = true;
        }
        else
        {
            bothSelected = false;
        }

        //Visually dislays when the player is ready
        if (p1CharacterChosen)
        {
            p1Name.GetComponent<Text>().text = "READY!";

            //Re-Activates the overlay, and sets new color
            readyOverlay1.SetActive(true);
            readyOverlay1.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.88f);
            readyOverlay1.GetComponent<Image>().sprite = p1ReadyOverlay;
        }
        //If the CHaracter is NOT ready...
        else
        {
            p1Name.GetComponent<Text>().text = "Player 1";

            //Displays nothing while choosing
            if (p1Ready)
            {
                readyOverlay1.SetActive(false);
            }

            //Displays controller when picking those
            else
            {
                readyOverlay1.SetActive(true);
                readyOverlay1.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                readyOverlay1.GetComponent<Image>().sprite = unready;
            }
        }
        if (p2CharacterChosen)
        {
            p2Name.GetComponent<Text>().text = "READY!";

            //Re-Activates the overlay, and sets new color
            readyOverlay2.SetActive(true);
            readyOverlay2.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.88f);
            readyOverlay2.GetComponent<Image>().sprite = p2ReadyOverlay;
        }
        else
        {
            p2Name.GetComponent<Text>().text = "Player 2";

            //Displays nothing while choosing
            if (p2Ready)
            {
                readyOverlay2.SetActive(false);
            }

            //Displays controller when picking those
            else
            {
                readyOverlay2.SetActive(true);
                readyOverlay2.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                readyOverlay2.GetComponent<Image>().sprite = unready;
            }
        }

        //Determines relative location of UI elements
        characterSelectObj.transform.localPosition = Vector2.Lerp(new Vector2(anchorPos, 0f),  new Vector2(-outPos, 0f), measure);
        MapSelectionObj.transform.localPosition = Vector2.Lerp(new Vector2(outPos, 0f), new Vector2(anchorPos, 0f), measure);
        preview.transform.localPosition = Vector2.Lerp(new Vector2(26f, 0f), Vector2.zero, measure);

        //Moves stuff into view based on what player's have selected
        if (bothSelected && measure < 1f)
        {
            //Disables names
            p1Name.SetActive(false);
            p2Name.SetActive(false);

            //Resets map preview each time
            if (measure == 0)
            {
                preview.GetComponent<MapPreview>().setMap();
                preview.GetComponent<MapPreview>().updateMap();
            }
            measure += speed * Time.deltaTime * (1f - Mathf.Clamp01(measure));
            if(measure > 0.999f)
            {
                measure = 1f;
            }
        }
        if(!bothSelected && measure > 0f)
        {
            measure -= speed * Time.deltaTime * Mathf.Clamp01(measure);
            if(measure < 0.001f)
            {
                measure = 0f;
            }
        }

        //Moves power info boxes in a similar manner to the above
        incrementInfoPos();

        //Determines which player is which
        if (!p1Ready || !p2Ready)
        {
            takeInput(t1);
            takeInput(t2);
            takeInput(t3);
        }
        else
        {
            takeInput(p1);
            takeInput(p2);
        }

        //Determines if the first player's overlay is active
        if (p1Ready)
        {
            ciP1.charNum = p1Selection;
            p1Overlay.SetActive(false);
            p1Name.SetActive(true);
            changePortrait(1);

            if (!p2Ready)
            {
                p2Text.text = "P2 Press Start";
                p2Text.color = Color.white;
            }
        }
        else
        {
            p1Overlay.SetActive(true);
            p1Name.SetActive(false);
            if (!p2Ready)
            {
                p2Text.text = "Wait for Player 1";
                p2Text.color = Color.grey;
            }
        }
        if (p2Ready)
        {
            ciP2.charNum = p2Selection;
            p2Overlay.SetActive(false);
            p2Name.SetActive(true);
            changePortrait(2);
        }
        else
        {
            p2Overlay.SetActive(true);
            p2Name.SetActive(false);
        }
	}

    //The same "take input" method is used for both players
    public void takeInput(Player pNum)
    {       
        //If player is ready...
        if(p1Ready && p1 == pNum)
        {
            //Backs out of control
            if (pNum.GetButtonDown("B"))
            {
                if (p1CharacterChosen)
                {
                    p1CharacterChosen = false;
                }
                else
                {
                    p1Ready = false;
                }             
            }

            //Calls player info box
            if (pNum.GetButton("Y"))
            {
                //Toggles called status
                if(!bothSelected && p1Ready)
                {
                    p1InfoCalled = true;
                }
            }
            else
            {
                p1InfoCalled = false;
            }

            //Locks in current selection
            if (pNum.GetButtonDown("Start") || pNum.GetButtonDown("A"))
            {
                if (p1Ready)
                {
                    p1CharacterChosen = true;
                    /*if(p2Selection == p1Selection)
                    {
                        p2Selection++;
                        skipLocked(ref p2Selection, true);
                    }*/
                }
                

                //CHANGES SCENES HERE!
                if (bothSelected)
                {
                    p1InfoCalled = false;
                    p2InfoCalled = false;
                    sf.fadeIn = false;
                    sf.targetScene = 2;
                }
            }

            //Once character is locked in, can no longer change
            if (!p1CharacterChosen)
            {
                selection(1);
            }
        }

        //If Player2 is ready...
        if (p2Ready && p2 == pNum)
        {

            if (pNum.GetButtonDown("B"))
            {
                if (p2CharacterChosen)
                {
                    p2CharacterChosen = false;
                }
                else
                {
                    p2Ready = false;
                }           
            }

            //Calls player info box
            if (pNum.GetButton("Y"))
            {
                //Toggles called status
                if (!bothSelected && p1Ready)
                {
                    p1InfoCalled = true;
                }
            }
            else
            {
                p1InfoCalled = false;
            }

            //Locks in current selection
            if (pNum.GetButtonDown("Start") || pNum.GetButtonDown("A"))
            {
                p2CharacterChosen = true;

               /* if (p2Selection == p1Selection)
                {
                    p1Selection++;
                    skipLocked(ref p1Selection, true);
                }*/

                //CHANGES SCENES HERE!
                if (bothSelected)
                {
                    p1InfoCalled = false;
                    p2InfoCalled = false;
                    sf.fadeIn = false;
                    sf.targetScene = 2;
                }
            }
            //Once character is locked in, can no longer change
            if (!p2CharacterChosen)
            {
                selection(2);
            }
        }

        //If player 1 hasn't pressed start...
        if (!p1Ready && pNum != p2)
        {
            if (pNum.GetButtonDown("Start") || pNum.GetButtonDown("A"))
            {
                p1 = pNum;
                p1Ready = true;
            }
        }
        //If player 1 is ready, but player 2 isn't...
        else if (!p2Ready && pNum != p1)
        {
            if (pNum.GetButtonDown("Start") || pNum.GetButtonDown("A"))
            {
                p2 = pNum;
                p2Ready = true;
            }
        }
    }

    //Used to control player character selection
    public void selection(int p)
    {
        //Player 1 controls
        if(p == 1 && p1 != null)
        {
            //Decrements p1
            if (p1.GetButtonDown("Left Bumper") || p1.GetButtonDown("Left-Dpad"))
            {

                p1Selection--;

                //Skips locked characters
                skipLocked(ref p1Selection, false);

                //Wraps around to the top
                if(p1Selection < 0)
                {
                    p1Selection = maxCharacterNum;
                }

                skipLocked(ref p1Selection, false);
            }

             if(p1.GetButton("Left Bumper") || p1.GetButton("Left-Dpad")){
                //Changes the apearance of the left bumper icon
                l.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                l.GetComponent<Image>().color = Color.grey;
            }
            else
            {
                //Changes the icon back
                l.transform.localScale = new Vector3(1f, 1f, 1f);
                l.GetComponent<Image>().color = Color.white;
            }

            //Increments p1
            if (p1.GetButtonDown("Right Bumper") || p1.GetButtonDown("Right-Dpad"))
            {
                p1Selection++;

                //Skips locked characters
                skipLocked(ref p1Selection, true);

                //Wraps around to the top
                if (p1Selection > maxCharacterNum)
                {
                    p1Selection = 0;
                }

                //Skips locked characters
                skipLocked(ref p1Selection, true);

            }

            if (p1.GetButton("Right Bumper") || p1.GetButton("Right-Dpad"))
            {
                //Changes the apearance of the right bumper icon
                r.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                r.GetComponent<Image>().color = Color.grey;
            }
            else
            {
                //Changes the icon back
                r.transform.localScale = new Vector3(1f, 1f, 1f);
                r.GetComponent<Image>().color = Color.white;
            }
        }

        //Player 2 Controls
        else if (p == 2 && p2 != null)
        {
            //Decrements p2
            if (p2.GetButtonDown("Left Bumper") || p1.GetButtonDown("Left-Dpad"))
            {
                p2Selection--;
                skipLocked(ref p2Selection, false);

                //Wraps around to the top
                if (p2Selection < 0)
                {
                    p2Selection = maxCharacterNum;
                }

                skipLocked(ref p2Selection, false);
            }
            //Changes bumper icons
            if (p2.GetButton("Left Bumper") || p1.GetButton("Left-Dpad"))
            {

                //Changes the apearance of the left bumper icon
                l2.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                l2.GetComponent<Image>().color = Color.grey;
            }
            else
            {
                //Changes the icon back
                l2.transform.localScale = new Vector3(1f, 1f, 1f);
                l2.GetComponent<Image>().color = Color.white;
            }

            //Increments p2
            if (p2.GetButtonDown("Right Bumper") || p1.GetButtonDown("Right-Dpad"))
            {
                p2Selection++;
                skipLocked(ref p2Selection, true);
                //Wraps around to the top
                if (p2Selection > maxCharacterNum)
                {
                    p2Selection = 0;
                }
                skipLocked(ref p2Selection, true);
            }

            //Changes the appearance of the bumper icons
            if (p2.GetButton("Right Bumper") || p1.GetButton("Right-Dpad"))
            {
                //Changes the apearance of the right bumper icon
                r2.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                r2.GetComponent<Image>().color = Color.grey;
            }
            else
            {
                //Changes the icon back
                r2.transform.localScale = new Vector3(1f, 1f, 1f);
                r2.GetComponent<Image>().color = Color.white;
            }

        }
    }

    public void skipLocked(ref int s, bool increasing)
    {
        bool success = true;

        if (increasing)
        {
            //Skips locked characters
            foreach (int i in lockedCharacters)
            {
                if (s == i)
                {
                    success = false;
                    s++;
                }
            }
        }
        else
        {
            //Skips locked characters
            foreach (int i in lockedCharacters)
            {
                if (s == i)
                {
                    success = false;
                    s--;
                }
            }
        }

        //Repeats again each time it fails to find
        if (!success)
        {
            skipLocked(ref s, increasing);
        }
    }

    //Alters the portraits to match
    public void changePortrait(ref Image portrait, ref Image icon, ref Image bg, ref Image gradient, ref Image grad2, ref Text name, int selection)
    {
        //Determines which portrait to use...
        Sprite tempPort = characterPortraits[selection];
        if (tempPort == null)
            tempPort = defaultPortrait;

        //Determines which icons to use...
        Sprite tempIcon = characterIcons[selection];
        if (tempIcon == null)
            tempIcon = defaultIcon;
        
        //Determines which colors to use...
        Color tempBG = bgColor[selection];
        Color tempGra = gradientColors[selection];
        if(tempBG == null || tempGra == null)
        {
            tempBG = Color.grey;
            tempGra = Color.grey;
        }

        //Determines character name...
        string tempN = characterNames[selection];
        if (tempN == null)
            tempN = "LOCKED";

        //Finally, sets all things!
        portrait.sprite = tempPort;
        icon.sprite = tempIcon;
        bg.color = tempBG;
        grad2.color = new Color(tempBG.r, tempBG.g, tempBG.b, 0.77f) ;
        gradient.color = tempGra;
        name.text = tempN;
    }

    //Overload takes player number, then grabs all relevnat parameters
    public void changePortrait(int pNum)
    {
        if(pNum == 1)
        {
            changePortrait(ref p1Portrait, ref p1Icon, ref p1BG, ref p1Gradient, ref p1BGOverlay, ref p1CharName, p1Selection);
        }
        else if (pNum == 2)
        {
            changePortrait(ref p2Portrait, ref p2Icon, ref p2BG, ref p2Gradient, ref p2BGOverlay, ref p2CharName, p2Selection);
        }
    }

    //Writes information to the DATA HANDLER
    public void writeData()
    {
        //Sets the relevant Rewired values for control
        DataHolder.dh.p1 = p1;
        DataHolder.dh.p2 = p2;
        DataHolder.dh.p1Pad = true;
        if (p1 == t3)
        {
            DataHolder.dh.p1Pad = false;
        }
        DataHolder.dh.p2Pad = true;
        if(p2 == t3)
        {
            DataHolder.dh.p2Pad = false;
        }

        //Writes information regarding the player selections
        DataHolder.dh.p1Num = p1Selection;
        DataHolder.dh.p2Num = p2Selection;
    }

    //Moves the info boxes above the character portraits
    public void incrementInfoPos()
    {
        //Moves the information overlays in a similar manner to the above
        p1Info.GetComponent<RectTransform>().localPosition = Vector2.Lerp(p1InfoOrig, p1InfoTar, p1InfoPos);
        p2Info.GetComponent<RectTransform>().localPosition = Vector2.Lerp(p2InfoOrig, p2InfoTar, p2InfoPos);

        //Increments p1Info's position
        if (p1InfoCalled && p1InfoPos < 1f)
        {
            p1InfoPos += speed * Time.deltaTime * (1f - Mathf.Clamp01(p1InfoPos));
        }
        else if (!p1InfoCalled && p1InfoPos > 0f)
        {
            p1InfoPos -= speed * Time.deltaTime * (Mathf.Clamp01(p1InfoPos) + 0.01f);
        }

        //Increments p2Info's position
        if (p2InfoCalled && p2InfoPos < 1f)
        {
            p2InfoPos += speed * Time.deltaTime * (1f - Mathf.Clamp01(p2InfoPos));
        }
        else if (!p2InfoCalled && p2InfoPos > 0f)
        {
            p2InfoPos -= speed * Time.deltaTime * (Mathf.Clamp01(p2InfoPos) + 0.01f);
        }

        //Snaps position trackers once they reach a certain value
        if (p1InfoPos > 0.999f)
            p1InfoPos = 1f;
        if (p1InfoPos < 0.001)
            p1InfoPos = 0f;
        if (p2InfoPos > 0.999f)
            p2InfoPos = 1f;
        if (p2InfoPos < 0.001)
            p2InfoPos = 0f;
    }
}
