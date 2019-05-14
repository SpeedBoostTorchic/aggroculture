using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfo : MonoBehaviour {

    //Reference to the current character selected
    public int charNum;

    [Header("Profile Elements")]
    public Text name;
    public Text powerName;
    public Text powerDesciption;
    public Text[] tips;
    public Text cooldown;
    public Text duration;
    

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //Matches display to profile
    public void matchProfile(profile p)
    {
        //Sets all the next to the values in the profile
        name.text = p.name;
        powerName.text = p.powerName;
        powerDesciption.text = p.powerDescription;
        cooldown.text = ""+p.cooldown;
        cooldown.text = ""+p.cooldown;

        //Sets all the tips
        for(int i = 0; i< tips.Length; i++)
        {
            tips[i].text = p.tips[i];
        }
    }
}

//A struct to hold all of the data for each profile
public class profile
{
    //Names and titles
    public string name;
    public string powerName;
    public string powerDescription;

    //Numerical information for the power
    public int cooldown;
    public int duration;

    //Gameplay tips that display below the profile
    public string[] tips;
}

//Holds the text that goes into the profiles themselves
public class profileEntries
{
    //The easily entred stuff....
    public static string[] name = { "Norm ♂", "Norm♀", "Rivan", "Flint", "Cash", "Regina", "Raine", "Bayou", "Narcia" };
    public static string[] powerName = { "NORM - alize", "NORM - alize", "A Driven Rival", "Slash-And-Burn", "Daddy's Wallet", "Hail the Queen", "Raine Dance", "Well Overflow", "If I Can't Have You" };
    public static int[] cooldowns = { 20,20,20,10,5,20,15,30, 30};
    public static string[] duration = { "5 sec", "15 sec", "--", "10 sec", "10 sec", "--", "30 sec", "--" };

    //Descriptoins
    public string placeholder = "This is a placeholder string. The power desciprtion has yet to be written.";
}