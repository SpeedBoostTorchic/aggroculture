using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uiCooldownBar : MonoBehaviour {

    public Image filledBar;
    public Text timer;
    public PlayerClass parentPlayer;
    protected float cooldownMax;
    protected float activeMax;
    protected int pNum;

    public Color baseColor;
    public Color darkColor;
    public float lerpBase;

	// Use this for initialization
	void Start () {
        cooldownMax = Glossary.gs.cooldowns[parentPlayer.charNum];
        pNum = parentPlayer.pNum;
	}
	
	// Update is called once per frame
	void Update () {

        //Fills the bar baed on how much the cooldown has transpired
        filledBar.fillAmount = 1f - (parentPlayer.powerCooldown / cooldownMax);

        //Determines the maxmimum time the ability remains active
        if(GameManager.gm.returnPlayerPower(pNum) > activeMax)
        {
            activeMax = GameManager.gm.returnPlayerPower(pNum);
        }

        //If the cooldown is maxed and power is inactive...
        if(parentPlayer.powerCooldown <= 0 && GameManager.gm.returnPlayerPower(pNum) <= 0)
        {
            //...Causes the bar to fluctuate in color
            lerpBase = Mathf.PingPong(lerpBase, 1);
            filledBar.color = Color.Lerp(baseColor, Color.white, lerpBase);        
            timer.text = "READY!";
        }
        //If the player power is currently active...
        else if (GameManager.gm.returnPlayerPower(pNum) > 0f)
        {
            //...changes the bar to yellow, and displays seconds remaining
            filledBar.color = Color.yellow;
            timer.text = (int)(GameManager.gm.returnPlayerPower(pNum)+1) + "s";
            filledBar.fillAmount = GameManager.gm.returnPlayerPower(pNum) / activeMax;
            lerpBase = 0f;
        }
        //If the player power is current on cooldown...
        else if (parentPlayer.powerCooldown > 0 && GameManager.gm.returnPlayerPower(pNum) <= 0)
        {
            filledBar.fillAmount = 1f - parentPlayer.powerCooldown / cooldownMax;
            filledBar.color = Color.Lerp(darkColor, baseColor, filledBar.fillAmount);
            timer.text = (int)(parentPlayer.powerCooldown) + "s";
        }
	}
}
