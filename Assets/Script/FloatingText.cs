using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour {

    //Basic info regarding the text
    public string text;
    public Text parent;
    public Text[] children;

    public float speed;         //How fast the text moves
    public float lifetime;      //How long the text lives
    private float startLifetime;

    //Determines where and how the text will show
    public int player;
    public bool positive = true;

    public GameObject investPic;
    public GameObject cashPic;

	// Use this for initialization
	void Start () {

        //Sets all text boxes to match the message
        parent.text = text;
        foreach(Text t in children)
        {
            t.text = text;
        }

        //Layer is P1's by default - changes to P2 if prompted
        if(player == 2)
        {
            //Player 2 is currently on Layer 9 <- CHECK FIRST HERE IF CHANGES
            layerRecursively(gameObject, 9);
        }
        else
        {
            layerRecursively(gameObject, 8);
        }

        //If the player makes money...
        if (positive)
        {
            cashPic.SetActive(true);
            investPic.SetActive(false);
        }
        //If the player islosing money...
        else
        {
            cashPic.SetActive(false);
            investPic.SetActive(true);
            parent.color = Color.magenta;
            speed *= -1f;
            transform.position += new Vector3(0f, Mathf.Abs(speed * lifetime)/1.5f, 0f);
        }

        startLifetime = lifetime;

    }
	
	// Update is called once per frame
	void Update () {

        //Destroys self after a set period of time
        lifetime -= Time.deltaTime;
        if(lifetime <= 0)
        {
            Destroy(gameObject);
        }

        //Moves self based o speed
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y + speed*Time.deltaTime, transform.position.z);
        transform.position = newPos;
        speed *= 0.99f;

        foreach(Transform c in gameObject.transform){

        }

        if(lifetime / startLifetime <= 0.5f)
        {
            float curT = lifetime / startLifetime + 0.3f;
            transform.localScale = new Vector3(curT, curT, curT);
        }
        
	}

    //Function sets all children on the same layer
    public void layerRecursively(GameObject obj, int x)
    {
        //Ends the loop if there are no children
        if (obj == null)
        {
            return;
        }

        //Else sets the object's layer to x...
        obj.layer = x;

        //...and continues the function
        foreach(Transform c in obj.transform)
        {
            if(c == null)
            {
                continue;
            }

            layerRecursively(c.gameObject, x);
        }
    }
}
