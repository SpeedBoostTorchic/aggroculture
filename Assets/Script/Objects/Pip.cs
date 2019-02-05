using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pip : MonoBehaviour {

    //Height Values used to represent the entry animation of the pip
    private float starHeight;
    public float curHeight;
    public float tarHeight;
    public float animSpeed;
    public bool animFin;

    //Variables here deal with the visuals associated with the pip
    public bool active;
    public Color curColor;
    public Color activeColor;
    public Color inactiveColor;
    public Image im;
    public RectTransform rt;
    public float t;

	// Use this for initialization
	void Awake () {
        animFin = false;
        starHeight = 0f;
        curHeight = starHeight;
        im = this.gameObject.GetComponent<Image>();
        rt = this.gameObject.GetComponent<RectTransform>();
        curColor = activeColor;
	}
	
	// Update is called once per frame
	void Update () {

        im.color = curColor;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, curHeight);

        //Entry animation handled here
        if (!animFin)
        {
            t += Time.time * animSpeed;
            curHeight = Mathf.Lerp(starHeight, tarHeight * 1.2f, t);
            curColor = Color.Lerp(curColor, Color.white, t);

            //When target height is reached, the entry anim is finished
            if(curHeight >= tarHeight * 1.19f)
            {
                animFin = true;
                t = 0f;
            }

        }
        //After it reaches its apex, the pip will slowly wind down
        else
        {
            t += Time.time * animSpeed;
            curHeight = Mathf.Lerp(curHeight, tarHeight, t);
            curColor = Color.Lerp(curColor, activeColor, t);
        }

        //Changes color based on if the pip is marked active or not
        if (active)
        {
            im.color = activeColor;
        }
        else
        {
            im.color = inactiveColor;
        }
        
	}
}
