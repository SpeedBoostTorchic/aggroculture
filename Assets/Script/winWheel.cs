using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class winWheel : MonoBehaviour {

    //Variables used for the intial entry
    public RectTransform rt;        //RT for this object, used for rise
    public float curY;
    public float tarY;
    public float speed;
    public float t;

    //Variables used to represent the images
    public Image p1Wheel;
    public RectTransform line;
    public RectTransform line2;
    public float totalMoney;
    public float p1Percentage;

    //The actual display in question
    public Text per;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        //Scrolls out of view when the game ends
        if (GameManager.gm.gameFinish)
        {
            //Used to rise into the top
            t -= Time.deltaTime * speed;
            curY = Mathf.Lerp(-500f, tarY, t);
            rt.anchoredPosition = new Vector2(0f, curY);
        }

        //Ensures normal calucaltions won't happen when ONLY one side is in debt
        if ((GameManager.gm.p1.money > 0 && GameManager.gm.p2.money > 0)
            || (GameManager.gm.p1.money < 0 && GameManager.gm.p2.money < 0))
        {
            //Determines and measure the total pool of money
            totalMoney = GameManager.gm.p1.money + GameManager.gm.p2.money;

            if(GameManager.gm.p1.money < 0)
            {
                p1Percentage = GameManager.gm.p2.money / totalMoney;
            }
            else
            {
                p1Percentage = GameManager.gm.p1.money / totalMoney;
            }
            
        }

        //Rise begins when the game starts
        if (GameManager.gm.bothReady && rt.anchoredPosition.y < tarY && !GameManager.gm.gameFinish)
        {
            //Used to rise into the top
            t += Time.deltaTime * speed;
            curY = Mathf.Lerp(-500f, tarY, t);
            rt.anchoredPosition = new Vector2(0f, curY);
        }

        //Ensures that the percentage can never drop below 0
        if (p1Percentage < 0.001f)
        {
            p1Percentage = 0.001f;
        }
        else if (p1Percentage > 0.99f)
        {
            p1Percentage = 0.999f;
        }

        //Alters the actual images
        p1Wheel.fillAmount = p1Percentage;
        line.rotation = Quaternion.Euler(new Vector3(0f, 0f, (p1Percentage-0.5f) * -180f));
        line2.rotation = Quaternion.Euler(new Vector3(0f, 0f, -line.rotation.z));

        //Changes the color of the percentage display based on who's winning
        if(p1Percentage > 0.55f)
        {
            per.color = Color.cyan;
        }else if (p1Percentage < 0.45f)
        {
            per.color = Color.magenta;
        }
        else
        {
            per.color = Color.white;
        }

        //Changes the text of the percentage display - will always be >=50%
        if (p1Percentage >= 0.5f)
        {
            per.text = (int)(100 * p1Percentage) + "%";
        }
        else
        {
            per.text = (int)(100 * (1-p1Percentage)) + "%";
        }

    }
}
