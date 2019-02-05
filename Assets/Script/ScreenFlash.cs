using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlash : MonoBehaviour {

    private Image im;

    //Determines color of the flash
    public Color curColor;
    public float alpha;

    //determines the speed of the fade-away
    public float speed;

    //Determines whether it fades out or in
    public bool fadeOut;

	// Use this for initialization
	void Start () {
        im = gameObject.GetComponent<Image>();
        curColor = im.color;
        alpha = curColor.a;
	}
	
	// Update is called once per frame
	void Update () {

        //Used to make the screen briefly flash a color
		if(alpha > 0 && fadeOut)
        {
            im.color = new Color(curColor.r, curColor.g, curColor.b, alpha);
            alpha -= Time.deltaTime * speed;
        }

        //Used to make the screen fade into a color
        if(alpha < 1 && !fadeOut)
        {
            im.color = new Color(curColor.r, curColor.g, curColor.b, alpha);
            alpha += Time.deltaTime * speed;
        }
	}

    //Flashes a color, then fades outs
    public void flash(Color c)
    {
        curColor = new Color (c.r,c.g,c.b,1f);
        alpha = 1.0f;
        fadeOut = true;
    }

    //Slowly fades into the assigned color
    public void fade(Color c, float s)
    {
        curColor = new Color(c.r, c.g, c.b, 0f);
        alpha = 0f;
        fadeOut = false;
        speed = s;
    }
}
