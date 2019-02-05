using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyBar : MonoBehaviour {

    public bool right;              //Is this a player-1 money bar?
    public float minLength;         //A percentage representing the minimum length of bar at $0
    public float maxMoney;          //A number representing the money count at maximum
    public float lengthAtMax;
    public float money;

    //Used to calculate the size of the player bar
    private float fa100;
    private float fa1000;
    private float fa10000;
    private float fa100000;

    //Appearance-related variables
    public GameObject textObject;
    public float textOff;
    public Color positiveColor;
    public Color negativeColor;

    //The bars themselves
    public Image topBar;
    public Image adjustBar;
    public float fa;
    public float prevFA;
    public float prevLerpSpeed;

	// Use this for initialization
	void Start () {
        fa = minLength;
        prevFA = fa;
	}
	
	// Update is called once per frame
	void Update () {

        //Changes the color of bar based on value of money
        if(money > 0)
        {
            topBar.color = positiveColor;
        }
        else
        {
            topBar.color = negativeColor;
        }
        adjustBar.color = Color.Lerp(Color.white, negativeColor, 2*(fa/prevFA));


        //Adjusts size of the bar based on money
        // fa = Mathf.Clamp(Mathf.Log(Mathf.Abs(money), maxMoney), 0.2f, 1f);
        fa = (Mathf.Abs(money) / (Mathf.Abs(money) + maxMoney)) * (1 - minLength) + minLength;

        if(prevFA > fa)
        {
            prevFA = Mathf.Lerp(prevFA, fa, Time.time * prevLerpSpeed);
        }
        else
        {
            prevFA = fa;
        }
        
        topBar.fillAmount = fa;
        adjustBar.fillAmount = prevFA;

        //Adjusts position of text based on size of bar
        if (right)
        {
            textObject.GetComponent<RectTransform>().anchoredPosition = new Vector2((lengthAtMax+textOff) * fa - textOff, textObject.GetComponent<RectTransform> ().anchoredPosition.y);
        }
        else
        {
            textObject.transform.position = new Vector2(transform.position.x - lengthAtMax * (1 - fa) + textOff, textObject.transform.position.y);
        }
	}


    private void findFillAmount()
    {
        fa100 = Mathf.Clamp(money/100, 0.05f, 0.25f);
        fa1000 = Mathf.Clamp( Mathf.Clamp(money-100, 0, money)/900, 0.05f, 0.25f);
        fa10000 = Mathf.Clamp(Mathf.Clamp(money - 1000, 0, money)/9000, 0.05f, 0.25f);
        fa100000 = Mathf.Clamp( Mathf.Clamp(money - 10000/90000, 0, money), 0.05f, 0.25f);

        fa = fa100 + fa1000 + fa10000 + fa100000;

    }
}
