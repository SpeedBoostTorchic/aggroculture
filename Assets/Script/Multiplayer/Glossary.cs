using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glossary : MonoBehaviour {

    //Singleton
    public static Glossary gs;

    //Various Dictionaries listed below
    [Header("Characters")]
    public Dictionary<string, int> character = new Dictionary<string, int>();
    public float[] cooldowns = new float[11];
    public string[,] activationQuotes = new string[11, 11];
    public string[] powerNames = new string[11];

    [Header("Cosmetic Alts")]
    public Sprite[] normalSky;
    public Sprite[] rainySky;
    public Color skyColor;
    public Color rainColor;

    public Sprite[] level1CropFront;
    public Sprite[] level1CropBack;
    public Sprite[] level2CropFront;
    public Sprite[] level2CropBack;
    public Sprite[] level3CropFront;
    public Sprite[] level3CropBack;

    //Each water gradient has color elements - the second is also the base color of normal water
    public Gradient normalWater;
    public Gradient poisonedWater;
    public Gradient enhancedWater;

    public Gradient normWaterTop;
    public Gradient poisonedWaterTop;
    public Gradient enhancedWaterTop;

    // The Glossary is used to hold all of the moment int definitions
    void Awake () {
        DontDestroyOnLoad(this);
        defineCharacters();
    }
    void Start()
    {
        //Ensures there is only one
        if(Glossary.gs == null)
        {
            Glossary.gs = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    private void defineCharacters()
    {
        //First defines the number of characters and their names
        character.Add("NormF", 0);
        character.Add("Norm", 1);
        character.Add("Rivan", 2);
        character.Add("Flint", 3);
        character.Add("Cash", 4);
        character.Add("Regina", 5);
        character.Add("Raine", 6);
        character.Add("Sui", 7);
        character.Add("Narcia", 8);
        character.Add("Rose", 9);
        character.Add("Thorn", 10);

        //Then defines their power activation quotes
        activationQuotes[0,0] = "Default";
        powerNames[0] = "Default";

        activationQuotes[character["NormF"], 0] = "Slow down...";
        activationQuotes[character["NormF"], 1] = "Chill out!";
        activationQuotes[character["NormF"], 2] = "It's just farming.";
        powerNames[character["NormF"]] = "Normalize";
        cooldowns[character["NormF"]] = 20f;

        activationQuotes[character["Norm"],0] = "Slow down...";
        activationQuotes[character["Norm"],1] = "Chill out!";
        activationQuotes[character["Norm"],2] = "It's just farming.";
        powerNames[character["Norm"]] = "Normalize";
        cooldowns[character["Norm"]] = 20f;

        activationQuotes[character["Rivan"],0] = "Never give up!";
        activationQuotes[character["Rivan"],1] = "Not over yet!";
        activationQuotes[character["Rivan"],2] = "I'll show you!";
        powerNames[character["Rivan"]] = "DETERMINATION";
        cooldowns[character["Rivan"]] = 20f;

        activationQuotes[character["Flint"],0] = "BURN! BABY, BURN!";
        activationQuotes[character["Flint"],1] = "DIE! DIE! DIE!";
        activationQuotes[character["Flint"],2] = "Ashes to ashes!";
        powerNames[character["Flint"]] = "Ignition";
        cooldowns[character["Flint"]] = 10f;

        activationQuotes[character["Cash"],0] = "Cha-ching!";
        activationQuotes[character["Cash"],1] = "Ballin'!";
        activationQuotes[character["Cash"],2] = "Pay your dues!";
        powerNames[character["Cash"]] = "O Fortuna!";
        cooldowns[character["Cash"]] = 5f;

        activationQuotes[character["Regina"],0] = "Show some respect!";
        activationQuotes[character["Regina"],1] = "Bow, peasant!";
        activationQuotes[character["Regina"],2] = "By my command!";
        powerNames[character["Regina"]] = "Queenly Command";
        cooldowns[character["Regina"]] = 20f;

        activationQuotes[character["Raine"],0] = "Wash it away...";
        activationQuotes[character["Raine"],1] = "Hmmm...";
        activationQuotes[character["Raine"],2] = "Like tears...";
        powerNames[character["Raine"]] = "Raine Dance";
        cooldowns[character["Raine"]] = 15f;

        activationQuotes[character["Sui"],0] = "Want a drink?";
        activationQuotes[character["Sui"],1] = "Have a taste!";
        activationQuotes[character["Sui"],2] = "Here it comes!";
        powerNames[character["Sui"]] = "Spring Forth";
        cooldowns[character["Sui"]] = 30f;

        activationQuotes[character["Narcia"],0] = "Look don't touch!";
        activationQuotes[character["Narcia"],1] = "Gimmie, gimmie!";
        activationQuotes[character["Narcia"],2] = "It's all mine!";
        powerNames[character["Narcia"]] = "Mine! Mine!";
        cooldowns[character["Narcia"]] = 30f;

        activationQuotes[character["Rose"],0] = "D-don't look!";
        activationQuotes[character["Rose"],1] = "Stop staring!";
        activationQuotes[character["Rose"],2] = "Trying my best!";
        powerNames[character["Rose"]] = "In Bloom";
        cooldowns[character["Rose"]] = 30f;

        activationQuotes[character["Thorn"],0] = "...Die";
        activationQuotes[character["Thorn"],1] = "I Hope you suffer!";
        activationQuotes[character["Thorn"],2] = "This rose has thorns!";
        powerNames[character["Thorn"]] = "Wither Away";
        cooldowns[character["Thorn"]] = 30f;

    }

    public string getQuote(int i)
    {
        int t = (int)Random.Range(0, 3);
        return activationQuotes[i,t];
    }
}
