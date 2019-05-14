using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class winWheel : MonoBehaviour {

    //Variables used for the intial entry
    public RectTransform rt;        //RT for this object, used for rise
    public float curY;
    public float starY;
    public float tarY;
    public float speed;
    public float t;
    public float maxAngle;

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
            curY = Mathf.Lerp(starY, tarY, t);
            rt.anchoredPosition = new Vector2(0f, curY);
        }

        //Ensures normal calucaltions won't happen when ONLY one side is in debt
        if ((GameManager.gm.p1.money > 0 && GameManager.gm.p2.money > 0)
            || (GameManager.gm.p1.money < 0 && GameManager.gm.p2.money < 0) && totalMoney != 0)
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
        //If player 1 is in debt, but player 2 isn't
        else if (GameManager.gm.p1.money <= 0 && GameManager.gm.p2.money > 0)
        {
            p1Percentage = 0.01f;
        }
        //If player 2 is in debt, but player 1 isn't
        else if (GameManager.gm.p2.money <= 0 && GameManager.gm.p1.money > 0)
        {
            p1Percentage = 0.99f;
        }
        else if (GameManager.gm.p2.money == GameManager.gm.p1.money || totalMoney == 0)
        {
            p1Percentage = 0.5f;
        }

        //Flips angle around if 360
        if(maxAngle == 360)
        {
            p1Percentage = 1f - p1Percentage;
        }

        //Rise//Fall onto screen begins when the game starts
        if ((GameManager.gm.bothReady && rt.anchoredPosition.y < tarY && !GameManager.gm.gameFinish)
            || (GameManager.gm.bothReady && rt.anchoredPosition.y > tarY && !GameManager.gm.roundFinish) && GameManager.gm.preCountDown <= 0f)
        {
            //Used to rise into the top
            t += Time.deltaTime * speed;
            curY = Mathf.Lerp(starY, tarY, t);
            rt.anchoredPosition = new Vector2(0f, curY);
        }

        //Ensures that the percentage can never drop below 0
        if (p1Percentage < 0.01f)
        {
            p1Percentage = 0.01f;
        }
        else if (p1Percentage > 0.99f)
        {
            p1Percentage = 0.99f;
        }

        //Alters the actual images
        p1Wheel.fillAmount = p1Percentage;
        line.rotation = Quaternion.Euler(new Vector3(0f, 0f, (p1Percentage-0.5f) * -maxAngle));
        line2.rotation = Quaternion.Euler(new Vector3(0f, 0f, -line.rotation.z));

        //Changes the color of the percentage display based on who's winning
        if(p1Percentage > 0.55f)
        {
            if(maxAngle == 180)
            {
                per.color = Color.cyan;
            }
            else
            {
                per.color = Color.magenta;
            }
                
            
        }else if (p1Percentage < 0.45f)
        {
            if(maxAngle == 180)
            {
                per.color = Color.magenta;
            }
            else
            {
                per.color = Color.cyan;
            }               
        }
        else
        {
            per.color = Color.white;
        }

        //Changes the text of the percentage display - will always be >=50%
        if (p1Percentage >= 0.5f)
        {
            if(maxAngle == 180)
            {
                per.text = (int)(100 * p1Percentage) + "%";
            }
            else
            {
                per.text = (int)(100 * (1 - p1Percentage)) + "%";
            }
            
        }
        else
        {
            if (maxAngle == 180)
            {
                per.text = (int)(100 * (1 - p1Percentage)) + "%";
            }
            else
            {
                per.text = (int)(100 * p1Percentage) + "%";
            }
        }

        //If the counter is 360 degrees, must be able to flip the percentage/line
        if(maxAngle == 360)
        {
            //Flips as the percentage reaches the halfway point
            if(p1Percentage < 0.5f)
            {
                line2.transform.localScale = new Vector3(1f, 1f, 1f);
                per.transform.parent.localScale = new Vector3(1f, -1f, 1f);

            }
            else if (p1Percentage > 0.5f)
            {
                line2.transform.localScale = new Vector3(-1f, 1f, 1f);
                per.transform.parent.localScale = new Vector3(-1f, -1f, 1f);
            }
        }

    }
}
