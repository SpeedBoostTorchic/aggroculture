using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DotScroller : MonoBehaviour {

    //Base Texture
    public Image im;

    //Speed & Dir
    public float xSpeed;
    public float ySpeed;

    //Size
    public float xScale;
    public float yScale;

	// Use this for initialization
	void Start () {
        im = gameObject.GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {

        //Dtermines the speed/direction at which the dots scroll
        float xOffset = xSpeed * Time.time;
        float yOffset = ySpeed * Time.time;

        //Changes position and scale of the material's texture
        im.material.mainTextureOffset = new Vector2(xOffset, yOffset);
        im.material.mainTextureScale = new Vector2(xScale, yScale);
    }
}
